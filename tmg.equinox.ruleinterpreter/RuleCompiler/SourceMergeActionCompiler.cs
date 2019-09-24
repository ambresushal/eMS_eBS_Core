using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    public class SourceMergeActionCompiler
    {
        List<Sourcemergeaction> _sourceMergeActions;
        List<SourceMergeAction> _compiledSourceMergeActions;

        public SourceMergeActionCompiler(List<Sourcemergeaction> sourceMergeActions)
        {
            _sourceMergeActions = sourceMergeActions;
            _compiledSourceMergeActions = new List<SourceMergeAction>();
        }

        public List<SourceMergeAction> GetSourceMergeActions()
        {
            foreach (var mergeAction in _sourceMergeActions)
            {
                _compiledSourceMergeActions.Add(SourceMergeAction(mergeAction));
            }
            return _compiledSourceMergeActions;
        }

        private SourceMergeAction SourceMergeAction(Sourcemergeaction sourceMergeAction)
        {
            SourceMergeAction compiledMergedSource = new SourceMergeAction();
            compiledMergedSource.MergeExpression = new Dictionary<string, RuleFilterExpression>();
            compiledMergedSource.SourceMergeType = string.IsNullOrEmpty(sourceMergeAction.sourcemergetype) ? FilterType.none : (FilterType)System.Enum.Parse(typeof(FilterType), sourceMergeAction.sourcemergetype);
            RuleExpressionCompiler compiler = new RuleExpressionCompiler(sourceMergeAction.sourcemergeexpression);

            switch (compiledMergedSource.SourceMergeType)
            {
                case FilterType.script:
                    compiledMergedSource.CodeBlock = compiler.GetCodeBlock();
                    break;
                case FilterType.none:
                    break;
                default:
                    break;
            }
            return compiledMergedSource;
        }
    }
}
