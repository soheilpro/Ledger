using System;
using System.Collections.Generic;
using System.IO;

namespace Ledger.Repl.Drawing
{
    public interface ITable
    {
        IList<ITableColumn> Columns
        {
            get;
        }

        IList<object> Rows
        {
            get;
        }

        void PrintText(TextWriter writer);
    }
}
