using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public class LedgerMerger
    {
        public virtual ILedger MergeLedgers(IEnumerable<ILedger> ledgers, IComparable endIndex)
        {
            var entries = new List<IEntry>();

            foreach (var ledger in ledgers)
                entries.AddRange(ledger.GetEntriesBefore(endIndex));

            entries.Sort(new EntryComparer());

            var mergedLedger = new Ledger();

            foreach (var entry in entries)
                mergedLedger.AddEntry(entry);

            return mergedLedger;
        }

        private class EntryComparer : IComparer<IEntry>
        {
            public int Compare(IEntry x, IEntry y)
            {
                return x.Index.CompareTo(y.Index);
            }
        }
    }
}
