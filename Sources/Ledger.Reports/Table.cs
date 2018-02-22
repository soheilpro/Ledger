using System;
using System.Collections.Generic;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public class Table : ITable
    {
        public IList<ITableColumn> Columns
        {
            get;
        } = new List<ITableColumn>();

        public IList<object> Rows
        {
            get;
            set;
        }

        public void PrintText(TextWriter writer)
        {
            const string ColumnSeparator = " | ";
            const char RowSeparator = '-';

            var values = new string[Rows.Count + 1, Columns.Count];

            // Get values
            for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                for (var rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
                    values[rowIndex, columnIndex] = Columns[columnIndex].GetStringValue(Rows[rowIndex]);

            // Calculate max column widths
            var maxColumnWidths = new int[Columns.Count];

            for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
            {
                maxColumnWidths[columnIndex] = Math.Max(maxColumnWidths[columnIndex], Columns[columnIndex].Title.Length);

                for (var rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
                    maxColumnWidths[columnIndex] = Math.Max(maxColumnWidths[columnIndex], values[rowIndex, columnIndex].Length);
            }

            // Header title
            for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
            {
                var column = Columns[columnIndex];

                writer.Write(Pad(column.Title, maxColumnWidths[columnIndex], TableColumnPadding.Center));

                if (columnIndex != Columns.Count - 1)
                    writer.Write(ColumnSeparator);
            }

            writer.WriteLine();

            // Header line
            for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
            {
                var column = Columns[columnIndex];

                writer.Write(new String(RowSeparator, maxColumnWidths[columnIndex]));

                if (columnIndex != Columns.Count - 1)
                    writer.Write(ColumnSeparator);
            }

            writer.WriteLine();

            // Rows
            for (var rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                {
                    var value = values[rowIndex, columnIndex];

                    writer.Write(Pad(value, maxColumnWidths[columnIndex], Columns[columnIndex].GetPadding()));

                    if (columnIndex != Columns.Count - 1)
                        writer.Write(ColumnSeparator);
                }

                writer.WriteLine();
            }
        }

        private string Pad(string value, int width, TableColumnPadding padding)
        {
            if (padding == TableColumnPadding.Left)
                return value.PadRight(width);

            if (padding == TableColumnPadding.Center)
            {
                if (value.Length < width && value.Length % 2 != 0)
                    value = " " + value;

                return value.PadLeft(width / 2 + value.Length / 2).PadRight(width);
            }

            if (padding == TableColumnPadding.Right)
                return value.PadLeft(width);

            throw new NotSupportedException();
        }
    }
}
