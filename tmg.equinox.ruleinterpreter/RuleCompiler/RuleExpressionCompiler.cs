using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    class RuleExpressionCompiler
    {
        string _filterExpression;
        public List<string> expressionList = new List<string>();

        public RuleExpressionCompiler(string filterExpression)
        {
            _filterExpression = filterExpression;
        }

        public Dictionary<string, RuleFilterExpression> CompileExpression()
        {
            Dictionary<string, RuleFilterExpression> expressionsDictionary = new Dictionary<string, RuleFilterExpression>();
            ExpressionCompiler expressionCompiler = new ExpressionCompiler();
            expressionsDictionary = expressionCompiler.BuildRuleFilterExpressionFromString(_filterExpression);
            return expressionsDictionary;
        }

        public string GetCodeBlock()
        {
            return _filterExpression;
        }

        public Dictionary<string, string> GetDictinctTypeFilterExpression()
        {
            return RuleEngineGlobalUtility.StringItemsToDictionary(_filterExpression);
        }

        public Dictionary<string, RuleFilterExpression> CompileSelfExpression()
        {
            Dictionary<string, RuleFilterExpression> selfExpressionsDictionary = new Dictionary<string, RuleFilterExpression>();
            RuleFilterExpression selfFilterExpression = new RuleFilterExpression();
            selfFilterExpression.ExecutionType = ExecutionType.self;
            selfFilterExpression.LeftOperand.OperandValue = _filterExpression.Substring(_filterExpression.IndexOf('@') + 1, 1).Trim().TrimStart().TrimEnd();
            selfExpressionsDictionary.Add("0", selfFilterExpression);

            return selfExpressionsDictionary;
        }

    }
}
