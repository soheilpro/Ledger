using Ledger.Core;

namespace Ledger.Rates
{
    public interface IRateProvider
    {
        decimal GetRate(IAsset source, IAsset destination, string index);
    }
}
