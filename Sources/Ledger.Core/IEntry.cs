using System;

namespace Ledger.Core
{
    public interface IEntry : IIndexable
    {
        IEntryItemCollection Items
        {
        	get;
        }
    }
}
