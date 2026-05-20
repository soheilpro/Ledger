using System;
using System.Linq;
using CommandLine;
using Ledger.Repl.Drawing;
using Ledger.Reports;

namespace Ledger.Repl.Commands
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
                return [
                    "e",
                ];
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

        public PrintEntryItemsCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, ["start", "end", "asset", "no-children"]);

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
            table.Columns.Add(new TableTextColumn<EntryItemsReportItem>("Index", row => row.Index));
            table.Columns.Add(new TableAccountColumn<EntryItemsReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<EntryItemsReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<EntryItemsReportItem>("Debit", row => row.Debit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReportItem>("Credit", row => row.Credit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<EntryItemsReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Rows = report.Items;

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
