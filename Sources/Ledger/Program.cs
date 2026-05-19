using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace Ledger
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new CommandLine.Parser(configuration => configuration.HelpWriter = null);

            parser.ParseArguments<Options>(args)
                .WithParsed<Options>(Run)
                .WithNotParsed((errors) => Console.WriteLine($"Usage: ledger <journal> [<journal> ...] [--rates <path>]"));
        }

        private static void Run(Options options)
        {
            var journalManager = new JournalManager(options.JournalPaths.ToArray());

            var context = new Context();
            context.JournalManager = journalManager;
            context.RatesPath = options.RatesPath;

            var controller = new Controller(context);

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
        }
    }
}
