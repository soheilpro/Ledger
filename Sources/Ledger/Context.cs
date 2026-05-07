using System;

namespace Ledger
{
    internal class Context : IContext
    {
        public IJournalManager JournalManager
        {
            get;
            set;
        }

        public string RatesPath
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
