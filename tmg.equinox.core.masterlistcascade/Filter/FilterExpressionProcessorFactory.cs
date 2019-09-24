using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.core.masterlistcascade.filter
{
    public static class FilterExpressionProcessorFactory
    {
        public static IFilterExpressionProcessor GetProcessor(FilterExpression filter)
        {
            IFilterExpressionProcessor processor;
            string filterOperator = "";
            if(filter != null)
            {
                filterOperator = filter.Operator;
            }
            switch (filterOperator)
            {
                case "IN":
                    processor = new PlanCodeFilterExpressionProcessor();
                    break;
                default:
                    processor = new DefaultFilterExpressionProcessor();
                    break;
            }
            return processor;
        }
    }
}
