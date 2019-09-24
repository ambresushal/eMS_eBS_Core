using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Core
{
    public class Interpreter
    {

        private static Interpreter instance;

        private Interpreter()
        {
            Init();
        }

        public static Interpreter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Interpreter();
                }
                return instance;
            }
        }

        private static List<string> ELSE_LIST = new List<string>();
        private static List<string> ELSE_IF_LIST = new List<string>();

        private int CHECK_AFTER_LOOPS;
        private int MAX_LOOPS;

        private string m_data;

        private int m_currentChar;
        public int CurrentChar { set { m_currentChar = value; } }

        private StringBuilder m_output = new StringBuilder();
        public string Output
        {
            get
            {
                string output = m_output.ToString().Trim();
                m_output.Clear();
                return output;
            }
        }

        public void AppendOutput(string text, bool newLine = true)
        {
            if (newLine)
            {
                m_output.AppendLine(text);
            }
            else
            {
                m_output.Append(text);
            }
        }

        public void Init()
        {
            ParserFunction.AddFunction(Constants.IF, new IfStatement(this));
            ParserFunction.AddFunction(Constants.WHILE, new WhileStatement(this));
            ParserFunction.AddFunction(Constants.BREAK, new BreakStatement());
            ParserFunction.AddFunction(Constants.CONTINUE, new ContinueStatement());

            ParserFunction.AddFunction(Constants.ABS, new AbsFunction());
            ParserFunction.AddFunction(Constants.APPEND, new AppendFunction());
            ParserFunction.AddFunction(Constants.CD, new CdFunction());
            ParserFunction.AddFunction(Constants.CD__, new Cd__Function());
            ParserFunction.AddFunction(Constants.DIR, new DirFunction(this));
            ParserFunction.AddFunction(Constants.ENV, new GetEnvFunction());
            ParserFunction.AddFunction(Constants.EXP, new ExpFunction());
            ParserFunction.AddFunction(Constants.FINDFILES, new FindfilesFunction(this));
            ParserFunction.AddFunction(Constants.FINDSTR, new FindstrFunction(this));
            ParserFunction.AddFunction(Constants.INDEX_OF, new IndexOfFunction());
            ParserFunction.AddFunction(Constants.KILL, new KillFunction(this));
            ParserFunction.AddFunction(Constants.PI, new PiFunction());
            ParserFunction.AddFunction(Constants.POW, new PowFunction());
            ParserFunction.AddFunction(Constants.PRINT, new PrintFunction(this));
            ParserFunction.AddFunction(Constants.PSINFO, new PsInfoFunction(this));
            ParserFunction.AddFunction(Constants.PSTIME, new ProcessorTimeFunction());
            ParserFunction.AddFunction(Constants.PWD, new PwdFunction(this));
            ParserFunction.AddFunction(Constants.RUN, new RunFunction(this));
            ParserFunction.AddFunction(Constants.SET, new SetVarFunction());
            ParserFunction.AddFunction(Constants.SETENV, new SetEnvFunction());
            ParserFunction.AddFunction(Constants.SIN, new SinFunction());
            ParserFunction.AddFunction(Constants.SIZE, new SizeFunction());
            ParserFunction.AddFunction(Constants.SQRT, new SqrtFunction());
            ParserFunction.AddFunction(Constants.SUBSTR, new SubstrFunction());
            ParserFunction.AddFunction(Constants.TOLOWER, new ToLowerFunction());
            ParserFunction.AddFunction(Constants.TOUPPER, new ToUpperFunction());
            //Custom Implementation
            ParserFunction.AddFunction(Constants.REPLACEALL, new ReplaceFunction());
            ParserFunction.AddFunction(Constants.SETSOURCE, new SetSourceFunction());
            ParserFunction.AddFunction(Constants.COUNT, new CountFunction());
            ParserFunction.AddFunction(Constants.SETTEXT, new SetTextFunction());
            ParserFunction.AddFunction(Constants.CONTAINS, new ContainsFunction());
            ParserFunction.AddFunction(Constants.TRUE, new TrueFunction());
            ParserFunction.AddFunction(Constants.APPENDARRAY, new AppendArrayFunction());
            ParserFunction.AddFunction(Constants.APPENDTEXT, new AppendTextFunction());
            ParserFunction.AddFunction(Constants.SETARRAY, new SetArrayFunction());
            ParserFunction.AddFunction(Constants.TOBOOLEAN, new ToBooleanFunction());
            ParserFunction.AddFunction(Constants.EQUALS, new EqualsFunction());
            ParserFunction.AddFunction(Constants.FILTERLIST, new FilterListFunction());
            ParserFunction.AddFunction(Constants.UPDATETEXT, new UpdateTextFunction());
            ParserFunction.AddFunction(Constants.INTERSECT, new IntersectFunction());
            ParserFunction.AddFunction(Constants.UNION, new UnionFunction());
            ParserFunction.AddFunction(Constants.EXCEPT, new ExceptFunction());
            ParserFunction.AddFunction(Constants.CROSSJOIN, new CrossJoinFunction());
            ParserFunction.AddFunction(Constants.SLICEARRAY, new SliceArrayFunction());
            ParserFunction.AddFunction(Constants.UPDATEARRAY, new UpdateArrayFunction());
            ParserFunction.AddFunction(Constants.GETVAL, new GetTokenValueFunction());
            ParserFunction.AddFunction(Constants.SETDEFAULTROW, new SetDefaultRowFunction());
            ParserFunction.AddFunction(Constants.CREATETOKEN, new CreateTokenFunction());
            ParserFunction.AddFunction(Constants.MERGEARRAY, new MergeArrayFunction());
            ParserFunction.AddFunction(Constants.CONVERTTOTYPE, new ConvertTypeToFunction());
            ParserFunction.AddFunction(Constants.COUNTARRAY, new CountArrayFunction());
            ParserFunction.AddFunction(Constants.SETTOKENVALUE, new SetTokenValueFunction());
            ParserFunction.AddFunction(Constants.GETARRAY, new GetArrayFunction());
            ParserFunction.AddFunction(Constants.DIVIDE, new DivideFunction());
            ELSE_LIST.Add(Constants.ELSE);
            ELSE_IF_LIST.Add(Constants.ELSE_IF);

            ReadConfig();
        }

        public void ReadConfig()
        {
            CHECK_AFTER_LOOPS = ReadConfig("checkAfterLoops", 256);
            MAX_LOOPS = ReadConfig("maxLoops", 256000);

            var languagesSection = ConfigurationManager.GetSection("Languages") as NameValueCollection;
            if (languagesSection == null || languagesSection.Count == 0)
            {
                return;
            }

            string languages = languagesSection["languages"];
            string[] supportedLanguages = languages.Split(",".ToCharArray());

            foreach (string language in supportedLanguages)
            {
                var languageSection = ConfigurationManager.GetSection(language) as NameValueCollection;

                AddTranslation(languageSection, Constants.IF);
                AddTranslation(languageSection, Constants.WHILE);
                AddTranslation(languageSection, Constants.BREAK);
                AddTranslation(languageSection, Constants.CONTINUE);

                AddTranslation(languageSection, Constants.APPEND);
                AddTranslation(languageSection, Constants.CD);
                AddTranslation(languageSection, Constants.CD__);
                AddTranslation(languageSection, Constants.DIR);
                AddTranslation(languageSection, Constants.ENV);
                AddTranslation(languageSection, Constants.FINDFILES);
                AddTranslation(languageSection, Constants.FINDSTR);
                AddTranslation(languageSection, Constants.INDEX_OF);
                AddTranslation(languageSection, Constants.KILL);
                AddTranslation(languageSection, Constants.PRINT);
                AddTranslation(languageSection, Constants.PSINFO);
                AddTranslation(languageSection, Constants.PWD);
                AddTranslation(languageSection, Constants.RUN);
                AddTranslation(languageSection, Constants.SET);
                AddTranslation(languageSection, Constants.SETENV);
                AddTranslation(languageSection, Constants.SIZE);
                AddTranslation(languageSection, Constants.SUBSTR);
                AddTranslation(languageSection, Constants.TOLOWER);
                AddTranslation(languageSection, Constants.TOUPPER);


                // Special dealing for else and elif since they are not separate
                // functions but are part of the if statement block.
                AddSubstatementTranslation(languageSection, Constants.ELSE, ELSE_LIST);
                AddSubstatementTranslation(languageSection, Constants.ELSE_IF, ELSE_IF_LIST);
            }
        }

        public int ReadConfig(string configName, int defaultValue = 0)
        {
            string config = ConfigurationManager.AppSettings[configName];
            int value = defaultValue;
            if (string.IsNullOrWhiteSpace(config) || !Int32.TryParse(config, out value))
            {
                return defaultValue;
            }

            return value;
        }

        public void AddTranslation(NameValueCollection languageDictionary, string originalName)
        {
            string translation = languageDictionary[originalName];
            if (string.IsNullOrWhiteSpace(translation))
            { // The translation is not provided for this function.
                return;
            }

            if (translation.IndexOfAny((" \t\r\n").ToCharArray()) >= 0)
            {
                throw new ArgumentException("Translation of [" + translation + "] contains white spaces");
            }

            ParserFunction originalFunction = ParserFunction.GetFunction(originalName);
            ParserFunction.AddFunction(translation, originalFunction);
        }

        public void AddSubstatementTranslation(NameValueCollection languageDictionary,
          string originalName, List<string> keywordsArray)
        {
            string translation = languageDictionary[originalName];
            if (string.IsNullOrWhiteSpace(translation))
            { // The translation is not provided for this sub statement.
                return;
            }

            if (translation.IndexOfAny((" \t\r\n").ToCharArray()) >= 0)
            {
                throw new ArgumentException("Translation of [" + translation + "] contains white spaces");
            }

            keywordsArray.Add(translation);
        }

        public void Process(string script)
        {
            m_data = Utils.ConvertToScript(script);
            m_currentChar = 0;

            while (m_currentChar < m_data.Length)
            {
                Parser.LoadAndCalculate(m_data, ref m_currentChar, Constants.END_PARSE_ARRAY);
                Utils.GoToNextStatement(m_data, ref m_currentChar);
            }
        }

        internal void ProcessWhile()
        {
            int startWhileCondition = m_currentChar;

            // A heuristic check against an infinite loop.
            int cycles = 0;
            int START_CHECK_INF_LOOP = CHECK_AFTER_LOOPS / 2;
            Parser.Result argCache1 = null;
            Parser.Result argCache2 = null;

            bool stillValid = true;
            while (stillValid)
            {
                m_currentChar = startWhileCondition;

                Parser.Result arg1 = GetNextIfToken();
                string comparison = Utils.GetComparison(m_data, ref m_currentChar);
                Parser.Result arg2 = GetNextIfToken();

                stillValid = EvalCondition(arg1, comparison, arg2);
                int startSkipOnBreakChar = m_currentChar;

                if (!stillValid)
                {
                    break;
                }

                // Check for an infinite loop if we are comparing same values:
                if (++cycles % START_CHECK_INF_LOOP == 0)
                {
                    if (cycles >= MAX_LOOPS || (arg1.IsEqual(argCache1) && arg2.IsEqual(argCache2)))
                    {
                        throw new ArgumentException("Looks like an infinite loop after " +
                                                     cycles + " cycles.");
                    }
                    argCache1 = arg1;
                    argCache2 = arg2;
                }

                Parser.Result result = ProcessBlock();
                if (result is Break)
                {
                    m_currentChar = startSkipOnBreakChar;
                    break;
                }
            }

            // The while condition is not true anymore: must skip the whole while
            // block before continuing with next statements.
            SkipBlock();
        }

        internal Parser.Result ProcessIf()
        {
            int startIfCondition = m_currentChar;
            Parser.Result result = null;

            Parser.Result arg1 = GetNextIfToken();
            string comparison = Utils.GetComparison(m_data, ref m_currentChar);
            Parser.Result arg2 = GetNextIfToken();

            bool isTrue = EvalCondition(arg1, comparison, arg2);

            if (isTrue)
            {
                result = ProcessBlock();

                if (result is Continue || result is Break)
                {
                    // Got here from the middle of the if-block. Skip it.
                    m_currentChar = startIfCondition;
                    SkipBlock();
                }
                SkipRestBlocks();

                return result;
            }

            // We are in Else. Skip everything in the If statement.
            SkipBlock();

            int endOfToken = m_currentChar;
            string nextToken = Utils.GetNextToken(m_data, ref endOfToken);

            if (ELSE_IF_LIST.Contains(nextToken))
            {
                m_currentChar = endOfToken + 1;
                result = ProcessIf();
            }
            else if (ELSE_LIST.Contains(nextToken))
            {
                m_currentChar = endOfToken + 1;
                result = ProcessBlock();
            }

            return new Parser.Result();
        }

        internal Parser.Result ProcessBlock()
        {
            int blockStart = m_currentChar;
            Parser.Result result = null;

            while (true)
            {
                int endGroupRead = Utils.GoToNextStatement(m_data, ref m_currentChar);
                if (endGroupRead > 0)
                {
                    return result != null ? result : new Parser.Result();
                }

                if (m_currentChar >= m_data.Length)
                {
                    throw new ArgumentException("Couldn't process block [" +
                                                 m_data.Substring(blockStart) + "]");
                }

                result = Parser.LoadAndCalculate(m_data, ref m_currentChar, Constants.END_PARSE_ARRAY);

                if (result is Continue || result is Break)
                {
                    return result;
                }
            }
        }

        internal void SkipBlock()
        {
            int blockStart = m_currentChar;
            int startCount = 0;
            int endCount = 0;
            while (startCount == 0 || startCount > endCount)
            {
                if (m_currentChar >= m_data.Length)
                {
                    throw new ArgumentException("Couldn't skip block [" + m_data.Substring(blockStart) + "]");
                }
                char currentChar = m_data[m_currentChar++];
                switch (currentChar)
                {
                    case Constants.START_GROUP: startCount++; break;
                    case Constants.END_GROUP: endCount++; break;
                }
            }

            if (startCount != endCount)
            {
                throw new ArgumentException("Mismatched parentheses in data");
            }
        }

        internal void SkipRestBlocks()
        {
            while (m_currentChar < m_data.Length)
            {
                int endOfToken = m_currentChar;
                string nextToken = Utils.GetNextToken(m_data, ref endOfToken);
                if (!ELSE_IF_LIST.Contains(nextToken) && !ELSE_LIST.Contains(nextToken))
                {
                    return;
                }
                m_currentChar = endOfToken;
                SkipBlock();
            }
        }

        internal bool EvalCondition(Parser.Result arg1, string comparison, Parser.Result arg2)
        {
            bool compare = arg1.String != null ? CompareStrings(arg1.String, comparison, arg2.String) :
                                                 arg1.Token != null ? CompareStrings(Convert.ToString(arg1.Token), comparison, Convert.ToString(arg2.Token))
                                                 : CompareNumbers(arg1.Value, comparison, arg2.Value);

            return compare;
        }

        internal bool CompareStrings(string str1, string comparison, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new ArgumentException("Inconsistent comparison data in [" +
                                             m_data.Substring(m_currentChar) + "]");
            }
            StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

            double arg1Val; double arg2Val;
            bool arg1IsValid = Double.TryParse(str1, out arg1Val);
            bool arg2IsValid = Double.TryParse(str2, out arg2Val);
            if (arg1IsValid && arg2IsValid)
            {
                return CompareNumbers(arg1Val, comparison, arg2Val);
            }
            switch (comparison)
            {
                case "==": return str1.Equals(str2, cmp);
                case "<>": return !str1.Equals(str2, cmp);
                case "<=": return string.Compare(str1, str2, cmp) <= 0;
                case ">=": return string.Compare(str1, str2, cmp) >= 0;
                case "<": return string.Compare(str1, str2, cmp) < 0;
                case ">": return string.Compare(str1, str2, cmp) > 0;

                default: throw new ArgumentException("Unknown comparison: " + comparison);
            }
        }

        internal bool CompareNumbers(double num1, string comparison, double num2)
        {
            if (Double.IsNaN(num1) || Double.IsNaN(num2))
            {
                throw new ArgumentException("Inconsistent comparison data in [" + m_data.Substring(m_currentChar) + "]");
            }
            switch (comparison)
            {
                case "==": return num1 == num2;
                case "<>": return num1 != num2;
                case "<=": return num1 <= num2;
                case ">=": return num1 >= num2;
                case "<": return num1 < num2;
                case ">": return num1 > num2;

                default: throw new ArgumentException("Unknown comparison: " + comparison);
            }
        }

        internal Parser.Result GetNextIfToken()
        {
            if (m_currentChar >= m_data.Length || m_data[m_currentChar] == Constants.END_ARG)
            {
                return new Parser.Result();
            }

            // There are 2 cases: either the next token is a string, so it is
            // in (double) quotes, or it is a mathematical expression to be parsed.
            if (m_data[m_currentChar] == Constants.QUOTE)
            {
                int end = m_data.IndexOf(Constants.QUOTE, m_currentChar + 1);
                if (end < m_currentChar)
                {
                    throw new ArgumentException("Couldn't extract token from " + m_data.Substring(m_currentChar));
                }

                m_currentChar = end;
                string var = m_data.Substring(m_currentChar, end - m_currentChar);
                return new Parser.Result(Double.NaN, var);
            }

            // Second case: parse a mathematical expression.
            return Parser.LoadAndCalculate(m_data, ref m_currentChar, Constants.COMPARE_ARRAY);
        }
    }
}
