using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Journal;
using Ledger.Repl.Drawing;
using Ledger.Reports;

namespace Ledger.Repl.Commands
{
    internal class PrintNetWorthCommand : CommandBase<PrintNetWorthOptions>
    {
        public override string Name
        {
            get
            {
                return "net-worth";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return [
                    "nw",
                ];
            }
        }

        public override string Arguments
        {
            get
            {
                return "<asset> [--at index]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print net worth.";
            }
        }

        public PrintNetWorthCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, ["at"]);

            if (arg.StartsWith("@"))
                return GetMarkSuggestions(arg, index, context);

            if (index == 1)
                return context.JournalManager.Journal.Entries
                    .SelectMany(entry => entry.Items)
                    .Where(entryItem => new QueryAccountPredicate("Equity:Capital:**").Matches(entryItem.Account))
                    .Select(entryItem => entryItem.Asset.ToString())
                    .Distinct()
                    .OrderBy(asset => asset)
                    .Where(asset => asset.StartsWith(arg, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintNetWorthOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();
            context.RatesManager.ReloadRates();

            var reportBuilder = new NetWorthReportBuilder()
            {
                Journal = context.JournalManager.Journal,
                Book = "default",
                Index = ResolveIndex(options.Index, context, false),
                Asset = new Asset(options.AssetId),
                RateProvider = context.RatesManager.RateProvider
            };

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(NetWorthReport report)
        {
            var table = new Table();
            table.Columns.Add(new TableAssetColumn<NetWorthReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<NetWorthReportItem>("Value", row => row.Value));
            table.Rows = [report.Item];

            table.PrintText(Console.Out);
        }
    }

    internal class PrintNetWorthOptions
    {
        [Value(0, Required = true, MetaName = "asset")]
        public string AssetId
        {
            get;
            set;
        }

        [Option("at")]
        public string Index
        {
            get;
            set;
        }
    }
}
