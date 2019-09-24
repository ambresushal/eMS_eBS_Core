using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;

namespace tmg.equinox.document.rulebuilder.executor
{
    public class CrossJoinExecution
    {
        List<JToken> _crossJoinResult;
        List<JToken> _jTokenCollection;
        List<JToken> _propertyValues;
        string _propertyName;
        public CrossJoinExecution(List<JToken> jTokenCollection, string propertyName, List<JToken> propertyValues)
        {
            _jTokenCollection = jTokenCollection;
            _propertyValues = propertyValues;
            _crossJoinResult = new List<JToken>();
            _propertyName = propertyName;
        }

        public List<JToken> CrossJoin()
        {
            List<JToken> _crossJoinInputCollection = RuleEngineGlobalUtility.Clone<List<JToken>>(_jTokenCollection);
            foreach (JToken item in _propertyValues)
            {
                JProperty jProperty = new JProperty(_propertyName, item);
                foreach (JToken propVal in _jTokenCollection.ToList())
                {
                    if(propVal.Contains(jProperty))
                    {
                        propVal.Replace(jProperty);
                    }

                    else
                    {
                        propVal.Last.AddAfterSelf(jProperty);
                    }

                }

                //_jTokenCollection.ToList().ForEach(x => x.Last.AddAfterSelf(jProperty));
                _crossJoinResult.AddRange(_jTokenCollection);
                _jTokenCollection = RuleEngineGlobalUtility.Clone<List<JToken>>(_crossJoinInputCollection);
            }
            return _crossJoinResult;
        }
    }
}
