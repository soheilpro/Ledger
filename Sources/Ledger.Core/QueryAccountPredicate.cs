using System;

namespace Ledger.Core
{
    public class QueryAccountPredicate : IAccountPredicate
    {
        private string _query;
        private string[] _queryParts;

        public string Query
        {
            get;
        }

        public string[] QueryParts
        {
            get
            {
                return _queryParts;
            }
        }

        public QueryAccountPredicate(string query)
        {
            _query = query;
            _queryParts = query.Split(':');
        }

        public bool Matches(IAccount account)
        {
            // Performance.MarkStart("QueryAccountPredicate.Match");

            var result = MatchesCore(account);

            // Performance.MarkEnd("QueryAccountPredicate.Match");

            return result;
        }

        private bool MatchesCore(IAccount account)
        {
            var idParts = ((Account)account).Id.Split(':');
            var index = -1;

            while (true)
            {
                index++;

                // We've reached the end of query parts.
                // If there are no more id parts, it's a match.
                if (index == _queryParts.Length)
                    return (index == idParts.Length);

                // We've reached the end of id parts.
                // If last query part is **, then it's a match.
                // This is to allow "a:**" to match "a". To disable it, always return false.
                if (index == idParts.Length)
                    return _queryParts[index].Equals("**", StringComparison.Ordinal);

                // And nothing else matters.
                if (_queryParts[index].Equals("**", StringComparison.Ordinal))
                    return true;

                // Move on to the next part.
                if (_queryParts[index].Equals("*", StringComparison.Ordinal))
                    continue;

                // Easy.
                var matches = false;

                foreach (var queryOrPart in _queryParts[index].Split('|'))
                {
                    // If it starts with ^ is should not equal to match.
                    if (queryOrPart.StartsWith("^", StringComparison.Ordinal))
                    {
                        if (!queryOrPart.Substring(1).Equals(idParts[index], StringComparison.Ordinal))
                        {
                            matches = true;
                            break;
                        }
                    }

                    if (queryOrPart.Equals(idParts[index], StringComparison.Ordinal))
                    {
                        matches = true;
                        break;
                    }
                }

                if (!matches)
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return (_query != null ? _query.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            return Equals((QueryAccountPredicate)obj);
        }

        protected bool Equals(QueryAccountPredicate other)
        {
            return string.Equals(_query, other._query);
        }

        public override string ToString()
        {
            return _query;
        }
    }
}
