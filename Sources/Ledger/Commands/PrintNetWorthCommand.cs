using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Reports;

namespace Ledger.Commands
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

        public override string Arguments
        {
            get
            {
                return "ASSET [--at index]";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Print net worth.";
            }
        }

        public PrintNetWorthCommand(IController controller) : base(controller)
        {
        }

        public override string[] GetSuggestions(string arg, int index, IContext context)
        {
            if (arg.StartsWith("--"))
                return GetOptionSuggesions(arg, index, context, new string[] { "at" });

            if (arg.StartsWith("@"))
                return GetMarkSuggesions(arg, index, context);

            if (index == 1)
                return context.JournalManager.Journal.Entries.SelectMany(entry => entry.Items).Select(entryItem => entryItem.Asset.ToString()).Distinct().OrderBy(asset => asset).Where(asset => asset.StartsWith(arg, StringComparison.OrdinalIgnoreCase)).ToArray();

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintNetWorthOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new NetWorthReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.Index = ResolveIndex(options.Index, context, false);
            reportBuilder.Asset = new Asset(options.AssetId);

            var report = reportBuilder.GetReport();

            report.Print(Console.Out);
        }
    }

    internal class PrintNetWorthOptions
    {
        [Value(0, Required = true, MetaName = "ASSET")]
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
