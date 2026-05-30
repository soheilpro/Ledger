using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;
using Ledger.Rates;

namespace Ledger.Reports
{
    public class ProfitLossReportBuilder : ReportBuilderBase
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

        public string StartIndex
        {
            get;
            set;
        }

        public string EndIndex
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

        public ProfitLossReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var startBalance = ledger.GetBalanceAt(book, StartIndex) ?? new Balance();
            var endBalance = ledger.GetBalanceAt(book, EndIndex) ?? new Balance();
            var accountIds = GetChildAccountIds(endBalance, AccountQuery);
            var reportItems = new List<ProfitLossReportItem>();
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

                if (targetAsset != null)
                {
                    decimal totalDebit = 0m;
                    decimal totalCredit = 0m;

                    foreach (var balanceItem in endBalance.Items.GetBalanceItemsCombined(predicate))
                    {
                        if (assetPredicate != null && !assetPredicate.Matches(balanceItem.Asset))
                            continue;

                        var rate = RateProvider.GetRate(balanceItem.Asset, targetAsset, EndIndex);
                        totalDebit += balanceItem.TotalDebit * rate;
                        totalCredit += balanceItem.TotalCredit * rate;

                        var startBalanceItem = startBalance.Items.GetBalanceItemCombined(predicate, balanceItem.Asset);

                        if (startBalanceItem == null)
                            continue;

                        totalDebit -= startBalanceItem.TotalDebit * rate;
                        totalCredit -= startBalanceItem.TotalCredit * rate;
                    }

                    reportItems.Add(CreateReportItem(accountId, targetAsset, totalDebit, totalCredit));
                    continue;
                }

                foreach (var balanceItem in endBalance.Items.GetBalanceItemsCombined(predicate))
                {
                    if (assetPredicate != null && !assetPredicate.Matches(balanceItem.Asset))
                        continue;

                    var reportItem = new ProfitLossReportItem()
                    {
                        Account = new Account(accountId),
                        Asset = balanceItem.Asset,
                        TotalDebit = balanceItem.TotalDebit,
                        TotalCredit = balanceItem.TotalCredit
                    };

                    reportItems.Add(reportItem);
                }
            }

            foreach (var reportItem in reportItems)
            {
                var startBalanceItem = startBalance.Items.GetBalanceItemCombined(new QueryAccountPredicate(reportItem.Account + ":**"), reportItem.Asset);

                if (startBalanceItem == null)
                    continue;

                reportItem.TotalDebit -= startBalanceItem.TotalDebit;
                reportItem.TotalCredit -= startBalanceItem.TotalCredit;
            }

            foreach (var reportItem in reportItems)
            {
                reportItem.BalanceDebit = reportItem.TotalDebit > reportItem.TotalCredit ? reportItem.TotalDebit - reportItem.TotalCredit : 0;
                reportItem.BalanceCredit = reportItem.TotalCredit > reportItem.TotalDebit ? reportItem.TotalCredit - reportItem.TotalDebit : 0;
            }

            foreach (var reportItem in reportItems)
            {
                var totalBalanceDebit = reportItems.Where(x => x.Asset.Equals(reportItem.Asset)).Sum(x => x.BalanceDebit);
                var totalBalanceCredit = reportItems.Where(x => x.Asset.Equals(reportItem.Asset)).Sum(x => x.BalanceCredit);

                reportItem.BalanceDebitPercent = totalBalanceDebit != 0 ? reportItem.BalanceDebit / totalBalanceDebit * 100 : 0;
                reportItem.BalanceCreditPercent = totalBalanceCredit != 0 ? reportItem.BalanceCredit / totalBalanceCredit * 100 : 0;
            }

            reportItems = reportItems.Where(reportItem => reportItem.BalanceDebit != 0 || reportItem.BalanceCredit != 0).ToList();
            reportItems = reportItems.OrderBy(reportItem => reportItem.BalanceCredit - reportItem.BalanceDebit).ToList();

            return new ProfitLossReport(reportItems.ToArray());
        }

        private static ProfitLossReportItem CreateReportItem(string accountId, IAsset asset, decimal totalDebit, decimal totalCredit)
        {
            var reportItem = new ProfitLossReportItem()
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
    }
}
