using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.web.DataSource.SyncManager
{
    public class JTokenEqualityComparer : EqualityComparer<JToken>
    {
        private Dictionary<string, string> _keysToCompare { get; set; }

        public JTokenEqualityComparer(Dictionary<string, string> keysToCompare)
            : base()
        {
            this._keysToCompare = keysToCompare;
        }

        public override bool Equals(JToken x, JToken y)
        {
            var sourceObj = x as IDictionary<string, JToken>;
            var targetObj = y as IDictionary<string, JToken>;

            bool isEqual = true;
            foreach (var item in _keysToCompare)
            {
                if (!sourceObj.ContainsKey(item.Key) && targetObj.ContainsKey(item.Value)) continue;
                if (sourceObj.ContainsKey(item.Key) && targetObj.ContainsKey(item.Value))
                {
                    string sourceValue = Convert.ToString(sourceObj[item.Key]);
                    string targetValue = Convert.ToString(targetObj[item.Value]);
                    if (string.IsNullOrEmpty(sourceValue) && string.IsNullOrEmpty(targetValue)) continue;
                    if (sourceValue != targetValue) { isEqual = false; break; }
                }
                else
                {
                    isEqual = false; break;
                }
            }
            return isEqual;
        }

        public override int GetHashCode(JToken obj)
        {
            var dict = obj as IDictionary<string, JToken>;
            string combine = "";
            foreach (var item in _keysToCompare)
            {
                if (dict.ContainsKey(item.Key) && dict[item.Key] != null)
                {
                    combine = combine + dict[item.Key].ToString();
                }
                else if (dict.ContainsKey(item.Value) && dict[item.Value] != null)
                {
                    combine = combine + dict[item.Value].ToString();
                }
            }
            return combine.GetHashCode();
        }
    }
}