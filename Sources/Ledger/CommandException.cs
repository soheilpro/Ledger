using System;

namespace Ledger
{
  public class CommandException : Exception
    {
        public CommandException(string message) : base(message)
        {
        }
    }
}
