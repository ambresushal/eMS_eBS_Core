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
    class GetCostShareFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            // 1. Get the value of First Parameter (Min or Max)
            Parser.Result minOrMax = Utils.GetItem(data, ref from);

            // 2. Get the value of Second Parameter (Copay or Coinsurance)
            Parser.Result copayOrCoins = Utils.GetItem(data, ref from);

            // 3. Get the value of Third Parameter (IN Tier - Min)
            JToken InTierMin = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIERMIN);
            string inTierMinVal = InTierMin != null ? InTierMin.ToString() : "";

            // 4. Get the value of Third Parameter (IN Tier - Max)
            JToken InTierMax = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIERMAX);
            string inTierMaxVal = InTierMax != null ? InTierMax.ToString() : "";

            // 5. Get the value of Third Parameter (IN Tier 1 - Min)
            JToken InTier1Min = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER1MIN);
            string inTier1MinVal = InTier1Min != null ? InTier1Min.ToString() : "";

            // 6. Get the value of Third Parameter (IN Tier 1 - Max)
            JToken InTier1Max = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER1MAX);
            string inTier1MaxVal = InTier1Max != null ? InTier1Max.ToString() : "";

            // 7. Get the value of Third Parameter (IN Tier 2 - Min)
            JToken InTier2Min = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER2MIN);
            string inTier2MinVal = InTier2Min != null ? InTier2Min.ToString() : "";

            // 8. Get the value of Third Parameter (IN Tier 2 - Max)
            JToken InTier2Max = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER2MAX);
            string inTier2MaxVal = InTier2Max != null ? InTier2Max.ToString() : "";

            // 9. Get the value of Third Parameter (IN Tier 3 - Min)
            JToken InTier3Min = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER3MIN);
            string inTier3MinVal = InTier3Min != null ? InTier3Min.ToString() : "";

            // 10 Get the value of Third Parameter (IN Tier 3 - Max)
            JToken InTier3Max = SourceManager.Get(Thread.CurrentThread, CostShareConstant.INTIER3MAX);
            string inTier3MaxVal = InTier3Max != null ? InTier3Max.ToString() : "";

            string costShare = CostShareHelper.Evaluate(minOrMax.String, copayOrCoins.String, inTierMinVal, inTierMaxVal, inTier1MinVal, inTier1MaxVal, inTier2MinVal, inTier2MaxVal, inTier3MinVal, inTier3MaxVal);
            result.String = costShare;

            return result;
        }

    }
}
