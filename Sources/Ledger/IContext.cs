using System;

namespace Ledger
{
    internal interface IContext
    {
        IJournalManager JournalManager
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
