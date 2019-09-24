using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core
{
    public class Utils
    {
        public static Parser.Result GetItem(string data, ref int from, bool expectInt = false)
        {
            Parser.Result value = new Parser.Result();
            if (data.Length <= from)
            {
                throw new ArgumentException("Incomplete function definition");
            }
            if (data[from] == Constants.NEXT_ARG)
            {
                from++;
            }

            // Utils.GoToNextStatement(data, ref from);
            if (from < data.Length && data[from] == Constants.QUOTE)
            {
                from++; // skip first quote
                if (from < data.Length && data[from] == Constants.QUOTE)
                { // the argument is ""
                    value.String = "";
                }
                else
                {
                    value.String = Utils.GetToken(data, ref from, Constants.QUOTE_ARRAY);
                }
                from++; // skip next separation char
            }
            else
            {
                Parser.Result existing = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_OR_END_ARRAY);
                value.Copy(existing);
            }

            if (expectInt && Double.IsNaN(value.Value))
            {
                throw new ArgumentException("Integer expected instead of [" + value.String + "]");
            }

            if (from < data.Length && data[from] == Constants.END_ARG)
            {
                from++;
            }
            return value;
        }

        public static string GetToken(string data, ref int from, char[] to)
        {
            if (data.Length <= from + 1)
            {
                throw new ArgumentException("End of line before extracting token");
            }
            if (data[from] == Constants.QUOTE)
            {
                from++;
            }

            int end = data.IndexOfAny(to, from);

            if (from == end)
            {
                from++;
                return string.Empty;
            }

            // Skip found characters that have a backslash before.
            while ((end > 0 && data[end - 1] == '\\') && end + 1 < data.Length)
            {
                end = data.IndexOfAny(to, end + 1);
            }

            if (end < from)
            {
                throw new ArgumentException("Couldn't extract token from " + data.Substring(from));
            }

            if (data[end - 1] == Constants.QUOTE)
            {
                end--;
            }

            string var = data.Substring(from, end - from);
            // \"yes\" --> "yes"
            var = var.Replace("\\\"", "\"");
            from = end + 1;

            return var;
        }

        public static string GetNextToken(string data, ref int from)
        {
            if (from >= data.Length)
            {
                return "";
            }

            int end = data.IndexOfAny(Constants.TOKEN_SEPARATION, from);

            if (end < from)
            {
                return "";
            }

            string var = data.Substring(from, end - from);
            from = end;
            return var;
        }

        public static int GoToNextStatement(string data, ref int from)
        {
            int endGroupRead = 0;
            while (from < data.Length)
            {
                char currentChar = data[from];
                switch (currentChar)
                {
                    case Constants.END_GROUP: endGroupRead++;
                        from++;
                        return endGroupRead;
                    case Constants.START_GROUP:
                    case Constants.QUOTE:
                    case Constants.END_STATEMENT:
                    case Constants.END_ARG: from++;
                        break;
                    default: return endGroupRead;
                }
            }
            return endGroupRead;
        }

        public static string GetComparison(string data, ref int from)
        {
            if (data.Length <= from + 1)
            {
                throw new ArgumentException("End of line before extracting comparison token");
            }

            if (!Utils.IsCompareSign(data[from + 1]))
            {
                if (data[from] == '<' || data[from] == '>')
                {
                    return data.Substring(from++, 1);
                }
                throw new ArgumentException("Couldn't extract comparison token from " + data.Substring(from));
            }

            string result = data.Substring(from, 2);
            from++; from++;
            return result;
        }

        public static bool IsCompareSign(char ch)
        {
            return ch == '<' || ch == '>' || ch == '=';
        }

        public static bool IsAndOrSign(char ch)
        {
            return ch == '&' || ch == '|';
        }

        // Checks whether there is an argument separator (e.g.  ',') before the end of the
        // function call. E.g. returns true for "a,b)" and "a(b,c),d)" and false for "b),c".
        public static bool SeparatorExists(string data, int from)
        {
            if (from >= data.Length)
            {
                return false;
            }

            int argumentList = 0;
            for (int i = from; i < data.Length; i++)
            {
                char ch = data[i];
                switch (ch)
                {
                    case Constants.NEXT_ARG:
                        return true;
                    case Constants.START_ARG:
                        argumentList++;
                        break;
                    case Constants.END_STATEMENT:
                    case Constants.END_GROUP:
                    case Constants.END_ARG:
                        if (--argumentList < 0)
                        {
                            return false;
                        }
                        break;
                }
            }

            return false;
        }

        public static List<string> GetFunctionArgs(string data, ref int from)
        {
            List<string> args = new List<string>();
            bool moreArgs = true;
            while (moreArgs)
            {
                moreArgs = Utils.SeparatorExists(data, from);
                Parser.Result item = Utils.GetItem(data, ref from);

                // Separate treatment for an array.
                // Only 1-dimensional arrays are supported at the moment.
                if (item.Tuple != null && item.Tuple.Count > 0)
                {
                    for (int i = 0; i < item.Tuple.Count; i++)
                    {
                        Parser.Result arg = item.Tuple[i];
                        args.Add((arg.String != null ? arg.String : arg.Value.ToString()) + '\n');
                    }
                    continue;
                }

                if (item.Token != null)
                {
                    args.Add(item.Token.ToString());
                }

                if (item.String == null && Double.IsNaN(item.Value))
                {
                    break;
                }
                args.Add(item.String != null ? item.String : item.Value.ToString());
            }

            return args;
        }

        public static int ExtractArrayElement(ref string varName)
        {
            int argStart = varName.IndexOf(Constants.START_ARG);
            if (argStart <= 0)
            {
                return -1;
            }

            int argEnd = varName.IndexOf(Constants.END_ARG, argStart + 1);
            if (argEnd <= argStart + 1)
            {
                return -1;
            }

            int getIndexFrom = argStart;
            Parser.Result existing = Parser.LoadAndCalculate(varName, ref getIndexFrom,
                                                             Constants.NEXT_OR_END_ARRAY);

            if (!Double.IsNaN(existing.Value) && existing.Value >= 0)
            {
                varName = varName.Substring(0, argStart);
                return (int)existing.Value;
            }

            return -1;
        }

        public static string ConvertToScript(string source)
        {
            StringBuilder sb = new StringBuilder(source.Length);
            bool inQuotes = false;
            bool inComments = false;
            char previous = '\0';

            int parentheses = 0;
            int groups = 0;

            for (int i = 0; i < source.Length; i++)
            {
                char ch = source[i];
                char next = i + 1 < source.Length ? source[i + 1] : '\0';
                switch (ch)
                {
                    case '/':
                        if (inComments || next == '/')
                        {
                            inComments = true;
                            continue;
                        }
                        break;
                    case '"':
                        if (!inComments)
                        {
                            if (previous != '\\') inQuotes = !inQuotes;
                        }
                        break;
                    case ' ':
                        if (inQuotes) sb.Append(ch);
                        continue;
                    case '\t':
                        if (inQuotes) sb.Append(ch);
                        continue;
                    case '\r':
                    case '\n':
                        inComments = false;
                        continue;
                    case Constants.END_ARG:
                        parentheses--;
                        break;
                    case Constants.START_ARG:
                        parentheses++;
                        break;
                    case Constants.END_GROUP: groups--;
                        break;
                    case Constants.START_GROUP: groups++;
                        break;
                    default: break;
                }
                if (!inComments)
                {
                    sb.Append(ch);
                }
                previous = ch;
            }

            if (parentheses != 0)
            {
                throw new ArgumentException("Uneven parentheses " + Constants.START_ARG + Constants.END_ARG);
            }
            if (groups != 0)
            {
                throw new ArgumentException("Uneven groups " + Constants.START_GROUP + Constants.END_GROUP);
            }

            return sb.ToString();
        }

        public static List<string> GetFiles(string path, string[] patterns)
        {
            try
            {
                List<string> files = patterns.SelectMany(
                    i => Directory.EnumerateFiles(path, i)
                  ).ToList<string>();

                Parallel.ForEach(Directory.EnumerateDirectories(path), (dir) =>
                {
                    List<string> extraFiles = GetFiles(dir, patterns);
                    if (extraFiles != null && extraFiles.Count > 0)
                    {
                        lock (s_mutexLock) { files.AddRange(extraFiles); }
                    }
                });

                return files;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<string> GetStringInFiles(string path, string search,
                                   string[] patterns, bool ignoreCase = true)
        {
            List<string> allFiles = GetFiles(path, patterns);
            List<string> results = new List<string>();

            if (allFiles == null && allFiles.Count == 0)
            {
                return results;
            }

            StringComparison caseSense = ignoreCase ? StringComparison.OrdinalIgnoreCase :
                                                      StringComparison.Ordinal;
            Parallel.ForEach(allFiles, (currentFile) =>
            {
                string contents = GetFileText(currentFile);
                if (contents.IndexOf(search, caseSense) >= 0)
                {
                    lock (s_mutexLock) { results.Add(currentFile); }
                }
            });

            return results;
        }

        public static string GetFileText(string name)
        {
            string fileContents = string.Empty;
            if (File.Exists(name))
            {
                fileContents = File.ReadAllText(name);
            }
            return fileContents;
        }

        public static string GetLine(int chars = 75)
        {
            return string.Format("-").PadRight(chars, '-');
        }

        private static readonly object s_mutexLock = new object();
    }
}
