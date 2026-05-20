using System;

namespace Ledger.Repl.Commands
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
