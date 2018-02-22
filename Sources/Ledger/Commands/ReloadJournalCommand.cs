using System;
using Ledger.Journal;

namespace Ledger.Commands
{
    internal class ReloadJournalCommand : CommandBase<ReloadJournalOptions>
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
                return new[] {
                    "r",
                };
            }
        }

        public override string HelpText
        {
            get
            {
                return "Reload the journal.";
            }
        }

        public ReloadJournalCommand(IController controller) : base(controller)
        {
        }

        protected override void Execute(ReloadJournalOptions options, IContext context)
        {
            context.JournalManager.ReloadJournal();
        }
    }

    internal class ReloadJournalOptions
    {
    }
}
