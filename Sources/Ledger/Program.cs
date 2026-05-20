using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Ledger.Rates;
using Ledger.Repl;
using Ledger.Tui;

namespace Ledger
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new Parser(configuration => configuration.HelpWriter = null);

            parser.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed((errors) => Console.WriteLine($"Usage: ledger <journal> [<journal> ...] [--rates <path>] [--tui]"));
        }

        private static void Run(Options options)
        {
            var journalManager = new JournalManager(options.JournalPaths.ToArray());
            var ratesManager = new RatesManager(options.RatesPath);

            var context = new Repl.Context()
            {
                JournalManager = journalManager,
                RatesManager = ratesManager
            };

            var context2 = new Tui.Context()
            {
                JournalManager = journalManager,
                RatesPath = options.RatesPath
            };

            IController controller = options.UseTui ? new TuiController(context2) : new ReplController(context);

            controller.Run();
        }

        private class Options
        {
            [Value(0, MetaName = "JournalPaths", Required = true, HelpText = "Path to one or more journal files.")]
            public IEnumerable<string> JournalPaths
            {
                get;
                set;
            }

            [Option("rates", HelpText = "Path to rates file.")]
            public string RatesPath
            {
                get;
                set;
            }

            [Option("tui", HelpText = "Launch the terminal UI instead of the REPL.")]
            public bool UseTui
            {
                get;
                set;
            }
        }
    }
}
