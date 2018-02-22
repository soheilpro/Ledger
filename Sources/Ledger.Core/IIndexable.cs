using System;

namespace Ledger.Core
{
    public interface IIndexable
    {
        IComparable Index
        {
            get;
        }
    }
}
