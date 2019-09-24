using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System.Text;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.ruleinterpreter.RuleProcessor
{
    public class SourceMergeScriptProcessor
    {
        JToken _target;
        Dictionary<string, JToken> _sources;
        CompiledDocumentRule _rule;
        private static object lockObject = new object();
        public SourceMergeScriptProcessor(JToken target, Dictionary<string, JToken> sources, CompiledDocumentRule rule)
        {
            this._target = target;
            this._sources = sources;
            this._rule = rule;
        }

        public JToken Process()
        {
            this.GenerateDictionary();
            string script = string.Concat(this.AddSourcesToScript(), this.GetScript());
            lock (lockObject)
            {
                try
                {
                    Interpreter.Instance.Process(script);
                    ParserFunction.RemoveVariables();
                    return _target = this.GetRuleOutput();
                }
                catch (System.Exception ex)
                {
                    ParserFunction.RemoveVariables();
                    SourceManager.Remove(Thread.CurrentThread);
                    ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    throw ex;
                }

            }
        }

        private string GetScript()
        {
            string script = string.Empty;
            var expression = _rule.SourceContainer.SourceMergeList.SourceMergeActions.Select(s => s.CodeBlock).FirstOrDefault();
            if (expression != null)
            {
                script = expression;
            }

            return script;
        }

        private string AddSourcesToScript()
        {
            StringBuilder sb = new StringBuilder();
            char name = 'a';
            foreach (var source in _sources)
            {
                sb.Append("SETSOURCE(" + source.Key.ToLower() + ",\"" + source.Key + "\");");
                name++;
            }

            return sb.ToString();
        }

        private void GenerateDictionary()
        {
            //Add sources to global dictionary
            foreach (var source in _sources)
            {
                SourceManager.Add(Thread.CurrentThread, source.Key, source.Value);
            }

            //Add Target to gglobal dictionary
            SourceManager.AddTarget(Thread.CurrentThread, _target);
        }

        private JToken GetRuleOutput()
        {
            JToken output = SourceManager.Get(Thread.CurrentThread, "target");
            SourceManager.Remove(Thread.CurrentThread);
            return output;
        }
    }
}
