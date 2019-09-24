using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.operatorutility
{
   public class EqualityComparerEvaluator
    {
       List<JToken> _target;
       List<JToken> _source;
       List<JToken> _sourceItem;
       Dictionary<string, string> _columnKeyValueDictionary;

       public EqualityComparerEvaluator(List<JToken> source, List<JToken> target, Dictionary<string, string> columnKeyValueDictionary)
       {
           _source = source;
           _target = target;
           _columnKeyValueDictionary= columnKeyValueDictionary;
       }

       public EqualityComparerEvaluator(List<JToken> source, Dictionary<string, string> columnKeyValueDictionary)
       {
           _source = source;
           _columnKeyValueDictionary = columnKeyValueDictionary;
       }


       public List<JToken> ResultForGreaterThan()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => (int)(x[item.Key]) > Int32.Parse(item.Value)).Select(sel => sel));
           }
          return _sourceItem;
       }

       public List<JToken> ResultForGreaterThanEqualTo()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => (int)(x[item.Key]) >= Int32.Parse(item.Value)).Select(sel => sel));
           }
           return _sourceItem;
       }

       public List<JToken> ResultForLessThan()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => (int)(x[item.Key]) < Int32.Parse(item.Value)).Select(sel => sel));
           }
           return _sourceItem;
       }

       public List<JToken> ResultForLessThanEqualTo()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => (int)(x[item.Key]) <= Int32.Parse(item.Value)).Select(sel => sel));
           }
           return _sourceItem;
       }

       public List<JToken> ResultForEqualToValue()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => x[item.Key].ToString() == item.Value).Select(sel => sel));
           }
           return _sourceItem;
       }

       public List<JToken> ResultForNotEqualToValue()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => x[item.Key].ToString() != item.Value).Select(sel => sel));
           }
           return _target;
       }

       public List<JToken> ResultForContainsValue()
       {
           foreach (var item in _columnKeyValueDictionary)
           {
               _sourceItem = (List<JToken>)(_sourceItem.Where(x => x[item.Key].Contains(item.Value)).Select(sel => sel));
           }
           return _sourceItem;
       }

       public List<JToken> ResultForIntersection()
       {
           return _source.Intersect(_target, new ObjectEqualityComparer(_columnKeyValueDictionary.Select(x=>x.Value).ToList())).ToList();
       }

       public List<JToken> ResultForUnion()
       {
           return _source.Union(_target, new ObjectEqualityComparer(_columnKeyValueDictionary.Select(x => x.Value).ToList())).ToList();
       }

       public List<JToken> ResultForExcept()
       {
           return _source.Except(_target, new ObjectEqualityComparer(_columnKeyValueDictionary.Select(x => x.Value).ToList())).ToList();
       }

       public List<JToken> ResultForDistinct()
       {
           return _source.Distinct(new ObjectEqualityComparer(_columnKeyValueDictionary.Select(x => x.Value).ToList())).ToList();
       }

       public List<JToken> ResultForColJoin()
       {
           return ResultForIntersection().Concat(ResultForExcept()).ToList();           
       }
    }
}
