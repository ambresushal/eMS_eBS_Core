using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ComplexFunctionEvaluatorFactory
    {
        public IPlaceHolderFunctionEvaluator GetInstance(string placeHolder, JToken token, Dictionary<string, JToken> _sources)
        {
            IPlaceHolderFunctionEvaluator evaluator = null;
            string evaluatorName = GetPlaceHolderFunctionEvaluatorName(placeHolder);
            switch (evaluatorName)
            {
                case "IIF":
                    evaluator = new IIFPlaceHolderFunctionEvaluator(placeHolder, token,_sources);
                    break;
                case "IIFA":
                    evaluator = new IIFAPlaceHolderFunctionEvaluator(placeHolder, token, _sources);
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
