using System;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public class BalanceCheckReport : ReportBase
    {
        private ReportItem[] _reportItems;

        public BalanceCheckReport(ReportItem[] reportItems)
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
            table.Columns.Add(new TableAssetColumn<ReportItem>("Asset", assetBalance => assetBalance.Asset));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Assets", assetBalance => assetBalance.Assets));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Liabilities + Equity", assetBalance => assetBalance.LiabilitiesAndEquity));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Liabilities", assetBalance => assetBalance.Liabilities));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Equity", assetBalance => assetBalance.Equity));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Diff", assetBalance => assetBalance.Diff));
            table.Rows = reportItems;

            return table;
        }

        public class ReportItem
        {
            public IAsset Asset
            {
                get;
                set;
            }

            public decimal Assets
            {
                get;
                set;
            }

            public decimal LiabilitiesAndEquity
            {
                get;
                set;
            }

            public decimal Liabilities
            {
                get;
                set;
            }

            public decimal Equity
            {
                get;
                set;
            }

            public decimal Diff
            {
                get;
                set;
            }
        }
    }
}
