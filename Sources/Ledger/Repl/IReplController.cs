using System;
using System.Collections.Generic;

namespace Ledger.Repl
{
    internal interface IReplController : IController
    {
        ICollection<ICommand> GetCommands();
    }
}
