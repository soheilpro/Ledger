using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class NetWorthReport : ReportBase
    {
        public NetWorthReport(ReportItem reportItem)
        {
            Item = reportItem;
        }

        public ReportItem Item
        {
            get;
        }

        public class ReportItem
        {
            public IAsset Asset
            {
                get;
                set;
            }

            public decimal Value
            {
                get;
                set;
            }
        }
    }
}
