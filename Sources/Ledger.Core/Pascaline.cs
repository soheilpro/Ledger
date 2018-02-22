using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public class Pascaline : IPascaline
    {
        public Pascaline()
        {
            EventProviders = new List<IEventProvider>();
            EventComparer = new DefaultEventComparer();
        }

        public ICollection<IEventProvider> EventProviders
        {
            get;
            set;
        }

        public IComparer<IEvent> EventComparer
        {
            get;
            set;
        }

        public void AddEvents(ILedger ledger)
        {
            // Performance.MarkStart("Pascaline.AddEvents:GetEvents");

            var events = new List<IEvent>();

            foreach (var eventProvider in EventProviders)
            {
                // Performance.MarkStart($"Pascaline.AddEvents:GetEvents({eventProvider})");

                events.AddRange(eventProvider.GetEvents().ToList());

                // Performance.MarkEnd($"Pascaline.AddEvents:GetEvents({eventProvider})");
            }

            // Performance.MarkEnd("Pascaline.AddEvents:GetEvents");

            // Performance.MarkStart("Pascaline.AddEvents:Sort");

            events.Sort(EventComparer);

            // Performance.MarkEnd("Pascaline.AddEvents:Sort");

            // Performance.MarkStart("Pascaline.AddEvents:AddEntries");

            foreach (var evnt in events)
            {
                // Performance.MarkStart($"Pascaline.AddEvents:AddEntries({evnt})");

                var entries = evnt.GetEntries(ledger);

                foreach (var entry in entries)
                {
                    if (!entry.Items.Any())
                        continue;

                    ledger.AddEntry(entry);
                }

                // Performance.MarkEnd($"Pascaline.AddEvents:AddEntries({evnt})");
            }

            // Performance.MarkEnd("Pascaline.AddEvents:AddEntries");
        }
    }
}
