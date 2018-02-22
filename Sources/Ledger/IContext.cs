using System;
using Ledger.Core;
using Ledger.Journal;

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
