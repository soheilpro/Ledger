namespace Ledger.Core.Tests;

public class EntryTests
{
    [Fact]
    public void AddItem_MapsSignedAmountsToDebitAndCredit()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var incomeAccount = new Account("Income:Salary");
        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(mainBook, cashAccount, usdAsset, 25m);
        entry.AddItem(mainBook, incomeAccount, usdAsset, -25m);

        var cashItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, cashAccount, usdAsset));
        var incomeItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, incomeAccount, usdAsset));

        Assert.Equal(25m, cashItem.Debit);
        Assert.Equal(0m, cashItem.Credit);
        Assert.Equal(0m, incomeItem.Debit);
        Assert.Equal(25m, incomeItem.Credit);
    }

    [Fact]
    public void AddItems_UsesExistingCreditBalanceBeforeDebitAccount()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var payableAccount = new Account("Liabilities:Payable");
        var prepaidAccount = new Account("Assets:Prepaid");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());
        ledger.AddEntry(CreateBalancedEntry(
            1,
            (cashAccount, 100m, 0m),
            (payableAccount, 0m, 100m)));

        var entry = new Entry
        {
            Index = 2
        };

        entry.AddItems(
            ledger,
            mainBook,
            new QueryAccountPredicate("Liabilities:**"),
            prepaidAccount,
            payableAccount,
            usdAsset,
            debit: 150m,
            credit: 0m);

        var payableItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, payableAccount, usdAsset));
        var prepaidItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, prepaidAccount, usdAsset));

        Assert.Equal(100m, payableItem.Debit);
        Assert.Equal(0m, payableItem.Credit);
        Assert.Equal(50m, prepaidItem.Debit);
        Assert.Equal(0m, prepaidItem.Credit);

        Entry CreateBalancedEntry(int index, params (Account Account, decimal Debit, decimal Credit)[] items)
        {
            var balancedEntry = new Entry
            {
                Index = index
            };

            foreach (var item in items)
                balancedEntry.AddItem(mainBook, item.Account, usdAsset, item.Debit, item.Credit);

            return balancedEntry;
        }
    }

    [Fact]
    public void AddItems_UsesOnlyBalancesForTheSpecifiedAsset()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var eurAsset = new Asset("EUR");
        var cashAccount = new Account("Assets:Cash");
        var payableAccount = new Account("Liabilities:Payable");
        var prepaidAccount = new Account("Assets:Prepaid");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());
        ledger.AddEntry(CreateBalancedEntry(
            1,
            (cashAccount, eurAsset, 100m, 0m),
            (payableAccount, eurAsset, 0m, 100m)));

        var entry = new Entry
        {
            Index = 2
        };

        entry.AddItems(
            ledger,
            mainBook,
            new QueryAccountPredicate("Liabilities:**"),
            prepaidAccount,
            payableAccount,
            usdAsset,
            debit: 40m,
            credit: 0m);

        Assert.Null(entry.Items.GetEntryItem(mainBook, payableAccount, usdAsset));

        var prepaidItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, prepaidAccount, usdAsset));
        Assert.Equal(40m, prepaidItem.Debit);
        Assert.Equal(0m, prepaidItem.Credit);

        Entry CreateBalancedEntry(int index, params (Account Account, Asset Asset, decimal Debit, decimal Credit)[] items)
        {
            var balancedEntry = new Entry
            {
                Index = index
            };

            foreach (var item in items)
                balancedEntry.AddItem(mainBook, item.Account, item.Asset, item.Debit, item.Credit);

            return balancedEntry;
        }
    }

    [Fact]
    public void AddItems_RejectsInvalidAmounts()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var payableAccount = new Account("Liabilities:Payable");
        var prepaidAccount = new Account("Assets:Prepaid");
        var entry = new Entry
        {
            Index = 1
        };

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaidAccount,
            payableAccount,
            usdAsset,
            debit: -1m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaidAccount,
            payableAccount,
            usdAsset,
            debit: 0m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaidAccount,
            payableAccount,
            usdAsset,
            debit: 1m,
            credit: 1m));
    }
}
