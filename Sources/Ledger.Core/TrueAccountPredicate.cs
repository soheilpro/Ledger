using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class TrueAccountPredicate : IAccountPredicate
    {
        public TrueAccountPredicate()
        {
        }

        public bool Matches(IAccount account)
        {
            return true;
        }

        public override string ToString()
        {
            return "true";
        }
    }
}
