using System;

namespace Ledger.Core
{
    public interface IAccountPredicate
    {
        bool Matches(IAccount account);
    }
}
