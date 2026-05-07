using System;

namespace Ledger
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

        bool ShouldExit
        {
            get;
            set;
        }
    }
}
