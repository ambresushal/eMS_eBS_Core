using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    public static class DocumentRuleSerializer
    {
        public static Documentrule Deserialize(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.DeserializeObject<RootObject>(json, settings).documentrule;
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
