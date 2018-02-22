using System;

namespace Ledger.Core
{
    public class EntryItem : IEntryItem
    {
        public IEntry Entry
        {
            get;
            set;
        }

        public IBook Book
        {
            get;
            set;
        }

        public IAccount Account
		{
        	get;
        	set;
        }

        public IAsset Asset
        {
            get;
            set;
        }

        public decimal Debit
        {
            get;
            set;
        }

        public decimal Credit
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"Book:{Book} Account:{Account} Asset:{Asset} Dr:{Debit} Cr:{Credit}";
        }
    }
}
