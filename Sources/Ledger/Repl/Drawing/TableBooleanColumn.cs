using System;

namespace Ledger.Repl.Drawing
{
    public class TableBooleanColumn<TRow> : TableColumn where TRow : class
    {
        private readonly Func<TRow, bool> _selector;
        private readonly string _trueText;
        private readonly string _falseText;

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
