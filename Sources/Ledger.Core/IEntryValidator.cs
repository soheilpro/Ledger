using System;

namespace Ledger.Core
{
    public interface IEntryValidator
    {
        void Validate(IEntry entry);
    }
}
