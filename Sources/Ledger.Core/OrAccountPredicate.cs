using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class OrAccountPredicate : IAccountPredicate
    {
        private ICollection<IAccountPredicate> _predicates;

        public ICollection<IAccountPredicate> Predicates
        {
            get
            {
                return _predicates;
            }
        }

        public OrAccountPredicate(params IAccountPredicate[] predicates)
        {
            _predicates = predicates;
        }

        public bool Matches(IAccount account)
        {
            return _predicates.Any(predicate => predicate.Matches(account));
        }

        public override string ToString()
        {
            return "(" + string.Join(" OR ", _predicates.Select(predicate => predicate.ToString())) + ")";
        }
    }
}
