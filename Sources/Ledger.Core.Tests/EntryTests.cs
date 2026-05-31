namespace Ledger.Core.Tests;

public class EntryTests
{
    [Fact]
    public void AddItem_MapsSignedAmountsToDebitAndCredit()
    {
        var mainBook = new Book("main");
        var usd = new Asset("USD");
        var cash = new Account("assets:cash");
        var income = new Account("income:salary");
        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(mainBook, cash, usd, 25m);
        entry.AddItem(mainBook, income, usd, -25m);

        var cashItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, cash, usd));
        var incomeItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, income, usd));

        Assert.Equal(25m, cashItem.Debit);
        Assert.Equal(0m, cashItem.Credit);
        Assert.Equal(0m, incomeItem.Debit);
        Assert.Equal(25m, incomeItem.Credit);
    }

    [Fact]
    public void AddItems_UsesExistingCreditBalanceBeforeDebitAccount()
    {
        var mainBook = new Book("main");
        var usd = new Asset("USD");
        var cash = new Account("assets:cash");
        var payable = new Account("liabilities:payable");
        var prepaid = new Account("assets:prepaid");
        var ledger = new Ledger();
        ledger.EntryValidators.Add(new IntegrityEntryValidator());
        ledger.AddEntry(CreateBalancedEntry(
            1,
            (cash, 100m, 0m),
            (payable, 0m, 100m)));

        var entry = new Entry
        {
            Index = 2
        };

        entry.AddItems(
            ledger,
            mainBook,
            new QueryAccountPredicate("liabilities:**"),
            prepaid,
            payable,
            usd,
            debit: 150m,
            credit: 0m);

        var payableItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, payable, usd));
        var prepaidItem = Assert.IsAssignableFrom<IEntryItem>(entry.Items.GetEntryItem(mainBook, prepaid, usd));

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
                balancedEntry.AddItem(mainBook, item.Account, usd, item.Debit, item.Credit);

            return balancedEntry;
        }
    }

    [Fact]
    public void AddItems_RejectsInvalidAmounts()
    {
        var mainBook = new Book("main");
        var usd = new Asset("USD");
        var payable = new Account("liabilities:payable");
        var prepaid = new Account("assets:prepaid");
        var entry = new Entry
        {
            Index = 1
        };

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaid,
            payable,
            usd,
            debit: -1m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaid,
            payable,
            usd,
            debit: 0m,
            credit: 0m));

        Assert.Throws<ArgumentException>(() => entry.AddItems(
            new Ledger(),
            mainBook,
            new TrueAccountPredicate(),
            prepaid,
            payable,
            usd,
            debit: 1m,
            credit: 1m));
    }
}
