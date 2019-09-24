using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace tmg.equinox.document.rulebuilder.model
{
    public class RuleTreeCompiler
    {
        public List<ParentRule> rule { get; set; }

    }

    public class ParentRule
    {
        public string  id { get; set; }
        public string type { get; set; }
        public List<ChildRules> rules { get; set; }
    }

    public class ChildRules
    {
        public string id { get; set; }
        public string type { get; set; }
        public List<ChildRules> rules { get; set; }
    }

    public class DocumentRuleData
    {
        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string CompileJson { get; set; }
        public string EventType { get; set; }
        public int FormDesignVersionId { get; set; }
        public CompiledDocumentRule CompileRule
        {
            get
            {
                return JsonConvert.DeserializeObject<CompiledDocumentRule>(CompileJson);
            }
            set
            {
                value = JsonConvert.DeserializeObject<CompiledDocumentRule>(CompileJson);
            }
        }
       
    }


}
