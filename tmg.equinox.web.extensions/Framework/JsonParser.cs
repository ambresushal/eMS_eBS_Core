using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.ruleinterpreter.operatorutility;

namespace tmg.equinox.web.Framework
{
    public class JsonParser
    {
        public Dictionary<string, object> GetFirstPart(JObject objJson)
        {
            Dictionary<string, object> partData = new Dictionary<string, object>();
            int i = 0;
            foreach (var item in objJson)
            {
                if (i == 0)
                {
                    partData.Add(item.Key, item.Value);
                    return partData;
                }
                i = i + 1;
                break;
            }
            return null;
        }

        public Dictionary<string, object> GetPart(string partName, JObject objJson)
        {
            Dictionary<string, object> partData = new Dictionary<string, object>();
            partData.Add(partName, objJson[partName]);
            return partData;
        }

        public JObject UpdatePart(string partName, string partData, JObject objJson)
        {
            objJson[partName] = JObject.Parse(partData);
            return objJson;
        }
    }
}