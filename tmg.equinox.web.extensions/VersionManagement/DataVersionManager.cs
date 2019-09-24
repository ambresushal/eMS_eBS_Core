using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.web.VersionManagement
{
    public class VersionDataSynchronizer
    {
        private string currentFormData;
        private string latestDefaultData;
        private ExpandoObject currentDataObj;
        private ExpandoObject latestDefaultObj;
        private ExpandoObjectConverter converter;

        public VersionDataSynchronizer(string currentFormData, string latestDefaultData)
        {
            this.currentFormData = currentFormData;
            this.latestDefaultData = latestDefaultData;
            converter = new ExpandoObjectConverter();
            currentDataObj = JsonConvert.DeserializeObject<ExpandoObject>(currentFormData, converter);
            latestDefaultObj = JsonConvert.DeserializeObject<ExpandoObject>(latestDefaultData, converter);
        }

        public string Synchronize()
        {
            var targetObj = latestDefaultObj as IDictionary<string, object>;
            var sourceObj = currentDataObj as IDictionary<string, object>;
            foreach (var exp in targetObj)
            {
                if (sourceObj.ContainsKey(exp.Key))
                {
                    CopyChildValues(sourceObj[exp.Key], exp.Value);
                }
            }
            return JsonConvert.SerializeObject(latestDefaultObj);
        }

        private void CopyChildValues(dynamic source, dynamic target)
        {
            if (source is ExpandoObject && target is ExpandoObject)
            {
                var src = source as IDictionary<string, object>;
                var tar = target as IDictionary<string, object>;
                foreach (var trKey in new List<string>(tar.Keys))
                {
                    if (src.ContainsKey(trKey))
                    {
                        var sr = src[trKey];
                        var tr = tar[trKey];
                        if ((tr is ExpandoObject && sr is ExpandoObject) || (tr is ExpandoObject && sr is string))
                        {
                            CopyChildValues(sr, tr);
                        }
                        else if (tr is System.Collections.Generic.IList<Object> && sr is System.Collections.Generic.IList<Object>)
                        {
                            var srList = sr as System.Collections.Generic.IList<Object>;
                            var trList = tr as System.Collections.Generic.IList<Object>;
                            if (srList.Count > 0 && trList.Count > 0)
                            {
                                foreach (var s in srList)
                                {
                                    string json = JsonConvert.SerializeObject(trList[0]);
                                    var converter = new ExpandoObjectConverter();
                                    ExpandoObject addTr = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
                                    trList.Add(addTr);
                                    CopyChildValues(s, addTr);
                                }
                                if (srList.Count > 0)
                                {
                                    trList.RemoveAt(0);
                                }
                            }
                        }
                        else if (tr is String && sr is System.Collections.Generic.IList<Object>)
                        {
                            if (sr != null)
                            {
                                IList<object> srcObject = sr as IList<object>;
                                if (srcObject != null && srcObject.Count > 0)
                                {
                                    JArray arr = new JArray();
                                    foreach (var item in srcObject)
                                    {
                                        if (item is ExpandoObject)
                                        {
                                            var eoAsDict = ((IDictionary<String, Object>)item);
                                            arr.Add(eoAsDict.ElementAt(0).Value);
                                        }
                                        else
                                            arr.Add(item);
                                    }
                                    tar[trKey] = arr;
                                }
                            }
                        }
                        else
                        {
                            if (sr != null)
                            {
                                tar[trKey] = sr.ToString();
                            }
                        }
                    }
                }
            }
        }

        public bool isSyncRequired()
        {
            return isSyncRequired(currentDataObj, latestDefaultObj);
        }
        private bool isSyncRequired(IDictionary<string, object> sourceObj, IDictionary<string, object> targetObj)
        {
            bool syncRequired = false;
            if (sourceObj.Keys.Count == targetObj.Keys.Count)
            {
                foreach (var key in sourceObj.Keys)
                {
                    if (targetObj.ContainsKey(key) == true)
                    {
                        if (sourceObj[key] is ExpandoObject && targetObj[key] is ExpandoObject)
                        {
                            syncRequired = isSyncRequired(sourceObj[key] as IDictionary<string, object>, targetObj[key] as IDictionary<string, object>);
                        }
                        else if (sourceObj[key] is System.Collections.Generic.IList<Object> && targetObj[key] is System.Collections.Generic.IList<Object>)
                        {
                            syncRequired = isSyncRequiredForRepeaterData(sourceObj[key] as System.Collections.Generic.IList<Object>, targetObj[key] as System.Collections.Generic.IList<Object>);
                        }
                    }
                    else
                    {
                        syncRequired = true;
                    }
                    if (syncRequired == true)
                    {
                        break;
                    }
                }
            }
            else
            {
                syncRequired = true;
            }
            return syncRequired;
        }

        private bool isSyncRequiredForRepeaterData(System.Collections.Generic.IList<Object> sr, System.Collections.Generic.IList<Object> tr)
        {
            bool syncRequired = false;
            //check if length of sr or tr is 0, then skip
            if ((sr != null && sr.Count > 0) && (tr != null && tr.Count > 0))
            {
                //sync only if keys are different
                IDictionary<string, object> srcObject = sr[0] as IDictionary<string, object>;
                IDictionary<string, object> tarObject = tr[0] as IDictionary<string, object>;
                srcObject.Remove("RowIDProperty");
                if (srcObject.Keys.Count == tarObject.Keys.Count)
                {
                    foreach (var key in srcObject.Keys)
                    {
                        if (!tarObject.ContainsKey(key))
                        {
                            syncRequired = true;
                        }
                        else
                        {
                            if (srcObject[key] is ExpandoObject && tarObject[key] is ExpandoObject)
                            {
                                syncRequired = isSyncRequired(srcObject[key] as IDictionary<string, object>, tarObject[key] as IDictionary<string, object>);
                            }
                            else if (srcObject[key] is System.Collections.Generic.IList<Object> && tarObject[key] is System.Collections.Generic.IList<Object>)
                            {
                                syncRequired = isSyncRequiredForRepeaterData(srcObject[key] as System.Collections.Generic.IList<Object>, tarObject[key] as System.Collections.Generic.IList<Object>);
                            }
                        }
                        if (syncRequired == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    syncRequired = true;
                }
            }
            else
            {
                syncRequired = true;
            }
            return syncRequired;
        }
    }
}