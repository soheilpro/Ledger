using Ledger.Rates;

namespace Ledger
{
    internal class RatesManager : IRatesManager
    {
        private readonly string _ratesPath;
        private IRateProvider _rateProvider;

        public IRateProvider RateProvider
        {
            get
            {
                if (_rateProvider == null)
                    ReloadRates();

                return _rateProvider;
            }
        }

        public RatesManager(string ratesPath)
        {
            _ratesPath = ratesPath;
        }

        public void ReloadRates()
        {
            _rateProvider = LoadRates();
        }

        private IRateProvider LoadRates()
        {
            return FileRateProvider.Load(_ratesPath);
        }
    }
}
