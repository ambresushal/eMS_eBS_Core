using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.rulecompiler
{
    class RuleFilterCompiler
    {

        Filter _filter;
        public RuleFilterCompiler(Filter filter)
        {
            _filter = filter;
        }

        public RuleFilter CompileSourceFilters()
        {
            RuleFilter ruleFilter = new RuleFilter();
            ruleFilter.FilterMergeType = string.IsNullOrEmpty(_filter.filtermergetype) ? FilterType.none : (FilterType)System.Enum.Parse(typeof(FilterType), _filter.filtermergetype);
            RuleExpressionCompiler compiler = new RuleExpressionCompiler(_filter.filtermergeexpression);
            ruleFilter.MergeAction = new Dictionary<string, RuleFilterExpression>();
            ruleFilter.KeyColumn = new Dictionary<string, string>();

            switch (ruleFilter.FilterMergeType)
            {
                case FilterType.distinct:
                    ruleFilter.KeyColumn = compiler.GetDictinctTypeFilterExpression();
                    break;
                case FilterType.expr:
                    ruleFilter.KeyColumn = RuleEngineGlobalUtility.StringItemsToDictionary(_filter.keycolumns);
                    ruleFilter.MergeAction = compiler.CompileExpression();
                    break;

                case FilterType.self:
                    ruleFilter.MergeAction = compiler.CompileSelfExpression();
                    break;

                case FilterType.none:
                    break;
                default:
                    break;
            }

            ruleFilter.Filters = new List<RuleFilterItem>();

            foreach (var filter in _filter.filterlist)
            {
                ruleFilter.Filters.Add(CompileFilterItem(filter));
            }
            return ruleFilter;
        }

        private RuleFilterItem CompileFilterItem(Filterlist filter)
        {
            RuleFilterItem item = new RuleFilterItem();
            item.FilterType = string.IsNullOrEmpty(filter.filtertype) ? FilterType.none : (FilterType)System.Enum.Parse(typeof(FilterType), filter.filtertype);
            RuleExpressionCompiler compiler = new RuleExpressionCompiler(filter.filterexpression);
            item.FilterName = filter.filtername;
            item.Expression = new Dictionary<string, RuleFilterExpression>();
            item.DistinctKeyColumn = new Dictionary<string, string>();

            switch (item.FilterType)
            {
                case FilterType.distinct:
                    item.DistinctKeyColumn = compiler.GetDictinctTypeFilterExpression();
                    break;
                case FilterType.expr:
                    item.Expression = compiler.CompileExpression();
                    break;
                case FilterType.none:
                    break;
                default:
                    break;
            }
            return item;
        }
    }
}
