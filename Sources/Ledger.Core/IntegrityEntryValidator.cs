using System;
using System.Collections.Generic;

namespace Ledger.Core
{
    public class IntegrityEntryValidator : IEntryValidator
    {
        public void Validate(IEntry entry)
        {
            if (entry.Index == null)
                throw new ValidationException($"Entry ({entry}) must have an index specified.");

            var bookAssetBalance = new Dictionary<Tuple<IBook, IAsset>, decimal>();

            foreach (var entryItem in entry.Items)
            {
                if (entryItem.Book == null)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) must have a book specified.");

                if (entryItem.Account == null)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) must have an account specified.");

                if (entryItem.Asset == null)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) must have an asset specified.");

                if (entryItem.Debit < 0)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) debit cannot be negative.");

                if (entryItem.Credit < 0)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) credit cannot be negative.");

                if (entryItem.Debit == 0 && entryItem.Credit == 0)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) must have either debit or credit specified.");

                if (entryItem.Debit != 0 && entryItem.Credit != 0)
                    throw new ValidationException($"Entry ({entry}) item ({entryItem}) cannot have both debit and credit specified.");

                var key = Tuple.Create(entryItem.Book, entryItem.Asset);
                decimal balance;

                bookAssetBalance.TryGetValue(key, out balance);
                bookAssetBalance[key] = balance + (entryItem.Debit - entryItem.Credit);
            }

            foreach (var keyValuePair in bookAssetBalance)
                if (keyValuePair.Value != 0)
                    throw new ValidationException($"Entry ({entry}) is not balanced.");
        }
    }
}
