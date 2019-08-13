using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public class OpeningEntryReportBuilder : ReportBuilderBase
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

        public OpeningEntryReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);

            var entryItems = new List<IEntryItem>();
            var assetBalanceItems = balance.Items.GetBalanceItems(new QueryAccountPredicate("Assets:**"));
            var liabilitiesBalanceItems = balance.Items.GetBalanceItems(new QueryAccountPredicate("Liabilities:**"));
            var equityBalanceItems = balance.Items.GetBalanceItemsCombined(new QueryAccountPredicate("Equity:**"));

            foreach (var balanceItem in assetBalanceItems)
                entryItems.Add(CreateEntryItem(balanceItem.Account, balanceItem.Asset, balanceItem.BalanceDebit, balanceItem.BalanceCredit));

            foreach (var balanceItem in liabilitiesBalanceItems)
                entryItems.Add(CreateEntryItem(balanceItem.Account, balanceItem.Asset, balanceItem.BalanceDebit, balanceItem.BalanceCredit));

            foreach (var balanceItem in equityBalanceItems)
                entryItems.Add(CreateEntryItem(new Account("Equity:Capital"), balanceItem.Asset, balanceItem.BalanceDebit, balanceItem.BalanceCredit));

            entryItems = entryItems.Where(entryItem => entryItem.Debit != 0 || entryItem.Credit != 0).ToList();
            entryItems = entryItems.OrderBy(entryItem => entryItem.Account, new AccountComparer()).ThenBy(entryItem => entryItem.Asset, new AssetComparer()).ToList();

            return new OpeningEntryReport(entryItems.ToArray());
        }

        private IEntryItem CreateEntryItem(IAccount account, IAsset asset, decimal debit, decimal credit)
        {
            var entryItem = new EntryItem();
            entryItem.Account = account;
            entryItem.Asset = asset;
            entryItem.Debit = debit;
            entryItem.Credit = credit;

            return entryItem;
        }
    }
}
