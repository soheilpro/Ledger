using System;

namespace Ledger.Core
{
    public interface IBalance : IIndexable
    {
        IBook Book
        {
            get;
        }

        IBalanceItemCollection Items
        {
            get;
        }
    }
}
