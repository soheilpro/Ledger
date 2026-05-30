using System;
using System.Linq;
using CommandLine;
using Ledger.Repl.Drawing;
using Ledger.Reports;

namespace Ledger.Repl.Commands
{
    internal class PrintProfitLossCommand : CommandBase<PrintProfitLossOptions>
    {
        public override string Name
        {
            get
            {
                return "profitloss";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return [
                    "pl",
                ];
            }
        }

        public override string Arguments
        {
            get
            {
                return "[account] [--start index] [--end index] [--asset asset] [--as asset]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print profit-loss.";
            }
        }

        public PrintProfitLossCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, ["start", "end", "asset", "as"]);

            if (arg.StartsWith("@"))
                return GetMarkSuggestions(arg, index, context);

            if (index == 1)
                return GetAccountSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintProfitLossOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            if (!string.IsNullOrEmpty(options.TargetAsset))
                context.RatesManager.ReloadRates();

            var reportBuilder = new ProfitLossReportBuilder()
            {
                Journal = context.JournalManager.Journal,
                RateProvider = context.RatesManager.RateProvider,
                Book = "default",
                AccountQuery = options.AccountQuery,
                StartIndex = ResolveIndex(options.StartIndex, context, false),
                EndIndex = ResolveIndex(options.EndIndex, context, false) ?? context.JournalManager.Journal.Entries.LastOrDefault().Index?.ToString(),
                AssetQuery = options.AssetQuery,
                TargetAsset = options.TargetAsset
            };

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(ProfitLossReport report)
        {
            var table = new Table();
            table.Columns.Add(new TableAccountColumn<ProfitLossReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<ProfitLossReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<ProfitLossReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<ProfitLossReportItem>("%", row => row.BalanceDebitPercent));
            table.Columns.Add(new TableAmountColumn<ProfitLossReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Columns.Add(new TableAmountColumn<ProfitLossReportItem>("%", row => row.BalanceCreditPercent));
            table.Rows = report.Items;

            table.PrintText(Console.Out);
        }
    }

    internal class PrintProfitLossOptions
    {
        [Value(0, Default = "Equity:ProfitLoss")]
        public string AccountQuery
        {
            get;
            set;
        }

        [Option("start")]
        public string StartIndex
        {
            get;
            set;
        }

        [Option("end")]
        public string EndIndex
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
    }
}
