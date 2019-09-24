using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.executor;

namespace tmg.equinox.ruleinterpreter.ruletargetsync
{
    public class UpdateTargetElement
    {
        List<JToken> _newlyAddedRows;
        List<JToken> _rowsToAdd;
        List<JToken> _rowsUpdate;
        Dictionary<string, string> _keyColumn;
        List<JToken> _target;
        List<JToken> _ruleOutput;

        public UpdateTargetElement(JToken targetJToken, JToken ruleOutputJToken, Dictionary<string, string> keyColumn)
        {
            _target = targetJToken.ToList();
            _ruleOutput = ruleOutputJToken.ToList();
            _keyColumn = keyColumn;
            Initialize();
        }

        private void Initialize()
        {
            CollectionExecutionComparer ruleOutputComparer;
            ruleOutputComparer = new CollectionExecutionComparer(_ruleOutput, _target, _keyColumn);

            _rowsToAdd = ruleOutputComparer.Except();
            _rowsUpdate = ruleOutputComparer.Intersection();
        }

        public List<JToken> GetRuleTargetOutput()
        {
            List<JToken> _targetResult = new List<JToken>();
            _targetResult.AddRange(_rowsToAdd);
            _targetResult.AddRange(_rowsUpdate);
            CollectionExecutionComparer distinctComparer = new CollectionExecutionComparer(_targetResult, _keyColumn);
            return distinctComparer.Distinct();
        }
    }

}
