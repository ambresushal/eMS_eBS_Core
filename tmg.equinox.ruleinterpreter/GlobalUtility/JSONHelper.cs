using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.jsonhelper
{
    public static class JSONHelper
    {

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && (token.ToString() == String.Empty) || (token.ToString()=="[]")) ||
                   (token.Type == JTokenType.Null);
        }

        public static bool IsTokenNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == "[]") ||
                   (token.Type == JTokenType.Null);
        }

        public static JObject JTokenCollectionToJObject(this List<JToken> jTokens, string jObjectPropertyName)
        {
            if (jTokens.GetType() == typeof(JArray))
                jTokens = jTokens.FirstOrDefault().ToList();

            JArray jArray = new JArray();
            JObject jObject = new JObject();
            jArray.Add(jTokens);
            jObject.Add(jObjectPropertyName, jArray);

            return jObject;
        }

        public static JObject ConvertJTokenToJObject(this JToken jToken, string propertyName)
        {
            JObject jObject = new JObject();
            jObject.Add(propertyName, jToken);
            return jObject;
        }

        public static JObject ConvertChildColumnToJproperties(this List<Child> children)
        {
            JObject childObject = new JObject();
            foreach (Child child in children)
            {
                JToken jToken = JToken.Parse(child.columns);
                JObject objChild = new JObject();
                foreach (string item in jToken.Children())
                {
                    objChild.Add(new JProperty(item, ""));
                }
                JArray jArray = new JArray(objChild);
                childObject.Add(child.name, jArray);
            }
            return childObject;
        }

        public static JToken ConvertJtokenListToJToken(this List<JToken> jTokens)
        {
            JArray jArray = new JArray();
            jArray.Add(jTokens);
            return jArray;
        }


        public static List<JToken> JSONPropertyMapper(this List<JToken> jtokens,Dictionary<string, string> properties)
        {
            List<JToken> mappedPropertJtokens = new List<JToken>();

            foreach (JToken item in jtokens)
            {
                mappedPropertJtokens.Add(Rename(item, properties));
            }
            return mappedPropertJtokens;
        }

        private  static JToken Rename(JToken json, Dictionary<string, string> map)
        {
            json = Rename(json, name => map.ContainsKey(name) ? map[name] : name);

            return json;
        }

        private static JToken Rename(JToken json, Func<string, string> map)
        {
            JProperty prop = json as JProperty;
            if (prop != null)
            {
                return new JProperty(map(prop.Name), Rename(prop.Value, map));
            }

            JArray arr = json as JArray;
            if (arr != null)
            {
                var cont = arr.Select(el => Rename(el, map));
                return new JArray(cont);
            }

            JObject o = json as JObject;
            if (o != null)
            {
                var cont = o.Properties().Select(el => Rename(el, map));
                return new JObject(cont);
            }

            return json;
        }


    }
}
