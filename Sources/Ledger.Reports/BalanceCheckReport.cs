using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class BalanceCheckReport : ReportBase
    {
        public BalanceCheckReport(BalanceCheckReportItem[] reportItems)
        {
            Items = reportItems;
        }

        public BalanceCheckReportItem[] Items
        {
            get;
        }
    }

    public class BalanceCheckReportItem
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
