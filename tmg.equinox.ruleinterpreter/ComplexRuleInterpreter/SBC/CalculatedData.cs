using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using tmg.equinox.sbccalculator.Model;
using tmg.equinox.sbccalculator;


namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.SBC
{
    class CalculatedData : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            var BRGServiceList = SourceManager.Get(Thread.CurrentThread, "BRG").ToString();
            var networkTierList = SourceManager.Get(Thread.CurrentThread, "NetworkTierList").ToString();
            var deductibleList = SourceManager.Get(Thread.CurrentThread, "DeductibleList").ToString();
            var rxBenefitReviewList = SourceManager.Get(Thread.CurrentThread, "RxBenefitReviewList").ToString();
            var defaultSBCServices = SourceManager.Get(Thread.CurrentThread, "DefaultSBCServices").ToString();
            List<BenefitReview> brgServices = JsonConvert.DeserializeObject<List<BenefitReview>>(BRGServiceList.ToString());
            List<NetworkList> NetworkTierList = JsonConvert.DeserializeObject<List<NetworkList>>(networkTierList.ToString());
            List<DeductibleList> DeductibleList = JsonConvert.DeserializeObject<List<DeductibleList>>(deductibleList.ToString());
            List<RxBenefitReview> RxBenefitReviewList = JsonConvert.DeserializeObject<List<RxBenefitReview>>(rxBenefitReviewList.ToString());
            CoverageExampleWrapper wrapperObj = new CoverageExampleWrapper();


            var FractureList = SourceManager.Get(Thread.CurrentThread, "FractureCoverageExample").ToString();
            var MaternityList = SourceManager.Get(Thread.CurrentThread, "MaternityCoverageExample").ToString();
            var DiabetesList = SourceManager.Get(Thread.CurrentThread, "DiabetesCoverageExample").ToString();
            wrapperObj.FractureCoverageExample = JsonConvert.DeserializeObject<List<CoverageExample>>(FractureList.ToString());
            wrapperObj.MaternityCoverageExample = JsonConvert.DeserializeObject<List<CoverageExample>>(MaternityList.ToString());
            wrapperObj.DiabetesCoverageExample = JsonConvert.DeserializeObject<List<CoverageExample>>(DiabetesList.ToString());

            CalculatedDataProcessor sBCProcessor = new CalculatedDataProcessor(brgServices, NetworkTierList, DeductibleList, RxBenefitReviewList, wrapperObj, defaultSBCServices);
            List<CalculatedDataModel> resultServiceList = sBCProcessor.Process();
            result.Token = JToken.FromObject(resultServiceList);
            return result;
        }
    }
}
