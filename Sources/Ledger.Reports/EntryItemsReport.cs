using System;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public class EntryItemsReport : ReportBase
    {
        private ReportItem[] _reportItem;

        public EntryItemsReport(ReportItem[] reportItem)
        {
            _reportItem = reportItem;
        }

        public override void Print(TextWriter writer)
        {
            var table = GetTable(_reportItem);

            table.PrintText(writer);
        }

        private ITable GetTable(ReportItem[] reportItems)
        {
            var table = new Table();
            table.Columns.Add(new TableTextColumn<ReportItem>("Index", row => row.Index));
            table.Columns.Add(new TableAccountColumn<ReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<ReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Debit", row => row.Debit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Credit", row => row.Credit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Rows = reportItems;

            return table;
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
