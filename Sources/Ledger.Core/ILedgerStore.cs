using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface ILedgerStore
    {
        void Store(IEntry entry, ICollection<IBalance> balances);

        IComparable GetFirstIndex();

        IComparable GetLastIndex();

        IEntry GetLastEntry();

        ICollection<IEntry> GetEntries(ICollection<IComparable> indexes);

        ICollection<IEntry> GetEntries(IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, bool reverse = false, int count = int.MaxValue);

        ICollection<IEntry> GetEntries();

        ICollection<IEntryItem> GetEntryItems(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive);

        IBalance GetLastBalance(IBook book);

        ICollection<IBalance> GetBalances(IBook book, IEnumerable<IComparable> indexes);

        ICollection<IBalance> GetBalances(IBook book, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, bool reverse = false, int count = int.MaxValue);

        ICollection<IBalance> GetBalances();
    }
}
