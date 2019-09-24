using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.expressionbuilder
{
    public class RuleEventCompiler
    {
        public DocumentRules documentrules { get; set; }
        public List<SourceDocuments> sourcedocuments { get; set; }
    }

    public class DocumentRules
    {
        public List<SourceSectionRules> sourcesectionrules { get; set; }
        public List<TargetSectionRules> targetsectionrules { get; set; }
    }

    public class TargetSectionRules
    {
        public string section { get; set; }
        public List<Rules> rules { get; set; }
    }

    public class SourceSectionRules
    {
        public string section { get; set; }
        public List<Rules> rules { get; set; }
    }

    public class Rules
    {
        public string id { get; set; }
        public string type { get; set; }
        public string[] events { get; set; }
    }

    public class SourceDocuments
    {
        public string document { get; set; }
        public List<SourceSectionRules> sourcesectionrules { get; set; }
    }
}