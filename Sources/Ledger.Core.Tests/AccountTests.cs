namespace Ledger.Core.Tests;

public class AccountTests
{
    [Theory]
    [InlineData("Assets")]
    [InlineData("Assets:Cash")]
    [InlineData("Liabilities")]
    [InlineData("Liabilities:Payable")]
    [InlineData("Equity:Capital")]
    [InlineData("Equity:Capital:Owner")]
    [InlineData("Equity:ProfitLoss:Income")]
    [InlineData("Equity:ProfitLoss:Income:Salary")]
    [InlineData("Equity:ProfitLoss:Expense")]
    [InlineData("Equity:ProfitLoss:Expense:Food")]
    public void Constructor_AcceptsSupportedAccountCategories(string accountId)
    {
        var account = new Account(accountId);

        Assert.Equal(accountId, account.Id);
    }

    [Theory]
    [InlineData("Equity")]
    [InlineData("Equity:ProfitLoss")]
    [InlineData("Income:Salary")]
    [InlineData("Expenses:Food")]
    [InlineData("Revenue:Sales")]
    public void Constructor_RejectsUnsupportedAccountCategories(string accountId)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Account(accountId));

        Assert.Contains("must be under one of these categories", exception.Message);
    }
}
