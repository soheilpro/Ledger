using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;

namespace Ledger.Reports
{
    public class AssetComparer : IComparer<IAsset>
    {
        public int Compare(IAsset x, IAsset y)
        {
            var xAssetId = ((Asset)x).Id;
            var yAssetId = ((Asset)y).Id;

            return xAssetId.CompareTo(yAssetId);
        }
    }
}
