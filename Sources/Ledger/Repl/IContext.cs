using System;

namespace Ledger.Repl
{
    internal interface IContext
    {
        IJournalManager JournalManager
        {
            get;
        }

        IRatesManager RatesManager
        {
            get;
        }

        bool ShouldExit
        {
            get;
            set;
        }
    }
}
