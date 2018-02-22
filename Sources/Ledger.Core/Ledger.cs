using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class Ledger : ILedger
    {
        private ILedgerStore _store;

        public ICollection<IEntryValidator> EntryValidators
        {
            get;
            set;
        }

        public ICollection<IBalanceValidator> BalanceValidators
        {
        	get;
            set;
        }

        public Ledger() : this(new MemoryLedgerStore())
        {
        }

        public Ledger(ILedgerStore store)
        {
            _store = store;

            EntryValidators = new List<IEntryValidator>();
            BalanceValidators = new List<IBalanceValidator>();
        }

        public IEnumerable<IBalance> AddEntry(IEntry entry)
        {
            // Performance.MarkStart("Ledger.AddEntry");

            // Performance.MarkStart("Ledger.AddEntry:Validate(entry)");

            foreach (var entryValidator in EntryValidators)
                entryValidator.Validate(entry);

            // Performance.MarkEnd("Ledger.AddEntry:Validate(entry)");

            var lastEntry = _store.GetLastEntry();

            if (lastEntry != null && entry.Index.CompareTo(_store.GetLastEntry().Index) <= 0)
                throw new ValidationException($"Entry ({entry}) cannot occur on or before the last entry ({lastEntry}).");

            var balances = new List<IBalance>();

            foreach (var entryItem in entry.Items)
            {
                var balance = balances.SingleOrDefault(x => x.Book.Equals(entryItem.Book));

                if (balance == null)
                {
                    balance = CreateBalance(entryItem.Book, entry.Index);
                    balances.Add(balance);
                }

                var balanceItem = new BalanceItem();
                balanceItem.Balance = balance;
                balanceItem.Account = entryItem.Account;
                balanceItem.Asset = entryItem.Asset;
                balanceItem.TotalDebit = entryItem.Debit;
                balanceItem.TotalCredit = entryItem.Credit;

                var existingBalanceItem = balance.Items.GetBalanceItem(entryItem.Account, entryItem.Asset);

                if (existingBalanceItem != null)
                {
                    balanceItem.TotalDebit += existingBalanceItem.TotalDebit;
                    balanceItem.TotalCredit += existingBalanceItem.TotalCredit;
                }

                balance.Items.AddOrUpdate(balanceItem, existingBalanceItem);
            }

            // Performance.MarkStart("Ledger.AddEntry:Validate(balance)");

            foreach (var balance in balances)
                foreach (var balanceValidator in BalanceValidators)
                    balanceValidator.Validate(balance);

            // Performance.MarkEnd("Ledger.AddEntry:Validate(balance)");

            // Performance.MarkStart("Ledger.AddEntry:Store");

            _store.Store(entry, balances);

            // Performance.MarkEnd("Ledger.AddEntry:Store");

            // Performance.MarkEnd("Ledger.AddEntry");

            return balances;
        }

        public IEntry GetLastEntry()
        {
            // Performance.MarkStart("Ledger.GetLastEntry");

            var result = _store.GetLastEntry();

            // Performance.MarkEnd("Ledger.GetLastEntry");

            return result;
        }

        public IEntry GetEntryAt(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntryAt");

            var result = _store.GetEntries(new[] { index }).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetEntryAt");

            return result;
        }

        public IEntry GetEntryBefore(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntryBefore");

            var result = _store.GetEntries(null, startInclusive: false, endIndex: index, endInclusive: false, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetEntryBefore");

            return result;
        }

        public IEntry GetEntryAtOrBefore(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntryAtOrBefore");

            var result = _store.GetEntries(null, startInclusive: true, endIndex: index, endInclusive: true, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetEntryAtOrBefore");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesBefore(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntriesBefore");

            var result = _store.GetEntries(null, startInclusive: false, endIndex: index, endInclusive: false, reverse: true);

            // Performance.MarkEnd("Ledger.GetEntriesBefore");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesAtOrBefore(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntriesAtOrBefore");

            var result = _store.GetEntries(null, startInclusive: true, endIndex: index, endInclusive: true, reverse: true);

            // Performance.MarkEnd("Ledger.GetEntriesAtOrBefore");

            return result;
        }

        public IEntry GetEntryAfter(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntryAfter");

            var result = _store.GetEntries(index, startInclusive: false, endIndex: null, endInclusive: false, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetEntryAfter");

            return result;
        }

        public IEntry GetEntryAtOrAfter(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntryAtOrAfter");

            var result = _store.GetEntries(index, startInclusive: true, endIndex: null, endInclusive: true, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetEntryAtOrAfter");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesAfter(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntriesAfter");

            var result = _store.GetEntries(index, startInclusive: false, endIndex: null, endInclusive: false, reverse: false);

            // Performance.MarkEnd("Ledger.GetEntriesAfter");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesAtOrAfter(IComparable index)
        {
            // Performance.MarkStart("Ledger.GetEntriesAtOrAfter");

            var result = _store.GetEntries(index, startInclusive: true, endIndex: null, endInclusive: true, reverse: false);

            // Performance.MarkEnd("Ledger.GetEntriesAtOrAfter");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesBetween(IComparable startIndex, IComparable endIndex)
        {
            // Performance.MarkStart("Ledger.GetEntriesBetween");

            var result = _store.GetEntries(startIndex, startInclusive: false, endIndex: endIndex, endInclusive: false);

            // Performance.MarkEnd("Ledger.GetEntriesBetween");

            return result;
        }

        public IEnumerable<IEntry> GetEntriesAtOrBetween(IComparable startIndex, IComparable endIndex)
        {
            // Performance.MarkStart("Ledger.GetEntriesBetween");

            var result = _store.GetEntries(startIndex, startInclusive: true, endIndex: endIndex, endInclusive: true);

            // Performance.MarkEnd("Ledger.GetEntriesAtOrBetween");

            return result;
        }

        public IEnumerable<IEntry> GetEntries()
        {
            // Performance.MarkStart("Ledger.GetEntries");

            var result = _store.GetEntries();

            // Performance.MarkEnd("Ledger.GetEntries");

            return result;
        }

        public IEnumerable<IEntryItem> GetEntryItemsBetween(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, IComparable endIndex)
        {
            return _store.GetEntryItems(book, accountPredicate, startIndex, startInclusive: false, endIndex: endIndex, endInclusive: false);
        }

        public IEnumerable<IEntryItem> GetEntryItemsAtOrBetween(IBook book, IAccountPredicate accountPredicate, IComparable startIndex, IComparable endIndex)
        {
            return _store.GetEntryItems(book, accountPredicate, startIndex, startInclusive: true, endIndex: endIndex, endInclusive: true);
        }

        public IBalance GetLastBalance(IBook book)
        {
            // Performance.MarkStart("Ledger.GetLastBalance");

            var result = _store.GetLastBalance(book);

            // Performance.MarkEnd("Ledger.GetLastBalance");

            return result;
        }

        public IBalance GetBalanceAt(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalanceAt");

            var result = _store.GetBalances(book, new[] { index }).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAt");

            return result;
        }

        public IBalance GetBalanceBefore(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalanceBefore");

            var result = _store.GetBalances(book, null, startInclusive: false, endIndex: index, endInclusive: false, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceBefore");

            return result;
        }

        public IBalance GetBalanceBefore(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalanceBefore");

            var result = _store.GetBalances(book, null, startInclusive: false, endIndex: maxIndex, endInclusive: false, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceBefore");

            return result;
        }

        public IBalance GetBalanceAtOrBefore(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalanceAtOrBefore");

            var result = _store.GetBalances(book, null, startInclusive: true, endIndex: index, endInclusive: true, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAtOrBefore");

            return result;
        }

        public IBalance GetBalanceAtOrBefore(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalanceAtOrBefore");

            var result = _store.GetBalances(book, null, startInclusive: true, endIndex: maxIndex, endInclusive: true, reverse: true, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAtOrBefore");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesBefore(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalancesBefore");

            var result = _store.GetBalances(book, null, startInclusive: false, endIndex: index, endInclusive: false, reverse: true);

            // Performance.MarkEnd("Ledger.GetBalancesBefore");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesBefore(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalancesBefore");

            var result = _store.GetBalances(book, null, startInclusive: false, endIndex: maxIndex, endInclusive: false, reverse: true);

            // Performance.MarkEnd("Ledger.GetBalancesBefore");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAtOrBefore(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalancesAtOrBefore");

            var result = _store.GetBalances(book, null, startInclusive: true, endIndex: index, endInclusive: true, reverse: true);

            // Performance.MarkEnd("Ledger.GetBalancesAtOrBefore");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAtOrBefore(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalancesAtOrBefore");

            var result = _store.GetBalances(book, null, startInclusive: true, endIndex: maxIndex, endInclusive: true, reverse: true);

            // Performance.MarkEnd("Ledger.GetBalancesAtOrBefore");

            return result;
        }

        public IBalance GetBalanceAfter(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalanceAfter");

            var result = _store.GetBalances(book, index, startInclusive: false, endIndex: null, endInclusive: false, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAfter");

            return result;
        }

        public IBalance GetBalanceAfter(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalanceAfter");

            var result = _store.GetBalances(book, index, startInclusive: false, endIndex: maxIndex, endInclusive: false, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAfter");

            return result;
        }

        public IBalance GetBalanceAtOrAfter(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalanceAtOrAfter");

            var result = _store.GetBalances(book, index, startInclusive: true, endIndex: null, endInclusive: true, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAtOrAfter");

            return result;
        }

        public IBalance GetBalanceAtOrAfter(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalanceAtOrAfter");

            var result = _store.GetBalances(book, index, startInclusive: true, endIndex: maxIndex, endInclusive: true, reverse: false, count: 1).SingleOrDefault();

            // Performance.MarkEnd("Ledger.GetBalanceAtOrAfter");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAfter(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalancesAfter");

            var result = _store.GetBalances(book, index, startInclusive: false, endIndex: null, endInclusive: false, reverse: false);

            // Performance.MarkEnd("Ledger.GetBalancesAfter");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAfter(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalancesAfter");

            var result = _store.GetBalances(book, index, startInclusive: false, endIndex: maxIndex, endInclusive: false, reverse: false);

            // Performance.MarkEnd("Ledger.GetBalancesAfter");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAtOrAfter(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.GetBalancesAtOrAfter");

            var result = _store.GetBalances(book, index, startInclusive: true, endIndex: null, endInclusive: true, reverse: false);

            // Performance.MarkEnd("Ledger.GetBalancesAtOrAfter");

            return result;
        }

        public IEnumerable<IBalance> GetBalancesAtOrAfter(IBook book, IComparable index, IComparable maxIndex)
        {
            // Performance.MarkStart("Ledger.GetBalancesAtOrAfter");

            var result = _store.GetBalances(book, index, startInclusive: true, endIndex: maxIndex, endInclusive: true, reverse: false);

            // Performance.MarkEnd("Ledger.GetBalancesAtOrAfter");

            return result;
        }

        public IEnumerable<IBalance> GetBalances()
        {
            // Performance.MarkStart("Ledger.GetBalances");

            var result = _store.GetBalances();

            // Performance.MarkEnd("Ledger.GetBalances");

            return result;
        }

        private Balance CreateBalance(IBook book, IComparable index)
        {
            // Performance.MarkStart("Ledger.CreateBalance");

            var lastBalance = GetLastBalance(book);

            var newBalance = lastBalance != null ? new Balance(lastBalance.Items) : new Balance();
            newBalance.Book = book;
            newBalance.Index = index;

            // Performance.MarkEnd("Ledger.CreateBalance");

            return newBalance;
        }
    }
}
