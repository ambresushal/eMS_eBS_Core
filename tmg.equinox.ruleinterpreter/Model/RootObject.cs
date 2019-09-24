using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.model
{
    public class Trigger
    {
        public string source { get; set; }
        public string @event { get; set; }
    }

    public class Action
    {
        public string action { get; set; }
    }

    public class Mappings
    {
        public string sourcefields { get; set; }
        public string targetfields { get; set; }
    }

    public class Filterlist
    {
        public string filtertype { get; set; }
        public string filtername { get; set; }
        public string filterexpression { get; set; }
    }

    public class Filter
    {
        public List<Filterlist> filterlist { get; set; }
        public string filtermergetype { get; set; }
        public string filtermergeexpression { get; set; }
        public string keycolumns { get; set; }
    }

    public class Source
    {
        public string sourcename { get; set; }
        public string sourceelement { get; set; }
        public string sourcedocumentfilter{ get; set; }

    }

    public class RuleConditions
    {
        public List<Source> sources { get; set; }
        public Sourcemergelist sourcemergelist { get; set; }
    }

    public class Sourcemergeaction
    {
        public string sourcemergetype { get; set; }
        public string sourcemergeexpression { get; set; }
    }

    public class Documentrule
    {
        public int DocumentRuleTypeID { get; set; }
        public string targetelement { get; set; }
        public RuleConditions ruleconditions { get; set; }
    }

    public class Sourcemergelist
    {
        public List<Sourcemergeaction> sourcemergeactions { get; set; }
    }

    public class RootObject
    {
        public Documentrule documentrule { get; set; }
        public CompiledDocumentRule compileDocRule { get; set; }
    }

    public class Child
    {
        public string name { get; set; }
        public string columns { get; set; }
    }

    public class Outputcolumns
    {
        public string columns { get; set; }
        public List<Child> children { get; set; }
    }

    
}
