using System;
using CommandLine;
using Ledger.Reports;

namespace Ledger.Repl.Commands
{
    internal class PrintOpeningEntryCommand : CommandBase<PrintOpeningEntryOptions>
    {
        public override string Name
        {
            get
            {
                return "opening-entry";
            }
        }

        public override string Arguments
        {
            get
            {
                return "[--at index]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print opening entry.";
            }
        }

        public PrintOpeningEntryCommand(IReplController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggestions(arg, index, context, new string[] { "at" });

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintOpeningEntryOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new OpeningEntryReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.Index = ResolveIndex(options.Index, context, false);

            var report = reportBuilder.GetReport();

            Print(report);
        }

        private static void Print(OpeningEntryReport report)
        {
            Console.Out.WriteLine("@entry ");
            Console.Out.WriteLine("@opening");

            foreach (var entryItem in report.EntryItems)
                Console.Out.WriteLine($"{entryItem.Account} {entryItem.Asset} {entryItem.Debit - entryItem.Credit}");
        }
    }

    internal class PrintOpeningEntryOptions
    {
        [Option("at")]
        public string Index
        {
            get;
            set;
        }
    }
}
