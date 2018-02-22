using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Ledger.Core;

namespace Ledger.Journal
{
    public class Journal : IJournal
    {
        public ICollection<IBalanceValidator> BalanceValidators
        {
            get;
            set;
        } = new List<IBalanceValidator>();

        public ICollection<IEntry> Entries
        {
            get;
            set;
        } = new List<IEntry>();

        public ICollection<IMark> Marks
        {
            get;
            set;
        } = new List<IMark>();
    }
}
