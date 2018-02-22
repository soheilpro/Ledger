using System;
using System.Collections.Generic;

namespace Ledger
{
    internal interface IController
    {
        ICollection<ICommand> GetCommands();

        void Run();
    }
}
