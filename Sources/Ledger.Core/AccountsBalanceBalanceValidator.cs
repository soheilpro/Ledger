using System;
using System.Linq;

namespace Ledger.Core
{
    public class AccountsBalanceBalanceValidator : IBalanceValidator
    {
        private IBook _book;
        private IAccountPredicate _accountPredicate1;
        private IAccountPredicate _accountPredicate2;

        public AccountsBalanceBalanceValidator(IBook book, IAccountPredicate accountPredicate1, IAccountPredicate accountPredicate2)
        {
            _book = book;
            _accountPredicate1 = accountPredicate1;
            _accountPredicate2 = accountPredicate2;
        }

        public void Validate(IBalance balance)
        {
            if (!balance.Book.Equals(_book))
                return;

            var balanceItems1 = balance.Items.GetBalanceItemsCombined(_accountPredicate1);
            var balanceItems2 = balance.Items.GetBalanceItemsCombined(_accountPredicate2);
            var assets = balanceItems1.Select(x => x.Asset).Union(balanceItems2.Select(x => x.Asset));

            foreach (var asset in assets)
            {
                var balanceItem1 = balanceItems1.SingleOrDefault(x => x.Asset.Equals(asset)) ?? new BalanceItem();
                var balanceItem2 = balanceItems2.SingleOrDefault(x => x.Asset.Equals(asset)) ?? new BalanceItem();

                if (balanceItem1.BalanceDebit - balanceItem1.BalanceCredit != balanceItem2.BalanceCredit - balanceItem2.BalanceDebit)
                    throw new ValidationException($"Balance ({balance}) asset ({asset}) are not equal.");
            }
        }
    }
}
