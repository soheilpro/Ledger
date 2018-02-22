using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IBalanceItemCollection : IEnumerable<IBalanceItem>
    {
        void Add(IBalanceItem balanceItem);

        void AddOrUpdate(IBalanceItem balanceItem, IBalanceItem existingBalanceItem);

        IBalanceItem GetBalanceItem(IAccount account);

        IBalanceItem GetBalanceItem(IAccount account, IAsset asset);

        IBalanceItem GetBalanceItem(IAccountPredicate predicate);

        IBalanceItem GetBalanceItem(IAccountPredicate predicate, IAsset asset);

        ICollection<IBalanceItem> GetBalanceItems(IAccountPredicate predicate);

        IBalanceItem GetBalanceItemCombined(IAccountPredicate predicate);

        IBalanceItem GetBalanceItemCombined(IAccountPredicate predicate, IAsset asset);

        ICollection<IBalanceItem> GetBalanceItemsCombined(IAccountPredicate predicate);
    }
}
