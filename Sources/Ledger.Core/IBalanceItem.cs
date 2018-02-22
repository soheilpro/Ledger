using System;

namespace Ledger.Core
{
    public interface IBalanceItem : IItem
    {
        IBalance Balance
        {
            get;
        }

        IAsset Asset
        {
            get;
        }

        decimal TotalDebit
        {
            get;
        }

        decimal TotalCredit
        {
            get;
        }

        decimal BalanceDebit
        {
            get;
        }

        decimal BalanceCredit
        {
            get;
        }
    }
}
