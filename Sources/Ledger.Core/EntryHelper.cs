using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public static class EntryHelper
    {
        public static void Copy(this IEntry sourceEntry, IEntry destinationEntry, IAccountMap accountMap, bool closingOpeningEntryType = false)
        {
            // Performance.MarkStart("EntryHelper.Copy");

            Validate(sourceEntry, accountMap);

            foreach (var accountMapItem in accountMap.Items)
            {
                if (accountMapItem.Account == null)
                    continue;

                var items = sourceEntry.Items.Where(x => x.Book.Equals(accountMapItem.Book) && accountMapItem.AccountPredicate.Matches(x.Account)).ToList();

                if (!items.Any())
                    continue;

                var entryItem = new EntryItem();
                entryItem.Entry = destinationEntry;
                entryItem.Account = accountMapItem.Account;
                entryItem.Book = accountMapItem.Book;
                entryItem.Asset = items.Select(x => x.Asset).Distinct().Single();
                entryItem.Debit = items.Sum(x => x.Debit);
                entryItem.Credit = items.Sum(x => x.Credit);

                if (entryItem.Debit != entryItem.Credit && entryItem.Debit != 0 && entryItem.Credit != 0 && closingOpeningEntryType)
                {
                    var minValue = entryItem.Debit < entryItem.Credit ? entryItem.Debit : entryItem.Credit;
                    entryItem.Debit -= minValue;
                    entryItem.Credit -= minValue;
                }

                destinationEntry.Items.Add(entryItem);
            }

            // Performance.MarkEnd("EntryHelper.Copy");
        }

        public static void CopyByBook(this IEntry sourceEntry, IEntry destinationEntry, IBook book, IAccountMap accountMap, bool closingOpeningEntryType = false)
        {
            // Performance.MarkStart("EntryHelper.Copy");

            Validate(sourceEntry, accountMap);

            foreach (var accountMapItem in accountMap.Items)
            {
                if (accountMapItem.Account == null)
                    continue;

                var items = sourceEntry.Items.Where(x => x.Book.Equals(book) && accountMapItem.AccountPredicate.Matches(x.Account)).ToList();

                if (!items.Any())
                    continue;

                var entryItem = new EntryItem();
                entryItem.Entry = destinationEntry;
                entryItem.Account = accountMapItem.Account;
                entryItem.Book = book;
                entryItem.Asset = items.Select(x => x.Asset).Distinct().Single();
                entryItem.Debit = items.Sum(x => x.Debit);
                entryItem.Credit = items.Sum(x => x.Credit);

                if (entryItem.Debit != entryItem.Credit && entryItem.Debit != 0 && entryItem.Credit != 0 && closingOpeningEntryType)
                {
                    var minValue = entryItem.Debit < entryItem.Credit ? entryItem.Debit : entryItem.Credit;
                    entryItem.Debit -= minValue;
                    entryItem.Credit -= minValue;
                }

                destinationEntry.Items.Add(entryItem);
            }

            // Performance.MarkEnd("EntryHelper.Copy");
        }

        private static void Validate(IEntry sourceEntry, IAccountMap accountMap)
        {
            // Performance.MarkStart("EntryHelper.Validate");

            var matches = new Dictionary<IAccount, (IAccount Target, IAccountPredicate Predicate)>();

            var entryItemAccountGroups = sourceEntry.Items.GroupBy(item => new {item.Account, item.Book}).ToList();
            foreach (var entryItemAccountGroup in entryItemAccountGroups)
            {
                foreach (var accountMapItem in accountMap.Items)
                {
                    if (!accountMapItem.Book.Equals(entryItemAccountGroup.First().Book))
                        continue;

                    var account = entryItemAccountGroup.First().Account;
                    if (!accountMapItem.AccountPredicate.Matches(account))
                        continue;

                    if (accountMapItem.Account == null)
                    {
                        matches[account] = (null, accountMapItem.AccountPredicate);
                        continue;
                    }

                    (IAccount Target, IAccountPredicate Predicate) existingMatch;

                    if (matches.TryGetValue(account, out existingMatch) && existingMatch.Target != null)
                        throw new ValidationException(string.Format("Duplicate selected account:{0}{1}{0}{2} -> {3}{0}{4} -> {5}", Environment.NewLine, account, existingMatch.Predicate, existingMatch.Target, accountMapItem.AccountPredicate, accountMapItem.Account));

                    matches[account] = (accountMapItem.Account, accountMapItem.AccountPredicate);
                }
            }

            if (matches.Count != entryItemAccountGroups.Count)
            {
                var missedAccounts = entryItemAccountGroups.Select(g => g.First().Account).Except(matches.Keys).ToList();

                throw new ValidationException($"Missed accounts:{Environment.NewLine}{string.Join(Environment.NewLine, missedAccounts)}");
            }

            // Performance.MarkEnd("EntryHelper.Validate");
        }

        public static IEnumerable<IEntryItem> GetEntryItems(this IEnumerable<IEntry> entries, IBook book, IAccountPredicate predicate)
        {
            return entries.SelectMany(x => x.Items).Where(x => x.Book.Equals(book) && predicate.Matches(x.Account));
        }
    }
}
