using System;
using System.IO;

namespace Ledger.Reports
{
    public interface IReport
    {
        void Print(TextWriter writer);
    }
}
