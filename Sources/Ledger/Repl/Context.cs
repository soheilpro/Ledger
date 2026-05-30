using System;

namespace Ledger.Repl
{
    internal class Context : IContext
    {
        public IJournalManager JournalManager
        {
            get;
            set;
        }

        public IRatesManager RatesManager
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
