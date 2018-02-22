using System;

namespace Ledger.Core
{
    public class AccountBalanceCreditZeroBalanceValidator : IBalanceValidator
    {
        private IBook _book;
        private IAccountPredicate _accountPredicate;

        public AccountBalanceCreditZeroBalanceValidator(IBook book, IAccountPredicate accountPredicate)
        {
            _book = book;
            _accountPredicate = accountPredicate;
        }

        public void Validate(IBalance balance)
        {
            if (!balance.Book.Equals(_book))
                return;

            foreach (var balanceItem in balance.Items.GetBalanceItems(_accountPredicate))
            {
                if (balanceItem.BalanceCredit > 0)
                    throw new ValidationException($"Balance ({balance}) account ({balanceItem.Account}) balance credit must be zero.");
            }
        }
    }
}
