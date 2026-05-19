using System;

namespace Ledger.Commands
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
