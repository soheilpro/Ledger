using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class TableTextColumn<TRow> : TableColumn where TRow : class
    {
        private Func<TRow, object> _selector;

        public TableTextColumn(string title, Func<TRow, object> selector) : base(title)
        {
            _selector = selector;
        }

        public override string GetStringValue(object row)
        {
            return _selector((TRow)row).ToString();
        }
    }
}
