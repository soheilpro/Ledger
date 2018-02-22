using System;

namespace Ledger.Core
{
    public static class BalanceItemHelper
    {
        public static decimal BalanceDebitOrDefault(this IBalanceItem balanceItem)
        {
            if (balanceItem == null)
                return 0;

            return balanceItem.BalanceDebit - balanceItem.BalanceCredit;
        }

        public static decimal BalanceCreditOrDefault(this IBalanceItem balanceItem)
        {
            if (balanceItem == null)
                return 0;

            return balanceItem.BalanceCredit - balanceItem.BalanceDebit;
        }

        public static decimal TotalDebitOrDefault(this IBalanceItem balanceItem)
        {
            if (balanceItem == null)
                return 0;

            return balanceItem.TotalDebit;
        }

        public static decimal TotalCreditOrDefault(this IBalanceItem balanceItem)
        {
            if (balanceItem == null)
                return 0;

            return balanceItem.TotalCredit;
        }
    }
}
