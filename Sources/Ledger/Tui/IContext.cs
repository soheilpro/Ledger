using System;

namespace Ledger.Tui
{
    internal interface IContext
    {
        IJournalManager JournalManager
        {
            get;
        }

        string RatesPath
        {
            get;
            set;
        }
    }
}
