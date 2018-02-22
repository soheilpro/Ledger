using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public interface IEventProvider
    {
        IEnumerable<IEvent> GetEvents();
    }
}
