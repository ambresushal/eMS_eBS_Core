using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace tmg.equinox.documentcomparer
{
    public class DocumentDataHelper
    {
        private string _repeaterName { get; set; }
        private string _repeaterKeys { get; set; }
        private string _sourceDocumentData { get; set; }
        private dynamic _objKey { get; set; }
        private JObject _objDocumentData { get; set; }

        public DocumentDataHelper(string repeaterName, string repeaterKeys, string docuemntData)
        {
            this._repeaterName = repeaterName;
            this._repeaterKeys = repeaterKeys;
            this._sourceDocumentData = docuemntData;
            this._objKey = JsonConvert.DeserializeObject(repeaterKeys);
            this._objDocumentData = JObject.Parse(docuemntData);
        }

        public dynamic GetRepeaterListData()
        {
            List<string> options;
            string value = null;
            try
            {
                foreach (var item in _objKey)
                {
                    options = new List<string>();
                    options.Add("Select One:Select One;");

                    string key = item["Key"];
                    JToken repeaterToken = _objDocumentData.SelectToken(_repeaterName);

                    foreach (var row in repeaterToken)
                    {
                        if (row[key] != null)
                        {
                            value = (string)row[key];
                            if (!string.IsNullOrEmpty(value.Trim()))
                            {
                                options.Add(value + ":" + value + ";");
                            }
                        }
                        else
                        {
                            foreach (var prop in row)
                            {
                                if (((JProperty)prop).Value.Type == JTokenType.Array)
                                {
                                    foreach (var childRow in prop)
                                    {
                                        foreach (var column in childRow)
                                        {
                                            if (column[key] != null)
                                            {
                                                value = (string)column[key];
                                                if (!string.IsNullOrEmpty(value.Trim()))
                                                {
                                                    options.Add(value + ":" + value + ";");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string dropdownitems = string.Join(string.Empty, options.Distinct().ToArray());
                    item["Data"] = dropdownitems.Remove(dropdownitems.Length - 1);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
            return _objKey;
        }


    }
}