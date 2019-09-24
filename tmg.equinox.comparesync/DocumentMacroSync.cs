using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.documentcomparer
{
    public class DocumentMacroSync
    {
        private string _macroJSON;
        private JObject _objMacro;

        public DocumentMacroSync(string macroJSON)
        {
            this._macroJSON = macroJSON;
            this._objMacro = JObject.Parse(this._macroJSON);
        }

        public List<string> GetElementFromMacro()
        {
            List<string> childElements = new List<string>();
            List<JProperty> rootSections = _objMacro.Properties().ToList();
            foreach (var section in rootSections)
            {
                GetSelectedSections(section, ref childElements);
            }
            return childElements;
        }

        private void GetSelectedSections(JProperty container, ref List<string> results)
        {
            List<JToken> tokens = container.Values().ToList();
            bool hasFields = false;
            JToken fields = null;
            foreach (JToken token in tokens)
            {
                if (token.Path.EndsWith(".Fields") == true)
                {
                    if (token.First().ToString() != "[]")
                    {
                        hasFields = true;
                        fields = token;
                    }
                }
            }
            if (hasFields == true)
            {
                results.Add(fields.Parent.Path);
            }
            foreach (JToken token in tokens)
            {
                if (!token.Path.EndsWith(".Fields") && !token.Path.EndsWith(".Keys") && !token.Path.EndsWith(".Label") && !token.Path.EndsWith(".IsMacroSelected"))
                {
                    GetSelectedSections((JProperty)token, ref results);
                }
            }
        }

        public List<string> GetChildElements(string path)
        {
            List<string> childs = new List<string>();

            JToken token = _objMacro.SelectToken(path)["Fields"];
            foreach (var item in token.Children())
            {
                childs.Add(path + "." + (string)item["Field"]);
            }

            return childs;
        }

        public List<string> GetRepeaterKey(string path)
        {
            List<string> keys = new List<string>();

            JToken token = _objMacro.SelectToken(path)["Keys"];
            foreach (var item in token.Children())
            {
                keys.Add(path + "." + (string)item["Key"]);
            }

            return keys;
        }

        public List<Criteria> GetRepeaterKeywithCriteria(string repeaterName)
        {
            List<Criteria> keys = new List<Criteria>();

            JToken token = _objMacro.SelectToken(repeaterName)["Keys"];
            foreach (var item in token.Children())
            {
                keys.Add(
                    new Criteria()
                    {
                        Key = (string)item["Key"],
                        Source = (string)item["SourceValue"],
                        Target = (string)item["TargetValue"],
                        Data = "",
                        Label = (string)item["Label"],
                        isChecked = (!string.IsNullOrEmpty((string)item["SourceValue"]) && string.IsNullOrEmpty((string)item["TargetValue"])),
                        UIElementPath = repeaterName + "." + (string)item["Key"]
                    });
            }

            return keys;
        }

        public List<SelectedItem> GetRepeaterList()
        {
            List<SelectedItem> repeaters = new List<SelectedItem>();
            List<JProperty> rootSections = _objMacro.Properties().ToList();
            foreach (var section in rootSections)
            {
                this.GetRepeaterFromMacro(section, ref repeaters);
            }
            return repeaters;
        }

        private void GetRepeaterFromMacro(JProperty container, ref List<SelectedItem> results)
        {
            List<JToken> tokens = container.Values().ToList();
            bool hasFields = false;
            bool hasKeys = false;
            JToken keysToken = null;

            foreach (JToken token in tokens)
            {
                if (token.Path.EndsWith(".Fields") == true)
                {
                    if (token.First().ToString() != "[]")
                    {
                        hasFields = true;
                    }
                }
                if (token.Path.EndsWith(".Keys") == true)
                {
                    hasKeys = true;
                    keysToken = token;
                }
            }
            if (hasFields == true)
            {
                if (hasKeys == true)
                {
                    SelectedItem objItem = new SelectedItem();
                    objItem.Value = keysToken.Parent.Path;
                    foreach (var key in ((JProperty)keysToken).Value)
                    {
                        if (!string.IsNullOrEmpty((string)key["SourceValue"]))
                        {
                            objItem.IsSet = true;
                            break;
                        }
                    }
                    results.Add(objItem);
                }
            }
            foreach (JToken token in tokens)
            {
                if (!token.Path.EndsWith(".Fields") && !token.Path.EndsWith(".Keys") && !token.Path.EndsWith(".Label") && !token.Path.EndsWith(".IsMacroSelected"))
                {
                    GetRepeaterFromMacro((JProperty)token, ref results);
                }
            }
        }
    }


    public class SelectedItem
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public bool IsSet { get; set; }
    }

    public class Criteria
    {
        public string UIElementPath { get; set; }
        public string Label { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool isChecked { get; set; }
    }
}