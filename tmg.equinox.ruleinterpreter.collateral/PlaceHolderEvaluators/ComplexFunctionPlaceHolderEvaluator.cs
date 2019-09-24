using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ComplexFunctionPlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        string _richText;
        List<JToken> _context;
        Dictionary<string, JToken> _sources;
        public ComplexFunctionPlaceHolderEvaluator(string richText, List<JToken> context, Dictionary<string, JToken> sources)
        {
            _richText = richText;
            _context = context;
            _sources = sources;
        }
        public string Evaluate()
        {
            string patternFunction = RegexConstants.ComplexFunctionRegex;
            string patternFunctionMultiple = RegexConstants.MultipleComplexFunctionRegex;
            string richText = _richText;
            List<Capture> captures = new List<Capture>();
            MatchCollection wrappedCaptureColl = Regex.Matches(richText, RegexConstants.WrappedComplexFunctionRegex,RegexOptions.None,TimeSpan.FromSeconds(2));
            MatchCollection collFunction = Regex.Matches(richText, patternFunction);
            MatchCollection collFunctionMultiple = Regex.Matches(richText, patternFunctionMultiple);
            List<int> captureIndexes = new List<int>();
            if (collFunctionMultiple.Count > 0)
            {
                captureIndexes.Add(collFunctionMultiple[0].Index);
                string rt = richText;
                int idx = 0;
                while (rt.Length > 0)
                {
                    int length = collFunctionMultiple[0].Index + collFunctionMultiple[0].Length;
                    if (idx == 0)
                    {
                        captureIndexes.Add(length);
                    }
                    else
                    {
                        captureIndexes.Add(length + captureIndexes[0]);
                    }
                    idx++;
                    rt = rt.Substring(length, rt.Length - length);
                    collFunction = Regex.Matches(rt, patternFunction);
                    if (collFunction.Count > 0)
                    {
                        captures.Add(collFunction[0]);
                    }
                    rt = collFunctionMultiple[0].Value;
                    collFunctionMultiple = Regex.Matches(collFunctionMultiple[0].Value, patternFunctionMultiple);
                    if (collFunctionMultiple.Count == 0)
                    {
                        collFunction = Regex.Matches(rt, patternFunction);
                        if (collFunction.Count > 0)
                        {
                            captures.Add(collFunction[0]);
                        }
                        break;
                    }
                }
            }
            else
            {
                if (collFunction.Count > 0)
                {
                    captures.Add(collFunction[0]);
                    captureIndexes.Add(collFunction[0].Index);

                }
            }
            captures = captures.OrderByDescending(a => a.Index).ToList();
            captureIndexes = captureIndexes.OrderByDescending(a => a).ToList();
            int indexesCount = 0;

            foreach (var capture in captures)
            {
                int index = 0;
                if (indexesCount < captureIndexes.Count)
                {
                    index = captureIndexes[indexesCount];
                    indexesCount++;
                }
                string rcText = GetValue(capture.ToString());
                rcText = rcText.TrimStart(',').TrimEnd(']');
                var wCapture = IsWrappedCapture(wrappedCaptureColl, capture, index);
                if (wCapture != null)
                {
                    richText = richText.Remove(wCapture.Index, wCapture.Value.Length);
                    if (captures.Count > 1)
                    {
                        richText = richText.Insert(wCapture.Index, rcText);
                    }
                    else
                    {
                        richText = richText.Insert(wCapture.Index, rcText);
                    }
                }
                else
                {
                    richText = richText.Remove(index, capture.Value.Length);
                    if (captures.Count > 1)
                    {
                        richText = richText.Insert(capture.Index + index, rcText);
                    }
                    else
                    {
                        richText = richText.Insert(capture.Index, rcText);
                    }
                }
            }

            return richText;
        }

        private string GetValue(string insert)
        {
            string result = "";

            string replace = insert.Trim(new char[] { '{', '}' });
            if (_context == null)
            {
                _context = JToken.Parse("{'dummy':'true'}").ToList();
            }
            if (_context != null)
            {
                var token = _context.First();
                if (replace.IndexOf(":") > 0)
                {
                    result = EvaluatePlaceHolderFunction(replace, token);
                }
                else
                {
                    if (token != null)
                    {
                        replace = replace.Trim(':');
                        var tok = token.SelectToken(replace);

                        if (tok != null)
                        {
                            result = tok.ToString();
                        }
                    }
                }

            }
            return result;
        }

        private string EvaluatePlaceHolderFunction(string expression, JToken token)
        {
            string result = "";
            ComplexFunctionEvaluatorFactory factory = new ComplexFunctionEvaluatorFactory();
            IPlaceHolderFunctionEvaluator evaluator = factory.GetInstance(expression, token, _sources);
            result = evaluator.Evaluate();
            return result;
        }

        private Capture IsWrappedCapture(MatchCollection wrappedCaptures, Capture capture, int captureIndex)
        {
            Capture wrappedCapture = null;
            foreach (Match item in wrappedCaptures)
            {
                {
                    if ((item.Index + 4) == captureIndex)
                    {
                        wrappedCapture = item;
                    }
                }
            }

            return wrappedCapture;
        }

    }
}
