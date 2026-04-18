using System;
using Ledger.Journal;

namespace Ledger
{
    internal interface IJournalManager
    {
        IJournal Journal
        {
            get;
        }

        string[] AccountIds
        {
            get;
        }

        string[] MarkNames
        {
            get;
        }

        void ReloadJournal();
    }
}
