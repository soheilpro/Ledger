using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ledger.Core;

namespace Ledger.Journal
{
    public class JournalReader
    {
        public IJournal Open(string file)
        {
            var journal = new Journal();
            var lastEntry = default(IEntry);
            var lastMarks = new List<IMark>();

            using (var lines = ((IEnumerable<string>)File.ReadAllLines(file)).GetEnumerator())
            {
                while (true)
                {
                    if (!lines.MoveNext())
                        break;

                    if (IsNullOrEmpty(lines.Current) || IsComment(lines.Current))
                        continue;

                    var directive = ReadDirective(lines.Current);

                    switch (directive.Name)
                    {
                        case "@balancevalidator":
                        {
                            var balanceValidator = ReadBalanceValidator(directive, lines);
                            journal.BalanceValidators.Add(balanceValidator);

                            break;
                        }

                        case "@entry":
                        {
                            var entry = ReadEntry(directive, lines);

                            if (journal.Entries.Any(existingEntry => existingEntry.Index.Equals(entry.Index)))
                                throw new JournalException($"Duplicate entry: {entry.Index}");

                            journal.Entries.Add(entry);

                            lastEntry = entry;

                            foreach (var mark in lastMarks)
                                ((Mark)mark).NextEntry = entry;

                            lastMarks.Clear();

                            break;
                        }

                        case "@mark":
                        {
                            var mark = ReadMark(directive, lines);

                            if (journal.Marks.Any(existingMark => existingMark.Name.Equals(mark.Name)))
                                throw new JournalException($"Duplicate mark: {mark.Name}");

                            journal.Marks.Add(mark);

                            ((Mark)mark).PreviousEntry = lastEntry;
                            lastMarks.Add(mark);

                            break;
                        }

                        default:
                        {
                            throw new JournalException($"Unknown directive: {directive.Name}");
                        }
                    }
                }
            }

            return journal;
        }

        private Directive ReadDirective(string line)
        {
            var match = new Regex(@"^\s*(?<name>@.*?)(\s+(?<data>.*))?$").Match(line);

            if (!match.Success)
                return null;

            var name = match.Groups["name"].Value;
            var data = match.Groups["data"].Value;

            return new Directive(name, data);
        }

        private IBalanceValidator ReadBalanceValidator(Directive directive, IEnumerator<string> lines)
        {
            var match = new Regex(@"^\s*(?<name>.*?)(\s+(?<arg>.*?))+\s*$").Match(directive.Data);

            var name = match.Groups["name"].Value;
            var args = match.Groups["arg"].Captures;

            switch (name)
            {
                case "AccountBalanceCreditZero":
                    return new AccountBalanceCreditZeroBalanceValidator(new Book(args[0].Value), new QueryAccountPredicate(args[1].Value));

                case "AccountBalanceDebitZero":
                    return new AccountBalanceDebitZeroBalanceValidator(new Book(args[0].Value), new QueryAccountPredicate(args[1].Value));

                case "AccountsBalance":
                    return new AccountsBalanceBalanceValidator(new Book(args[0].Value), new QueryAccountPredicate(args[1].Value), new QueryAccountPredicate(args[2].Value));

                default:
                    throw new JournalException($"Unknown balance validator: {name}");
            }
        }

        private IEntry ReadEntry(Directive directive, IEnumerator<string> lines)
        {
            var entryItemRegex = new Regex(@"^\s*((?<book>.*?)\s+)?(?<account>.*?)\s+(?<asset>.*?)\s+(?<amount>-?[\d,.]+)\s*$");
            var entry = new Entry();
            entry.Index = directive.Data;

            while (true)
            {
                if (!lines.MoveNext() || IsNullOrEmpty(lines.Current))
                    return entry;

                if (IsComment(lines.Current))
                    continue;

                directive = ReadDirective(lines.Current);

                if (directive != null)
                {
                    switch (directive.Name)
                    {
                        case "@note":
                            // TODO
                            break;

                        case "@payee":
                            // TODO
                            break;

                        default:
                            throw new JournalException($"Unknown directive: {directive.Name}");
                    }
                }
                else
                {
                    var match = entryItemRegex.Match(lines.Current);
                    var book = match.Groups["book"].Success ? new Book(match.Groups["book"].Value) : new Book("default");
                    var account = new Account(match.Groups["account"].Value);
                    var asset = new Asset(match.Groups["asset"].Value);
                    var amount = decimal.Parse(match.Groups["amount"].Value, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);

                    entry.AddItem(book, account, asset, amount);
                }
            }
        }

        private IMark ReadMark(Directive directive, IEnumerator<string> lines)
        {
            var mark = new Mark();
            mark.Name = directive.Data;

            return mark;
        }

        private bool IsNullOrEmpty(string line)
        {
            return String.IsNullOrEmpty(line) || line.Trim().Length == 0;
        }

        private bool IsComment(string line)
        {
            return line.Trim().StartsWith("#");
        }

        private class Directive
        {
            public string Name
            {
                get;
                set;
            }

            public string Data
            {
                get;
                set;
            }

            public Directive(string name, string data)
            {
                this.Name = name;
                this.Data = data;
            }
        }
    }
}
