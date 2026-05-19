using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class OpeningEntryReport : ReportBase
    {
        public OpeningEntryReport(IEntryItem[] entryItems)
        {
            EntryItems = entryItems;
        }

        public IEntryItem[] EntryItems
        {
            get;
        }
    }
}
