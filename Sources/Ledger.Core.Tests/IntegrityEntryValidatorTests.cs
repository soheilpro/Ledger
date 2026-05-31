namespace Ledger.Core.Tests;

public class IntegrityEntryValidatorTests
{
    [Fact]
    public void RejectsUnbalancedEntries()
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
