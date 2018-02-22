using System;

namespace Ledger.Core
{
    public interface IBalanceValidator
    {
        void Validate(IBalance balance);
    }
}
