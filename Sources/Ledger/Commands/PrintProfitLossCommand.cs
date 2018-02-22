using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Reports;

namespace Ledger.Commands
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
                return new[] {
                    "pl",
                };
            }
        }

        public override string Arguments
        {
            get
            {
                return "[account] [--start index] [--end index] [--asset asset]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print profit-loss.";
            }
        }

        public PrintProfitLossCommand(IController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggesions(arg, index, context, new string[] { "start", "end", "asset" });

            if (arg.StartsWith("@"))
                return GetMarkSuggesions(arg, index, context);

            if (index == 1)
                return GetAccountSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintProfitLossOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new ProfitLossReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.AccountQuery = options.AccountQuery;
            reportBuilder.StartIndex = ResolveIndex(options.StartIndex, context, false);
            reportBuilder.EndIndex = ResolveIndex(options.EndIndex, context, false) ?? context.JournalManager.Journal.Entries.LastOrDefault().Index?.ToString();
            reportBuilder.AssetQuery = options.AssetQuery;

            var report = reportBuilder.GetReport();

            report.Print(Console.Out);
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
    }
}
