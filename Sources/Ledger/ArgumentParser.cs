using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ledger
{
    internal class ArgumentParser
    {
        public string[] Parse(string text, bool ignoreUnclosedQuotes = false)
        {
            return ParseCore(text, ignoreUnclosedQuotes).ToArray();
        }

        private IEnumerable<string> ParseCore(string text, bool ignoreUnclosedQuotes)
        {
            var argument = new StringBuilder();

            for (var i = 0; i < text.Length; i++)
            {
                var currentChar = text[i];

                if (currentChar == ' ')
                {
                    if (argument.Length > 0)
                    {
                        yield return argument.ToString();
                        argument.Clear();
                    }

                    continue;
                }

                if (currentChar == '\'' || currentChar == '\"')
                {
                    var quoteChar = currentChar;
                    int j;

                    for (j = i + 1; j < text.Length; j++)
                    {
                        currentChar = text[j];

                        if (currentChar == quoteChar)
                        {
                            i = j;
                            break;
                        }

                        argument.Append(currentChar);
                    }

                    if (j == text.Length)
                    {
                        if (ignoreUnclosedQuotes)
                            break;
                        else
                            throw new UnclosedQuotationMarkException();
                    }

                    continue;
                }

                argument.Append(currentChar);
            }

            if (argument.Length > 0)
                yield return argument.ToString();
        }

        public class UnclosedQuotationMarkException : Exception
        {
        }
    }
}
