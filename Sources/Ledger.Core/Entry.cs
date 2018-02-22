using System;

namespace Ledger.Core
{
    public class Entry : IEntry
    {
        protected bool Equals(Entry other)
        {
            return Index.Equals(other.Index);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Entry)obj);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public IComparable Index
        {
            get;
            set;
        }

        public IEntryItemCollection Items
        {
        	get;
        }

        public Entry()
        {
            Items = new EntryItemCollection(this);
        }

        public virtual void AddItem(IBook book, IAccount account, IAsset asset, decimal debit, decimal credit)
        {
            var entryItem = new EntryItem();
            entryItem.Entry = this;
            entryItem.Book = book;
            entryItem.Account = account;
            entryItem.Asset = asset;
            entryItem.Debit = debit;
            entryItem.Credit = credit;

            this.Items.Add(entryItem);
        }

        public virtual void AddItem(IBook book, IAccount account, IAsset asset, decimal amount)
        {
            if (amount > 0)
                AddItem(book, account, asset, amount, 0);
            else
                AddItem(book, account, asset, 0, -amount);
        }

        public virtual void AddItems(ILedger ledger, IBook book, IAccountPredicate predicate, IAccount debitAccount, IAccount creditAccount, IAsset asset, decimal debit, decimal credit)
        {
            if (debit < 0)
                throw new ArgumentException("Debit cannot be negative.", "debit");

            if (credit < 0)
                throw new ArgumentException("Credit cannot be negative.", "credit");

            if (debit == 0 && credit == 0)
                throw new ArgumentException("Must have either debit or credit specified.");

            if (debit != 0 && credit != 0)
                throw new ArgumentException("Cannot have both debit and credit specified.");

            var balanceDebit = 0M;
            var balanceCredit = 0M;

            var balance = ledger.GetLastBalance(book);

            if (balance != null)
            {
                var balanceItem = balance.Items.GetBalanceItemCombined(predicate, asset);

                if (balanceItem != null)
                {
                    balanceDebit = balanceItem.BalanceDebit;
                    balanceCredit = balanceItem.BalanceCredit;
                }
            }

            if (debit != 0)
            {
                if (balanceCredit != 0)
                {
                    if (balanceCredit >= debit)
                    {
                        AddItem(book, creditAccount, asset, debit, 0);
                    }
                    else
                    {
                        AddItem(book, creditAccount, asset, balanceCredit, 0);
                        AddItem(book, debitAccount, asset, debit - balanceCredit, 0);
                    }

                    return;
                }

                AddItem(book, debitAccount, asset, debit, 0);
            }

            if (credit != 0)
            {
                if (balanceDebit != 0)
                {
                    if (balanceDebit >= credit)
                    {
                        AddItem(book, debitAccount, asset, 0, credit);
                    }
                    else
                    {
                        AddItem(book, debitAccount, asset, 0, balanceDebit);
                        AddItem(book, creditAccount, asset, 0, credit - balanceDebit);
                    }

                    return;
                }

                AddItem(book, creditAccount, asset, 0, credit);
            }
        }

        public virtual void AddItems(ILedger ledger, IBook book, IAccountPredicate predicate, IAccount debitAccount, IAccount creditAccount, IAsset asset, decimal amount)
        {
            if (amount > 0)
                AddItems(ledger, book, predicate, debitAccount, creditAccount, asset, amount, 0);
            else
                AddItems(ledger, book, predicate, debitAccount, creditAccount, asset, 0, -amount);
        }

        public override string ToString()
        {
            return Index.ToString();
        }
    }
}
