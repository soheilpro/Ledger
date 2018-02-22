using System;

namespace Ledger.Core
{
    public interface IEntryItem : IItem
    {
        IEntry Entry
        {
            get;
        }

        IBook Book
        {
            get;
        }

        IAsset Asset
        {
            get;
        }

        decimal Debit
        {
            get;
            set;
        }

        decimal Credit
        {
            get;
            set;
        }
    }
}
