using System;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger
{
    internal class JournalManager : IJournalManager
    {
        private string _journalPath;
        private IJournal _journal;
        private string[] _accountIds;
        private string[] _markNames;

        public IJournal Journal
        {
            get
            {
                if (_journal == null)
                    ReloadJournal();

                return _journal;
            }
        }

        public string[] AccountIds
        {
            get
            {
                if (_journal == null)
                    ReloadJournal();

                return _accountIds;
            }
        }

        public string[] MarkNames
        {
            get
            {
                if (_journal == null)
                    ReloadJournal();

                return _markNames;
            }
        }

        public JournalManager(string journalPath)
        {
            _journalPath = journalPath;
        }

        public void ReloadJournal()
        {
            _journal = LoadJournal();
            _accountIds = _journal.Entries.SelectMany(entry => entry.Items).Select(entryItem => entryItem.Account.ToString()).Distinct().OrderBy(account => account).ToArray();
            _markNames = _journal.Marks.Select(mark => mark.Name).OrderBy(name => name).ToArray();
        }

        private IJournal LoadJournal()
        {
            var journalReader = new JournalReader();

            return journalReader.Open(_journalPath);
        }
    }
}
