using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ledger.Core;

namespace Ledger.Journal
{
    public class FileRateProvider : IRateProvider
    {
        private readonly Dictionary<IAsset, Dictionary<IAsset, decimal>> _graph = new Dictionary<IAsset, Dictionary<IAsset, decimal>>();

        public static FileRateProvider Load(string path)
        {
            var result = new FileRateProvider();

            if (string.IsNullOrWhiteSpace(path))
                return result;

            if (!File.Exists(path))
                return result;

            foreach (var line in File.ReadAllLines(path))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#", StringComparison.Ordinal))
                    continue;

                var parts = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 3)
                    throw new ValidationException($"Invalid exchange rate line: {line}");

                var sourceAsset = new Asset(parts[0]);
                var destinationAsset = new Asset(parts[1]);
                var rate = decimal.Parse(parts[2], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

                result.AddRate(sourceAsset, destinationAsset, rate);
            }

            return result;
        }

        public decimal GetRate(IAsset source, IAsset destination, string index)
        {
            if (source.Equals(destination))
                return 1m;

            var queue = new Queue<(IAsset Asset, decimal Rate)>();
            var visited = new HashSet<IAsset>();
            queue.Enqueue((source, 1m));
            visited.Add(source);

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (!_graph.TryGetValue(current.Asset, out var neighbors))
                    continue;

                foreach (var neighbor in neighbors)
                {
                    if (visited.Contains(neighbor.Key))
                        continue;

                    var nextRate = current.Rate * neighbor.Value;

                    if (neighbor.Key.Equals(destination))
                        return nextRate;

                    visited.Add(neighbor.Key);
                    queue.Enqueue((neighbor.Key, nextRate));
                }
            }

            throw new ValidationException($"No exchange rate available from '{source}' to '{destination}'.");
        }

        private void AddRate(IAsset source, IAsset destination, decimal rate)
        {
            if (!_graph.ContainsKey(source))
                _graph[source] = new Dictionary<IAsset, decimal>();

            _graph[source][destination] = rate;
        }
    }
}
