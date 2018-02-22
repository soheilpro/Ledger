using System;
using Ledger.Core;

namespace Ledger.Journal
{
    public class Mark : IMark
    {
        public string Name
        {
            get;
            set;
        }

        public IEntry PreviousEntry
        {
            get;
            set;
        }

        public IEntry NextEntry
        {
            get;
            set;
        }
    }
}
