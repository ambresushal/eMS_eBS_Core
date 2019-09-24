using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core
{
    public class Parser
    {
        public static bool Verbose { get; set; }

        public class Result
        {
            public Result(double doubleResult = Double.NaN,
                          string stringResult = null,
                          List<Result> tupleResult = null, JToken tokenResult = null)
            {
                Value = doubleResult;
                String = stringResult;
                Tuple = tupleResult;
                Token = tokenResult;
            }

            public void Copy(Result other)
            {
                Value = other.Value;
                String = other.String;
                Tuple = other.Tuple;
                Token = other.Token;
            }

            public bool IsEqual(Result other)
            {
                if (other == null ||
                   string.IsNullOrWhiteSpace(this.String) != string.IsNullOrWhiteSpace(other.String) ||
                   Double.IsNaN(Value) != Double.IsNaN(other.Value) ||
                   (Tuple == null) != (other.Tuple == null))
                {
                    return false;
                }

                if ((Double.IsNaN(Value) || Value == other.Value) &&
                    (this.String == null || this.String.Equals(other.String)) &&
                    (Tuple == null || Tuple.SequenceEqual(other.Tuple)))
                {
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                Value = Double.NaN;
                String = null;
                Tuple = null;
                Token = null;
            }

            public double Value { get; set; }
            public string String { get; set; }
            public List<Result> Tuple { get; set; }
            public JToken Token { get; set; }
        }

        class Cell
        {
            internal Cell(double value, char action)
            {
                Value = value;
                Action = action;
            }

            internal double Value { get; set; }
            internal char Action { get; set; }
        }

        public static Result Process(string data)
        {
            // Get rid of spaces and check parenthesis
            string expression = Preprocess(data);
            int from = 0;

            return LoadAndCalculate(data, ref from, Constants.END_LINE_ARRAY);
        }

        static string Preprocess(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException("Loaded empty data");
            }

            int parentheses = 0;
            StringBuilder result = new StringBuilder(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                char ch = data[i];
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\n': continue;
                    case Constants.END_ARG: parentheses--;
                        break;
                    case Constants.START_ARG: parentheses++;
                        break;
                }
                result.Append(ch);
            }

            if (parentheses != 0)
            {
                throw new ArgumentException("Uneven parentheses");
            }

            return result.ToString();
        }

        public static Result LoadAndCalculate(string data, ref int from, char[] to)
        {
            if (from >= data.Length || to.Contains(data[from]))
            {
                return new Result();
            }

            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder item = new StringBuilder();

            do
            { // Main processing cycle of the first part.
                char ch = data[from++];
                if (StillCollecting(item.ToString(), ch, to))
                { // The char still belongs to the previous operand.
                    item.Append(ch);
                    if (from < data.Length && !to.Contains(data[from]))
                    {
                        continue;
                    }
                }

                // We are done getting the next token. The getValue() call below may
                // recursively call loadAndCalculate(). This will happen if extracted
                // item is a function or if the next item is starting with a START_ARG '('.
                string parsingItem = item.ToString();
                ParserFunction func = new ParserFunction(data, ref from, parsingItem, ch);
                Result current = func.GetValue(data, ref from);

                if (Double.IsNaN(current.Value))
                { // If there is no numerical result, we are not in a math expression.
                    return current;
                }

                char action = ValidAction(ch) ? ch
                                              : UpdateAction(data, ref from, ch, to);

                listToMerge.Add(new Cell(current.Value, action));
                item.Clear();

            } while (from < data.Length && !to.Contains(data[from]));

            if (from < data.Length && data[from] == Constants.END_ARG)
            { // This happens when called recursively inside of the math expression:
                // move one char forward.
                from++;
            }

            Cell baseCell = listToMerge[0];
            int index = 1;

            Result result = new Result();
            result.Value = Merge(baseCell, ref index, listToMerge);

            return result;
        }

        static bool StillCollecting(string item, char ch, char[] to)
        {
            if (to.Contains(ch))
            {
                return false;
            }
            // Stop collecting if either got END_ARG ')' or to char, e.g. ','.
            char stopCollecting = (to.Contains(Constants.END_ARG) || to.Contains(Constants.END_LINE)) ?
                                   Constants.END_ARG : Constants.END_LINE;
            return (item.Length == 0 && (ch == '-' || ch == Constants.END_ARG)) ||
                  !(ValidAction(ch) || ch == Constants.START_ARG || ch == stopCollecting);
        }

        static bool ValidAction(char ch)
        {
            return ch == '%' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '^';
        }

        static char UpdateAction(string item, ref int from, char ch, char[] to)
        {
            if (from >= item.Length || item[from] == Constants.END_ARG || to.Contains(item[from]))
            {
                return Constants.END_ARG;
            }

            int index = from;
            char res = ch;
            while (!ValidAction(res) && index < item.Length)
            { // Look for the next character in string until a valid action is found.
                res = item[index++];
            }

            from = ValidAction(res) ? index
                                    : index > from ? index - 1
                                                   : from;
            return res;
        }

        // From outside this function is called with mergeOneOnly = false.
        // It also calls itself recursively with mergeOneOnly = true, meaning
        // that it will return after only one merge.
        static double Merge(Cell current, ref int index, List<Cell> listToMerge,
                            bool mergeOneOnly = false)
        {
            if (Verbose)
            {
                PrintList(listToMerge, index - 1);
            }

            while (index < listToMerge.Count)
            {
                Cell next = listToMerge[index++];

                while (!CanMergeCells(current, next))
                { // If we cannot merge cells yet, go to the next cell and merge
                    // next cells first. E.g. if we have 1+2*3, we first merge next
                    // cells, i.e. 2*3, getting 6, and then we can merge 1+6.
                    Merge(next, ref index, listToMerge, true /* mergeOneOnly */);
                }
                MergeCells(current, next);
                if (mergeOneOnly)
                {
                    break;
                }
            }

            if (Verbose)
            {
                Console.WriteLine("Calculated: {0}", current.Value);
            }
            return current.Value;
        }

        static void MergeCells(Cell leftCell, Cell rightCell)
        {
            switch (leftCell.Action)
            {
                case '^': leftCell.Value = Math.Pow(leftCell.Value, rightCell.Value);
                    break;
                case '%': leftCell.Value %= rightCell.Value;
                    break;
                case '*': leftCell.Value *= rightCell.Value;
                    break;
                case '/':
                    if (rightCell.Value == 0)
                    {
                        throw new ArgumentException("Division by zero");
                    }
                    leftCell.Value /= rightCell.Value;
                    break;
                case '+': leftCell.Value += rightCell.Value;
                    break;
                case '-': leftCell.Value -= rightCell.Value;
                    break;
            }
            leftCell.Action = rightCell.Action;
        }

        static bool CanMergeCells(Cell leftCell, Cell rightCell)
        {
            return GetPriority(leftCell.Action) >= GetPriority(rightCell.Action);
        }

        static int GetPriority(char action)
        {
            switch (action)
            {
                case '^': return 4;
                case '%':
                case '*':
                case '/': return 3;
                case '+':
                case '-': return 2;
            }
            return 0;
        }

        static void PrintList(List<Cell> list, int from)
        {
            Console.Write("Merging list:");
            for (int i = from; i < list.Count; i++)
            {
                Console.Write(" ({0}, '{1}')", list[i].Value, list[i].Action);
            }
            Console.WriteLine();
        }

    }

    public class ParserFunction
    {
        public ParserFunction()
        {
            m_impl = this;
        }

        // A "virtual" Constructor
        internal ParserFunction(string data, ref int from, string item, char ch)
        {
            if (item.Length == 0 && ch == Constants.START_ARG)
            {
                // There is no function, just an expression in parentheses
                m_impl = s_idFunction;
                return;
            }

            m_impl = GetFunction(item);
        }

        public static ParserFunction GetFunction(string item)
        {
            ParserFunction impl;
            if (s_functions.TryGetValue(item, out impl))
            {
                // Function exists and is registered (e.g. pi, exp, etc.)
                return impl;
            }

            // Function not found, will try to parse this as a number.
            s_strtodFunction.Item = item;
            impl = s_strtodFunction;
            return impl;
        }

        public static bool FunctionExists(string item)
        {
            return s_functions.ContainsKey(item);
        }

        public static bool RemoveVariables()
        {
            var functionsToDelete = s_functions.Where(s => s.Value.GetType() == typeof(GetVarFunction)).Select(s => s.Key).ToList();
            foreach (string func in functionsToDelete)
            {
                s_functions.Remove(func);
            }
            return true;
        }

        public static void AddFunction(string name, ParserFunction function)
        {
            s_functions[name] = function;
        }

        public Parser.Result GetValue(string data, ref int from)
        {
            return m_impl.Evaluate(data, ref from);
        }

        protected virtual Parser.Result Evaluate(string data, ref int from)
        {
            // The real implementation will be in the derived classes.
            return new Parser.Result();
        }

        private ParserFunction m_impl;
        private static Dictionary<string, ParserFunction> s_functions = new Dictionary<string, ParserFunction>();

        private static StrtodFunction s_strtodFunction = new StrtodFunction();
        private static IdentityFunction s_idFunction = new IdentityFunction();
    }

    class StrtodFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            double num;
            if (!Double.TryParse(Item, out num))
            {
                throw new ArgumentException("Couldn't parse token [" + Item + "]");
            }
            return new Parser.Result(num);
        }
        public string Item { private get; set; }
    }

    class IdentityFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
        }
    }

}
