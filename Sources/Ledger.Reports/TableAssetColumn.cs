using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class TableAssetColumn<TRow> : TableColumn where TRow : class
    {
        private Func<TRow, IAsset> _selector;

        public TableAssetColumn(string title, Func<TRow, IAsset> selector) : base(title)
        {
            _selector = selector;
        }

        public override string GetStringValue(object row)
        {
            var asset = _selector((TRow)row) as Asset;

            return asset.Id;
        }
    }
}
