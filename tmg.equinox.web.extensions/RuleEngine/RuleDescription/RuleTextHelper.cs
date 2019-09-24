using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.RuleEngine.RuleDescription
{
    public class RuleTextHelper
    {
        public static string GetLogicalOperatorById(int operatorId)
        {
            string operatorType = "and";
            if (operatorId == 2)
            {
                operatorType = "or";
            }

            return operatorType;
        }

        public static string GetOperatorTypeById(int operatorId)
        {
            string operatorType = string.Empty;
            switch (operatorId)
            {
                case 1:
                    operatorType = "equals";
                    break;
                case 2:
                    operatorType = "greater than";
                    break;
                case 3:
                    operatorType = "less than";
                    break;
                case 4:
                    operatorType = "contains";
                    break;
                case 5:
                    operatorType = "not equals";
                    break;
                case 6:
                    operatorType = "greater than or equal to";
                    break;
                case 7:
                    operatorType = "less than or equal to";
                    break;
                case 8:
                    operatorType = "is null";
                    break;
                case 9:
                    operatorType = "custom";
                    break;
                case 10:
                    operatorType = "not contains";
                    break;
            }

            return operatorType;
        }
    }
}
