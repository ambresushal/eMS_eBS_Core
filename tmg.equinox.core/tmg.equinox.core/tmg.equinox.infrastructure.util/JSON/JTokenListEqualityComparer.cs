using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer.RepeaterCompareUtils
{
    public class JTokenListEqualityComparer : EqualityComparer<JToken>
    {

        private List<string> keyElements;
        public JTokenListEqualityComparer(List<string> keys) : base()
        {
            keyElements = keys;
        }

        public override bool Equals(JToken x, JToken y)
        {
            bool result = false;
            var dictX = x as IDictionary<string, JToken>;
            var dictY = y as IDictionary<string, JToken>;
            int match = 0;
            foreach (string ele in keyElements)
            {
                if (((dictX.ContainsKey(ele) && dictX[ele] != null)) && ((dictY.ContainsKey(ele) && dictY[ele] != null)))
                {
                    if (dictX[ele].ToString().Trim() == dictY[ele].ToString().Trim())
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
            foreach (string ele in keyElements)
            {
                if (dict.ContainsKey(ele) && dict[ele] != null)
                {
                    combine = combine + dict[ele].ToString().Trim();
                }
            }
            return combine.GetHashCode();
        }
    }
}
