using System;
using System.IO;
using Ledger.Core;
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
