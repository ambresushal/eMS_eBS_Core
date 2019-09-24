using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.executor
{
    public class CollectionValueCompareExecution
    {
        List<JToken> _source;
        string _leftOperand;
        string _rightOperand;

        public CollectionValueCompareExecution(List<JToken> source, string leftOperand, string rightOperand)
        {
            _source = source;
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
        }

        public List<JToken> GreaterThan()
        {
            return (_source.Where(x => (int)(x[_leftOperand]) > Int32.Parse(_rightOperand)).Select(sel => sel)).ToList();
        }

        public List<JToken> GreaterThanEqualTo()
        {
            return (_source.Where(x => (int)(x[_leftOperand]) >= Int32.Parse(_rightOperand)).Select(sel => sel)).ToList();
        }

        public List<JToken> LessThan()
        {
            return (_source.Where(x => (int)(x[_leftOperand]) < Int32.Parse(_rightOperand)).Select(sel => sel)).ToList();
        }

        public List<JToken> LessThanEqualTo()
        {
            return (_source.Where(x => (int)(x[_leftOperand]) < Int32.Parse(_rightOperand)).Select(sel => sel)).ToList();
        }

        public List<JToken> EqualTo()
        {
            return (_source.Where(x => x[_leftOperand].ToString() == _rightOperand).Select(sel => sel)).ToList(); 
        }

        public List<JToken> NotEqualTo()
        {
            return (_source.Where(x => x[_leftOperand].ToString() != _rightOperand).Select(sel => sel)).ToList();
        }

        public List<JToken> Contains()
        {
            return (_source.Where(x => x[_leftOperand].Contains(_rightOperand)).Select(sel => sel)).ToList();
        }
    }
}
