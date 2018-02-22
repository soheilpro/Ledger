using System;
using System.IO;
using CommandLine;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new CommandLine.Parser(configuration => configuration.HelpWriter = null);

            parser.ParseArguments<Options>(args)
                .WithParsed<Options>(Run)
                .WithNotParsed((errors) => Console.WriteLine($"Usage: ledger <journal>"));
        }

        private static void Run(Options options)
        {
            var journalManager = new JournalManager(options.JournalPath);

            var context = new Context();
            context.JournalManager = journalManager;

            var controller = new Controller(context);

            controller.Run();
        }

        private class Options
        {
            [Value(0, MetaName = "JournalPath", Required = true, HelpText = "Path to the journal file.")]
            public string JournalPath
            {
                get;
                set;
            }
        }
    }
}
