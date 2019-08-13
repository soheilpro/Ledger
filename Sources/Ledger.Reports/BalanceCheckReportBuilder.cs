using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class BalanceCheckReportBuilder : ReportBuilderBase
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

        public string AssetQuery
        {
            get;
            set;
        }

        public BalanceCheckReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);
            var balanceItems = balance.Items.GetBalanceItemsCombined(new TrueAccountPredicate());
            var reportItems = new List<BalanceCheckReport.ReportItem>();

            foreach (var balanceItem in balanceItems)
            {
                var reportItem = new BalanceCheckReport.ReportItem();
                reportItem.Asset = balanceItem.Asset;
                reportItem.Assets = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Assets:**"), balanceItem.Asset).BalanceDebitOrDefault();
                reportItem.Liabilities = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Liabilities:**"), balanceItem.Asset).BalanceDebitOrDefault();
                reportItem.Equity = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Equity:**"), balanceItem.Asset).BalanceDebitOrDefault();
                reportItem.LiabilitiesAndEquity = reportItem.Liabilities + reportItem.Equity;
                reportItem.Diff = reportItem.Assets + reportItem.LiabilitiesAndEquity;

                reportItems.Add(reportItem);
            }

            if (!string.IsNullOrEmpty(AssetQuery))
            {
                var assetPredicate = new QueryAssetPredicate(AssetQuery);

                reportItems = reportItems.Where(reportItem => assetPredicate.Matches(reportItem.Asset)).ToList();
            }

            reportItems = reportItems.OrderBy(reportItem => reportItem.Asset, new AssetComparer()).ToList();

            return new BalanceCheckReport(reportItems.ToArray());
        }
    }
}
