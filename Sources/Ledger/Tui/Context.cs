using System;

namespace Ledger.Tui
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
    }
}
