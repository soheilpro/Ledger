namespace Ledger.Core.Tests;

public class LedgerTests
{
    [Fact]
    public void AddEntry_CreatesNewBalancesWithoutMutatingPriorSnapshots()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Income:Salary");
        var expenseAccount = new Account("Expenses:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        ledger.AddEntry(CreateBalancedEntry(
            1,
            (cashAccount, 100m, 0m),
            (incomeAccount, 0m, 100m)));

        var firstBalance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(mainBook, 1));

        ledger.AddEntry(CreateBalancedEntry(
            2,
            (expenseAccount, 40m, 0m),
            (cashAccount, 0m, 40m)));

        var firstCash = Assert.IsAssignableFrom<IBalanceItem>(firstBalance.Items.GetBalanceItem(cashAccount, usdAsset));
        var secondBalance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(mainBook, 2));
        var secondCash = Assert.IsAssignableFrom<IBalanceItem>(secondBalance.Items.GetBalanceItem(cashAccount, usdAsset));
        var expenseBalance = Assert.IsAssignableFrom<IBalanceItem>(secondBalance.Items.GetBalanceItem(expenseAccount, usdAsset));

        Assert.Equal(100m, firstCash.TotalDebit);
        Assert.Equal(0m, firstCash.TotalCredit);
        Assert.Null(firstBalance.Items.GetBalanceItem(expenseAccount, usdAsset));

        Assert.Equal(100m, secondCash.TotalDebit);
        Assert.Equal(40m, secondCash.TotalCredit);
        Assert.Equal(40m, expenseBalance.TotalDebit);
        Assert.Equal(0m, expenseBalance.TotalCredit);

        Entry CreateBalancedEntry(int index, params (Account Account, decimal Debit, decimal Credit)[] items)
        {
            var entry = new Entry
            {
                Index = index
            };

            foreach (var item in items)
                entry.AddItem(mainBook, item.Account, usdAsset, item.Debit, item.Credit);

            return entry;
        }
    }

    [Fact]
    public void AddEntry_RejectsEntriesThatDoNotAdvanceTheIndex()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Income:Salary");
        var expenseAccount = new Account("Expenses:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        ledger.AddEntry(CreateBalancedEntry(
            2,
            (cashAccount, 100m, 0m),
            (incomeAccount, 0m, 100m)));

        var exception = Assert.Throws<ValidationException>(() => ledger.AddEntry(CreateBalancedEntry(
            2,
            (expenseAccount, 10m, 0m),
            (cashAccount, 0m, 10m))));

        Assert.Contains("cannot occur on or before the last entry", exception.Message);

        Entry CreateBalancedEntry(int index, params (Account Account, decimal Debit, decimal Credit)[] items)
        {
            var entry = new Entry
            {
                Index = index
            };

            foreach (var item in items)
                entry.AddItem(mainBook, item.Account, usdAsset, item.Debit, item.Credit);

            return entry;
        }
    }

    [Fact]
    public void EntryAndBalanceQueries_UseExpectedRangeBoundaries()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Income:Salary");
        var expenseAccount = new Account("Expenses:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        ledger.AddEntry(CreateBalancedEntry(1, (cashAccount, 100m, 0m), (incomeAccount, 0m, 100m)));
        ledger.AddEntry(CreateBalancedEntry(2, (expenseAccount, 30m, 0m), (cashAccount, 0m, 30m)));
        ledger.AddEntry(CreateBalancedEntry(3, (expenseAccount, 20m, 0m), (cashAccount, 0m, 20m)));

        Assert.Equal(1, ledger.GetEntryBefore(2)?.Index);
        Assert.Equal(2, ledger.GetEntryAtOrBefore(2)?.Index);
        Assert.Equal(3, ledger.GetEntryAfter(2)?.Index);
        Assert.Collection(ledger.GetEntriesBetween(1, 3), entry => Assert.Equal(2, entry.Index));
        Assert.Collection(
            ledger.GetEntriesAtOrBetween(1, 3),
            entry => Assert.Equal(1, entry.Index),
            entry => Assert.Equal(2, entry.Index),
            entry => Assert.Equal(3, entry.Index));

        Assert.Equal(1, ledger.GetBalanceBefore(mainBook, 2)?.Index);
        Assert.Equal(2, ledger.GetBalanceAtOrBefore(mainBook, 2)?.Index);
        Assert.Equal(3, ledger.GetBalanceAfter(mainBook, 2)?.Index);

        Entry CreateBalancedEntry(int index, params (Account Account, decimal Debit, decimal Credit)[] items)
        {
            var entry = new Entry
            {
                Index = index
            };

            foreach (var item in items)
                entry.AddItem(mainBook, item.Account, usdAsset, item.Debit, item.Credit);

            return entry;
        }
    }
}
