using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using fv = tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Data;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Enums;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using tmg.equinox.domain.entities;
using System.Data.OleDb;
using tmg.equinox.schema.Base;
using System.IO;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.queueprocess.PBPExport;
using tmg.equinox.backgroundjob;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.pbpexport;
using System.Transactions;
using System.Collections;
using System.Configuration;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using System.Threading;
using tmg.equinox.notification;
using tmg.equinox.applicationservices.viewmodels.PBPImport;

namespace tmg.equinox.applicationservices
{
    internal class VBIDExportService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IMasterListService _masterListService;
        private IUIElementService _uiElementService;
        private IExitValidateService _exitValidateService;
        private ExitValidateViewModel _evModel;
        private string UserName { get; set; }
        private static readonly ILog _logger = LogProvider.For<PBPImportService>();

        private string[] VBID19ATables = { "PBPB1_2_B19A_VBID", "PBPB1_B19A_VBID", "PBPB19_2_VBID", "PBPB19_3_VBID", "PBPB19_VBID", "PBPB2_B19A_VBID" };
        private string[] VBID19BTables = { "PBPB1_2_B19B_VBID", "PBPB1_B19B_VBID", "PBPB10_VBID", "PBPB13_2_VBID", "PBPB13_3_VBID", "PBPB13_VBID", "PBPB14_2_VBID",
            "PBPB14_3_VBID", "PBPB14_VBID", "PBPB16_VBID", "PBPB17_VBID", "PBPB18_VBID", "PBPB19_4_VBID", "PBPB2_B19B_VBID", "PBPB3_VBID", "PBPB4_VBID", "PBPB7_VBID",
            "PBPB9_VBID"};
        private string[] VBIDMRXTables = { "PBPMRX", "PBPMRX_P_VBID", "PBPMRX_T_VBID", "PBPMRX_VBID" };
        private string VBID19AJsonPath = "PackageInformation.IsReducedCostSharingApplicable";
        private string VBID19BJsonPath = "PackageInformation.IsAdditionalBenefitsApplicable";
        private string VBIDMRXJsonPath = "PackageInformation.IsRxApplicable";

        internal VBIDExportService(IUnitOfWorkAsync unitOfWork, IUIElementService uiElementService, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceService formInstanceService, IFormInstanceDataServices formInstanceDataService, IMasterListService masterListService, IExitValidateService exitValidateService,ExitValidateViewModel evModel)
        {
            this._unitOfWork = unitOfWork;
            _uiElementService = uiElementService;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceService = formInstanceService;
            _formInstanceDataService = formInstanceDataService;
            _masterListService = masterListService;
            _exitValidateService = exitValidateService;
            _evModel = evModel;
        }

        internal void ProcessVBIDExport(PBPExportQueueViewModel model, int anchorDocumentId, string QID, int year)
        {
            Export(model.FolderVersionID, anchorDocumentId, model.VBIDFilePath, QID, year);
        }

        internal void ProcessVBIDExportForExitValidate(ExitValidateViewModel model, string QID, int year)
        {
            Export(model.FolderVersionID, model.AnchorDocumentID.Value, model.VBIDFilePath, QID, year);
        }

        private void Export(int folderVersionId, int anchorID, string vbidFilePath, string QID, int year)
        {
            string tblName = string.Empty;
            string FieldName = string.Empty;
            OleDbHelper dbHelper = new OleDbHelper();
            try
            {
                string connectingString = dbHelper.GetOleDbConnectingString(vbidFilePath);
                List<fv.FormInstanceViewModel> instances = _folderVersionService.GetFormInstanceListForAnchor(1, folderVersionId, anchorID);
                var vbidInstances = from inst in instances where inst.FormDesignID == 2409 select inst;
                List<VBIDExportToMDBMappingViewModel> mapping = this.GetExportMappings(year).ToList();
                try
                {
                    int vbidInstanceCount = vbidInstances.Count();
                    if (vbidInstanceCount > 0)
                    {
                        List<int> instanceNums = GetOrderedVBIDInstanceNums(vbidInstances.ToList());
                        int vbidGrID = 1;
                        for (int loopIdx = 0; loopIdx < vbidInstanceCount; loopIdx++)
                        {
                            var fiNameIdx = instanceNums[loopIdx];
                            string fName = "VBID View " + fiNameIdx;
                            var vbids = from v in vbidInstances where v.FormInstanceName == fName select v;
                            var vbid = vbids.First();
                            if (vbid != null)
                            {
                                string json = this.GetJSONString(vbid.FormInstanceID);
                                string vbidGroupID = vbidGrID.ToString();
                                JObject source = JObject.Parse(json);

                                Dictionary<string, object> dict = JsonHelper.DeserializeAndFlatten(source.ToString());

                                var hiddensections = dict.Where(t => t.Key == "NineteenAReducedCostSharingforVBIDUFGroup1.VisibleSections").Select(t => t.Value).FirstOrDefault();
                                string sectionName = hiddensections == null ? string.Empty : hiddensections.ToString();
                                bool isvisible = false;

                                string VBID19A = dict.Where(t => t.Key == VBID19AJsonPath).Select(t => t.Value).FirstOrDefault().ToString();
                                if (string.IsNullOrEmpty(VBID19A)) { VBID19A = "NO"; }
                                string VBID19B = dict.Where(t => t.Key == VBID19BJsonPath).Select(t => t.Value).FirstOrDefault().ToString();
                                if (string.IsNullOrEmpty(VBID19B)) { VBID19B = "NO"; }
                                string VBIDMRX = dict.Where(t => t.Key == VBIDMRXJsonPath).Select(t => t.Value).FirstOrDefault().ToString();
                                if (string.IsNullOrEmpty(VBIDMRX)) { VBIDMRX = "NO"; }

                                List<string> MDBtables = mapping.Select(t => t.TableName).Distinct().ToList();
                                List<string> tables1 = MDBtables;
                                var filteredTables = from tab in tables1 select tab;
                                foreach (string tbl in filteredTables)
                                {
                                    isvisible = false;
                                    tblName = tbl;

                                    string cmdText = "INSERT INTO " + tblName + "(";
                                    string values = " VALUES (";

                                    Dictionary<string, string> commandParam = new Dictionary<string, string>();

                                    List<VBIDExportToMDBMappingViewModel> tableMapping = (from map in mapping
                                                                                          where map.TableName == tbl
                                                                                          && map.IsRepeater == false
                                                                                          orderby map.FieldName
                                                                                          select map).ToList();
                                    foreach (VBIDExportToMDBMappingViewModel model in tableMapping)
                                    {
                                        model.JsonPath = model.JsonPath.Trim();

                                        string sectionpath = string.Empty;
                                        if (!string.IsNullOrEmpty(model.JsonPath) && model.JsonPath.Contains("."))
                                            sectionpath = model.JsonPath.Substring(0, model.JsonPath.IndexOf("."));
                                        if (!string.IsNullOrEmpty(model.JsonPath) && !sectionName.Contains(";" + sectionpath + ";"))
                                            isvisible = true;

                                        string val = GetModelFieldValue(model, dict, QID, vbidGroupID);
                                        if (val != string.Empty && val != null)
                                        {
                                            if (!commandParam.ContainsKey(model.FieldName))
                                            {
                                                commandParam.Add(model.FieldName, val);
                                            }
                                        }
                                    }

                                    var cmdTextColumn = "";

                                    foreach (string key in commandParam.Keys)
                                    {
                                        cmdText = cmdText + key + ",";
                                        values = values + "?,";
                                    }

                                    if (commandParam.Count() > 0)
                                    {
                                        if ((VBID19A == "NO" && VBID19ATables.Contains(tblName)) || (VBID19B == "NO" && VBID19BTables.Contains(tblName)) || (VBIDMRX == "NO" && VBIDMRXTables.Contains(tblName)))
                                        {
                                        }
                                        else if (isvisible == false && (commandParam.Count() == 2 && commandParam.ContainsKey("QID") && commandParam.ContainsKey("PBP_VBID_GROUP_ID")))
                                        {
                                        }
                                        else
                                        {
                                            values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                            cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;

                                            using (OleDbConnection connection = new OleDbConnection(connectingString))
                                            {
                                                using (OleDbCommand cmdNew = new OleDbCommand())
                                                {
                                                    foreach (string key in commandParam.Keys)
                                                    {
                                                        cmdNew.Parameters.AddWithValue(key, commandParam[key]);
                                                    }
                                                    connection.Open();
                                                    cmdNew.CommandText = cmdText;
                                                    cmdNew.Connection = connection;
                                                    cmdNew.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<string> columns = (from map in mapping
                                                                where map.TableName == tbl
                                                                orderby map.FieldName
                                                                select map.FieldName).ToList();
                                        foreach (string column in columns)
                                        {
                                            if (!cmdText.Contains(column + ","))
                                            {
                                                cmdText = cmdText + column + ",";
                                                values = values + "?,";
                                            }
                                        }
                                        values = values.Remove(values.LastIndexOf(','), 1) + ")";
                                        cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;
                                        cmdTextColumn = cmdText;
                                    }
                                }
                                var filteredRepeaterTables = from tab in tables1 where tab.Contains("PBPMRX_T_VBID") || tab.Contains("PBPMRX_P_VBID") select tab;
                                foreach (string tbl in filteredRepeaterTables)
                                {
                                    //get repeaters
                                    //for each tier, get fields from repeaters, generate sql query and execute
                                    if (VBIDMRX == "NO" && VBIDMRXTables.Contains(tblName))
                                    {
                                    }
                                    else
                                    {
                                        if (tbl == "PBPMRX_T_VBID")
                                        {
                                            //Pre-ICL
                                            //get fields
                                            var preICLMappings = (from map in mapping
                                                                  where map.TableName == tbl
                                                                  && map.IsRepeater == true && map.JsonPath.StartsWith("VBIDPreICL") && map.JsonPath.Contains("[0]")
                                                                  orderby map.FieldName
                                                                  select map).GroupBy(a => a.FieldName);

                                            var preICLRepeaterList = (from map in mapping
                                                                      where map.TableName == tbl
                                                                      && map.IsRepeater == true && map.JsonPath.Contains("[0]") && map.JsonPath.StartsWith("VBIDPreICL")
                                                                      select map.JsonPath.Split('[')[0]).Distinct().Where(a => !String.IsNullOrEmpty(a));
                                            var nrl = from map in mapping
                                                      where map.TableName == tbl
                                                      && map.IsRepeater == true && !map.JsonPath.Contains("[0]") && map.IsActive == true
                                                      select map;
                                            List<VBIDExportToMDBMappingViewModel> nonRepeaterList = new List<VBIDExportToMDBMappingViewModel>();
                                            if (nrl != null && nrl.Count() > 0)
                                            {
                                                nonRepeaterList = nrl.ToList();
                                            }
                                            if (preICLRepeaterList != null && preICLRepeaterList.Count() > 0 && preICLMappings != null && preICLMappings.Count() > 0)
                                            {
                                                InsertVBIDTierRows(preICLMappings, preICLRepeaterList.ToList(), nonRepeaterList, source, dict, tbl, QID, vbidGroupID, connectingString, "MRX_GROUP_TIERS_ICL");
                                            }

                                            //Gap
                                            var gapMappings = (from map in mapping
                                                               where map.TableName == tbl
                                                               && map.IsRepeater == true && map.JsonPath.StartsWith("VBIDGap")
                                                               orderby map.FieldName
                                                               select map).GroupBy(a => a.FieldName).ToList();
                                            var gapRepeaterList = (from map in mapping
                                                                   where map.TableName == tbl
                                                                   && map.IsRepeater == true && map.JsonPath.Contains("[0]") && map.JsonPath.StartsWith("VBIDGap")
                                                                   select map.JsonPath.Split('[')[0]).Distinct().Where(a => !String.IsNullOrEmpty(a));
                                            if (gapRepeaterList != null && gapRepeaterList.Count() > 0 && gapMappings != null && gapMappings.Count() > 0)
                                            {
                                                InsertVBIDTierRows(gapMappings, gapRepeaterList.ToList(), nonRepeaterList, source, dict, tbl, QID, vbidGroupID, connectingString, "MRX_GROUP_TIERS_GAP");
                                            }
                                        }
                                        if (tbl == "PBPMRX_P_VBID")
                                        {
                                            //Gap
                                            var nrl = from map in mapping
                                                      where map.TableName == tbl
                                                      && map.IsRepeater == true && !map.JsonPath.Contains("[0]") && map.IsActive == true
                                                      select map;
                                            List<VBIDExportToMDBMappingViewModel> nonRepeaterList = new List<VBIDExportToMDBMappingViewModel>();
                                            if (nrl != null && nrl.Count() > 0)
                                            {
                                                nonRepeaterList = nrl.ToList();
                                            }
                                            var oopMappings = (from map in mapping
                                                               where map.TableName == tbl
                                                               && map.IsRepeater == true && map.JsonPath.StartsWith("VBIDOOP") && map.JsonPath.Contains("[0]")
                                                               orderby map.FieldName
                                                               select map).GroupBy(a => a.FieldName);

                                            var oopRepeaterList = (from map in mapping
                                                                   where map.TableName == tbl
                                                                   && map.IsRepeater == true && map.JsonPath.Contains("[0]") && map.JsonPath.StartsWith("VBIDOOP")
                                                                   select map.JsonPath.Split('[')[0]).Distinct().Where(a => !String.IsNullOrEmpty(a));
                                            if (oopRepeaterList != null && oopRepeaterList.Count() > 0 && oopMappings != null && oopMappings.Count() > 0)
                                            {
                                                InsertVBIDTierRows(oopMappings, oopRepeaterList.ToList(), nonRepeaterList, source, dict, tbl, QID, vbidGroupID, connectingString, "MRX_GROUP_TIERS_OOP");
                                            }
                                        }
                                    }
                                }
                                vbidGrID++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
        }

        public IEnumerable<VBIDExportToMDBMappingViewModel> GetExportMappings(int year)
        {
            IEnumerable<VBIDExportToMDBMappingViewModel> models = from m in this._unitOfWork.RepositoryAsync<VBIDExportToMDBMapping>().Get()
                                                                  where m.IsActive == true
                                                                  && m.Year == year
                                                                  select (new VBIDExportToMDBMappingViewModel
                                                                  {
                                                                      TableName = m.TableName,
                                                                      FieldName = m.FieldName,
                                                                      JsonPath = m.JsonPath,
                                                                      IsRepeater = m.IsRepeater,
                                                                      Length = m.Length,
                                                                      MappingType = m.MappingType,
                                                                      IsActive = m.IsActive,
                                                                      IsBlankAllow = m.IsBlankAllow,
                                                                      DefaultValue = m.DefaultValue,
                                                                      IsCustomRule = m.IsCustomRule,
                                                                      DataType = m.DataType,
                                                                  });

            return models;
        }
        public string GetJSONString(int formInstanceID)
        {
            FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                                      .Where(s => s.FormInstanceID == formInstanceID)
                                                      .Select(s => s).ToList().FirstOrDefault();
            if (formInstanceDataMap == null)
                return string.Empty;
            return formInstanceDataMap.FormData;
        }

        public void InsertVBIDTierRows(IEnumerable<IGrouping<string, VBIDExportToMDBMappingViewModel>> fields, List<string> repeaters, List<VBIDExportToMDBMappingViewModel> nonRepeaterFields, JObject jsonObject, Dictionary<string, object> dict, string tableName, string QID, string vbidGroupID, string connectionString, string tiersSelectionFieldName)
        {
            //get number of Tiers
            var tiersList = nonRepeaterFields.Where(a => a.FieldName == tiersSelectionFieldName);
            if (tiersList != null && tiersList.Count() > 0)
            {
                var val = GetModelFieldValue(tiersList.First(), dict, QID, vbidGroupID);
                if (!String.IsNullOrEmpty(val))
                {
                    List<string> tiers = GetTiers(val);
                    int tierIdx = 0;
                    foreach (string tier in tiers)
                    {
                        string cmdText = "INSERT INTO " + tableName + "(";
                        string values = " VALUES (";
                        Dictionary<string, string> commandParam = new Dictionary<string, string>();
                        foreach (var grp in fields)
                        {
                            var model = grp.First();
                            string jsonPath = String.Copy(model.JsonPath.Trim());
                            model.JsonPath = model.JsonPath.Replace("[0]", "." + (tierIdx).ToString());
                            string fieldValue = GetModelFieldValue(model, dict, QID, vbidGroupID);
                            model.JsonPath = jsonPath;
                            if (!String.IsNullOrEmpty(fieldValue))
                            {
                                if (!commandParam.ContainsKey(model.FieldName))
                                {
                                    commandParam.Add(model.FieldName, fieldValue);
                                }
                            }
                        }
                        foreach (var fld in nonRepeaterFields)
                        {
                            string jsonPath = String.Copy(fld.JsonPath.Trim());
                            string fieldValue = GetModelFieldValue(fld, dict, QID, vbidGroupID);
                            if (!String.IsNullOrEmpty(fieldValue))
                            {
                                if (!commandParam.ContainsKey(fld.FieldName))
                                {
                                    commandParam.Add(fld.FieldName, fieldValue);
                                }
                            }
                        }
                        foreach (string key in commandParam.Keys)
                        {
                            cmdText = cmdText + key + ",";
                            values = values + "?,";
                        }

                        if (commandParam.Count() > 0)
                        {
                            values = values.Remove(values.LastIndexOf(','), 1) + ")";
                            cmdText = cmdText.Remove(cmdText.LastIndexOf(','), 1) + ")" + values;

                            using (OleDbConnection connection = new OleDbConnection(connectionString))
                            {
                                using (OleDbCommand cmdNew = new OleDbCommand())
                                {
                                    foreach (string key in commandParam.Keys)
                                    {
                                        cmdNew.Parameters.AddWithValue(key, commandParam[key]);
                                    }
                                    connection.Open();
                                    cmdNew.CommandText = cmdText;
                                    cmdNew.Connection = connection;
                                    cmdNew.ExecuteNonQuery();
                                }
                            }
                        }

                        tierIdx++;
                    }
                }
            }
        }

        private string GetModelFieldValue(VBIDExportToMDBMappingViewModel model, Dictionary<string, object> dict, string QID, string vbidGroupID)
        {
            string val = "";
            string fieldName = model.FieldName;
            if (model.FieldName == "QID")
                val = QID;
            else if ((model.FieldName == "PBP_VBID_GROUP_ID") || (model.TableName == "PBPMRX_VBID" && model.FieldName == "MRX_TIER_GROUP_ID"))
                val = vbidGroupID;
            else
            {
                var Value = dict.Where(t => t.Key == model.JsonPath).Select(t => t.Value).FirstOrDefault();
                if (Value != null)
                {
                    val = Value.ToString();
                }
            }
            if (model.MappingType == "MULTISELECT")
                val = ConvertMultiSelectValue(dict, model, false, null);
            if (model.MappingType == "MULTIVALUECSV" || model.MappingType == "MULTISELECTCSV")
                val = ConvertMultiSelectCSVValue(dict, model, false, string.Empty);

            if (val != null)
            {
                if (val.Equals("true") || val.Equals("True"))
                {
                    val = "1";
                }
                else if (val.Equals("false") || val.Equals("False"))
                {
                    val = "0";
                }
            }
            if (model.IsCustomRule)
            {
                val = ApplySort(val);
            }

            if (val.ToString().Length > model.Length)
            {
                string data = val.ToString();
                val = data.Substring(0, model.Length);
                if (_evModel != null)
                {
                    //_exitValidateService.AddExitValidateVBIDExportError(_evModel, model);
                }
                if (!string.IsNullOrEmpty(val) && model.DataType.Equals("int"))
                {
                    int intVal = 0;
                    bool isInt = Int32.TryParse(val.Split('.').FirstOrDefault(), out intVal);
                    val = isInt ? intVal.ToString() : val;
                }
            }
            return val;
        }

        private string ApplySort(string CSV)
        {
            if (!String.IsNullOrEmpty(CSV))
            {
                string[] CSVArr = CSV.Split(';').Where(v => !String.IsNullOrEmpty(v)).ToArray();
                if (CSVArr.Length > 1)
                {
                    Array.Sort(CSVArr, new AlphanumComparatorFast());
                    CSV = String.Join(";", CSVArr);
                    CSV += string.Join(CSV, ";");
                }
            }
            return CSV;
        }

        private string ConvertMultiSelectCSVValue(Dictionary<string, object> dict, VBIDExportToMDBMappingViewModel model, bool isRepeater, string repetareJsonPath)
        {
            string CSV = string.Empty;
            string jsonPath = string.Empty;
            if (isRepeater)
                jsonPath = repetareJsonPath;
            else
                jsonPath = model.JsonPath;

            var selectedOptions = dict.Where(t => t.Key.Contains(jsonPath))
                                  .OrderBy(s => s.Value);

            if (jsonPath != "" && selectedOptions != null && selectedOptions.Count() > 0)
            {
                foreach (var option in selectedOptions)
                {
                    if (option.Value != null)
                    {
                        if (!String.IsNullOrWhiteSpace(option.Value.ToString()))
                        {
                            CSV = CSV + option.Value + ";";
                        }
                    }
                }
            }
            CSV = CSV.Replace(";;", ";");
            //CSV = CSV.TrimEnd(';');
            CSV = CSV.Replace("[", string.Empty);
            try
            {
                string alphaNumericSort = ConfigurationManager.AppSettings["ExportAlphaNumericSort"] ?? string.Empty;
                if (alphaNumericSort == "Yes")
                {
                    if (!String.IsNullOrEmpty(CSV))
                    {
                        string[] CSVArr = CSV.Split(';').Where(v => !String.IsNullOrEmpty(v)).ToArray();
                        if (CSVArr.Length > 1)
                        {
                            Array.Sort(CSVArr, new AlphanumComparatorFast());
                            CSV = String.Join(";", CSVArr);
                            CSV += string.Join(CSV, ";");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return CSV;
        }

        public dynamic ConvertMultiSelectValue(Dictionary<string, object> dict, VBIDExportToMDBMappingViewModel model, bool isRepeater, dynamic repeaterData)
        {
            dynamic result = "";
            string CSV = "";
            if (!isRepeater && model.JsonPath != "")
            {
                var selectedOptions = dict.Where(t => t.Key.Contains(model.JsonPath))
                                      .OrderBy(s => s.Value);
                foreach (var option in selectedOptions)
                    if (option.Value != null)
                    {
                        if (!String.IsNullOrWhiteSpace(option.Value.ToString()))
                        {
                            if (!CSV.Contains(option.Value.ToString()))
                                CSV = CSV + option.Value + ",";
                        }
                    }
            }
            if (isRepeater)
                CSV = repeaterData.ToString();
            //CSV = CSV.TrimEnd(';');
            if (CSV != null && CSV.Length > 0)
            {
                string targetVal = "0";
                string[] strArr = CSV.Split(',');
                if (strArr != null && strArr.Length > 0)
                {
                    foreach (string str in strArr)
                    {
                        Int64 intVal = 0;
                        bool isInt = Int64.TryParse(str, out intVal);
                        if (isInt)
                        {
                            int num = Convert.ToInt32(str, 2);
                            int res = Convert.ToInt32(targetVal, 2);
                            targetVal = Convert.ToString(res + num, 2);
                        }
                    }
                    result = targetVal;
                }
            }
            if (CSV.Length > 0 && result.ToString().Length < model.Length)
            {
                Int64 intVal = 0;
                bool isInt = Int64.TryParse(result, out intVal);
                if (isInt)
                    result = string.Format("{0}", intVal.ToString("D" + model.Length.ToString()));// Pad a Number with Leading Zeros
            }
            return result;
        }
        private List<String> GetTiers(string tiers)
        {
            List<String> tiersList = new List<String>();
            int idx = 1;
            foreach (char c in tiers)
            {
                if (c == '1')
                {
                    tiersList.Add(idx.ToString());
                }
                idx++;
            }
            return tiersList;
        }

        private List<int> GetOrderedVBIDInstanceNums(List<fv.FormInstanceViewModel> vbidInstances)
        {
            var nums = from vbid in vbidInstances select vbid.FormInstanceName.Replace("VBID View ", "");
            List<int> instanceNums = new List<int>();
            int instanceVal;
            foreach (var num in nums)
            {
                int.TryParse(num, out instanceVal);
                if (instanceVal > 0)
                {
                    int res = instanceVal;
                    instanceNums.Add(res);
                }
            }
            instanceNums.Sort();
            return instanceNums;
        }
    }
}
