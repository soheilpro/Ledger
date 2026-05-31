using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core.Tests;

internal static class LedgerTestData
{
    public static readonly Book MainBook = new("main");
    public static readonly Asset Usd = new("USD");
    public static readonly Account Cash = new("assets:cash");
    public static readonly Account Income = new("income:salary");
    public static readonly Account Expense = new("expenses:food");
    public static readonly Account Payable = new("liabilities:payable");
    public static readonly Account Prepaid = new("assets:prepaid");

    public static Ledger CreateLedger()
    {
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());
        return ledger;
    }

    public static Entry CreateBalancedEntry(int index, params (Account Account, decimal Debit, decimal Credit)[] items)
    {
        var entry = new Entry
        {
            Index = index
        };

        foreach (var item in items)
            entry.AddItem(MainBook, item.Account, Usd, item.Debit, item.Credit);

        return entry;
    }

    public static int[] ToIndexes(IEnumerable<IIndexable> indexables)
    {
        return indexables.Select(x => (int)x.Index).ToArray();
    }
}
