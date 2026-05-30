using System;

namespace Ledger.Repl.Commands
{
    internal class ReloadCommand : CommandBase<ReloadOptions>
    {
        public override string Name
        {
            get
            {
                return "reload";
            }
        }

        public override string[] Aliases
        {
            get
            {
                return [
                    "r",
                ];
            }
        }

        public override string HelpText
        {
            get
            {
                return "Reload the journal and rates files.";
            }
        }

        public ReloadCommand(IReplController controller) : base(controller)
        {
        }

        protected override void Execute(ReloadOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();
            context.RatesManager.ReloadRates();
        }
    }

    internal class ReloadOptions
    {
    }
}
