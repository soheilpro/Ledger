using System;
using Ledger.Core;

namespace Ledger.Journal
{
    public interface IMark
    {
        string Name
        {
            get;
        }

        IEntry PreviousEntry
        {
            get;
        }

        IEntry NextEntry
        {
            get;
        }
    }
}
