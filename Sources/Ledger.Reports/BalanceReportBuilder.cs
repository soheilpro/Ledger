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

        public bool AllLevels
        {
            get;
            set;
        }

        public bool NoChildren
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
            var accountPredicate = new QueryAccountPredicate(NoChildren ? AccountQuery : AccountQuery + ":**");
            var balance = ledger.GetBalanceAtOrBefore(book, Index);
            var accountIds = AllLevels ? GetAllAccountIds(balance, accountPredicate) : GetAccountIds(balance, accountPredicate);
            var reportItems = new List<BalanceReport.ReportItem>();

            foreach (var accountId in accountIds)
            {
                if (!accountPredicate.Matches(new Account(accountId)))
                    continue;

                var predicate = new QueryAccountPredicate(!AllLevels ? accountId : accountId + ":**");

                foreach (var balanceItem in balance.Items.GetBalanceItemsCombined(predicate))
                {
                    var reportItem = new BalanceReport.ReportItem();
                    reportItem.Account = new Account(accountId);
                    reportItem.Asset = balanceItem.Asset;
                    reportItem.TotalDebit = balanceItem.TotalDebit;
                    reportItem.TotalCredit = balanceItem.TotalCredit;
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
    }
}
