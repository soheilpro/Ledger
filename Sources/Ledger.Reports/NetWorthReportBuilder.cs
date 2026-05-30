using System;
using Ledger.Core;
using Ledger.Journal;
using Ledger.Rates;

namespace Ledger.Reports
{
    public class NetWorthReportBuilder : ReportBuilderBase
    {
        public IJournal Journal
        {
            get;
            set;
        }

        public string Book
        {
            get;
            set;
        }

        public string Index
        {
            get;
            set;
        }

        public IAsset Asset
        {
            get;
            set;
        }

        public IRateProvider RateProvider
        {
            get;
            set;
        }

        public NetWorthReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);
            var capitalBalanceItems = balance.Items.GetBalanceItemsCombined(new QueryAccountPredicate("Equity:**"));
            var value = 0m;

            foreach (var capitalBalanceItem in capitalBalanceItems)
            {
                var sourceAsset = capitalBalanceItem.Asset;
                var sourceAssetNetWorth = capitalBalanceItem.BalanceCreditOrDefault();

                if (sourceAsset.Equals(Asset))
                {
                    value += sourceAssetNetWorth;
                    continue;
                }

                value += sourceAssetNetWorth * RateProvider.GetRate(sourceAsset, Asset, Index);
            }

            return new NetWorthReport(new NetWorthReportItem
            {
                Asset = Asset,
                Value = value,
            });
        }

    }
}
