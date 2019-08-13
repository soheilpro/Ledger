using System;

namespace Ledger.Reports
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
