using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class NetWorthReportBuilder : ReportBuilderBase
    {
        public IJournal Journal
        {
            get;
            set;
        }

        public string Book
        {
            get;
            set;
        }

        public string Index
        {
            get;
            set;
        }

        public IAsset Asset
        {
            get;
            set;
        }

        public string ExchangeRatesPath
        {
            get;
            set;
        }

        public NetWorthReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);
            var sourceAssets = balance.Items.GetBalanceItemsCombined(new QueryAccountPredicate("Equity:Capital:**")).Select(balanceItem => balanceItem.Asset).Distinct().ToArray();
            var exchangeRates = ExchangeRates.Load(ExchangeRatesPath);
            var netWorth = 0m;

            foreach (var sourceAsset in sourceAssets)
            {
                var assets = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Assets:**"), sourceAsset).BalanceDebitOrDefault();
                var liabilities = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Liabilities:**"), sourceAsset).BalanceDebitOrDefault();
                var amount = assets + liabilities;

                if (sourceAsset.Equals(Asset))
                {
                    netWorth += amount;
                    continue;
                }

                netWorth += amount * exchangeRates.GetRate(sourceAsset, Asset);
            }

            return new NetWorthReport(new NetWorthReport.ReportItem
            {
                Asset = Asset,
                NetWorth = netWorth,
            });
        }

        private class ExchangeRates
        {
            private readonly Dictionary<IAsset, Dictionary<IAsset, decimal>> _graph = new Dictionary<IAsset, Dictionary<IAsset, decimal>>();

            public static ExchangeRates Load(string path)
            {
                var result = new ExchangeRates();

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

            public decimal GetRate(IAsset source, IAsset destination)
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
}
