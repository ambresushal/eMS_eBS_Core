using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class PlaceHolderEvaluatorFactory
    {
        public IPlaceHolderEvaluator GetInstance(string placeHolder,JToken token)
        {
            IPlaceHolderEvaluator evaluator = null;
            string evaluatorName = GetPlaceHolderEvaluatorName(placeHolder);
            switch (evaluatorName)
            {
                case "CS":
                case "CSMIN":
                case "CSMAX":
                    evaluator = new CostSharePlaceHolderEvaluator(placeHolder, token);
                    break;
                case "IIF":
                    evaluator = new IIFPlaceHolderEvaluator(placeHolder, token);
                    break;
            }
            return evaluator;
        }

        private string GetPlaceHolderEvaluatorName(string placeHolder)
        {
            string evaluatorName = placeHolder.Split(':')[0];
            return evaluatorName;
        }

    }
}
