using System;

namespace Ledger.Core
{
    public interface IAccountMapItem
    {
        IAccount Account
        {
            get;
        }

        IBook Book
        {
            get;
        }

        IAccountPredicate AccountPredicate
        {
            get;
        }
    }
}
