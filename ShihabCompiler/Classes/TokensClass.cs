using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShihabCompiler.Classes
{
    class TokensClass
    {
        private static char[] delimiters_no_digits = new char[] {
            '[', ']','(',')',
            '\\', ':', '"', '.', '?', '~', '!',
            '@', '^',' ', '\r', '\n', '\t' };

        public string[] Tokenize(string text)
        {
            string[] tokens = text.Split(delimiters_no_digits,
                                    StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                // Change token only when it starts and/or ends with "'" and 
                // it has at least 2 characters.

                if (token.Length > 1)
                {
                    if (token.StartsWith("'") && token.EndsWith("'"))
                        tokens[i] = token.Substring(1, token.Length - 2);  // remove the starting and ending "'"

                    else if (token.StartsWith("'"))
                        tokens[i] = token.Substring(1); // remove the starting "'"

                    else if (token.EndsWith("'"))
                        tokens[i] = token.Substring(0, token.Length - 1); // remove the last "'"
                }
                //tokens[i] = tokens[i] + " ";
            }

            return tokens;
        }
    }
}
