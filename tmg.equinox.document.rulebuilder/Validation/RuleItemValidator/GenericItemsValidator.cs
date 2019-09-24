using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.validation
{
    public class GenericItemsValidator
    {
        public void ValidateExpression(FilterType filterType, string filterExpression)
        {

            switch (filterType)
            {
                case FilterType.distinct:
                    ValidateDistinctTypeExpression(filterExpression);
                    break;
                case FilterType.expr:
                    ValidateRuleTypeExpression(filterExpression);
                    break;
                case FilterType.none:
                    break;
                default:
                    break;
            }

        }

        public void ValidateFilterType()
        {


        }

        private void ValidateDistinctTypeExpression(string filterExpression)
        {

        }

        private void ValidateRuleTypeExpression(string filterExpression)
        {

        }
    }
}
