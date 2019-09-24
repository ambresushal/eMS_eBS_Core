using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplateRowSource
    {
        public string SourceName { get; set; }
        public JToken Source { get; set; }
        public ReportTemplateRowSourceType SourceType { get; set; }
        public RepeatForExpression RepeatFor { get; set; }

    }
}
