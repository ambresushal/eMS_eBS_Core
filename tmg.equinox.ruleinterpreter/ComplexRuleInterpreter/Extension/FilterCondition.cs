using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper
{
    public class Condition
    {
        public string LogicalOperator { get; set; }
        public List<Expression> Expression { get; set; }
    }

    public class Expression
    {
        public string Container { get; set; }
        public string Column { get; set; }
        public string Operand { get; set; }
        public string Value { get; set; }
    }
}
