namespace Ledger.Core.Tests;

public class QueryAccountPredicateTests
{
    [Fact]
    public void SupportsWildcardOrAndNegationMatching()
    {
        var cashCheckingAccount = new Account("Assets:Cash:Checking");

        Assert.True(new QueryAccountPredicate("Assets:**").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:*:Checking").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:Cash|Bank:Checking").Matches(cashCheckingAccount));
        Assert.True(new QueryAccountPredicate("Assets:^Savings:Checking").Matches(cashCheckingAccount));
        Assert.False(new QueryAccountPredicate("Assets:^Cash:Checking").Matches(cashCheckingAccount));
        Assert.False(new QueryAccountPredicate("Liabilities:**").Matches(cashCheckingAccount));
    }
}
