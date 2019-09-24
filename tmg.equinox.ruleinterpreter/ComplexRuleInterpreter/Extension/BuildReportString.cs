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
    class BuildReportStringFunction : ParserFunction
    {

        Dictionary<string, JToken> _sources = new Dictionary<string, JToken>();
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            JToken masterListSource = SourceManager.Get(Thread.CurrentThread, "A");
            //get expression templates
            _sources = SourceManager.GetAll(Thread.CurrentThread);
            ExpressionParser parser = new ExpressionParser(_sources);
            List<ExpressionTemplate> templates = ParseExpressions(masterListSource);
            List<ExpressionTemplate> nonStaticTemplates = templates.Where(a => !a.TemplateExp.Trim().StartsWith("STATIC") && !a.TemplateExp.Trim().StartsWith("CUSTOM") && a.IsValid == true).ToList();
            List<ExpressionTemplate> staticTemplates = templates.Where(a => a.TemplateExp.Trim().StartsWith("STATIC") || a.TemplateExp.Trim().StartsWith("CUSTOM")).ToList();
            //generate expressions
            List<string> expressions = new List<string>();
            if (nonStaticTemplates.Count > 0)
            {
                expressions = parser.EvaluateExpressions(nonStaticTemplates);
            }
            expressions.AddRange(staticTemplates.Select(a => a.TemplateExp));
            //get matching records
            //set target
            var results = masterListSource.Where(a => expressions.Contains(a["Key"].ToString())).OrderBy(a => a, new ParaSequenceComparer());
            StringBuilder sb = new StringBuilder();
            string input = string.Empty;
            string outputString = string.Empty;

            foreach (var res in results)
            {
                input = res["RichText"].ToString();
                PlaceHolderProcessor processor = new PlaceHolderProcessor(input, null, _sources);
                outputString = processor.Process();
                sb.Append(outputString);
            }
            SourceManager.Set(Thread.CurrentThread, "target", sb.ToString());
            return result;
        }


        //redundant - mon=ved ExpressionParser - can be removed
        private List<ExpressionTemplate> ParseExpressions(JToken masterListSource)
        {
            List<ExpressionTemplate> templates = new List<ExpressionTemplate>();
            IReportExpressionResolver resolver = new ReportExpressionResolver();

            foreach (var token in masterListSource)
            {
                ExpressionTemplate template = resolver.GetExpressionTemplate(token["Key"].ToString());
                if (token["Key"].ToString().Contains("@") && token["Key"].ToString() != "STATIC")
                {
                    RepeaterTemplateResolver rptResolver = new RepeaterTemplateResolver(token["Key"].ToString(), _sources);
                    template = rptResolver.GetRepeaterExpressionTemplate();
                }
                var templ = templates.Where(t => t.TemplateExp == template.TemplateExp);
                if (templ == null || templ.Count() == 0)
                {
                    templates.Add(template);
                }
            }
            return templates;
        }

        private string AliasPlaceHolderResolver(string input)
        {
            AliasPlaceHolderEvaluator ap = new AliasPlaceHolderEvaluator(input, _sources);
            return ap.Evaluate();
        }

        private string FieldInsertPlaceHolderResolver(string input)
        {
            FieldPlaceHolderEvaluator fi = new FieldPlaceHolderEvaluator(input, _sources);
            return fi.Evaluate();
        }

    }
}
