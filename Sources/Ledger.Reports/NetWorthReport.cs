using System;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public class NetWorthReport : ReportBase
    {
        private ReportItem _reportItem;

        public NetWorthReport(ReportItem reportItem)
        {
            _reportItem = reportItem;
        }

        public override void Print(TextWriter writer)
        {
            var table = new Table();
            table.Columns.Add(new TableAssetColumn<ReportItem>("Asset", row => row.Asset));
            table.Columns.Add(new TableAmountColumn<ReportItem>("Net Worth", row => row.NetWorth));
            table.Rows = new[] { _reportItem };
            table.PrintText(writer);
        }

        public class ReportItem
        {
            public IAsset Asset
            {
                get;
                set;
            }

            public decimal NetWorth
            {
                get;
                set;
            }
        }
    }
}
