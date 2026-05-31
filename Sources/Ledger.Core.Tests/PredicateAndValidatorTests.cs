namespace Ledger.Core.Tests;

public class PredicateAndValidatorTests
{
    [Fact]
    public void QueryAccountPredicate_SupportsWildcardOrAndNegationMatching()
    {
        var account = new Account("assets:cash:checking");

        Assert.True(new QueryAccountPredicate("assets:**").Matches(account));
        Assert.True(new QueryAccountPredicate("assets:*:checking").Matches(account));
        Assert.True(new QueryAccountPredicate("assets:cash|bank:checking").Matches(account));
        Assert.True(new QueryAccountPredicate("assets:^savings:checking").Matches(account));
        Assert.False(new QueryAccountPredicate("assets:^cash:checking").Matches(account));
        Assert.False(new QueryAccountPredicate("liabilities:**").Matches(account));
    }

    [Fact]
    public void IntegrityEntryValidator_RejectsUnbalancedEntries()
    {
        var mainBook = new Book("main");
        var usd = new Asset("USD");
        var cash = new Account("assets:cash");
        var entry = new Entry
        {
            Index = 1
        };

        entry.AddItem(mainBook, cash, usd, 10m, 0m);

        var exception = Assert.Throws<ValidationException>(() => new IntegrityEntryValidator().Validate(entry));

        Assert.Contains("is not balanced", exception.Message);
    }
}
