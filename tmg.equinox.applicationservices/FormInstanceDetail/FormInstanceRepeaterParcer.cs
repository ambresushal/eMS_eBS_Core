using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.applicationservices.FormInstanceDetail
{
    public class FormInstanceRepeaterParcer
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork;
        #endregion

        #region Constructor
        public FormInstanceRepeaterParcer(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method is used to join Master List form instance data with repeater data from FormInstanceRepeaterDataMap table.
        /// </summary>
        /// <param name="retMasterjsondata"></param>
        /// <param name="formInstanceRepeaterData"></param>
        /// <returns></returns>
        public string GetMasterListFormInstanceRepeaterData(IDictionary<string, string> retMasterjsondata, IList<FormInstanceRepeaterDataMap> formInstanceRepeaterData)
        {
            string masterListJsonString = string.Empty;
            string jsonString = string.Empty;
            try
            {
                if (retMasterjsondata.ContainsKey("Master List"))
                {
                    jsonString = retMasterjsondata["Master List"].ToString();
                }

                var converter = new ExpandoObjectConverter();
                dynamic masterListRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, converter);

                if (formInstanceRepeaterData.Count > 0)
                {
                    foreach (var reapeaterData in formInstanceRepeaterData)
                    {
                        dynamic formInstanceRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(reapeaterData.RepeaterData, converter);

                        string[] elements = reapeaterData.FullName.Split('.');
                        string keyname = elements.Last();
                        int count = elements.Length - 1;
                        int i = -1;

                        IDictionary<string, object> values = masterListRepeaterObject as IDictionary<string, object>;

                        IDictionary<string, object> repeaterValues = formInstanceRepeaterObject as IDictionary<string, object>;

                        foreach (var element in elements)
                        {
                            i++;
                            if (values is ExpandoObject)
                            {
                                if (i == count)
                                {
                                    if (values.ContainsKey(keyname))
                                    {
                                        values[keyname] = repeaterValues[keyname];
                                    }
                                }
                                else
                                {
                                    if (values.ContainsKey(element))
                                    {
                                        values = values[element] as IDictionary<string, object>;
                                    }

                                }
                            }
                        }
                    }
                    masterListJsonString = JsonConvert.SerializeObject(masterListRepeaterObject);

                }


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return masterListJsonString;
        }

        public string GetFormInstanceRepeaterJsonString(string fullPath, string repeaterData)
        {
            var converter = new ExpandoObjectConverter();
            dynamic repeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(repeaterData, converter);
            var repeaterInstanceData = repeaterObject as IDictionary<string, object>;

            List<object> newList = new List<object>();
            string repeaterName = fullPath.Split('.').Last();

            IList<object> repeaterDeltaObjectList = repeaterInstanceData[repeaterName] as IList<object>;
            for (int i = 0; i < repeaterDeltaObjectList.Count(); i++)
            {
                IDictionary<string, object> modifyRowData = repeaterDeltaObjectList[i] as IDictionary<string, object>;
                var rowObject = modifyRowData["rowObject"] as IDictionary<string, object>;
                newList.Add(rowObject);
            }
            repeaterInstanceData[repeaterName] = newList;
            IDictionary<string, object> newFormInstanceRepeaterData = new Dictionary<string, object>();
            newFormInstanceRepeaterData.Add(repeaterName, repeaterInstanceData[repeaterName]);
            string newFormInstanceRepeaterJsonString = JsonConvert.SerializeObject(newFormInstanceRepeaterData);

            return newFormInstanceRepeaterJsonString;
        }

        public string MergeFormInstanceRepeaterData(domain.entities.Models.FormInstanceRepeaterDataMap itemToUpdate, string fullPath, string updatedFormInstanceRepeaterData, List<string> repeaterDuplicationElement)
        {
            var converter = new ExpandoObjectConverter();
            dynamic forminstanceRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(itemToUpdate.RepeaterData, converter);
            var formInstanceRepeaterData = forminstanceRepeaterObject as IDictionary<string, object>;
            

            //modify repater Data
            dynamic updatedFormInstanceRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(updatedFormInstanceRepeaterData, converter);
            var updatedRepeaterData = updatedFormInstanceRepeaterObject as IDictionary<string, object>;

            string repeaterName = fullPath.Split('.').Last();

            
            string jsonString = "";
            if (formInstanceRepeaterData.ContainsKey(repeaterName))
            {
                var repeaterInstanceData = formInstanceRepeaterData[repeaterName] as System.Collections.Generic.IList<Object>;
                IList<int> indexesToFilter = new List<int>();
                if (updatedRepeaterData.ContainsKey(repeaterName))
                {
                    IList<object> repeaterDeltaObjectList = updatedRepeaterData[repeaterName] as IList<object>;
                    for (int i = 0; i < repeaterDeltaObjectList.Count(); i++)
                    {
                        IDictionary<string, object> repeaterRowData = repeaterDeltaObjectList[i] as IDictionary<string, object>;
                        IDictionary<string, object> rowdata = repeaterRowData["rowObject"] as IDictionary<string, object>;
                        string rowAction = repeaterRowData["rowAction"].ToString();
                        int matchCount = 0;
                        if ((rowAction == "Update" || rowAction == "Delete") && rowdata.Count() > 0)
                        {
                            for (int idx = 0; idx < repeaterInstanceData.Count(); idx++)
                            {
                                IDictionary<string, object> singleRowData = repeaterInstanceData[idx] as IDictionary<string, object>;
                                bool isRowExits = false;

                                matchCount = 0;
                                if (singleRowData["RowIDProperty"].ToString() == rowdata["RowIDProperty"].ToString())
                                {
                                    isRowExits = true;
                                }

                                foreach (string element in repeaterDuplicationElement)
                                {
                                    if (singleRowData.ContainsKey(element) && rowdata.ContainsKey(element))
                                    {
                                        if (singleRowData[element].ToString() == rowdata[element].ToString())
                                            matchCount++;
                                    }
                                }
                                if (matchCount == repeaterDuplicationElement.Count() && isRowExits)
                                {
                                    if (rowAction == "Update")
                                    {
                                        var repeaterElementlist = rowdata.Keys;
                                        foreach (string mapping in repeaterElementlist)
                                        {
                                            if (mapping != "RowIDProperty")
                                                singleRowData[mapping] = rowdata[mapping];
                                        }
                                    }
                                    else
                                        indexesToFilter.Add(idx);
                                    break;
                                }
                                if (matchCount < repeaterDuplicationElement.Count() && (rowAction == "Update" || rowAction == "Delete"))
                                {
                                    if (isRowExits)
                                    {
                                        if (rowAction == "Update")
                                        {
                                            var repeaterElementlist = rowdata.Keys;
                                            foreach (string mapping in repeaterElementlist)
                                            {
                                                if (mapping != "RowIDProperty")
                                                    singleRowData[mapping] = rowdata[mapping];
                                            }
                                        }
                                        else
                                            indexesToFilter.Add(idx);
                                        break;
                                    }
                                }
                            }
                        }
                        if (matchCount == 0 && rowAction == "Add")
                        {
                            Dictionary<string, object> addNewList = new Dictionary<string, object>();
                            foreach (string mapping in rowdata.Keys)
                            {
                                if (mapping != "RowIDProperty")
                                    addNewList.Add(mapping, rowdata[mapping]);
                                else
                                {
                                    if (repeaterInstanceData.Count() > 0)
                                    {
                                        IDictionary<string, object> updatedValues = new Dictionary<string, object>();
                                        updatedValues.Add("" + repeaterName + "", repeaterInstanceData);
                                        string data = JsonConvert.SerializeObject(updatedValues);
                                        JObject repeaterInstanceObject = JObject.Parse(data);
                                        JArray formInstanceRepeaterDataInJArray = (JArray)repeaterInstanceObject[repeaterName];
                                        addNewList.Add(mapping, (GetLatestRowId(formInstanceRepeaterDataInJArray) + 1).ToString());
                                    }
                                    else
                                    {
                                        addNewList.Add(mapping, (repeaterInstanceData.Count()).ToString());
                                    }
                                }
                                    
                            }
                            repeaterInstanceData.Add(addNewList);
                        }
                    }
                    if (indexesToFilter.Count() > 0)
                    {
                        IOrderedEnumerable<int> deleteOrder = indexesToFilter.OrderByDescending(d => d);
                        foreach (int delIndex in deleteOrder)
                        {
                            repeaterInstanceData.RemoveAt(delIndex);
                        }
                    }
                    jsonString = JsonConvert.SerializeObject(formInstanceRepeaterData);
                }
            }

            return jsonString;
        }



        #endregion Public Methods

        #region Private Methods

        private int GetLatestRowId(JArray repeaterInstanceData)
        {
            List<JToken> formInstanceRepeaterDataList = repeaterInstanceData.ToList();
            var row = formInstanceRepeaterDataList.OrderByDescending(p => (int)p["RowIDProperty"]).FirstOrDefault();
            int rowNo = Convert.ToInt32(row["RowIDProperty"].ToString());
            return rowNo;        
        }
        #endregion Private Methods

    }
}
