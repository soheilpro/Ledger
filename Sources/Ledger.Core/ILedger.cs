using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface ILedger
    {
        ICollection<IEntryValidator> EntryValidators
        {
            get;
        }

        ICollection<IBalanceValidator> BalanceValidators
        {
        	get;
        }

        IEnumerable<IBalance> AddEntry(IEntry entry);

        IEntry GetLastEntry();

        IEntry GetEntryAt(IComparable index);

        IEntry GetEntryBefore(IComparable index);

        IEntry GetEntryAtOrBefore(IComparable index);

        IEnumerable<IEntry> GetEntriesBefore(IComparable index);

        IEnumerable<IEntry> GetEntriesAtOrBefore(IComparable index);

        IEntry GetEntryAfter(IComparable index);

        IEntry GetEntryAtOrAfter(IComparable index);

        IEnumerable<IEntry> GetEntriesAfter(IComparable index);

        IEnumerable<IEntry> GetEntriesAtOrAfter(IComparable index);

        IEnumerable<IEntry> GetEntriesBetween(IComparable startIndex, IComparable endIndex);

        IEnumerable<IEntry> GetEntriesAtOrBetween(IComparable startIndex, IComparable endIndex);

        IEnumerable<IEntry> GetEntries();

        IEnumerable<IEntryItem> GetEntryItemsBetween(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, IComparable endIndex);

        IEnumerable<IEntryItem> GetEntryItemsAtOrBetween(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, IComparable endIndex);

        IBalance GetLastBalance(IBook book);

        IBalance GetBalanceAt(IBook book, IComparable index);

        IBalance GetBalanceBefore(IBook book, IComparable index);

        IBalance GetBalanceBefore(IBook book, IComparable index, IComparable maxIndex);

        IBalance GetBalanceAtOrBefore(IBook book, IComparable index);

        IBalance GetBalanceAtOrBefore(IBook book, IComparable index, IComparable maxIndex);

        IEnumerable<IBalance> GetBalancesBefore(IBook book, IComparable index);

        IEnumerable<IBalance> GetBalancesBefore(IBook book, IComparable index, IComparable maxIndex);

        IEnumerable<IBalance> GetBalancesAtOrBefore(IBook book, IComparable index);

        IEnumerable<IBalance> GetBalancesAtOrBefore(IBook book, IComparable index, IComparable maxIndex);

        IBalance GetBalanceAfter(IBook book, IComparable index);

        IBalance GetBalanceAfter(IBook book, IComparable index, IComparable maxIndex);

        IBalance GetBalanceAtOrAfter(IBook book, IComparable index);

        IBalance GetBalanceAtOrAfter(IBook book, IComparable index, IComparable maxIndex);

        IEnumerable<IBalance> GetBalancesAfter(IBook book, IComparable index);

        IEnumerable<IBalance> GetBalancesAfter(IBook book, IComparable index, IComparable maxIndex);

        IEnumerable<IBalance> GetBalancesAtOrAfter(IBook book, IComparable index);

        IEnumerable<IBalance> GetBalancesAtOrAfter(IBook book, IComparable index, IComparable maxIndex);

        IEnumerable<IBalance> GetBalances();
    }
}
