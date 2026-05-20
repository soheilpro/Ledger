using System;

namespace Ledger.Repl.Commands
{
    internal class ExitCommand : CommandBase<ExitOptions>
    {
        public override string Name
        {
            get
            {
                return "exit";
            }
        }

        public override string HelpText
        {
            get
            {
                return "Quit.";
            }
        }

        public ExitCommand(IReplController controller) : base(controller)
        {
        }

        protected override void Execute(ExitOptions options, IContext context)
        {
            context.ShouldExit = true;
        }
    }

    internal class ExitOptions
    {
    }
}
