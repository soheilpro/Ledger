using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class ProfitLossReport : ReportBase
    {
        public ProfitLossReport(ProfitLossReportItem[] reportItems)
        {
            Items = reportItems;
        }

        public ProfitLossReportItem[] Items
        {
            get;
        }
    }

    public class ProfitLossReportItem
    {
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
            get;
            set;
        }

        public decimal BalanceCredit
        {
            get;
            set;
        }

        public decimal BalanceDebitPercent
        {
            get;
            set;
        }

        public decimal BalanceCreditPercent
        {
            get;
            set;
        }
    }
}
