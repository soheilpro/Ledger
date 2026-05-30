using System;
using CommandLine;
using Ledger.Repl.Drawing;
using Ledger.Reports;

namespace Ledger.Repl.Commands
{
    internal class PrintBalancesCommand : CommandBase<PrintBalancesOptions>
    {
        public override string Name
        {
            get
            {
                return "balances";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return [
                    "b",
                ];
            }
        }

        public override string Arguments
        {
            get
            {
                return "[account] [--at index] [--asset asset] [--as asset] [--zero]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print balances.";
            }
        }

        public PrintBalancesCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, ["at", "asset", "as", "zero"]);

            if (arg.StartsWith("@"))
                return GetMarkSuggestions(arg, index, context);

            if (index == 1)
                return GetAccountSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintBalancesOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            if (!string.IsNullOrEmpty(options.TargetAsset))
                context.RatesManager.ReloadRates();

            var reportBuilder = new BalanceReportBuilder()
            {
                Journal = context.JournalManager.Journal,
                RateProvider = context.RatesManager.RateProvider,
                Book = "default",
                AccountQuery = options.AccountQuery,
                Index = ResolveIndex(options.Index, context, false),
                AssetQuery = options.AssetQuery,
                TargetAsset = options.TargetAsset,
                IncludeZeroBalances = options.IncludeZeroBalances
            };

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(BalanceReport report)
        {
            var table = new Table();
            table.Columns.Add(new TableAccountColumn<BalanceReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<BalanceReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<BalanceReportItem>("Total Debit", row => row.TotalDebit));
            table.Columns.Add(new TableAmountColumn<BalanceReportItem>("Total Credit", row => row.TotalCredit));
            table.Columns.Add(new TableAmountColumn<BalanceReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<BalanceReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Rows = report.Items;

            table.PrintText(Console.Out);
        }
    }

    internal class PrintBalancesOptions
    {
        [Value(0, Default = "**")]
        public string AccountQuery
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

        [Option("asset")]
        public string AssetQuery
        {
            get;
            set;
        }

        [Option("as")]
        public string TargetAsset
        {
            get;
            set;
        }

        [Option("zero")]
        public bool IncludeZeroBalances
        {
            get;
            set;
        }
    }
}
