using System;
using System.Linq;
using CommandLine;
using Ledger.Reports;

namespace Ledger.Commands
{
    internal class PrintEntryItemsCommand : CommandBase<PrintEntryItemsOptions>
    {
        public override string Name
        {
            get
            {
                return "entry-items";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return new[] {
                    "e",
                };
            }
        }

        public override string Arguments
        {
            get
            {
                return "[account] [--start index] [--end index] [--asset asset] [--no-children]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print entry items.";
            }
        }

        public PrintEntryItemsCommand(IController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, new string[] { "start", "end", "asset", "no-children" });

            if (arg.StartsWith("@"))
                return GetMarkSuggestions(arg, index, context);

            if (index == 1)
                return GetAccountSuggestions(arg, index, context);

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintEntryItemsOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new EntryItemsReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.AccountQuery = options.AccountQuery;
            reportBuilder.StartIndex = ResolveIndex(options.StartIndex, context, true);
            reportBuilder.EndIndex = ResolveIndex(options.EndIndex, context, false) ?? context.JournalManager.Journal.Entries.LastOrDefault().Index.ToString();
            reportBuilder.AssetQuery = options.AssetQuery;
            reportBuilder.NoChildren = options.NoChildren;

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(EntryItemsReport report)
        {
            var table = new Table();
            table.Columns.Add(new TableTextColumn<EntryItemsReport.ReportItem>("Index", row => row.Index));
            table.Columns.Add(new TableAccountColumn<EntryItemsReport.ReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<EntryItemsReport.ReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<EntryItemsReport.ReportItem>("Debit", row => row.Debit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReport.ReportItem>("Credit", row => row.Credit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReport.ReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReport.ReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Rows = report.ReportItems;

            table.PrintText(Console.Out);
        }
    }

    internal class PrintEntryItemsOptions
    {
        [Value(0, Default = "**")]
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

        [Option("no-children")]
        public bool NoChildren
        {
            get;
            set;
        }
    }
}
