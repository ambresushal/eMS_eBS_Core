using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.rulecompiler
{
    public static class DocumentRuleSerializer
    {
        public static Documentrule Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<RootObject>(json).documentrule;
        }

        public static string Serialize(CompiledDocumentRule  rule)
        {
          return JsonConvert.SerializeObject(rule);
        }

        public static CompiledDocumentRule DeseralizedToCompiledRule(string compileJSON)
        {
            return JsonConvert.DeserializeObject<CompiledDocumentRule>(compileJSON);
        }

    }
}
