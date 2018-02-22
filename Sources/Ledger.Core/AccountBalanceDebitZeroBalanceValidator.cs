using System;

namespace Ledger.Core
{
    public class AccountBalanceDebitZeroBalanceValidator : IBalanceValidator
    {
        private IBook _book;
        private IAccountPredicate _accountPredicate;

        public AccountBalanceDebitZeroBalanceValidator(IBook book, IAccountPredicate accountPredicate)
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
                if (balanceItem.BalanceDebit > 0)
                    throw new ValidationException($"Balance ({balance}) account ({balanceItem.Account}) balance debit must be zero.");
            }
        }
    }
}
