using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class BalanceReport : ReportBase
    {
        public BalanceReport(ReportItem[] reportItems)
        {
            ReportItems = reportItems;
        }

        public ReportItem[] ReportItems
        {
            get;
        }

        public class ReportItem
        {
            public IAccount Account
            {
                get;
                set;
            }

            public IBalance Balance
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
        }
    }
}
