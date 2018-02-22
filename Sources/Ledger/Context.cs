using System;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger
{
    internal class Context : IContext
    {
        public IJournalManager JournalManager
        {
            get;
            set;
        }

        public bool ShouldExit
        {
            get;
            set;
        }
    }
}
