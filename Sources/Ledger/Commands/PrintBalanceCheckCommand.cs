using System;
using CommandLine;
using Ledger.Reports;

namespace Ledger.Commands
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

        public PrintBalanceCheckCommand(IController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggesions(arg, index, context, new string[] { "at", "asset" });

            if (arg.StartsWith("@"))
                return GetMarkSuggesions(arg, index, context);

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

            report.Print(Console.Out);
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
