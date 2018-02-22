using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public class DefaultEventComparer : IComparer<IEvent>
    {
        public int Compare(IEvent x, IEvent y)
        {
            return x.Index.CompareTo(y.Index);
        }
    }
}
