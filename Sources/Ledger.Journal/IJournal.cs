using System;
using System.Collections.Generic;
using Ledger.Core;

namespace Ledger.Journal
{
    public interface IJournal
    {
        ICollection<IBalanceValidator> BalanceValidators
        {
            get;
            set;
        }

        ICollection<IEntry> Entries
        {
            get;
            set;
        }

        ICollection<IMark> Marks
        {
            get;
            set;
        }
    }
}
