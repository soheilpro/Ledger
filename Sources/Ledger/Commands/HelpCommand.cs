using System;
using System.Linq;
using Ledger.Core;

namespace Ledger.Commands
{
    internal class HelpCommand : CommandBase<HelpOptions>
    {
        public override string Name
        {
            get
            {
                return "help";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Show this help screen.";
            }
        }

        public HelpCommand(IController controller) : base(controller)
        {
        }

        protected override void Execute(HelpOptions options, IContext context)
        {
            foreach (var command in Controller.GetCommands().OrderBy(command => command.Name))
            {
                using (new ColoredConsole(ConsoleColor.DarkCyan))
                    Console.Write(command.Name);

                using (new ColoredConsole(ConsoleColor.DarkCyan))
                {
                    if (!string.IsNullOrEmpty(command.Arguments))
                        Console.Write($" {command.Arguments}");

                    Console.WriteLine();
                }

                Console.WriteLine(command.HelpText);

                if (command.Aliases.Length > 0)
                {
                    Console.Write("Alias: ");

                    for (var i = 0; i < command.Aliases.Length; i++)
                    {
                        var alias = command.Aliases[i];

                        if (i > 0)
                            Console.Write(", ");

                        using (new ColoredConsole(ConsoleColor.DarkCyan))
                            Console.Write(alias);
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
    }

    internal class HelpOptions
    {
    }
}
