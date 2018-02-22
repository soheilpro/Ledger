using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IEntryItemCollection : IEnumerable<IEntryItem>
    {
        void Add(IEntryItem entryItem);

        IEntryItem GetEntryItem(IBook book, IAccount account);

        IEntryItem GetEntryItem(IBook book, IAccount account, IAsset asset);

        IEntryItem GetEntryItem(IBook book, IAccountPredicate predicate);

        ICollection<IEntryItem> GetEntryItems(IAccountPredicate predicate);

        ICollection<IEntryItem> GetEntryItems(IBook book, IAccountPredicate predicate);

        IEntryItem GetEntryItemCombined(IBook book, IAccountPredicate predicate);

        IEntryItem GetEntryItemCombined(IBook book, IAccountPredicate predicate, IAsset asset);

        ICollection<IEntryItem> GetEntryItemsCombined(IBook book, IAccountPredicate predicate);
    }
}
