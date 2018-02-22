using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    internal class EntryItemCollection : ItemCollection<IEntryItem>, IEntryItemCollection
    {
        public EntryItemCollection(Entry owner) : base(owner)
        {
        }

        public IEntryItem GetEntryItem(IBook book, IAccount account)
        {
            return FindItems(account).SingleOrDefault(entryItem => entryItem.Book.Equals(book));
        }

        public IEntryItem GetEntryItem(IBook book, IAccount account, IAsset asset)
        {
            return FindItems(account).SingleOrDefault(entryItem => entryItem.Book.Equals(book) && entryItem.Asset.Equals(asset));
        }

        public IEntryItem GetEntryItem(IBook book, IAccountPredicate predicate)
        {
            return FindItems(predicate).SingleOrDefault(entryItem => entryItem.Book.Equals(book));
        }

        public ICollection<IEntryItem> GetEntryItems(IAccountPredicate predicate)
        {
            return FindItems(predicate);
        }

        public ICollection<IEntryItem> GetEntryItems(IBook book, IAccountPredicate predicate)
        {
            return FindItems(predicate).Where(entryItem => entryItem.Book.Equals(book)).ToList();
        }

        public IEntryItem GetEntryItemCombined(IBook book, IAccountPredicate predicate)
        {
            return GetEntryItemsCombined(book, predicate).SingleOrDefault();
        }

        public IEntryItem GetEntryItemCombined(IBook book, IAccountPredicate predicate, IAsset asset)
        {
            return GetEntryItemsCombined(book, predicate).SingleOrDefault(x => x.Asset.Equals(asset));
        }

        public ICollection<IEntryItem> GetEntryItemsCombined(IBook book, IAccountPredicate predicate)
        {
            return FindItems(predicate).Where(entryItem => entryItem.Book.Equals(book)).GroupBy(x => x.Asset).Select(grp =>
            {
                return (IEntryItem)new EntryItem
                {
                    Entry = null, // TODO
                    Book = book,
                    Account = null, // TODO
                    Asset = grp.Key,
                    Debit = grp.Sum(x => x.Debit),
                    Credit = grp.Sum(x => x.Credit)
                };
            }).ToList();
        }
    }
}
