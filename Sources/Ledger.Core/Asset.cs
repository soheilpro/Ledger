using System;

namespace Ledger.Core
{
    public class Asset : IAsset
    {
        public string Id {
            get;
            set;
        }

        public Asset(string id)
        {
            Id = id;
        }

        public override bool Equals(object other)
        {
            var otherAsset = other as Asset;

            if (otherAsset == null)
                return false;

            return Id.Equals(otherAsset.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
