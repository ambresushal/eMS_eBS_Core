using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpretercollateral;
using System.Text.RegularExpressions;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class BuildReportTableFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't get variable");
            }
            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result currentValue = func.GetValue(data, ref from);
            JToken masterListSource = SourceManager.Get(Thread.CurrentThread, "A");

            if(currentValue.String != null && masterListSource != null)
            {
                LanguageFormatEvaluator evaluator = new LanguageFormatEvaluator();
                Dictionary<string, JToken> sources = SourceManager.GetAll(Thread.CurrentThread);

                sources = evaluator.GetLanguageFormatSource(sources, masterListSource, currentValue.String);
                ReportTemplateProcessor processor = new ReportTemplateProcessor(masterListSource, sources, currentValue.String);

                string targetText = processor.Process();
                AliasPlaceHolderEvaluator ap = new AliasPlaceHolderEvaluator(targetText, sources);
                targetText = ap.Evaluate();

                FieldPlaceHolderEvaluator fi = new FieldPlaceHolderEvaluator(targetText, sources);
                targetText = fi.Evaluate();

                List<JToken> context = masterListSource.First().ToList();
                ComplexFunctionPlaceHolderEvaluator cfpe = new ComplexFunctionPlaceHolderEvaluator(targetText, context[0].ToList(), sources);
                targetText = cfpe.Evaluate();

                if (String.IsNullOrEmpty(targetText) == false)
                {
                    SourceManager.Set(Thread.CurrentThread, "target", targetText);
               }
            }
            return result;
        }
    }
}
