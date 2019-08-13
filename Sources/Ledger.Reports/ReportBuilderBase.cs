using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public abstract class ReportBuilderBase
    {
        protected ILedger GetLedger(IJournal journal)
        {
            var ledger = new Ledger.Core.Ledger();
            ledger.EntryValidators.Add(new IntegrityEntryValidator());
            ledger.EntryValidators.Add(new AccountsBalanceEntryValidator(new QueryAccountPredicate("Assets:**"), new QueryAccountPredicate("Liabilities|Equity:**")));
            ledger.BalanceValidators.Add(new AccountsBalanceBalanceValidator(new Book("Default"), new QueryAccountPredicate("Assets:**"), new QueryAccountPredicate("Liabilities|Equity:**")));
            ledger.BalanceValidators.Add(new AccountBalanceCreditZeroBalanceValidator(new Book("Default"), new QueryAccountPredicate("Equity:ProfitLoss:Expense:**")));
            ledger.BalanceValidators.Add(new AccountBalanceDebitZeroBalanceValidator(new Book("Default"), new QueryAccountPredicate("Equity:ProfitLoss:Income:**")));

            foreach (var entry in journal.Entries)
                ledger.AddEntry(entry);

            return ledger;
        }

        protected static IEnumerable<string> GetAccountIds(IBalance balance, IAccountPredicate accountPredicate)
        {
            return balance.Items.GetBalanceItems(accountPredicate).Select(balanceItem => ((Account)balanceItem.Account).Id).Distinct();
        }

        protected static IEnumerable<string> GetAllAccountIds(IBalance balance, IAccountPredicate accountPredicate)
        {
            var accountIds = balance.Items.GetBalanceItems(accountPredicate).Select(balanceItem => ((Account)balanceItem.Account).Id);
            var accountIdsList = new List<string>();

            foreach (var accountId in accountIds)
            {
                var accountIdParts = accountId.Split(':');

                for (var i = 1; i <= accountIdParts.Length; i++)
                    accountIdsList.Add(string.Join(":", accountIdParts.Take(i)));
            }

            return accountIdsList.Distinct();
        }

        protected static IEnumerable<string> GetChildAccountIds(IBalance balance, string account)
        {
            var accountPredicate = new QueryAccountPredicate(account + ":**");
            var accountIds = balance.Items.GetBalanceItems(accountPredicate).Select(balanceItem => ((Account)balanceItem.Account).Id);
            var level = account.Split(':').Length;

            return accountIds.Select(accountId => string.Join(":", accountId.Split(':').Take(level))).Distinct();
        }
    }
}
