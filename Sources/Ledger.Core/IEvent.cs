using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IEvent
    {
        IComparable Index
        {
            get;
        }

        IEnumerable<IEntry> GetEntries(ILedger ledger);
    }
}
