using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.api.Framework;

namespace tmg.equinox.services.api.Controllers
{
    public class ProductController : BaseApiController
    {
        private IFormInstanceService _formInstanceService;
        private IFolderVersionServices _folderVersionServices;

        public ProductController(IFormInstanceService formInstanceService,IFolderVersionServices folderVersionServices)
        {
            this._formInstanceService = formInstanceService;
            this._folderVersionServices = folderVersionServices;
        }


        [HttpGet]
        [Route("~/api/v1/Product")]
        [ResponseType(typeof(JsonFieldMappingViewModelExtended))]
        public HttpResponseMessage GetJsonMappingFields()
        {
            List<JsonFieldMappingViewModelExtended> jsonMappingList = new List<JsonFieldMappingViewModelExtended>();
            jsonMappingList = _formInstanceService.GetJsonFieldsData();

            return CreateResponse(new { Status = "Success", Result = jsonMappingList });
        }


        /// <param name="data"></param>
        /// <returns>Returns products json.</returns>
        [HttpPost]
        [Route("~/api/v1/Product")]
        [ResponseType(typeof(ResultJsonViewModel))]
        //[EnableCors("*","*","*")]
        public HttpResponseMessage GetProductJson([FromBody] ProductData data)
        {
            List<JsonFieldMappingViewModelExtended> jsonMappingList = null;

            ////List<JsonFieldMappingViewModel> lstTmpJSON = null;
            ////JsonFieldMappingViewModel tmpJSON = null;
            List<ResultJsonViewModel> resultData = new List<ResultJsonViewModel>();
            ResultJsonViewModel resultJSON = null;
            Dictionary<string, string> tmpDict = null;
            
            try
            {
                string resultMessage = string.Empty;
                string ContractNumber = data.ContractNumber;
                string effYear = data.EffectiveYear;
                string jsonDisplayField = ConfigurationManager.AppSettings["JsonDisplayField"];

                if (data == null)
                    resultMessage = "ContractNumber and EffectiveYear are missing.";
                else if (ContractNumber == string.Empty && effYear == string.Empty)
                    resultMessage = "ContractNumber and EffectiveYear are missing.";
                else if (ContractNumber == string.Empty)
                    resultMessage = "ContractNumber is missing.";
                else if (effYear == string.Empty)
                    resultMessage = "EffectiveYear is missing.";

                if (resultMessage != string.Empty)
                    return CreateResponse(new { Status = "Failure", Message = resultMessage });

                jsonMappingList = new List<JsonFieldMappingViewModelExtended>();
                jsonMappingList = _formInstanceService.GetJsonFieldsData();         //getting sample JSON

                List<string> lstMasterListType = new List<string>();

                if (jsonMappingList.Count() > 0)
                {
                    string[] contractNos = ContractNumber.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    lstMasterListType = jsonMappingList.Select(w => string.IsNullOrEmpty(w.DesignType) == false ? w.DesignType : "").Distinct().ToList();

                    JsonFieldMappingViewModel tmpMLJson = new JsonFieldMappingViewModel();

                    foreach (string docName in contractNos)
                    {
                        List<FormInstanceViewModel> documentData = new List<FormInstanceViewModel>();

                        if (lstMasterListType!=null && lstMasterListType.Count() > 0)
                        {
                            FormInstanceViewModel tmpDocumentData = null;
                            foreach(string strDesignType in lstMasterListType)
                            {
                                if(strDesignType != "")
                                {
                                    string effectiveYear = effYear;
                                    string documentName = docName;
                                    string designType = strDesignType.Replace("MasterList.", "");
                                   
                                    if (strDesignType.StartsWith("MasterList.") == true)
                                    {
                                        effectiveYear = "";
                                        documentName = designType;
                                    }

                                    tmpDocumentData = new FormInstanceViewModel();
                                    int formDesignID = this._folderVersionServices.GetFormDesignIDByFormName(designType);
                                    tmpDocumentData = _formInstanceService.GetFormInstancesByName(documentName, effectiveYear, formDesignID);
                                    if (tmpDocumentData != null)
                                        documentData.Add(tmpDocumentData);
                                }
                            }
                        }

                        if (documentData != null && documentData.Count() > 0)
                        {
                            ////lstTmpJSON = new List<JsonFieldMappingViewModel>();
                            resultJSON = new ResultJsonViewModel();
                            tmpDict = new Dictionary<string, string>();

                            resultJSON.ContractNumber = docName;
                           
                            foreach (var records in jsonMappingList)
                            {
                                string recDesignType = string.IsNullOrEmpty(records.DesignType) == false ? records.DesignType.Trim() : string.Empty;
                                FormInstanceViewModel tmpDocumentData = null;
                                                                
                                if (recDesignType.StartsWith("MasterList") == false)
                                {
                                    tmpDocumentData = documentData.Where(x => x.FormDesignName == recDesignType).FirstOrDefault();
                                    if (tmpDocumentData != null)
                                    {
                                        ////tmpJSON = new JsonFieldMappingViewModel();
                                        ////tmpJSON.JSONPath = records.JSONPath;
                                        ////tmpJSON.Label = records.Label;
                                        ////tmpJSON.Value = Convert.ToString(JToken.Parse(tmpDocumentData.FormData).SelectToken(records.JSONPath));

                                        string displayText = String.IsNullOrEmpty(jsonDisplayField) == true ? records.JSONPath : String.IsNullOrEmpty(records.Label) ? records.JSONPath : records.Label;
                                        
                                        if (tmpDict.Keys.Contains(displayText) == false)
                                            tmpDict.Add(displayText, Convert.ToString(JToken.Parse(tmpDocumentData.FormData).SelectToken(records.JSONPath)));

                                        if (tmpDict != null)
                                            resultJSON.Data = JObject.Parse(JsonConvert.SerializeObject(tmpDict, Formatting.None));
                                        else
                                            resultJSON.Data = JObject.Parse(string.Empty);

                                        ////lstTmpJSON.Add(tmpJSON);
                                    }
                                    else
                                    {
                                        resultJSON.Data = new JObject(); 
                                    }
                                }
                                else
                                {
                                    if (recDesignType.StartsWith("MasterList") == true) 
                                    {
                                        tmpDocumentData = documentData.Where(x => x.FormDesignName == recDesignType.Replace("MasterList.", "")).FirstOrDefault();
                                        var mlFrmData = JToken.Parse(tmpDocumentData.FormData).SelectToken(records.JSONPath);
                                        
                                        if (mlFrmData != null && mlFrmData.Any())
                                        {
                                            var planData = mlFrmData.ToList().Where(row => Convert.ToString(row["Plan"]) == docName);

                                            foreach (var mldata in planData)
                                            {
                                                ////tmpJSON = new JsonFieldMappingViewModel();
                                                ////tmpJSON.JSONPath = records.FieldName;
                                                ////tmpJSON.Label = records.Label + " - " + Convert.ToString(mldata["PrescriptionTier"]);
                                                ////tmpJSON.Value = Convert.ToString(mldata[records.FieldName]);

                                                string displayText = Convert.ToString(records.Label);
                                                if (displayText.Trim() == "")
                                                    displayText = Convert.ToString(mldata["PrescriptionTier"]);
                                                else
                                                    displayText += " - " + Convert.ToString(mldata["PrescriptionTier"]);

                                                if(tmpDict.Keys.Contains(displayText) == false)
                                                    tmpDict.Add(displayText, Convert.ToString(mldata[records.FieldName]));

                                                if (tmpDict != null)
                                                    resultJSON.Data = JObject.Parse(JsonConvert.SerializeObject(tmpDict, Formatting.None));
                                                else
                                                    resultJSON.Data = new JObject();

                                                ////lstTmpJSON.Add(tmpJSON);
                                                
                                            }
                                        }
                                    }
                                }
                            }
                             
                            ////resultJSON.Data = lstTmpJSON;
                           
                           resultData.Add(resultJSON);
                        }
                        else
                        {
                            resultMessage = "Invalid ContractNumber: " + docName;
                            return CreateResponse(new { Status = "Failure", Message = resultMessage });
                        }
                    }
                }
                else
                {
                    resultMessage = "Json Mapping Fields are not defined in database table.";
                    return CreateResponse(new { Status = "Failure", Message = resultMessage });
                }
            
                return CreateResponse(new { Status = "Success", Result = resultData });

            }
            catch (Exception e)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(e, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Data!");
            }
            return CreateResponse(new { Status = "Success", Result = resultData });
        }
    }

}
