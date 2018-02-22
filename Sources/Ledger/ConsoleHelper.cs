using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;

namespace Ledger
{
    internal static class ConsoleHelper
    {
        public static void PrintError(string message)
        {
            using (new ColoredConsole(ConsoleColor.Red))
                Console.WriteLine(message);
        }
    }
}
