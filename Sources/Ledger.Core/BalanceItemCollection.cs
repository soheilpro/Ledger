using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    internal class BalanceItemCollection : ItemCollection<IBalanceItem>, IBalanceItemCollection
    {
        public BalanceItemCollection(Balance balance) : base(balance)
        {
        }

        public BalanceItemCollection(Balance balance, IBalanceItemCollection items) : base(balance, (ItemCollection<IBalanceItem>)items)
        {
        }

        public IBalanceItem GetBalanceItem(IAccount account)
        {
            return FindItems(account).SingleOrDefault();
        }

        public IBalanceItem GetBalanceItem(IAccount account, IAsset asset)
        {
            return FindItems(account).SingleOrDefault(balanceItem => balanceItem.Asset.Equals(asset));
        }

        public IBalanceItem GetBalanceItem(IAccountPredicate predicate)
        {
            return FindItems(predicate).SingleOrDefault();
        }

        public IBalanceItem GetBalanceItem(IAccountPredicate predicate, IAsset asset)
        {
            return FindItems(predicate).SingleOrDefault(balanceItem => balanceItem.Asset.Equals(asset));
        }

        public ICollection<IBalanceItem> GetBalanceItems(IAccountPredicate predicate)
        {
            return FindItems(predicate);
        }

        public IBalanceItem GetBalanceItemCombined(IAccountPredicate predicate)
        {
            return GetBalanceItemsCombined(predicate).SingleOrDefault();
        }

        public IBalanceItem GetBalanceItemCombined(IAccountPredicate predicate, IAsset asset)
        {
            return GetBalanceItemsCombined(predicate).SingleOrDefault(balanceItem => balanceItem.Asset.Equals(asset));
        }

        public ICollection<IBalanceItem> GetBalanceItemsCombined(IAccountPredicate predicate)
        {
            return FindItems(predicate).GroupBy(balanceItem => balanceItem.Asset).Select(grp =>
            {
                return (IBalanceItem)new BalanceItem
                {
                    Balance = null, // TODO
                    Account = null, // TODO
                    Asset = grp.Key,
                    TotalDebit = grp.Sum(x => x.TotalDebit),
                    TotalCredit = grp.Sum(x => x.TotalCredit)
                };
            }).ToList();
        }
    }
}
