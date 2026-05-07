using Ledger.Core;

namespace Ledger.Journal
{
    public interface IRateProvider
    {
        decimal GetRate(IAsset source, IAsset destination, string index);
    }
}
