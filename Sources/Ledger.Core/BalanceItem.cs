using System;

namespace Ledger.Core
{
    public class BalanceItem : IBalanceItem
    {
        public IBalance Balance
        {
            get;
            set;
        }

        public IAccount Account
        {
        	get;
        	set;
        }

        public IAsset Asset
        {
            get;
            set;
        }

        public decimal TotalDebit
        {
            get;
            set;
        }

        public decimal TotalCredit
        {
            get;
            set;
        }

        public decimal BalanceDebit
        {
            get
            {
                return TotalDebit > TotalCredit ? TotalDebit - TotalCredit : 0;
            }
        }

        public decimal BalanceCredit
        {
            get
            {
                return TotalCredit > TotalDebit ? TotalCredit - TotalDebit : 0;
            }
        }

        public override string ToString()
        {
            return $"Account:{Account} Asset:{Asset} TotalDr:{TotalDebit} TotalCr:{TotalCredit} BalanceDr:{BalanceDebit} BalanceCr:{BalanceCredit}";
        }
    }
}
