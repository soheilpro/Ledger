using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class TableAmountColumn<TRow> : TableColumn where TRow : class
    {
        private Func<TRow, decimal> _selector;

        public TableAmountColumn(string title, Func<TRow, decimal> selector) : base(title)
        {
            _selector = selector;
        }

        public override TableColumnPadding GetPadding()
        {
            return TableColumnPadding.Right;
        }

        public override string GetStringValue(object row)
        {
            var amount = _selector((TRow)row);

            return amount.ToString("#,##0.##");
        }
    }
}
