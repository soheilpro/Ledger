using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class BalanceReportBuilder : ReportBuilderBase
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

            foreach (var accountId in accountIds)
            {
                var predicate = new QueryAccountPredicate(accountId + ":**");

                foreach (var balanceItem in balance.Items.GetBalanceItemsCombined(predicate))
                {
                    var reportItem = new BalanceReportItem
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
            }

            if (!string.IsNullOrEmpty(AssetQuery))
            {
                var assetPredicate = new QueryAssetPredicate(AssetQuery);

                reportItems = reportItems.Where(reportItem => assetPredicate.Matches(reportItem.Asset)).ToList();
            }

            if (!IncludeZeroBalances)
                reportItems = reportItems.Where(reportItem => reportItem.BalanceDebit != 0 || reportItem.BalanceCredit != 0).ToList();

            reportItems = reportItems.OrderBy(reportItem => reportItem.Account, new AccountComparer()).ThenBy(reportItem => reportItem.Asset, new AssetComparer()).ToList();

            return new BalanceReport(reportItems.ToArray());
        }

        private static IEnumerable<string> GetReportAccountIds(IBalance balance, string accountQuery)
        {
            if (accountQuery.EndsWith(":**", StringComparison.Ordinal))
                return GetDescendantAccountIds(balance, accountQuery.Substring(0, accountQuery.Length - 3));

            if (accountQuery.Equals("**", StringComparison.Ordinal))
                return GetDescendantAccountIds(balance, string.Empty);

            return GetChildAccountIds(balance, accountQuery);
        }
    }
}
