using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension;
using tmg.equinox.document.rulebuilder.jsonhelper;
namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core
{
    class Continue : Parser.Result
    { }

    class ContinueStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Continue();
        }
    }

    class Break : Parser.Result
    { }

    class BreakStatement : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Break();
        }
    }


    class PiFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            return new Parser.Result(3.141592653589793);
        }
    }

    class ExpFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            result.Value = Math.Exp(result.Value);
            return result;
        }
    }

    class PowFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg1 = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_ARG_ARRAY);
            from++; // eat separation
            Parser.Result arg2 = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);

            arg1.Value = Math.Pow(arg1.Value, arg2.Value);
            return arg1;
        }
    }

    class DivideFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            Parser.Result arg1 = Parser.LoadAndCalculate(data, ref from, Constants.NEXT_ARG_ARRAY);
            from++; // eat separation
            Parser.Result arg2 = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);

            double arg1Val;
            bool arg1IsValid = Double.TryParse(arg1.String, out arg1Val);

            double arg2Val;
            bool arg2IsValid = Double.TryParse(arg2.String, out arg2Val);


            if (arg1IsValid && arg2IsValid)
            {
                result.Value = Math.Round(arg1Val / arg2Val, 2);
                result.String = result.Value.ToString("F2");
            }
            return result;
        }
    }


    class SinFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Sin(arg.Value);
            return arg;
        }
    }

    class SqrtFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Sqrt(arg.Value);
            return arg;
        }
    }

    class AbsFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result arg = Parser.LoadAndCalculate(data, ref from, Constants.END_ARG_ARRAY);
            arg.Value = Math.Abs(arg.Value);
            return arg;
        }
    }

    class PsInfoFunction : ParserFunction
    {
        internal PsInfoFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string pattern = Utils.GetItem(data, ref from).String;
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Couldn't extract process name");
            }

            int MAX_PROC_NAME = 26;
            m_interpreter.AppendOutput(Utils.GetLine());
            m_interpreter.AppendOutput(String.Format("{0} {1} {2} {3} {4}",
              "Process Id".PadRight(15), "Process Name".PadRight(MAX_PROC_NAME),
              "Working Set".PadRight(15), "Virt Mem".PadRight(15), "CPU Time".PadRight(25)));

            Process[] processes = Process.GetProcessesByName(pattern);
            List<Parser.Result> results = new List<Parser.Result>(processes.Length);
            for (int i = 0; i < processes.Length; i++)
            {
                Process pr = processes[i];
                int workingSet = (int)(((double)pr.WorkingSet64) / 1000000.0);
                int virtMemory = (int)(((double)pr.VirtualMemorySize64) / 1000000.0);
                string procTitle = pr.ProcessName + " " + pr.MainWindowTitle.Split(null)[0];
                if (procTitle.Length > MAX_PROC_NAME)
                {
                    procTitle = procTitle.Substring(0, MAX_PROC_NAME);
                }
                string procTime = string.Empty;
                try
                {
                    procTime = pr.TotalProcessorTime.ToString().Substring(0, 11);
                }
                catch (Exception) { }

                results.Add(new Parser.Result(Double.NaN,
                              string.Format("{0,15} {1," + MAX_PROC_NAME + "} {2,15} {3,15} {4,25}",
                              pr.Id, procTitle,
                              workingSet, virtMemory, procTime)));
                m_interpreter.AppendOutput(results.Last().String);
            }
            m_interpreter.AppendOutput(Utils.GetLine());

            if (data.Length > from && data[from] == Constants.NEXT_ARG)
            {
                from++; // eat end of statement semicolon
            }

            return new Parser.Result(Double.NaN, null, results);
        }

        private Interpreter m_interpreter;
    }

    class KillFunction : ParserFunction
    {
        internal KillFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result id = Utils.GetItem(data, ref from, true /* expectInt */);

            int processId = (int)id.Value;
            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
                m_interpreter.AppendOutput("Process " + processId + " killed");
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't kill process " + processId +
                                            " (" + exc.Message + ")");
            }

            return new Parser.Result();
        }

        private Interpreter m_interpreter;
    }

    class RunFunction : ParserFunction
    {
        internal RunFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string processName = Utils.GetItem(data, ref from).String;

            if (string.IsNullOrWhiteSpace(processName))
            {
                throw new ArgumentException("Couldn't extract process name");
            }

            List<string> args = Utils.GetFunctionArgs(data, ref from);
            int processId = -1;
            try
            {
                Process pr = Process.Start(processName, string.Join("", args.ToArray()));
                processId = pr.Id;
            }
            catch (System.ComponentModel.Win32Exception exc)
            {
                throw new ArgumentException("Couldn't start [" + processName + "]: " + exc.Message);
            }

            m_interpreter.AppendOutput("Process " + processName + " started, id: " + processId);
            return new Parser.Result(processId);
        }

        private Interpreter m_interpreter;
    }

    class GetEnvFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            string res = Environment.GetEnvironmentVariable(varName);

            return new Parser.Result(Double.NaN, res);
        }
    }

    class SetEnvFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set env variable");
            }

            Parser.Result varValue = Utils.GetItem(data, ref from);
            string strValue = varValue.String != null ? varValue.String :
                                                        varValue.Value.ToString();
            Environment.SetEnvironmentVariable(varName, strValue);

            return new Parser.Result(Double.NaN, varName);
        }
    }

    class PrintFunction : ParserFunction
    {
        internal PrintFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            List<string> args = Utils.GetFunctionArgs(data, ref from);
            m_interpreter.AppendOutput(string.Join("", args.ToArray()));

            return new Parser.Result();
        }
        private Interpreter m_interpreter;
    }

    class IfStatement : ParserFunction
    {
        internal IfStatement(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            m_interpreter.CurrentChar = from;

            Parser.Result result = m_interpreter.ProcessIf();

            return result;
        }

        private Interpreter m_interpreter;
    }

    class WhileStatement : ParserFunction
    {
        internal WhileStatement(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            m_interpreter.CurrentChar = from;
            m_interpreter.ProcessWhile();

            return new Parser.Result();
        }

        private Interpreter m_interpreter;
    }

    class ProcessorTimeFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Process pr = Process.GetCurrentProcess();
            TimeSpan ts = pr.TotalProcessorTime;

            return new Parser.Result(ts.TotalMilliseconds);
        }
    }

    class PwdFunction : ParserFunction
    {
        internal PwdFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string path = Directory.GetCurrentDirectory();
            m_interpreter.AppendOutput(path);

            return new Parser.Result(Double.NaN, path);
        }

        private Interpreter m_interpreter;
    }

    class Cd__Function : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string newDir = null;

            try
            {
                string pwd = Directory.GetCurrentDirectory();
                newDir = Directory.GetParent(pwd).FullName;
                Directory.SetCurrentDirectory(newDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not change directory: " + exc.Message);
            }

            return new Parser.Result(Double.NaN, newDir);
        }
    }

    class CdFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string newDir = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);

            try
            {
                Directory.SetCurrentDirectory(newDir);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not change directory: " + exc.Message);
            }

            return new Parser.Result(Double.NaN, newDir);
        }
    }

    class FindstrFunction : ParserFunction
    {
        internal FindstrFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string search = Utils.GetToken(data, ref from, Constants.QUOTE_ARRAY);
            List<string> patterns = Utils.GetFunctionArgs(data, ref from);

            bool ignoreCase = true;
            if (patterns.Count > 0 && patterns.Last().Equals("case"))
            {
                ignoreCase = false;
                patterns.RemoveAt(patterns.Count - 1);
            }
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }

            List<Parser.Result> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetStringInFiles(pwd, search, patterns.ToArray(), ignoreCase);

                m_interpreter.AppendOutput("----- Files containing [" + search + "] with pattern [" +
                                            string.Join(", ", patterns.ToArray()) + "] -----");
                results = new List<Parser.Result>(files.Count);
                foreach (string filename in files)
                {
                    results.Add(new Parser.Result(Double.NaN, filename));
                    m_interpreter.AppendOutput(filename);
                }
                m_interpreter.AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't find pattern: " + exc.Message);
            }

            return new Parser.Result(Double.NaN, null, results);
        }

        private Interpreter m_interpreter;
    }

    class FindfilesFunction : ParserFunction
    {
        internal FindfilesFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            List<string> patterns = Utils.GetFunctionArgs(data, ref from);
            if (patterns.Count == 0)
            {
                patterns.Add("*.*");
            }

            List<Parser.Result> results = null;
            try
            {
                string pwd = Directory.GetCurrentDirectory();
                List<string> files = Utils.GetFiles(pwd, patterns.ToArray());

                //m_interpreter.AppendOutput("----- Found files with pattern [" +
                //                           string.Join(", ", patterns.ToArray()) + "] -----");
                results = new List<Parser.Result>(files.Count);
                foreach (string filename in files)
                {
                    results.Add(new Parser.Result(Double.NaN, filename));
                    //m_interpreter.AppendOutput(filename);
                }

                //m_interpreter.AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Couldn't list directory: " + exc.Message);
            }

            return new Parser.Result(Double.NaN, null, results);
        }

        private Interpreter m_interpreter;
    }

    class DirFunction : ParserFunction
    {
        internal DirFunction(Interpreter interpreter)
        {
            m_interpreter = interpreter;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string dirname = (data.Length <= from || data[from] == Constants.END_STATEMENT) ?
                                           Directory.GetCurrentDirectory() :
                                           Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);

            try
            {
                var results = Directory.EnumerateFiles(dirname);
                m_interpreter.AppendOutput("----- Files in [" + dirname + "] -----");
                foreach (string filename in results)
                {
                    m_interpreter.AppendOutput(filename);
                }
                m_interpreter.AppendOutput(Utils.GetLine());
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Could not list directory: " + exc.Message);
            }

            return new Parser.Result();
        }

        private Interpreter m_interpreter;
    }

    class SetVarFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't set variable before end of line");
            }

            Parser.Result varValue = Utils.GetItem(data, ref from);

            //TODO: Temporary Fix : if a variable has token value then set to token and set null to string
            if (!string.IsNullOrWhiteSpace(varValue.String))
            {
                try
                {
                    JObject val = JObject.Parse(varValue.String);
                    if (val != null)
                    {
                        varValue.Token = val;
                        varValue.String = null;
                    }
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }
            //END

            // Check if the variable to be set has the form of x(0),
            // meaning that this is an array element.
            int arrayIndex = Utils.ExtractArrayElement(ref varName);
            if (arrayIndex >= 0)
            {
                bool exists = ParserFunction.FunctionExists(varName);
                Parser.Result currentValue = exists ?
                      ParserFunction.GetFunction(varName).GetValue(data, ref from) :
                      new Parser.Result();

                List<Parser.Result> tuple = currentValue.Tuple == null ?
                                              new List<Parser.Result>() :
                                              currentValue.Tuple;
                if (tuple.Count > arrayIndex)
                {
                    tuple[arrayIndex] = varValue;
                }
                else
                {
                    for (int i = tuple.Count; i < arrayIndex; i++)
                    {
                        tuple.Add(new Parser.Result(Double.NaN, string.Empty));
                    }
                    tuple.Add(varValue);
                }

                varValue = new Parser.Result(Double.NaN, null, tuple);
            }

            ParserFunction.AddFunction(varName, new GetVarFunction(varValue));

            return new Parser.Result(Double.NaN, varName);
        }
    }

    class GetVarFunction : ParserFunction
    {
        internal GetVarFunction(Parser.Result value)
        {
            m_value = value;
        }

        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // First check if this element is part of an array:
            if (from < data.Length && data[from - 1] == Constants.START_ARG)
            {
                // There is an index given - it may be for an element of the tuple.
                if (m_value.Tuple == null || m_value.Tuple.Count == 0)
                {
                    throw new ArgumentException("No tuple exists for the index");
                }

                Parser.Result index = Utils.GetItem(data, ref from, true /* expectInt */);
                if (index.Value < 0 || index.Value >= m_value.Tuple.Count)
                {
                    throw new ArgumentException("Incorrect index [" + index.Value +
                                                "] for tuple of size " + m_value.Tuple.Count);
                }
                return m_value.Tuple[(int)index.Value];
            }

            return m_value;
        }

        private Parser.Result m_value;
    }

    class AppendFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't append variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value to be added or appended.
            Parser.Result newValue = Utils.GetItem(data, ref from);

            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg1 = FunctionHelper.GetArgument(currentValue);
            string arg2 = FunctionHelper.GetArgument(newValue);

            // 5. The variable becomes a string after adding a string to it.
            newValue.Reset();
            newValue.String = arg1 + arg2;

            ParserFunction.AddFunction(varName, new GetVarFunction(newValue));

            return newValue;
        }
    }

    class ToUpperFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                       currentValue.Value.ToString();

            Parser.Result newValue = new Parser.Result(Double.NaN, arg.ToUpper());
            return newValue;
        }
    }

    class ToLowerFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                       currentValue.Value.ToString();

            Parser.Result newValue = new Parser.Result(Double.NaN, arg.ToLower());
            return newValue;
        }
    }

    class SizeFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Take either the length of the underlying tuple or
            // string part if it is defined,
            // or the numerical part converted to a string otherwise.
            int size = currentValue.Tuple != null ? currentValue.Tuple.Count :
                       currentValue.String != null ? currentValue.String.Length :
                       currentValue.Token != null? currentValue.Token.ToString().Length : currentValue.Value.ToString().Length;

            Parser.Result newValue = new Parser.Result(size);
            return newValue;
        }
    }

    class SubstrFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            string substring;

            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string arg = currentValue.String != null ? currentValue.String :
                                                           (currentValue.Token != null && currentValue.Token.ToString() != "") ?
                                                           currentValue.Token.ToString() : currentValue.Value.ToString();
            // 4. Get the initial index of the substring.
            bool lengthAvailable = Utils.SeparatorExists(data, from);

            Parser.Result init = Utils.GetItem(data, ref from, true /* expectInt */);

            // 5. Get the length of the substring if available.
            if (lengthAvailable)
            {
                Parser.Result length = Utils.GetItem(data, ref from, true /* expectInt */);
                if (init.Value + length.Value > arg.Length)
                {
                    throw new ArgumentException("The total substring length is larger than  [" +
                                                 arg + "]");
                }
                substring = arg.Substring((int)init.Value, (int)length.Value);
            }
            else
            {
                substring = arg.Substring((int)init.Value);
            }
            Parser.Result newValue = new Parser.Result(Double.NaN, substring);

            return newValue;
        }
    }

    class IndexOfFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            // 1. Get the name of the variable.
            string varName = Utils.GetToken(data, ref from, Constants.NEXT_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't append variable");
            }

            // 2. Get the current value of the variable.
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);

            // 3. Get the value to be looked for.
            Parser.Result searchValue = Utils.GetItem(data, ref from);

            // 4. Take either the string part if it is defined,
            // or the numerical part converted to a string otherwise.
            string basePart = currentValue.String != null ? currentValue.String :
                                                           (currentValue.Token != null && currentValue.Token.ToString() != "") ?
                                                           currentValue.Token.ToString() : currentValue.Value.ToString();
            string search = searchValue.String != null ? searchValue.String :
                                                         searchValue.Value.ToString();

            int result = basePart.IndexOf(search);
            return new Parser.Result(result, null);
        }
    }



}
