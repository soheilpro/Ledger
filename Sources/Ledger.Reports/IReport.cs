using System;
using System.IO;
using Ledger.Core;

namespace Ledger.Reports
{
    public interface IReport
    {
        void Print(TextWriter writer);
    }
}
