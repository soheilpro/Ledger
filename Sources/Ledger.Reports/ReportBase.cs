using System;
using System.IO;

namespace Ledger.Reports
{
    public abstract class ReportBase : IReport
    {
        public abstract void Print(TextWriter writer);
    }
}
