using System;
using Ledger.Core;
using Ledger.Journal;

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

        public NetWorthReport GetReport()
        {
            var ledger = GetLedger(Journal);
            var book = new Book(Book);
            var balance = ledger.GetBalanceAtOrBefore(book, Index);

            var assets = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Assets:**"), Asset).BalanceDebitOrDefault();
            var liabilities = balance.Items.GetBalanceItemCombined(new QueryAccountPredicate("Liabilities:**"), Asset).BalanceDebitOrDefault();

            return new NetWorthReport(new NetWorthReport.ReportItem
            {
                Asset = Asset,
                NetWorth = assets + liabilities,
            });
        }
    }
}
