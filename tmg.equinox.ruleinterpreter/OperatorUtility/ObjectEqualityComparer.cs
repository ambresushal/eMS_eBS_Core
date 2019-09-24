using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.operatorutility
{
    public class ObjectEqualityComparer : EqualityComparer<JToken>
    {

        private Dictionary<string,string> keyElements;

        public ObjectEqualityComparer(List<string> keys)
            : base()
        {
            keyElements = new Dictionary<string, string>();
            foreach (string key in keys)
            {
                if (key.IndexOf(':') > -1)
                {
                    keyElements.Add(key.Split(':')[0], key.Split(':')[1]);
                }
                else
                {
                    keyElements.Add(key, key);
                }
            }
        }

        public override bool Equals(JToken x, JToken y)
        {
            bool result = false;
            var dictX = x as IDictionary<string, JToken>;
            var dictY = y as IDictionary<string, JToken>;
            int match = 0;
            foreach (var ele in keyElements)
            {
                if (((dictX.ContainsKey(ele.Key) && dictX[ele.Key] != null)) && ((dictY.ContainsKey(ele.Value) && dictY[ele.Value] != null)))
                {
                    if (dictX[ele.Key].ToString().Trim().Replace("&amp;", "&") == dictY[ele.Value].ToString().Trim().Replace("&amp;", "&"))
                    {
                        match++;
                    }
                }
            }
            if (match == keyElements.Count)
            {
                result = true;
            }
            return result;
        }

        public override int GetHashCode(JToken obj)
        {
            var dict = obj as IDictionary<string, JToken>;
            string combine = "";
            foreach (var ele in keyElements)
            {
                if (dict.ContainsKey(ele.Key) && dict[ele.Key] != null)
                {
                    combine = combine + dict[ele.Key].ToString().Trim().Replace("&amp;", "&");
                }
                else if (dict.ContainsKey(ele.Value) && dict[ele.Value] != null)
                {
                    combine = combine + dict[ele.Value].ToString().Trim().Replace("&amp;", "&");
                }
            }
            return combine.GetHashCode();
        }
    }
}
