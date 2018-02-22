using System;
using System.IO;
using System.Linq;
using Ledger.Core;
using Ledger.Journal;

namespace Ledger.Reports
{
    public abstract class ReportBase : IReport
    {
        public abstract void Print(TextWriter writer);
    }
}
