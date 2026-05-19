using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class EntryItemsReport : ReportBase
    {
        public EntryItemsReport(EntryItemsReportItem[] reportItems)
        {
            Items = reportItems;
        }

        public EntryItemsReportItem[] Items
        {
            get;
        }
    }

    public class EntryItemsReportItem
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
