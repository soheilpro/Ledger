using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class EntryItemsReport : ReportBase
    {
        public EntryItemsReport(ReportItem[] reportItems)
        {
            ReportItems = reportItems;
        }

        public ReportItem[] ReportItems
        {
            get;
        }

        public class ReportItem
        {
            public IComparable Index
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

            public decimal Debit
            {
                get;
                set;
            }

            public decimal Credit
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
