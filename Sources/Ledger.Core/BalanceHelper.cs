using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public static class BalanceHelper
    {
        public static void Copy(this IBalance sourceBalance, IBalance destinationBalance, IAccountMap accountMap)
        {
            // Performance.MarkStart("BalanceHelper.Copy(balance)");

            var sourceAccounts = new List<IAccount>(sourceBalance.Items.Select(balanceItem => balanceItem.Account));
            var selectedAccounts = new List<IAccount>();

            foreach (var accountMapItem in accountMap.Items)
            {
                if (!accountMapItem.Book.Equals(sourceBalance.Book))
                    continue;

                var balanceItems = sourceBalance.Items.GetBalanceItems(accountMapItem.AccountPredicate);

                if (!balanceItems.Any())
                    continue;

                var newBalanceItem = new BalanceItem();
                newBalanceItem.Balance = destinationBalance;
                newBalanceItem.Account = accountMapItem.Account;
                newBalanceItem.Asset = balanceItems.First().Asset;
                newBalanceItem.TotalDebit = balanceItems.Sum(balanceItem => balanceItem.TotalDebit);
                newBalanceItem.TotalCredit = balanceItems.Sum(balanceItem => balanceItem.TotalCredit);

                destinationBalance.Items.Add(newBalanceItem);

                selectedAccounts.AddRange(balanceItems.Select(balanceItem => balanceItem.Account));
            }

            // Performance.MarkStart("BalanceHelper.Copy(balance):Validate");

            var missedSourceAccounts = sourceAccounts.Except(selectedAccounts).ToList();

            if (missedSourceAccounts.Any())
                throw new ValidationException($"Missed accounts:{Environment.NewLine}{string.Join(Environment.NewLine, missedSourceAccounts)}");

            var duplicateSelectedSourceAccounts = selectedAccounts.GroupBy(account => account).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            if (duplicateSelectedSourceAccounts.Any())
                throw new ValidationException($"Duplicate selected accounts:{Environment.NewLine}{string.Join(Environment.NewLine, duplicateSelectedSourceAccounts)}");

            // Performance.MarkEnd("BalanceHelper.Copy(balance):Validate");

            // Performance.MarkEnd("BalanceHelper.Copy(balance)");
        }

        public static void Copy(this IBalance sourceBalance, IEntry entry)
        {
            // Performance.MarkStart("BalanceHelper.Copy(entry)");

            foreach (var balanceItem in sourceBalance.Items)
            {
                if (balanceItem.BalanceDebit == 0 && balanceItem.BalanceCredit == 0)
                    continue;

                var item = new EntryItem();
                item.Entry = entry;
                item.Book = sourceBalance.Book;
                item.Account = balanceItem.Account;
                item.Asset = balanceItem.Asset;
                item.Debit = balanceItem.BalanceDebit;
                item.Credit = balanceItem.BalanceCredit;

                entry.Items.Add(item);
            }

            // Performance.MarkEnd("BalanceHelper.Copy(entry)");
        }

        public static IBalance Subtract(this IBalance endBalance, IBalance startBalance)
        {
            // Performance.MarkStart("BalanceHelper.Subtract");

            var balance = new Balance();
            balance.Index = endBalance.Index;
            balance.Book = endBalance.Book;

            foreach (var endBalanceItem in endBalance.Items)
            {
                var startBalanceItem = startBalance.Items.GetBalanceItem(endBalanceItem.Account);

                var balanceItem = new BalanceItem();

                if (startBalanceItem != null)
                {
                    if (!endBalanceItem.Asset.Equals(startBalanceItem.Asset))
                        throw new ValidationException("Assets not equal.");

                    balanceItem.Asset = endBalanceItem.Asset;
                    balanceItem.Account = endBalanceItem.Account;
                    balanceItem.Balance = endBalance;

                    var totalDebit = endBalanceItem.TotalDebit - startBalanceItem.TotalDebit;
                    var totalCredit = endBalanceItem.TotalCredit - startBalanceItem.TotalCredit;

                    if (totalDebit < 0)
                    {
                        totalCredit -= totalDebit;
                        totalDebit = 0;
                    }

                    if (totalCredit < 0)
                    {
                        totalDebit -= totalCredit;
                        totalCredit = 0;
                    }

                    balanceItem.TotalDebit = totalDebit;
                    balanceItem.TotalCredit = totalCredit;
                }
                else
                {
                    balanceItem = (BalanceItem)endBalanceItem;
                }

                balance.Items.Add(balanceItem);
            }

            // Performance.MarkEnd("BalanceHelper.Subtract");

            return balance;
        }
    }
}
