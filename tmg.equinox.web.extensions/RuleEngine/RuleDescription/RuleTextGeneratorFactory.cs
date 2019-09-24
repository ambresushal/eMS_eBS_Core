using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.RuleEngine.RuleDescription
{
    public class RuleTextGeneratorFactory
    {
        public static RuleTextGenerator GetGenerator(int ruleTypeId)
        {
            RuleTextGenerator obj = null;
            switch (ruleTypeId)
            {
                case 1:
                    obj = new EnableRuleTextGenerator();
                    break;
                case 3:
                    obj = new ValueRuleTextGenerator();
                    break;
                case 4:
                    obj = new VisibleRuleTextGenerator();
                    break;
                case 2:
                    obj = new ValidationRuleTextGenerator();
                    break;
                case 5:
                    obj = new RequiredRuleTextGenerator();
                    break;
                case 6:
                    obj = new ErrorRuleTextGenerator();
                    break;
                case 8:
                    obj = new HighlightRuleTextGenerator();
                    break;
                case 9:
                    obj = new DialogRuleTextGenerator();
                    break;
                case 10:
                    obj = new CustomRuleTextGenerator();
                    break;
            }
            return obj;
        }
    }
}
