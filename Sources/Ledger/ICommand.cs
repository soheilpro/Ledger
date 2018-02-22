using System;

namespace Ledger
{
    internal interface ICommand
    {
        string Name
        {
            get;
        }

        string[] Aliases
        {
            get;
        }

        string Arguments
        {
            get;
        }

        string HelpText
        {
            get;
        }

        string[] GetSuggestions(string arg, int index, IContext context);

        void Execute(string[] args, IContext context);
    }
}
