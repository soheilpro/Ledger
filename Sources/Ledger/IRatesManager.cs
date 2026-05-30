using Ledger.Rates;

namespace Ledger
{
    internal interface IRatesManager
    {
        IRateProvider RateProvider
        {
            get;
        }

        void ReloadRates();
    }
}
