using System;

namespace Ledger.Repl
{
  public class CommandException : Exception
    {
        public CommandException(string message) : base(message)
        {
        }
    }
}
