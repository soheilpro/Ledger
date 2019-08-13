using System;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public class OpeningEntryReport : ReportBase
    {
        private IEntryItem[] _entryItems;

        public OpeningEntryReport(IEntryItem[] entryItems)
        {
            _entryItems = entryItems;
        }

        public override void Print(TextWriter writer)
        {
            writer.WriteLine("@entry ");
            writer.WriteLine("@note Opening Entry");

            foreach (var entryItem in _entryItems)
                writer.WriteLine($"{entryItem.Account} {entryItem.Asset} {entryItem.Debit - entryItem.Credit}");
        }
    }
}
