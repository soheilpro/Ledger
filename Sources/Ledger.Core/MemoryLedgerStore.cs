using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class MemoryLedgerStore : ILedgerStore
    {
        private IList<IEntry> _entries = new List<IEntry>();
        private IList<IEntryItem> _entryItems = new List<IEntryItem>();
        private IDictionary<IBook, IList<IBalance>> _balancesMap = new Dictionary<IBook, IList<IBalance>>();

        public virtual void Store(IEntry entry, ICollection<IBalance> balances)
        {
            _entries.Add(entry);

            foreach (var entryItem in entry.Items)
                _entryItems.Add(entryItem);

            foreach (var balance in balances)
            {
                IList<IBalance> balancesList;

                if (!_balancesMap.TryGetValue(balance.Book, out balancesList))
                {
                    balancesList = new List<IBalance>();
                    _balancesMap[balance.Book] = balancesList;
                }

                balancesList.Add(balance);
            }
        }

        public IComparable GetFirstIndex()
        {
            if (_entries.Count == 0)
                return null;

            return _entries[0].Index;
        }

        public IComparable GetLastIndex()
        {
            if (_entries.Count == 0)
                return null;

            return _entries[_entries.Count - 1].Index;
        }

        public virtual IEntry GetLastEntry()
        {
            if (_entries.Count == 0)
                return null;

            return _entries[_entries.Count - 1];
        }

        public virtual ICollection<IEntry> GetEntries(ICollection<IComparable> indexes)
        {
            return _entries.Where(entry => indexes.Any(index => entry.Index.CompareTo(index) == 0)).ToList();
        }

        public virtual ICollection<IEntry> GetEntries(IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, bool reverse = false, int count = int.MaxValue)
        {
            if (reverse)
                return GetRangeReverse(_entries, endIndex, endInclusive, startIndex, startInclusive, count);

            return GetRange(_entries, startIndex, startInclusive, endIndex, endInclusive, count);
        }

        public virtual ICollection<IEntry> GetEntries()
        {
            return _entries;
        }

        public ICollection<IEntryItem> GetEntryItems(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive)
        {
            var result = _entryItems.Where(x => x.Book.Equals(book));

            if (startIndex != null)
            {
                if (startInclusive)
                    result = result.Where(x => x.Entry.Index.CompareTo(startIndex) >= 0);
                else
                    result = result.Where(x => x.Entry.Index.CompareTo(startIndex) > 0);
            }

            if (endIndex != null)
            {
                if (endInclusive)
                    result = result.Where(x => x.Entry.Index.CompareTo(endIndex) <= 0);
                else
                    result = result.Where(x => x.Entry.Index.CompareTo(endIndex) < 0);
            }

            return result.Where(x => accountPredicate.Matches(x.Account)).ToList();
        }

        public virtual IBalance GetLastBalance(IBook book)
        {
            var balances = GetBalances(book);

            if (balances.Count == 0)
                return null;

            return balances[balances.Count - 1];
        }

        public virtual ICollection<IBalance> GetBalances(IBook book, IEnumerable<IComparable> indexes)
        {
            return GetBalances(book).Where(balance => indexes.Any(index => balance.Index.CompareTo(index) == 0)).ToList();
        }

        public virtual ICollection<IBalance> GetBalances(IBook book, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, bool reverse = false, int count = int.MaxValue)
        {
            var balances = GetBalances(book);

            if (reverse)
                return GetRangeReverse(balances, endIndex, endInclusive, startIndex, startInclusive, count);

            return GetRange(balances, startIndex, startInclusive, endIndex, endInclusive, count);
        }

        public ICollection<IBalance> GetBalances()
        {
            // Since this method is never used in calculations, I don't think the original order of combined balances matter
            return _balancesMap.Values.SelectMany(balances => balances).ToList();
        }

        private IList<IBalance> GetBalances(IBook book)
        {
            IList<IBalance> balances;

            if (!_balancesMap.TryGetValue(book, out balances))
                return new List<IBalance>();

            return balances;
        }

        private static ICollection<T> GetRange<T>(IList<T> indexables, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, int count) where T : IIndexable
        {
            if (indexables.Count == 0)
                return new List<T>();

            var start = 0;

            if (startIndex != null)
            {
                var newStart = FindIndex(indexables, startIndex, startInclusive, 1);

                if (newStart == -1)
                    return new List<T>();

                start = newStart;
            }

            var end = indexables.Count - 1;

            if (endIndex != null)
            {
                var newEnd = FindIndex(indexables, endIndex, endInclusive, -1);

                if (newEnd == -1)
                    return new List<T>();

                end = newEnd;
            }

            if (end - start + 1 > count)
                end = start + count - 1;

            var result = new List<T>();

            for (var i = start; i <= end; i++)
                result.Add(indexables[i]);

            return result;
        }

        private static ICollection<T> GetRangeReverse<T>(IList<T> indexables, IComparable startIndex, bool startInclusive, IComparable endIndex, bool endInclusive, int count) where T : IIndexable
        {
            if (indexables.Count == 0)
                return new List<T>();

            var start = indexables.Count - 1;

            if (startIndex != null)
            {
                var newStart = FindIndex(indexables, startIndex, startInclusive, -1);

                if (newStart == -1)
                    return new List<T>();

                start = newStart;
            }

            var end = 0;

            if (endIndex != null)
            {
                var newEnd = FindIndex(indexables, endIndex, endInclusive, 1);

                if (newEnd == -1)
                    return new List<T>();

                end = newEnd;
            }

            if (start - end + 1 > count)
                end = start - count + 1;

            var result = new List<T>();

            for (var i = start; i >= end; i--)
                result.Add(indexables[i]);

            return result;
        }

        private static int FindIndex<T>(IList<T> indexables, IComparable index, bool inclusive, int direction) where T : IIndexable
        {
            // Uses binary search to find an item or its left or right neighbouring item based on the direction parameter

            var left = 0;
            var right = indexables.Count - 1;

            while (true)
            {
                var middle = (left + right) / 2;

                if (left - right == 1)
                {
                    var findIndex = -1;

                    switch (direction)
                    {
                        case -1:
                            findIndex = right;
                            break;

                        case 1:
                            findIndex = left;
                            break;
                    }

                    if (findIndex > indexables.Count - 1 || findIndex < -1)
                        findIndex = -1;

                    return findIndex;
                }

                var result = index.CompareTo(indexables[middle].Index);

                if (result == 0)
                {
                    if (inclusive)
                        return middle;

                    if (direction == 0)
                        return -1;

                    var findIndex = middle + direction;

                    if (findIndex > indexables.Count - 1)
                        return -1;

                    return findIndex;
                }

                if (result < 0)
                    right = middle - 1;
                else
                    left = middle + 1;
            }
        }
    }
}
