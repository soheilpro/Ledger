using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class AccountsBalanceEntryValidator : IEntryValidator
    {
        private IAccountPredicate _accountPredicate1;
        private IAccountPredicate _accountPredicate2;

        public AccountsBalanceEntryValidator(IAccountPredicate accountPredicate1, IAccountPredicate accountPredicate2)
        {
            _accountPredicate1 = accountPredicate1;
            _accountPredicate2 = accountPredicate2;
        }

        public void Validate(IEntry entry)
        {
            var entryItems1 = entry.Items.GetEntryItems(_accountPredicate1);
            var entryItems2 = entry.Items.GetEntryItems(_accountPredicate2);

            var bookAssetBalance = new Dictionary<Tuple<IBook, IAsset>, decimal>();

            foreach (var entryItem in entryItems1.Union(entryItems2))
            {
                var key = Tuple.Create(entryItem.Book, entryItem.Asset);
                decimal balance;

                bookAssetBalance.TryGetValue(key, out balance);
                bookAssetBalance[key] = balance + (entryItem.Debit - entryItem.Credit);
            }

            foreach (var keyValuePair in bookAssetBalance)
                if (keyValuePair.Value != 0)
                    throw new ValidationException($"Entry ({entry}) book ({keyValuePair.Key.Item1}) asset ({keyValuePair.Key.Item2}) are not equal.");
        }
    }
}
