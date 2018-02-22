using System;

namespace Ledger.Core
{
    public class Book : IBook
    {
        public string Id {
            get;
            set;
        }

        public Book(string id)
        {
            Id = id;
        }

        public override bool Equals(object other)
        {
            var otherBook = other as Book;

            if (otherBook == null)
                return false;

            return Id.Equals(otherBook.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
