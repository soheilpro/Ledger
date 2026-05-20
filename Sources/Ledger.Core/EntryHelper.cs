using System;
using System.Collections.Generic;
using System.Linq;

namespace Ledger.Core
{
    public static class EntryHelper
    {
        public static IEnumerable<IEntryItem> GetEntryItems(this IEnumerable<IEntry> entries, IBook book, IAccountPredicate predicate)
        {
            return entries.SelectMany(x => x.Items).Where(x => x.Book.Equals(book) && predicate.Matches(x.Account));
        }
    }
}
