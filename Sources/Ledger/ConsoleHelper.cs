using System;

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
