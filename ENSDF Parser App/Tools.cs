using ENSDF_Parser.Extractor;
using System;
using System.Collections.Generic;
using System.Text;

namespace ENSDF_Parser_App
{
    internal class Tools
    {
        /// <summary>
        /// Parses expression of type arr[*args*]
        /// </summary>
        /// <param name="input">The string to be parsed</param>
        /// <returns>A pair of the keyword before the brackets (not null), and a list of the arguments (possibly null)</returns>
        public static (string KeyWord, List<string>? Args) ParseSquareBrackets(string input)
        {
            if (input == null || input == "") return ("", null);
            if (!input.Contains('[')) return (input, null);
            int i = 0;
            while (i < input.Length && input[i] != '[') i++;
            string keyword = input.Substring(0, i);
            int j = i + 1;
            while (j < input.Length && input[j] != ']') j++;
            string indices_str = input.Substring(i + 1, j - i - 1);
            List<string> indices = indices_str.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            return (keyword, indices);
        }

        public static List<string> ParsePath(string path)
        {
            return path.Replace("\"", "").Split('.').ToList();
        }

        public static string ValuesToString(List<Value> values, string template)
        {
            string text = "";
            foreach (Value v in values)
            {
                var str = v.GetString();
                text += template.Replace("val", str.val).Replace("dval", str.dval);
            }
            return text;
        }

        public static Queue<string> TokenizeInput(string input)
        {
            Queue<string> tokens = new Queue<string>();
            string current = "";
            char closingToken = ' ';

            foreach (var c in input)
            {
                if (c == closingToken)
                {
                    current += c;
                    tokens.Enqueue(current);
                    current = "";
                    closingToken = ' ';
                }
                else if (c == '"')
                {
                    closingToken = '"';
                    if (current != "") tokens.Enqueue(current);
                    current = c.ToString();
                }
                else if (c == '[')
                {
                    current += c;
                    closingToken = ']';
                }
                else if (c == '(')
                {
                    current += c;
                    closingToken = ')';
                }
                else
                {
                    current += c;
                }
            }

            return tokens;
        }
    }
}
