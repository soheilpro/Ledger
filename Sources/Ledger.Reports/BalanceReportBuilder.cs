using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;
using Ledger.Rates;

namespace Ledger.Reports
{
    public class BalanceReportBuilder : ReportBuilderBase
    {
        public IJournal Journal
        {
            get;
            set;
        }

        public IRateProvider RateProvider
        {
            get;
            set;
        }

        public string Book
        {
            get;
            set;
        }

        public string AccountQuery
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

        public string TargetAsset
        {
            get;
            set;
        }

        public bool IncludeZeroBalances
        {
            get;
            set;
        }

        public BalanceReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);
            var accountIds = GetReportAccountIds(balance, AccountQuery);
            var reportItems = new List<BalanceReportItem>();
            var assetPredicate = default(QueryAssetPredicate);
            var targetAsset = default(IAsset);

            if (!string.IsNullOrEmpty(AssetQuery))
                assetPredicate = new QueryAssetPredicate(AssetQuery);

            if (!string.IsNullOrEmpty(TargetAsset))
            {
                if (RateProvider == null)
                    throw new InvalidOperationException("RateProvider is required when TargetAsset is specified.");

                targetAsset = new Asset(TargetAsset);
            }

            foreach (var accountId in accountIds)
            {
                var predicate = new QueryAccountPredicate(accountId + ":**");
                decimal totalDebit = 0m;
                decimal totalCredit = 0m;

                foreach (var balanceItem in balance.Items.GetBalanceItemsCombined(predicate))
                {
                    if (assetPredicate != null && !assetPredicate.Matches(balanceItem.Asset))
                        continue;

                    if (targetAsset != null)
                    {
                        var rate = RateProvider.GetRate(balanceItem.Asset, targetAsset, Index);
                        totalDebit += balanceItem.TotalDebit * rate;
                        totalCredit += balanceItem.TotalCredit * rate;
                        continue;
                    }

                    var reportItem = new BalanceReportItem()
                    {
                        Account = new Account(accountId),
                        Asset = balanceItem.Asset,
                        TotalDebit = balanceItem.TotalDebit,
                        TotalCredit = balanceItem.TotalCredit
                    };
                    reportItem.BalanceDebit = reportItem.TotalDebit > reportItem.TotalCredit ? reportItem.TotalDebit - reportItem.TotalCredit : 0;
                    reportItem.BalanceCredit = reportItem.TotalCredit > reportItem.TotalDebit ? reportItem.TotalCredit - reportItem.TotalDebit : 0;

                    reportItems.Add(reportItem);
                }

                if (targetAsset != null)
                    reportItems.Add(CreateReportItem(accountId, targetAsset, totalDebit, totalCredit));
            }

            if (!IncludeZeroBalances)
                reportItems = reportItems.Where(reportItem => reportItem.BalanceDebit != 0 || reportItem.BalanceCredit != 0).ToList();

            reportItems = reportItems.OrderBy(reportItem => reportItem.Account, new AccountComparer()).ThenBy(reportItem => reportItem.Asset, new AssetComparer()).ToList();

            return new BalanceReport(reportItems.ToArray());
        }

        private static BalanceReportItem CreateReportItem(string accountId, IAsset asset, decimal totalDebit, decimal totalCredit)
        {
            var reportItem = new BalanceReportItem()
            {
                Account = new Account(accountId),
                Asset = asset,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit
            };
            reportItem.BalanceDebit = reportItem.TotalDebit > reportItem.TotalCredit ? reportItem.TotalDebit - reportItem.TotalCredit : 0;
            reportItem.BalanceCredit = reportItem.TotalCredit > reportItem.TotalDebit ? reportItem.TotalCredit - reportItem.TotalDebit : 0;

            return reportItem;
        }

        private static IEnumerable<string> GetReportAccountIds(IBalance balance, string accountQuery)
        {
            if (accountQuery.EndsWith(":**", StringComparison.Ordinal))
                return GetAccountIds(balance, new QueryAccountPredicate(accountQuery));

            if (accountQuery.Equals("**", StringComparison.Ordinal))
                return GetAccountIds(balance, new QueryAccountPredicate(accountQuery));

            return GetChildAccountIds(balance, accountQuery);
        }
    }
}
