namespace Ledger.Core.Tests;

public class EntryTests
{
    [Fact]
    public void AddItem_MapsSignedAmountsToDebitAndCredit()
    {
        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(LedgerTestData.MainBook, LedgerTestData.Cash, LedgerTestData.Usd, 25m);
        entry.AddItem(LedgerTestData.MainBook, LedgerTestData.Income, LedgerTestData.Usd, -25m);

        var cashItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(LedgerTestData.MainBook, LedgerTestData.Cash, LedgerTestData.Usd));
        var incomeItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(LedgerTestData.MainBook, LedgerTestData.Income, LedgerTestData.Usd));

        Assert.Equal(25m, cashItem.Debit);
        Assert.Equal(0m, cashItem.Credit);
        Assert.Equal(0m, incomeItem.Debit);
        Assert.Equal(25m, incomeItem.Credit);
    }

    [Fact]
    public void AddItems_UsesExistingCreditBalanceBeforeDebitAccount()
    {
        var ledger = LedgerTestData.CreateLedger();
        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(
            1,
            (LedgerTestData.Cash, 100m, 0m),
            (LedgerTestData.Payable, 0m, 100m)));

        var entry = new Entry
        {
            Index = 2
        };

        entry.AddItems(
            ledger,
            LedgerTestData.MainBook,
            new QueryAccountPredicate("liabilities:**"),
            LedgerTestData.Prepaid,
            LedgerTestData.Payable,
            LedgerTestData.Usd,
            debit: 150m,
            credit: 0m);

        var payableItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(LedgerTestData.MainBook, LedgerTestData.Payable, LedgerTestData.Usd));
        var prepaidItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(LedgerTestData.MainBook, LedgerTestData.Prepaid, LedgerTestData.Usd));

        Assert.Equal(100m, payableItem.Debit);
        Assert.Equal(0m, payableItem.Credit);
        Assert.Equal(50m, prepaidItem.Debit);
        Assert.Equal(0m, prepaidItem.Credit);
    }

    [Fact]
    public void AddItems_RejectsInvalidAmounts()
    {
        var entry = new Entry
        {
            Index = 1
        };

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            LedgerTestData.MainBook,
            new TrueAccountPredicate(),
            LedgerTestData.Prepaid,
            LedgerTestData.Payable,
            LedgerTestData.Usd,
            debit: -1m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            LedgerTestData.MainBook,
            new TrueAccountPredicate(),
            LedgerTestData.Prepaid,
            LedgerTestData.Payable,
            LedgerTestData.Usd,
            debit: 0m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            LedgerTestData.MainBook,
            new TrueAccountPredicate(),
            LedgerTestData.Prepaid,
            LedgerTestData.Payable,
            LedgerTestData.Usd,
            debit: 1m,
            credit: 1m));
    }
}
