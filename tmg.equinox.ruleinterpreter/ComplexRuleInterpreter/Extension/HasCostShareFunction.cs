using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class HasCostShareFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            result.String = CostShareConstant.FALSE; ;    //Set default to false

            // 1. Get the value of First Parameter (Copayment or Coinsurance)
            Parser.Result copayOrCoins = Utils.GetItem(data, ref from);

            // 2. Get the value of all sources
            Dictionary<string, JToken> sources = SourceManager.GetAll(Thread.CurrentThread);
            foreach (string source in sources.Keys)
            {
                if (!string.Equals(source, CostShareConstant.TARGET, StringComparison.OrdinalIgnoreCase))
                {
                    JToken token = sources[source];
                    int index = 1;
                    int tierCount = token.Children().Count();
                    foreach (var field in token.Children())
                    {
                        JProperty prop = ((JProperty)field);
                        if (index < tierCount)
                        {
                            index = index + 1;
                            string fieldvalue = Convert.ToString(prop.Value);
                            if (!string.IsNullOrEmpty(fieldvalue))
                            {
                                if (string.Equals(CostShareConstant.COPAY, copayOrCoins.String, StringComparison.OrdinalIgnoreCase) && fieldvalue.Contains(CostShareConstant.DOLLAR))
                                {
                                    result.String = CostShareConstant.TRUE;
                                    break;
                                }

                                if (string.Equals(CostShareConstant.COINS, copayOrCoins.String, StringComparison.OrdinalIgnoreCase) && fieldvalue.Contains(CostShareConstant.PERCENT))
                                {
                                    result.String = CostShareConstant.TRUE;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

    }
}
