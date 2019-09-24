using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.executor;
using tmg.equinox.document.rulebuilder.operatorutility;

namespace tmg.equinox.document.rulebuilder.executor
{
    public class CollectionExecutionComparer
    {
        List<JToken> _target;
        List<JToken> _source;
        Dictionary<string, string> _keyColumns;

        public CollectionExecutionComparer(List<JToken> source, Dictionary<string, string> keyColumns)
        {
            _source = source;
            _keyColumns = keyColumns;
        }

        public CollectionExecutionComparer(List<JToken> source, List<JToken> target, Dictionary<string, string> keyColumns)
        {
            _source = source;
            _target = target;
            _keyColumns = keyColumns;
        }

        public List<JToken> Intersection()
        {
            return _source.Intersect(_target, new ObjectEqualityComparer(_keyColumns.Select(x => x.Value).ToList())).ToList();
        }

        public List<JToken> Union()
        {
            return _source.Union(_target, new ObjectEqualityComparer(_keyColumns.Select(x => x.Value).ToList())).ToList();
        }

        public List<JToken> Except()
        {
            return _source.Except(_target, new ObjectEqualityComparer(_keyColumns.Select(x => x.Value).ToList())).ToList();
        }

        public List<JToken> Distinct()
        {
            return _source.Distinct(new ObjectEqualityComparer(_keyColumns.Select(x => x.Value).ToList())).ToList();
        }

        public List<JToken> ColJoin()
        {
            CollectionExecutionComparer collectionCompare = new CollectionExecutionComparer(_target, _source, _keyColumns);
            JArray targetObject = JArray.FromObject(collectionCompare.Intersection());
            JArray sourceObject = JArray.FromObject(Intersection());

            var mergeSettingsMerge = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Merge

            };
            
            sourceObject.Merge(targetObject, mergeSettingsMerge);
            return sourceObject.ToList();
        }
    }
}
