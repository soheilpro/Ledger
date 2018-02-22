using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class AndAccountPredicate : IAccountPredicate
    {
        private ICollection<IAccountPredicate> _predicates;

        public ICollection<IAccountPredicate> Predicates
        {
            get
            {
                return _predicates;
            }
        }

        public AndAccountPredicate(params IAccountPredicate[] predicates)
        {
            _predicates = predicates;
        }

        public bool Matches(IAccount account)
        {
            return _predicates.All(predicate => predicate.Matches(account));
        }

        public override string ToString()
        {
            return "(" + string.Join(" AND ", _predicates.Select(predicate => predicate.ToString())) + ")";
        }
    }
}
