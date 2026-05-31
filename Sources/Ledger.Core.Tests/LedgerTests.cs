namespace Ledger.Core.Tests;

public class LedgerTests
{
    [Fact]
    public void AddEntry_CreatesNewBalancesWithoutMutatingPriorSnapshots()
    {
        var ledger = LedgerTestData.CreateLedger();

        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(
            1,
            (LedgerTestData.Cash, 100m, 0m),
            (LedgerTestData.Income, 0m, 100m)));

        var firstBalance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(LedgerTestData.MainBook, 1));

        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(
            2,
            (LedgerTestData.Expense, 40m, 0m),
            (LedgerTestData.Cash, 0m, 40m)));

        var firstCash = Assert.IsAssignableFrom<IBalanceItem>(firstBalance.Items.GetBalanceItem(LedgerTestData.Cash, LedgerTestData.Usd));
        var secondBalance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(LedgerTestData.MainBook, 2));
        var secondCash = Assert.IsAssignableFrom<IBalanceItem>(secondBalance.Items.GetBalanceItem(LedgerTestData.Cash, LedgerTestData.Usd));
        var expenseBalance = Assert.IsAssignableFrom<IBalanceItem>(secondBalance.Items.GetBalanceItem(LedgerTestData.Expense, LedgerTestData.Usd));

        Assert.Equal(100m, firstCash.TotalDebit);
        Assert.Equal(0m, firstCash.TotalCredit);
        Assert.Null(firstBalance.Items.GetBalanceItem(LedgerTestData.Expense, LedgerTestData.Usd));

        Assert.Equal(100m, secondCash.TotalDebit);
        Assert.Equal(40m, secondCash.TotalCredit);
        Assert.Equal(40m, expenseBalance.TotalDebit);
        Assert.Equal(0m, expenseBalance.TotalCredit);
    }

    [Fact]
    public void AddEntry_RejectsEntriesThatDoNotAdvanceTheIndex()
    {
        var ledger = LedgerTestData.CreateLedger();

        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(
            2,
            (LedgerTestData.Cash, 100m, 0m),
            (LedgerTestData.Income, 0m, 100m)));

        var exception = Assert.Throws<ValidationException>(() => ledger.AddEntry(LedgerTestData.CreateBalancedEntry(
            2,
            (LedgerTestData.Expense, 10m, 0m),
            (LedgerTestData.Cash, 0m, 10m))));

        Assert.Contains("cannot occur on or before the last entry", exception.Message);
    }

    [Fact]
    public void EntryAndBalanceQueries_UseExpectedRangeBoundaries()
    {
        var ledger = LedgerTestData.CreateLedger();

        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(1, (LedgerTestData.Cash, 100m, 0m), (LedgerTestData.Income, 0m, 100m)));
        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(2, (LedgerTestData.Expense, 30m, 0m), (LedgerTestData.Cash, 0m, 30m)));
        ledger.AddEntry(LedgerTestData.CreateBalancedEntry(3, (LedgerTestData.Expense, 20m, 0m), (LedgerTestData.Cash, 0m, 20m)));

        Assert.Equal(1, ledger.GetEntryBefore(2)?.Index);
        Assert.Equal(2, ledger.GetEntryAtOrBefore(2)?.Index);
        Assert.Equal(3, ledger.GetEntryAfter(2)?.Index);
        Assert.Equal(new[] { 2 }, LedgerTestData.ToIndexes(ledger.GetEntriesBetween(1, 3)));
        Assert.Equal(new[] { 1, 2, 3 }, LedgerTestData.ToIndexes(ledger.GetEntriesAtOrBetween(1, 3)));

        Assert.Equal(1, ledger.GetBalanceBefore(LedgerTestData.MainBook, 2)?.Index);
        Assert.Equal(2, ledger.GetBalanceAtOrBefore(LedgerTestData.MainBook, 2)?.Index);
        Assert.Equal(3, ledger.GetBalanceAfter(LedgerTestData.MainBook, 2)?.Index);
    }
}
