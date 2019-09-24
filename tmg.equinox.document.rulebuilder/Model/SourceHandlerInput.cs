using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.model
{
  public  class SourceHandlerInput
    {
      public string RuleAlias { get; set; }
      public int FormInstanceId { get; set;}
      public List<SourceSection> SourceSection { get; set; }
    }

    public class SourceSection
    {
        public string SectionName { get; set;}
        public List<SourceInput> SourceInput { get; set; }
    }

    public class SourceInput
    {
        public string SourceName { get; set;}
        public List<string> SectionElementPaths { get; set; }
    }

    public class SourceElement
    {
        public string RuleAlias { get; set; }
        public string Section { get; set; }
        public string SourceName { get; set; }
        public List<string> SectionElementPaths { get; set;}
    }

    public class SourceHandlerOutput
    {
        public Dictionary<string, JToken> SourceData { get; set; }
        public JToken TargetData { get; set;}
    }
}
