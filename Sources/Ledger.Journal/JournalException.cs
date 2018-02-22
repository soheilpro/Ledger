using System;

namespace Ledger.Journal
{
  public class JournalException : Exception
    {
        public JournalException(string message) : base(message)
        {
        }
    }
}
