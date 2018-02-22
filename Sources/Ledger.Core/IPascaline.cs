using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IPascaline
    {
        ICollection<IEventProvider> EventProviders
        {
            get;
        }

        IComparer<IEvent> EventComparer
        {
            get;
        }

        void AddEvents(ILedger ledger);
    }
}
