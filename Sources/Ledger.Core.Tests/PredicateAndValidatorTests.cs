namespace Ledger.Core.Tests;

public class PredicateAndValidatorTests
{
    [Fact]
    public void QueryAccountPredicate_SupportsWildcardOrAndNegationMatching()
    {
        var cashCheckingAccount = new Account("Assets:Cash:Checking");

        Assert.True(new QueryAccountPredicate("Assets:**").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:*:Checking").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:Cash|Bank:Checking").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:^Savings:Checking").Matches(cashCheckingAccount));
        Assert.False(new QueryAccountPredicate("Assets:^Cash:Checking").Matches(cashCheckingAccount));
        Assert.False(new QueryAccountPredicate("Liabilities:**").Matches(cashCheckingAccount));
    }

    [Fact]
    public void IntegrityEntryValidator_RejectsUnbalancedEntries()
    {
        var mainBook = new Book("main");
        var usdAsset = new Asset("USD");
        var cashAccount = new Account("Assets:Cash");
        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(mainBook, cashAccount, usdAsset, 10m, 0m);

        var exception = Assert.Throws<ValidationException>(() => new IntegrityEntryValidator().Validate(entry));

        Assert.Contains("is not balanced", exception.Message);
    }
}
