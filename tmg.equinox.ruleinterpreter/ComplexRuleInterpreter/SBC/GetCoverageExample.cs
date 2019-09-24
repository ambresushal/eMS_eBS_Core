using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.sbccalculator.Model;
using tmg.equinox.sbccalculator;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.SBC
{
    class GetCoverageExample : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();
            string varName = Utils.GetToken(data, ref from, Constants.END_ARG_ARRAY);
            if (from >= data.Length)
            {
                throw new ArgumentException("Couldn't find variable");
            }

            ParserFunction func = ParserFunction.GetFunction(varName);
            Parser.Result sourceValue = func.GetValue(data, ref from);

            var masterList = SourceManager.Get(Thread.CurrentThread, "MasterListServices").ToString();
            var BRGServiceList = SourceManager.Get(Thread.CurrentThread, "BRG").ToString();
            var networkTierList = SourceManager.Get(Thread.CurrentThread, "NetworkTierList").ToString();
            var deductibleList = SourceManager.Get(Thread.CurrentThread, "DeductibleList").ToString();
            var existingList = SourceManager.Get(Thread.CurrentThread, "CoverageExampleList").ToString();
            var rxBenefitReviewList = SourceManager.Get(Thread.CurrentThread, "RxBenefitReviewList").ToString();
            var rxCostShareList = SourceManager.Get(Thread.CurrentThread, "RxCostShareList").ToString();

            List<BenefitReview> brgServices = JsonConvert.DeserializeObject<List<BenefitReview>>(BRGServiceList.ToString());
            List<CoverageExample> masterListServices = JsonConvert.DeserializeObject<List<CoverageExample>>(masterList.ToString());
            List<NetworkList> NetworkTierList = JsonConvert.DeserializeObject<List<NetworkList>>(networkTierList.ToString());
            List<DeductibleList> DeductibleList = JsonConvert.DeserializeObject<List<DeductibleList>>(deductibleList.ToString());
            List<RxBenefitReview> RxBenefitReviewList = JsonConvert.DeserializeObject<List<RxBenefitReview>>(rxBenefitReviewList.ToString());
            List<CoverageExample> existingCoverageExampleList = JsonConvert.DeserializeObject<List<CoverageExample>>(existingList.ToString());

            List<RxCostShare> RxCostShareList = JsonConvert.DeserializeObject<List<RxCostShare>>(rxCostShareList.ToString());

            SbcCalculatorProcessor sBCProcessor = new SbcCalculatorProcessor(brgServices, masterListServices, NetworkTierList, DeductibleList, RxBenefitReviewList, existingCoverageExampleList, sourceValue.String, RxCostShareList);
            List<CoverageExample> resultServiceList = sBCProcessor.Process();
            result.Token = JToken.FromObject(resultServiceList);
            return result;
        }
    }
}
