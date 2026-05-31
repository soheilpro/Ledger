namespace Ledger.Core.Tests;

public class LedgerTests
{
    [Fact]
    public void AddEntry_CreatesNewBalancesWithoutMutatingPriorSnapshots()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Equity:ProfitLoss:Income:Salary");
        var expenseAccount = new Account("Equity:ProfitLoss:Expense:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        var openingEntry = new Entry
        {
            Index = 1
        };
        openingEntry.AddItem(mainBook, cashAccount, usdAsset, 100m, 0m);
        openingEntry.AddItem(mainBook, incomeAccount, usdAsset, 0m, 100m);
        ledger.AddEntry(openingEntry);

        var firstBalance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(mainBook, 1));

        var secondEntry = new Entry
        {
            Index = 2
        };
        secondEntry.AddItem(mainBook, expenseAccount, usdAsset, 40m, 0m);
        secondEntry.AddItem(mainBook, cashAccount, usdAsset, 0m, 40m);
        ledger.AddEntry(secondEntry);

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
    }

    [Fact]
    public void AddEntry_RejectsEntriesThatDoNotAdvanceTheIndex()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Equity:ProfitLoss:Income:Salary");
        var expenseAccount = new Account("Equity:ProfitLoss:Expense:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        var openingEntry = new Entry
        {
            Index = 2
        };
        openingEntry.AddItem(mainBook, cashAccount, usdAsset, 100m, 0m);
        openingEntry.AddItem(mainBook, incomeAccount, usdAsset, 0m, 100m);
        ledger.AddEntry(openingEntry);

        var duplicateIndexEntry = new Entry
        {
            Index = 2
        };
        duplicateIndexEntry.AddItem(mainBook, expenseAccount, usdAsset, 10m, 0m);
        duplicateIndexEntry.AddItem(mainBook, cashAccount, usdAsset, 0m, 10m);

        var exception = Assert.Throws<ValidationException>(() => ledger.AddEntry(duplicateIndexEntry));

        Assert.Contains("cannot occur on or before the last entry", exception.Message);
    }

    [Fact]
    public void AddEntry_KeepsBalancesSeparateForDifferentAssets()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var eurAsset = new Asset("EUR");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Equity:ProfitLoss:Income:Salary");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(mainBook, cashAccount, usdAsset, 100m, 0m);
        entry.AddItem(mainBook, incomeAccount, usdAsset, 0m, 100m);
        entry.AddItem(mainBook, cashAccount, eurAsset, 60m, 0m);
        entry.AddItem(mainBook, incomeAccount, eurAsset, 0m, 60m);

        ledger.AddEntry(entry);

        var balance = Assert.IsAssignableFrom<IBalance>(ledger.GetBalanceAt(mainBook, 1));
        var usdCash = Assert.IsAssignableFrom<IBalanceItem>(balance.Items.GetBalanceItem(cashAccount, usdAsset));
        var eurCash = Assert.IsAssignableFrom<IBalanceItem>(balance.Items.GetBalanceItem(cashAccount, eurAsset));

        Assert.Equal(100m, usdCash.TotalDebit);
        Assert.Equal(0m, usdCash.TotalCredit);
        Assert.Equal(60m, eurCash.TotalDebit);
        Assert.Equal(0m, eurCash.TotalCredit);
        Assert.Equal(2, balance.Items.GetBalanceItemsCombined(new QueryAccountPredicate("Assets:**")).Count);
    }

    [Fact]
    public void EntryAndBalanceQueries_UseExpectedRangeBoundaries()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Equity:ProfitLoss:Income:Salary");
        var expenseAccount = new Account("Equity:ProfitLoss:Expense:Food");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());

        var firstEntry = new Entry
        {
            Index = 1
        };
        firstEntry.AddItem(mainBook, cashAccount, usdAsset, 100m, 0m);
        firstEntry.AddItem(mainBook, incomeAccount, usdAsset, 0m, 100m);
        ledger.AddEntry(firstEntry);

        var secondEntry = new Entry
        {
            Index = 2
        };
        secondEntry.AddItem(mainBook, expenseAccount, usdAsset, 30m, 0m);
        secondEntry.AddItem(mainBook, cashAccount, usdAsset, 0m, 30m);
        ledger.AddEntry(secondEntry);

        var thirdEntry = new Entry
        {
            Index = 3
        };
        thirdEntry.AddItem(mainBook, expenseAccount, usdAsset, 20m, 0m);
        thirdEntry.AddItem(mainBook, cashAccount, usdAsset, 0m, 20m);
        ledger.AddEntry(thirdEntry);

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
    }
}
