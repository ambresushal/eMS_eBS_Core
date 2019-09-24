using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper;
using System.Text.RegularExpressions;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension
{
    class SetServicesFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            result.String = string.Empty;    //Set default to false

            List<int> indexes = new List<int>();

            // 1. Get the value of First Parameter (Copayment or Coinsurance)
            Parser.Result copayOrCoins = Utils.GetItem(data, ref from);

            // 2. Get the value of Second Parameter (List of Services - Array)
            Parser.Result serviceList = Utils.GetItem(data, ref from);
            string[] list = serviceList.String.Split(',');
            JArray services = JArray.FromObject(list);

            // 3. Get the value of all sources
            Dictionary<string, JToken> sources = SourceManager.GetAll(Thread.CurrentThread);
            foreach (string source in sources.Keys)
            {
                if (!string.Equals(source, CostShareConstant.TARGET, StringComparison.OrdinalIgnoreCase))
                {
                    JToken token = sources[source];
                    int counter = 1;
                    int tierCount = token.Children().Count();
                    foreach (var field in token.Children())
                    {
                        JProperty prop = ((JProperty)field);
                        if (counter < tierCount)
                        {
                            counter = counter + 1;
                            string fieldvalue = Convert.ToString(prop.Value);
                            if (!string.IsNullOrEmpty(fieldvalue))
                            {
                                int index;
                                if (string.Equals(CostShareConstant.COPAY, copayOrCoins.String, StringComparison.OrdinalIgnoreCase) && fieldvalue.Contains(CostShareConstant.DOLLAR))
                                {
                                    bool isNumber = Int32.TryParse(Regex.Match(source, CostShareConstant.PATTERN).Value, out index);
                                    if (isNumber)
                                    {
                                        indexes.Add(index);
                                    }
                                }

                                if (string.Equals(CostShareConstant.COINS, copayOrCoins.String, StringComparison.OrdinalIgnoreCase) && fieldvalue.Contains(CostShareConstant.PERCENT))
                                {
                                    bool isNumber = Int32.TryParse(Regex.Match(source, CostShareConstant.PATTERN).Value, out index);
                                    if (isNumber)
                                    {
                                        indexes.Add(index);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = services.Count; i >= 1; i--)
            {
                if (!indexes.Contains(i))
                {
                    services.RemoveAt(i - 1);
                }
            }

            result.String = services.ToString();
            return result;
        }

    }
}
