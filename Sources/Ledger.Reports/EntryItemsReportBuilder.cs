using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class EntryItemsReportBuilder : ReportBuilderBase
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

        public bool NoChildren
        {
            get;
            set;
        }

        public EntryItemsReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var accountPredicate = new QueryAccountPredicate(NoChildren ? AccountQuery : AccountQuery + ":**");
            var entryItems = ledger.GetEntriesAtOrBetween(StartIndex, EndIndex).GetEntryItems(book, accountPredicate);
            var reportItems = new List<EntryItemsReport.ReportItem>();

            foreach (var entryItem in entryItems)
            {
                var balanceItem = ledger.GetBalanceAt(book, entryItem.Entry.Index).Items.GetBalanceItem(entryItem.Account, entryItem.Asset);

                var reportItem = new EntryItemsReport.ReportItem();
                reportItem.Index = entryItem.Entry.Index;
                reportItem.Account = entryItem.Account;
                reportItem.Asset = entryItem.Asset;
                reportItem.Debit = entryItem.Debit;
                reportItem.Credit = entryItem.Credit;
                reportItem.BalanceDebit = balanceItem.BalanceDebit;
                reportItem.BalanceCredit = balanceItem.BalanceCredit;

                reportItems.Add(reportItem);
            }

            if (!string.IsNullOrEmpty(AssetQuery))
            {
                var assetPredicate = new QueryAssetPredicate(AssetQuery);

                reportItems = reportItems.Where(reportItem => assetPredicate.Matches(reportItem.Asset)).ToList();
            }

            return new EntryItemsReport(reportItems.ToArray());
        }
    }
}
