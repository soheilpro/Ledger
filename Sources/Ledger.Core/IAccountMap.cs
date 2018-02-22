using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IAccountMap
    {
        ICollection<IAccountMapItem> Items
        {
            get;
        }
    }
}
