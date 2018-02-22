using System;
using Ledger.Core;

namespace Ledger.Reports
{
    public abstract class TableColumn : ITableColumn
    {
        public string Title
        {
            get;
            private set;
        }

        public TableColumn(string title)
        {
            Title = title;
        }

        public virtual TableColumnPadding GetPadding()
        {
            return TableColumnPadding.Left;
        }

        public abstract string GetStringValue(object row);
    }
}
