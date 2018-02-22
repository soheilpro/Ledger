using System;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class ProfitLossReport : ReportBase
    {
        private ReportItem[] _reportItems;

        public ProfitLossReport(ReportItem[] reportItems)
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
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Debit", row => row.BalanceDebit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("%", row => row.BalanceDebitPercent));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Balance Credit", row => row.BalanceCredit));
            table.Columns.Add(new TableAmountColumn<ReportItem>("%", row => row.BalanceCreditPercent));
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
}
