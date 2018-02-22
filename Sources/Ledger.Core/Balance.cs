using System;

namespace Ledger.Core
{
    public class Balance : IBalance
    {
        public IBook Book
        {
            get;
            set;
        }

        public IComparable Index
        {
            get;
            set;
        }

        public IBalanceItemCollection Items
        {
            get;
        }

        public Balance()
        {
            Items = new BalanceItemCollection(this);
        }

        public Balance(IBalanceItemCollection items)
        {
            Items = new BalanceItemCollection(this, items);
        }

        public override string ToString()
        {
            return $"Book:{Book} Index:{Index}";
        }
    }
}
