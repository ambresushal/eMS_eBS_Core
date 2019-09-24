using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ExpressionTemplate
    {
        public string LogicalOperator { get; set; }
        public string TemplateExp { get; set; }
        public string Template { get; set; }
        public List<DocumentAlias> Variables { get; set; }
        public bool IsValid { get; set; }
    }
}
