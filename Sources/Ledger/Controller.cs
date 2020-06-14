using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Commands;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger
{
    internal class Controller : IController, IAutoCompleteHandler
    {
        public IContext _context;
        public ICollection<ICommand> _commands;

        public Controller(IContext context)
        {
            _context = context;
            _commands = new ICommand[] {
                new ExitCommand(this),
                new HelpCommand(this),
                new PrintBalancesCommand(this),
                new PrintBalanceCheckCommand(this),
                new PrintEntryItemsCommand(this),
                new PrintProfitLossCommand(this),
                new PrintOpeningEntryCommand(this),
                new ReloadJournalCommand(this),
            };
        }

        public ICollection<ICommand> GetCommands()
        {
            return _commands;
        }

        public void Run()
        {
            ReadLine.AutoCompletionHandler = this;

            var argumentParser = new ArgumentParser();

            while (!_context.ShouldExit)
            {
                Console.WriteLine();

                var line = ReadLine.Read("> ").Trim();

                if (line.Length == 0)
                {
                    line = ReadLine.GetHistory().LastOrDefault();

                    if (line == null)
                        continue;
                }
                else
                {
                    ReadLine.AddHistory(line);
                }

                string[] args;

                try
                {
                    args = argumentParser.Parse(line).ToArray();
                }
                catch (ArgumentParser.UnclosedQuotationMarkException)
                {
                    using (new ColoredConsole(ConsoleColor.Red))
                        Console.WriteLine("Unclosed quotation mark.");

                    continue;
                }

                var command = FindCommand(args[0]);

                if (command == null)
                {
                    using (new ColoredConsole(ConsoleColor.Red))
                        Console.WriteLine("Invalid command.");

                    continue;
                }

                try
                {
                    command.Execute(args.Skip(1).ToArray(), _context);
                }
                catch (CommandException exception)
                {
                    ConsoleHelper.PrintError(exception.Message);
                }
                catch (JournalException exception)
                {
                    ConsoleHelper.PrintError(exception.Message);
                }
                catch (ValidationException exception)
                {
                    ConsoleHelper.PrintError(exception.Message);
                }
            }
        }

        private ICommand FindCommand(string name)
        {
            foreach (var command in _commands)
            {
                if (command.Name == name)
                    return command;

                foreach (var alias in command.Aliases)
                    if (alias == name)
                        return command;
            }

            return null;
        }

        char[] IAutoCompleteHandler.Separators {
            get;
            set;
        } = new char[] { ' ' };

        string[] IAutoCompleteHandler.GetSuggestions(string line, int cursorIndex)
        {
            var args = new ArgumentParser().Parse(line, true);

            if (line.EndsWith(' '))
                args = args.Union(new string[] { string.Empty }).ToArray();

            if (args.Length == 0)
                return null;

            if (args.Length == 1)
                return _commands.Where(cmd => cmd.Name.StartsWith(args[0], StringComparison.OrdinalIgnoreCase)).Select(cmd => cmd.Name).ToArray();

            var command = FindCommand(args[0]);

            if (command == null)
                return null;

            var index = args.Length - 1;
            var arg = args[index];
            var suggestions = command.GetSuggestions(arg, index, _context);

            if (suggestions != null)
                suggestions = suggestions.Select(suggestion => suggestion.Contains(" ") ? $"\"{suggestion}\"" : suggestion).ToArray();

            return suggestions;
        }
    }
}
