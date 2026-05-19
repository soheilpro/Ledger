using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class NetWorthReport : ReportBase
    {
        public NetWorthReport(NetWorthReportItem reportItem)
        {
            Item = reportItem;
        }

        public NetWorthReportItem Item
        {
            get;
        }
    }

    public class NetWorthReportItem
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
