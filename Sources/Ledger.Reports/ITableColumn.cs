using System;
using Ledger.Core;

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
