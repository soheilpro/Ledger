using System;
using Ledger.Core;

namespace Ledger.Repl.Drawing
{
    public class TableAccountColumn<TRow> : TableColumn where TRow : class
    {
        private readonly Func<TRow, IAccount> _selector;

        public TableAccountColumn(string title, Func<TRow, IAccount> selector) : base(title)
        {
            _selector = selector;
        }

        public override string GetStringValue(object row)
        {
            var account = _selector((TRow)row) as Account;

            if (account == null)
                return string.Empty;

            return account.Id;
        }
    }
}
