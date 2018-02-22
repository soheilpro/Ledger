using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class ProfitLossReportBuilder : ReportBuilderBase
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

        public ProfitLossReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var startBalance = ledger.GetBalanceAt(book, StartIndex) ?? new Balance();
            var endBalance = ledger.GetBalanceAt(book, EndIndex) ?? new Balance();
            var accountIds = GetChildAccountIds(endBalance, AccountQuery);
            var reportItems = new List<ProfitLossReport.ReportItem>();

            foreach (var accountId in accountIds)
            {
                foreach (var balanceItem in endBalance.Items.GetBalanceItemsCombined(new QueryAccountPredicate(accountId + ":**")))
                {
                    var reportItem = new ProfitLossReport.ReportItem();
                    reportItem.Account = new Account(accountId);
                    reportItem.Asset = balanceItem.Asset;
                    reportItem.TotalDebit = balanceItem.TotalDebit;
                    reportItem.TotalCredit = balanceItem.TotalCredit;

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

            if (!string.IsNullOrEmpty(AssetQuery))
            {
                var assetPredicate = new QueryAssetPredicate(AssetQuery);

                reportItems = reportItems.Where(reportItem => assetPredicate.Matches(reportItem.Asset)).ToList();
            }

            reportItems = reportItems.Where(reportItem => reportItem.BalanceDebit != 0 || reportItem.BalanceCredit != 0).ToList();
            reportItems = reportItems.OrderBy(reportItem => reportItem.BalanceCredit - reportItem.BalanceDebit).ToList();

            return new ProfitLossReport(reportItems.ToArray());
        }
    }
}
