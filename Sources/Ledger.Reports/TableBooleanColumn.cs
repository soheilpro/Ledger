using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public class TableBooleanColumn<TRow> : TableColumn where TRow : class
    {
        private Func<TRow, bool> _selector;
        private string _trueText;
        private string _falseText;

        public TableBooleanColumn(string title, Func<TRow, bool> selector, string trueText, string falseText) : base(title)
        {
            _selector = selector;
            _trueText = trueText;
            _falseText = falseText;
        }

        public override TableColumnPadding GetPadding()
        {
            return TableColumnPadding.Center;
        }

        public override string GetStringValue(object row)
        {
            return _selector((TRow)row) ? _trueText : _falseText;
        }
    }
}
