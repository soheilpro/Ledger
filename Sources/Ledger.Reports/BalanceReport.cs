using System;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class BalanceReport : ReportBase
    {
        private ReportItem[] _reportItems;

        public BalanceReport(ReportItem[] reportItems)
        {
            _reportItems = reportItems;
        }

        public override void Print(TextWriter writer)
        {
            var table = GetTable(_reportItems);

            table.PrintText(writer);
        }

        private ITable GetTable(ReportItem[] reportItems)
        {
            var table = new Table();
            table.Columns.Add(new TableAccountColumn<ReportItem>("Account", row => row.Account));
            table.Columns.Add(new TableAssetColumn<ReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Total Debit", row => row.TotalDebit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Total Credit", row => row.TotalCredit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Rows = reportItems;

            return table;
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
