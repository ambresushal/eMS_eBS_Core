using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class SimpleFunctionEvaluatorFactory
    {
        public IPlaceHolderFunctionEvaluator GetInstance(string placeHolder,JToken token,Dictionary<string,JToken> sources, List<LanguageFormats> languageFormats)
        {
            IPlaceHolderFunctionEvaluator evaluator = null;
            string evaluatorName = GetPlaceHolderFunctionEvaluatorName(placeHolder);
            switch (evaluatorName)
            {
                case "CS":
                case "CSMIN":
                case "CSMAX":
                    evaluator = new CostSharePlaceHolderFunctionEvaluator(placeHolder, token ,languageFormats);
                    break;
                case "FMT":
                    evaluator = new FMTPlaceHolderFunctionEvaluator(placeHolder, token, sources, languageFormats);
                    break;
            }
            return evaluator;
        }

        private string GetPlaceHolderFunctionEvaluatorName(string placeHolder)
        {
            string evaluatorName = placeHolder.Split(':')[0];
            return evaluatorName;
        }

    }
}
