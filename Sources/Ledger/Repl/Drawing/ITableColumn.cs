using System;

namespace Ledger.Repl.Drawing
{
    public interface ITableColumn
    {
        string Title
        {
            get;
        }

        TableColumnPadding GetPadding();

        string GetStringValue(object row);
    }
}
