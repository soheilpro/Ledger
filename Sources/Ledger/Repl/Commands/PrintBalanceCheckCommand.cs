using System;
using CommandLine;
using Ledger.Repl.Drawing;
using Ledger.Reports;

namespace Ledger.Repl.Commands
{
    internal class PrintBalanceCheckCommand : CommandBase<PrintBalanceCheckOptions>
    {
        public override string Name
        {
            get
            {
                return "balance-check";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return new[] {
                    "bc",
                };
            }
        }

        public override string Arguments
        {
            get
            {
                return "[--at index] [--asset asset]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Check balances.";
            }
        }

        public PrintBalanceCheckCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, new string[] { "at", "asset" });

            if (arg.StartsWith("@"))
                return GetMarkSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintBalanceCheckOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new BalanceCheckReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.Index = ResolveIndex(options.Index, context, false);
            reportBuilder.AssetQuery = options.AssetQuery;

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(BalanceCheckReport report)
        {
            var table = new Table();
            table.Columns.Add(new TableAssetColumn<BalanceCheckReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<BalanceCheckReportItem>("Assets", row => row.Assets));
            table.Columns.Add(new TableAmountColumn<BalanceCheckReportItem>("Liabilities + Equity", row => row.LiabilitiesAndEquity));
            table.Columns.Add(new TableAmountColumn<BalanceCheckReportItem>("Liabilities", row => row.Liabilities));
            table.Columns.Add(new TableAmountColumn<BalanceCheckReportItem>("Equity", row => row.Equity));
            table.Columns.Add(new TableAmountColumn<BalanceCheckReportItem>("Diff", row => row.Diff));
            table.Rows = report.Items;

            table.PrintText(Console.Out);
        }
    }

    internal class PrintBalanceCheckOptions
    {
        [Option("at")]
        public string Index
        {
            get;
            set;
        }

        [Option("asset")]
        public string AssetQuery
        {
            get;
            set;
        }
    }
}
