using System;

namespace Ledger.Core
{
    public interface IEntry : IIndexable
    {
        EntryType Type
        {
            get;
        }

        IEntryItemCollection Items
        {
        	get;
        }
    }
}
