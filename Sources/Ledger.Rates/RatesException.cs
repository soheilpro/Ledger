using System;

namespace Ledger.Rates
{
  public class RatesException : Exception
    {
        public RatesException(string message) : base(message)
        {
        }
    }
}
