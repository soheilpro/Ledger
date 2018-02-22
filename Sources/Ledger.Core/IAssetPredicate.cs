using System;

namespace Ledger.Core
{
    public interface IAssetPredicate
    {
        bool Matches(IAsset Asset);
    }
}
