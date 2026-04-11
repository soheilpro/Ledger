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

            if (TryGetRate(source, destination, out var rate))
                return rate;

            throw new ValidationException($"No exchange rate available from '{source}' to '{destination}'.");
        }

        private bool TryGetRate(IAsset source, IAsset destination, out decimal rate)
        {
            var queue = new Queue<(IAsset Asset, decimal Rate)>();
            var visited = new HashSet<IAsset>();
            queue.Enqueue((source, 1m));
            visited.Add(source);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                foreach (var neighbor in GetNeighbors(current.Asset))
                {
                    if (visited.Contains(neighbor.Key))
                        continue;

                    var nextRate = current.Rate * neighbor.Value;

                    if (neighbor.Key.Equals(destination))
                    {
                        rate = nextRate;
                        return true;
                    }

                    visited.Add(neighbor.Key);
                    queue.Enqueue((neighbor.Key, nextRate));
                }
            }

            rate = 0m;
            return false;
        }

        private IEnumerable<KeyValuePair<IAsset, decimal>> GetNeighbors(IAsset source)
        {
            if (_graph.TryGetValue(source, out var directNeighbors))
            {
                foreach (var directNeighbor in directNeighbors)
                    yield return directNeighbor;
            }

            foreach (var rates in _graph)
            {
                if (!rates.Value.TryGetValue(source, out var reverseRate))
                    continue;

                if (reverseRate == 0m)
                    throw new ValidationException($"Exchange rate from '{rates.Key}' to '{source}' is zero and cannot be reversed.");

                if (directNeighbors != null && directNeighbors.ContainsKey(rates.Key))
                    continue;

                yield return new KeyValuePair<IAsset, decimal>(rates.Key, 1m / reverseRate);
            }
        }

        private void AddRate(IAsset source, IAsset destination, decimal rate)
        {
            if (!_graph.ContainsKey(source))
                _graph[source] = new Dictionary<IAsset, decimal>();

            _graph[source][destination] = rate;
        }
    }
}
