using System;
using System.Linq;
using CommandLine;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Commands
{
    internal abstract class CommandBase<TOptions> : ICommand
    {
        public IController Controller
        {
            get;
        }

        public abstract string Name {
            get;
        }

        public virtual string[] Aliases
        {
            get
            {
                return new string[0];
            }
        }

        public virtual string Arguments {
            get
            {
                return null;
            }
        }

        public abstract string HelpText {
            get;
        }

        public CommandBase(IController controller)
        {
            Controller = controller;
        }

        public virtual string[] GetSuggestions(string arg, int index, IContext context)
        {
            return null;
        }

        protected string[] GetOptionSuggesions(string arg, int index, IContext context, string[] options)
        {
            var name = arg.Substring(2);

            return options.Where(option => option.StartsWith(name, StringComparison.OrdinalIgnoreCase)).Select(markName => "--" + markName).ToArray();
        }

        protected string[] GetAccountSuggestions(string arg, int index, IContext context)
        {
            var accountPartsLength = arg.Split(':').Length;

            return context.JournalManager.AccountIds.Where(accountId => accountId.StartsWith(arg, StringComparison.OrdinalIgnoreCase)).Select(account => string.Join(':', account.Split(':').Take(accountPartsLength))).Distinct().ToArray();
        }

        protected string[] GetMarkSuggesions(string arg, int index, IContext context)
        {
            var name = arg.Substring(1);

            return context.JournalManager.MarkNames.Where(markName => markName.StartsWith(name, StringComparison.OrdinalIgnoreCase)).Select(markName => "@" + markName).ToArray();
        }

        public void Execute(string[] args, IContext context)
        {
            var parser = new CommandLine.Parser(configuration => configuration.HelpWriter = null);

            parser.ParseArguments<TOptions>(args)
                .WithParsed<TOptions>(options => Execute(options, context))
                .WithNotParsed(errors =>
                {
                    var error = errors.First();

                    if (error is CommandLine.MissingRequiredOptionError)
                        ConsoleHelper.PrintError($"Missing required argument.");
                    else if (error is CommandLine.UnknownOptionError e)
                        ConsoleHelper.PrintError($"Unknown argument: {e.Token}");
                    else
                        ConsoleHelper.PrintError(error.ToString());
                });
        }

        protected abstract void Execute(TOptions options, IContext context);

        protected string ResolveIndex(string index, IContext context, bool forward)
        {
            if (index == null)
                return null;

            if (index.StartsWith("@"))
            {
                var name = index.Substring(1);
                var mark = context.JournalManager.Journal.Marks.SingleOrDefault(m => m.Name == name);

                if (mark == null)
                    throw new CommandException($"Invalid mark: {name}");

                return forward ? mark.NextEntry?.Index?.ToString() : mark.PreviousEntry?.Index.ToString();
            }

            return index;
        }
    }
}
