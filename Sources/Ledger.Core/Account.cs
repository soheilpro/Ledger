using System;
using System.Linq;

namespace Ledger.Core
{
    public class Account : IAccount
    {
        private static readonly string[] ValidCategoryPrefixes =
        {
            "Assets",
            "Liabilities",
            "Equity:Capital",
            "Equity:ProfitLoss:Income",
            "Equity:ProfitLoss:Expense"
        };

        private string _id;
        private string[] _idParts;

        public string Id {
            get
            {
                return _id;
            }
            set
            {
                _id = ValidateId(value);
                _idParts = _id.Split(':');
            }
        }

        public string[] IdParts
        {
            get
            {
                if (_idParts == null)
                    _idParts = Id.Split(':');

                return _idParts;
            }
        }

        public Account(string id)
        {
            Id = id;
        }

        private static string ValidateId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Account ID must be specified.", nameof(id));

            if (ValidCategoryPrefixes.Any(prefix => id.Equals(prefix, StringComparison.Ordinal) || id.StartsWith(prefix + ":", StringComparison.Ordinal)))
                return id;

            throw new ArgumentException($"Account ({id}) must be under one of these categories: Assets, Liabilities, Equity:Capital, Equity:ProfitLoss:Income, Equity:ProfitLoss:Expense.", nameof(id));
        }

        public override bool Equals(object other)
        {
            var otherAccount = other as Account;

            if (otherAccount == null)
                return false;

            return Id.Equals(otherAccount.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(object other)
        {
            var otherAccount = (Account)other;
            var i = 0;
            var accountPartsCount = _idParts.Length - 1;
            var otherAccountPartsCount = otherAccount._idParts.Length - 1;

            while (true)
            {
                if (i > accountPartsCount && i > otherAccountPartsCount)
                    return 0;

                if (i > accountPartsCount && i <= otherAccountPartsCount)
                    return -1;

                if (i <= accountPartsCount && i > otherAccountPartsCount)
                    return 1;

                var result = string.Compare(_idParts[i], otherAccount._idParts[i], StringComparison.Ordinal);

                if (result != 0)
                    return result;

                i++;
            }
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
