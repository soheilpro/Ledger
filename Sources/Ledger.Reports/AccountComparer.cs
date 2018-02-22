using System;
using System.Collections.Generic;
using System.Linq;
using Ledger.Core;

namespace Ledger.Reports
{
    public class AccountComparer : IComparer<IAccount>
    {
        private string[] _order = new[] { "Assets", "Liabilities", "Equity" };

        public int Compare(IAccount x, IAccount y)
        {
            var xAccountId = ((Account)x).Id;
            var yAccountId = ((Account)y).Id;

            var xIndex = Array.IndexOf(_order, xAccountId.Split(':').First());
            var yIndex = Array.IndexOf(_order, yAccountId.Split(':').First());

            if (xIndex != yIndex)
                return xIndex - yIndex;

            return xAccountId.CompareTo(yAccountId);
        }
    }
}
