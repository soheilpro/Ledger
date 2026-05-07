using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Journal;
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

        public override string[] Aliases
        {
            get
            {
                return new[] {
                    "nw",
                };
            }
        }

        public override string Arguments
        {
            get
            {
                return "<asset> [--at index]";
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
                return context.JournalManager.Journal.Entries
                    .SelectMany(entry => entry.Items)
                    .Where(entryItem => new QueryAccountPredicate("Equity:Capital:**").Matches(entryItem.Account))
                    .Select(entryItem => entryItem.Asset.ToString())
                    .Distinct()
                    .OrderBy(asset => asset)
                    .Where(asset => asset.StartsWith(arg, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

            return base.GetSuggestions(arg, index, context);
        }

        protected override void Execute(PrintNetWorthOptions options, IContext context)
        {
            if (string.IsNullOrWhiteSpace(context.RatesPath))
            {
                ConsoleHelper.PrintError("Missing rates file. Start ledger with '--rates <path>'.");
                return;
            }

            context.JournalManager.ReloadJournal();

            var reportBuilder = new NetWorthReportBuilder();
            reportBuilder.Journal = context.JournalManager.Journal;
            reportBuilder.Book = "default";
            reportBuilder.Index = ResolveIndex(options.Index, context, false);
            reportBuilder.Asset = new Asset(options.AssetId);
            reportBuilder.RateProvider = FileRateProvider.Load(context.RatesPath);

            var report = reportBuilder.GetReport();

            report.Print(Console.Out);
        }
    }

    internal class PrintNetWorthOptions
    {
        [Value(0, Required = true, MetaName = "asset")]
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
