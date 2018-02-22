using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Reports;

namespace Ledger.Commands
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
                return new[] {
                    "b",
                };
            }
        }

        public override string Arguments
        {
            get
            {
                return "[account] [--at index] [--all-levels] [--no-children] [--non-zero]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print balances.";
            }
        }

        public PrintBalancesCommand(IController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggesions(arg, index, context, new string[] { "at", "asset", "all-levels", "no-children", "non-zero" });

            if (arg.StartsWith("@"))
                return GetMarkSuggesions(arg, index, context);

            if (index == 1)
                return GetAccountSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintBalancesOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new BalanceReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.AccountQuery = options.AccountQuery;
            reportBuilder.Index = ResolveIndex(options.Index, context, false);
            reportBuilder.AssetQuery = options.AssetQuery;
            reportBuilder.AllLevels = options.AllLevels;
            reportBuilder.NoChildren = options.NoChildren;
            reportBuilder.NonZeroBalancesOnly = options.NonZeroBalancesOnly;

            var report = reportBuilder.GetReport();

            report.Print(Console.Out);
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

        [Option("all-levels")]
        public bool AllLevels
        {
            get;
            set;
        }

        [Option("no-children")]
        public bool NoChildren
        {
            get;
            set;
        }

        [Option("non-zero")]
        public bool NonZeroBalancesOnly
        {
            get;
            set;
        }
    }
}
