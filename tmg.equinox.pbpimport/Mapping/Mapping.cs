using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json.Linq;
//using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices;
using System.Configuration;
//using tmg.equinox.ruleprocessor.formdesignmanager;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.ruleprocessor.formdesignmanager;
//using tmg.equinox.infrastructure.exceptionhandling;
//using tmg.equinox.applicationservices.PBPImport;



namespace tmg.equinox.pbpimport
{
    public class Mapping
    {
        private IFolderVersionServices _folderVersionService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPImportActivityLogServices _PBPImportActivityLogServices { get; set; }
        private IPBPImportHelperServices _PBPImportHelperServices { get; set; }
        private IPBPMappingServices _PBPMappingServices { get; set; }
        private IPBPImportService PBPImportService { get; set; }
        public Mapping(IFolderVersionServices folderVersionService, IFormDesignService formDesignService, IUnitOfWorkAsync unitOfWorkAsync, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPImportHelperServices PBPImportHelperServices, IPBPMappingServices PBPMappingServices, IPBPImportService _PBPImportService)
        {
            this._folderVersionService = folderVersionService;
            this._formDesignService = formDesignService;
            this._unitOfWorkAsync = unitOfWorkAsync;
            _PBPImportActivityLogServices = PBPImportActivityLogServices;
            _PBPImportHelperServices = PBPImportHelperServices;
            _PBPMappingServices = PBPMappingServices;
            this.PBPImportService = _PBPImportService;
        }

        public ServiceResult ProcessMedicareMapping(PBPPlanConfigViewModel ViewModel, int medicareFormDesignVersionID, bool IsFullMigration, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList,int planYear)
        {
            ServiceResult Result = new ServiceResult();
            bool ResultStatus = false;
            string DocumentJson = string.Empty;
            try
            {
                DocumentJson = GetFormInstanceJson((int)ViewModel.UserAction, ViewModel.FormInstanceId, medicareFormDesignVersionID);
                string defaultJSONData = ProcessMapping(DocumentJson, ViewModel, PBPImportTablesList, IsFullMigration, DocumentName.MEDICARE, planYear);
                defaultJSONData = UpdateIsPBPImportFlag(defaultJSONData);
                try
                {
                    ResultStatus = _folderVersionService.SaveFormInstanceDataCompressed(ViewModel.FormInstanceId, defaultJSONData);
                }
                catch (Exception ex)
                {
                    ResultStatus = false;
                    _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                    //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                    _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "Error for save Medicare forminstance()", " for QID", null, ViewModel.QID, ex);
                }
                if (ResultStatus)
                {
                    Result.Result = ServiceResultStatus.Success;

                    //if medcare success then pbp view mapping will process
                    Result = ProcessPBPViewMapping(ViewModel, IsFullMigration, PBPImportTablesList, ViewModel.Year);

                    ViewModel.PlanName = GetPlanName(defaultJSONData);
                    ViewModel.PlanNumber = GetPlanNumber(defaultJSONData);
                    ViewModel.ebsPlanName = ViewModel.PlanName;
                    ViewModel.eBsPlanNumber = ViewModel.PlanNumber;
                    //if (ViewModel.UserAction.Equals((int)PBPUserActionList.AddPlanIneBS))
                    //{
                    ViewModel.DocumentId = _PBPImportHelperServices.GetMedicareDocumentID(ViewModel.FolderVersionId, ViewModel.FormInstanceId);
                    //}
                    //this.PBPImportService.UpdatePBPPlanDetails(ViewModel, ViewModel.UserAction);
                }
                else
                {
                    Result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessMedicareMapping()", " Error for QID", null, ViewModel.QID, ex);
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        public ServiceResult ProcessPBPViewMapping(PBPPlanConfigViewModel ViewModel, bool IsFullMigration, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList,int planYear)
        {
            ServiceResult Result = new ServiceResult();
            bool ResultStatus = false;
            string DocumentJson = string.Empty;
            try
            {
                _PBPImportHelperServices.InitializeVariables(this._unitOfWorkAsync);
                int PBPViewFormDesignVersionID = _PBPImportHelperServices.GetEffectiveFormDesignVersionID(DocumentName.PBPVIEW, ViewModel.Year);
                int PbpViewFormInstaneId = _PBPImportHelperServices.GetPBPViewFormInstanceID(ViewModel.FolderVersionId, PBPViewFormDesignVersionID, ViewModel.FormInstanceId);
                DocumentJson = GetFormInstanceJson(ViewModel.UserAction, PbpViewFormInstaneId, PBPViewFormDesignVersionID);
                //Added Mapping Data  in to json PBP
                string defaultJSONData = ProcessMapping(DocumentJson, ViewModel, PBPImportTablesList, IsFullMigration, DocumentName.PBPVIEW, planYear);
                //Update PBPView Json With Updated values
                try
                {
                    ResultStatus = _folderVersionService.SaveFormInstanceDataCompressed(PbpViewFormInstaneId, defaultJSONData);
                }
                catch (Exception ex)
                {
                    ResultStatus = false;
                    _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                    //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                    _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "Error for save PBPView forminstance()", " for QID", null, ViewModel.QID, ex);
                }
                if (ResultStatus)
                {
                    Result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    Result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessPBPViewMapping()", " Error for QID", null, ViewModel.QID, ex);
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
            }

            return Result;
        }

        private string ProcessMapping(string emptyJson, PBPPlanConfigViewModel ViewModel, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList, bool isFullMigration, string MappingType,int planYear)
        {


            string UpdatedJson = string.Empty;
            PBPDataMapViewModel PBPDataMapDetails = null;
            IEnumerable<PBPMappingViewModel> PBPMappingList = null;
            //_PBPMappingServices.InitializeVariables(this._unitOfWorkAsync);
            List<PBPDataMapViewModel> PBPDataMapList = GetPBPDataMapList(ViewModel.PBPImportQueueID, ViewModel.QID);
            JObject Source = null;
            _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
            ValueManipulator ValueManipulatorObj = new ValueManipulator();

            try
            {
                JObject Target = JObject.Parse(emptyJson);
                foreach (var item in PBPImportTablesList)
                {
                    try
                    {
                        PBPDataMapDetails = PBPDataMapList.Where(s => s.TableName.Equals(item.PBPTableName))
                                            .FirstOrDefault();
                        if (PBPDataMapDetails != null)
                        {
                            if(!string.IsNullOrEmpty(PBPDataMapDetails.JsonData))
                            Source = JObject.Parse(PBPDataMapDetails.JsonData);
                        }
                    }
                    catch (Exception ex)
                    {
                        _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessMapping()", "Erorr in Source Mapping", null, ViewModel.QID, ex);
                    }
                    if (Source != null)
                    {
                        PBPMappingList = _PBPMappingServices.GetMapping(item.PBPTableName, isFullMigration, MappingType,planYear);
                        foreach (var itemViewMap in PBPMappingList)
                        {
                            if (Source != null)
                            {
                                try
                                {
                                    if (Target.SelectToken(itemViewMap.ElementPath) != null)
                                    {
                                        try
                                        {
                                            if (itemViewMap.IsCustomRule==true && itemViewMap.CustomRuleTypeId > 0)
                                            {
                                                string value = ValueManipulatorObj.ConvertValue(itemViewMap.CustomRuleTypeId, Source.SelectToken(itemViewMap.PBPFieldName).ToString(), MappingType);
                                                if (itemViewMap.PBPFieldName == "PBP_A_VBID_INDICATOR" && value == "")
                                                {
                                                    Target.SelectToken(itemViewMap.ElementPath)[itemViewMap.FieldPath] = "NO";
                                                }
                                                else
                                                {
                                                    Target.SelectToken(itemViewMap.ElementPath)[itemViewMap.FieldPath] = value;
                                                }
                                            }
                                            else if (ValueManipulatorObj.IsNeedToManipulatorValue(itemViewMap.PBPFieldName))
                                            {
                                                string ManipulatedValue = ValueManipulatorObj.ValueManipulatorProcessor(itemViewMap.PBPFieldName, Source.SelectToken(itemViewMap.PBPFieldName).ToString(), MappingType);
                                                try
                                                {
                                                    Target.SelectToken(itemViewMap.ElementPath)[itemViewMap.FieldPath] = JToken.Parse(ManipulatedValue);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Target.SelectToken(itemViewMap.ElementPath)[itemViewMap.FieldPath] = ManipulatedValue;
                                                }
                                            }
                                            else
                                            {
                                                Target.SelectToken(itemViewMap.ElementPath)[itemViewMap.FieldPath] = Source.SelectToken(itemViewMap.PBPFieldName);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                            string ErrorMessage = string.Empty;
                                            ErrorMessage = "PBPImportQueueID : -" + ViewModel.PBPImportQueueID + "QID : -" + ViewModel.QID;
                                            ErrorMessage += "PBP Table : -" + itemViewMap.PBPTableName + "MappingId : -" + itemViewMap.MappingId;
                                            ErrorMessage += "Target : -" + itemViewMap.ElementPath + "." + itemViewMap.FieldPath;
                                            ErrorMessage += "Source : -" + itemViewMap.PBPFieldName;
                                            _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessMapping()", ErrorMessage, null, ViewModel.QID, ex);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessMapping()", "Error in Source to target Mapping Mapping", null, ViewModel.QID, ex);
                                }
                            }
                        }
                        Source = null;
                    }
                }
                //insert Counties on base plans 
                if (MappingType.Equals(DocumentName.MEDICARE))
                {
                    Target = InsertCounties(ViewModel, Target);
                }
                if (MappingType.Equals(DocumentName.PBPVIEW))
                {
                    Target = InsertPBP_A_SERVICE_AREA(ViewModel, Target);
                }
                UpdatedJson = JsonConvert.SerializeObject(Target);
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "ProcessMapping()", " Error In Method ProcessMapping()", null, ViewModel.QID, ex);
            }
            return UpdatedJson;
        }

        private IEnumerable<PBPDataMapViewModel> GetPBPDataMapList(int pBPImportQueueID)
        {
            IEnumerable<PBPDataMapViewModel> pbpDataMapList = null;

            pbpDataMapList = (from c in this._unitOfWorkAsync.RepositoryAsync<PBPDataMap>()
                                                 .Query()
                                                 .Filter(c => c.PBPImportQueueID == pBPImportQueueID)
                                                 .Get()
                              select new PBPDataMapViewModel
                              {
                                  PBPDataMapId = c.PBPDataMapId,
                                  QID = c.QID,
                                  TableName = c.TableName,
                                  FieldName = c.FieldName,
                                  JsonData = c.JsonData,
                                  PBPImportQueueID = c.PBPImportQueueID

                              }).OrderBy(s => s.PBPImportQueueID).ToList();

            if (pbpDataMapList == null)
            {
                return new List<PBPDataMapViewModel>();
            }
            return pbpDataMapList.ToList();
        }

        private PBPDataMapViewModel GetPBPDataMapList(int pBPImportQueueID, string tablename, string QID)
        {
            PBPDataMapViewModel pbpDataMapDetails = null;

            pbpDataMapDetails = (from c in this._unitOfWorkAsync.RepositoryAsync<PBPDataMap>()
                                                 .Query()
                                                 .Filter(c => c.PBPImportQueueID.Equals(pBPImportQueueID)
                                                            && c.QID.Equals(QID)
                                                            && c.TableName.Equals(tablename))
                                                 .Get()
                                 select new PBPDataMapViewModel
                                 {
                                     PBPDataMapId = c.PBPDataMapId,
                                     QID = c.QID,
                                     TableName = c.TableName,
                                     FieldName = c.FieldName,
                                     JsonData = c.JsonData,
                                     PBPImportQueueID = c.PBPImportQueueID

                                 }).ToList().FirstOrDefault();

            if (pbpDataMapDetails == null)
            {
                return new PBPDataMapViewModel();
            }
            return pbpDataMapDetails;
        }

        public void SavePBPDataMapList(DataTable targerTable)
        {
            List<PBPDataMap> PBPDataMapList = targerTable.AsEnumerable().Select(m => new PBPDataMap()
            {
                QID = m.Field<string>("QID"),
                TableName = m.Field<string>("TableName"),
                FieldName = m.Field<string>("FieldName"),
                JsonData = m.Field<string>("JsonData"),
                PBPImportQueueID = Convert.ToInt32(m.Field<string>("PBPImportQueueID")),
            }).ToList();
            if (PBPDataMapList.Count > 0)
            {
                this._unitOfWorkAsync.RepositoryAsync<PBPDataMap>().InsertRange(PBPDataMapList);
                this._unitOfWorkAsync.Save();
            }
        }

        public void SavePBPDataMap(PBPDataMap pbpdataMap)
        {
            if (pbpdataMap != null)
            {
                this._unitOfWorkAsync.RepositoryAsync<PBPDataMap>().Insert(pbpdataMap);
                this._unitOfWorkAsync.Save();
            }
        }

        public DataTable CreateTableWithColumn()
        {
            DataTable targerTable = new DataTable();

            DataColumn column = new DataColumn("QID");
            targerTable.Columns.Add(column);
            column = new DataColumn("TableName");
            targerTable.Columns.Add(column);
            column = new DataColumn("FieldName");
            targerTable.Columns.Add(column);
            column = new DataColumn("JsonData");
            targerTable.Columns.Add(column);
            column = new DataColumn("PBPImportQueueID");
            targerTable.Columns.Add(column);
            return targerTable;
        }

        public string CreateJsonForRow(DataRow dRow, DataColumnCollection columns)
        {
            Dictionary<string, object> childRow = new Dictionary<string, object>();
            foreach (DataColumn col in columns)
            {
                childRow.Add(col.ColumnName, dRow[col]);
            }

            string json = JsonConvert.SerializeObject(childRow);
            return json;
        }

        private string ControlHandler(PBPMappingViewModel viewMoldel, string json, string value, int controlType)
        {
            string Data = string.Empty;
            switch (controlType)
            {
                case 1:
                    Data = RepeaterHandler(viewMoldel, json, value);
                    break;

                case 2:
                    Data = ChildPopUpHandler(viewMoldel, json, value);
                    break;

                case 3:
                    Data = MultiSelectHandler(viewMoldel, json, value);
                    break;

                    //case 4:
                    //    Data =
                    //    break;
            }
            return Data;
        }

        private string RepeaterHandler(PBPMappingViewModel viewMoldel, string json, string value)
        {
            string ControlJson = string.Empty;
            JObject Target = JObject.Parse(json);

            ControlJson = Target.SelectToken(viewMoldel.ElementPath)[viewMoldel.FieldPath].ToString();


            string Data = string.Empty;

            return Data;
        }

        private string ChildPopUpHandler(PBPMappingViewModel viewMoldel, string json, string value)
        {
            string Data = string.Empty;

            return Data;
        }

        private string MultiSelectHandler(PBPMappingViewModel viewMoldel, string json, string value)
        {
            string Data = string.Empty;

            return Data;
        }

        private string UpdateIsPBPImportFlag(string json)
        {
            string UpdatedJson = string.Empty;
            try
            {
                JObject Target = JObject.Parse(json);
                Target.SelectToken("Miscellaneous")["IsPBPImport"] = "Yes";
                UpdatedJson = JsonConvert.SerializeObject(Target);
            }
            catch (Exception ex)
            {
                UpdatedJson = json;
            }
            return UpdatedJson;
        }

        private string GetPlanName(string sourceJson)
        {
            string PlanName = string.Empty;
            try
            {
                JObject Target = JObject.Parse(sourceJson);
                PlanName = Convert.ToString(Target.SelectToken("SECTIONASECTIONA1")["PlanName"]);
            }
            catch (Exception ex)
            {
                PlanName = "";
            }
            return PlanName;
        }

        private string GetPlanNumber(string sourceJson)
        {
            string PlanNumber = string.Empty;
            try
            {
                JObject Target = JObject.Parse(sourceJson);
                PlanNumber = Convert.ToString(Target.SelectToken("SECTIONASECTIONA1")["ContractNumber"]);
            }
            catch (Exception ex)
            {
                PlanNumber = "";
            }
            return PlanNumber;
        }

        private string GetFormInstanceJson(int UserActionId, int formInstanceId, int formDesignVersionID)
        {
            string DocumentJson = string.Empty;
            if (UserActionId.Equals((int)PBPUserActionList.MapItWithAnothereBSPlan) || UserActionId.Equals((int)PBPUserActionList.UpdatePlan))
            {
                DocumentJson = this._folderVersionService.GetFormInstanceDataCompressed(1, formInstanceId);
                //if there is no json in Fldr.FormInstanceDataMap then get blank json
                if (String.IsNullOrEmpty(DocumentJson))
                {
                    FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionID, this._formDesignService);
                    FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);
                    DocumentJson = detail.JSONData;
                }
            }
            else
            {
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, formDesignVersionID, this._formDesignService);
                FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(false);
                DocumentJson = detail.JSONData;
            }
            return DocumentJson;
        }

        private string GetPlanType(JObject Source)
        {
            string PlanType = String.Empty;
            try
            {
                PlanType = Source.SelectToken("SECTIONASECTIONA1")["PlanType"].ToString();
            }
            catch (Exception ex)
            {
                PlanType = "";
            }
            return PlanType;
        }

        private JObject InsertCounties(PBPPlanConfigViewModel ViewModel, JObject Target)
        {
            JObject Source = null;
            //PDP PlanType
            string PlanType = "Medicare Prescription Drug Plan";
            PBPMappingViewModel PBPMappingViewModel = null;
            PBPDataMapViewModel PBPDataMapDetails = null;

            try
            {
                if (Target != null)
                {
                    string SourcePlanType = GetPlanType(Target);
                    if (SourcePlanType.Equals(PlanType))
                    {
                        PBPMappingViewModel = GetCounties("pbpregions", "PBP_A_REGION_NAME");
                        PBPDataMapDetails = GetPBPDataMapList(ViewModel.PBPImportQueueID, PBPMappingViewModel.PBPTableName, ViewModel.QID);
                        if (PBPDataMapDetails.JsonData != null)
                        {
                            Source = JObject.Parse(PBPDataMapDetails.JsonData);
                            Target.SelectToken(PBPMappingViewModel.ElementPath)[PBPMappingViewModel.FieldPath] = Source.SelectToken(PBPMappingViewModel.PBPFieldName);
                        }
                    }
                    else
                    {
                        PBPMappingViewModel = GetCounties("PLAN_AREAS", "PBP_A_COUNTY_NAME");
                        PBPDataMapDetails = GetPBPDataMapList(ViewModel.PBPImportQueueID, PBPMappingViewModel.PBPTableName, ViewModel.QID);
                        if (PBPDataMapDetails.JsonData != null)
                        {
                            Source = JObject.Parse(PBPDataMapDetails.JsonData);
                            Target.SelectToken(PBPMappingViewModel.ElementPath)[PBPMappingViewModel.FieldPath] = Source.SelectToken(PBPMappingViewModel.PBPFieldName);
                            try
                            {
                                Target.SelectToken("SectionA.SectionA1")["StateandCounty"] = Source.SelectToken("PLAN_AREAS");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "InsertCounties()", " Error In Method InsertCounties()", null, ViewModel.QID, ex);
            }
            return Target;
        }

        private PBPMappingViewModel GetCounties(string tableName, string filedName)
        {
            PBPMappingViewModel PBPViewMapList = (from queue in this._unitOfWorkAsync.RepositoryAsync<PBPMedicareMap>()
                       .Get()
                       .Where(s => s.IsActive == true
                             && s.IsFullMigration.Equals(true)
                             && s.PBPTableName.Equals(tableName)
                             && s.PBPFieldName.Equals(filedName)
                             )
                                                  select new PBPMappingViewModel
                                                  {
                                                      MappingId = queue.PBPMedicareMapID,
                                                      ElementPath = queue.ElementPath,
                                                      FieldPath = queue.FieldPath,
                                                      PBPFieldName = queue.PBPFieldName,
                                                      PBPTableName = queue.PBPTableName,
                                                      CustomRuleTypeId = queue.CustomRuleTypeId,
                                                      IsActive = queue.IsActive,
                                                      IsCustomRule = queue.IsCustomRule,
                                                      Year = queue.Year,
                                                  }).FirstOrDefault();
            return PBPViewMapList;
        }

        private List<PBPDataMapViewModel> GetPBPDataMapList(int pBPImportQueueID, string QID)
        {
            List<PBPDataMapViewModel> pbpDataMapDetailList = null;

            pbpDataMapDetailList = (from c in this._unitOfWorkAsync.RepositoryAsync<PBPDataMap>()
                                    .Query()
                                    .Filter(c => c.PBPImportQueueID.Equals(pBPImportQueueID)
                                    && c.QID.Equals(QID)
                                    ).Get()
                                    select new PBPDataMapViewModel
                                    {
                                        PBPDataMapId = c.PBPDataMapId,
                                        QID = c.QID,
                                        TableName = c.TableName,
                                        FieldName = c.FieldName,
                                        JsonData = c.JsonData,
                                        PBPImportQueueID = c.PBPImportQueueID
                                    }).ToList();

            if (pbpDataMapDetailList == null)
            {
                return new List<PBPDataMapViewModel>();
            }
            return pbpDataMapDetailList;
        }

        private JObject InsertPBP_A_SERVICE_AREA(PBPPlanConfigViewModel ViewModel, JObject Target)
        {
            JObject Source = null;
            //PDP PlanType
            string PlanType = "Medicare Prescription Drug Plan";
            PBPMappingViewModel PBPMappingViewModel = null;
            PBPDataMapViewModel PBPDataMapDetails = null;

            try
            {

                PBPMappingViewModel = (from c in this._unitOfWorkAsync.RepositoryAsync<PBPViewMap>()
                                     .Query()
                                     .Filter(c => c.PBPFieldName.Equals("PBP_A_SERVICE_AREA")
                                     && c.PBPTableName.Equals("PBP")
                                     && c.Year.Equals(ViewModel.Year)
                                     &&c.IsActive==true
                                     ).Get()
                                     select new PBPMappingViewModel
                                     {
                                         ElementPath=c.ElementPath,
                                         FieldPath=c.FieldPath,
                                         PBPFieldName=c.PBPFieldName
                                     }).FirstOrDefault();


                if (Target != null)
                {
                    string SourcePlanType = GetPlanType(Target);
                    if (SourcePlanType.Equals(PlanType))
                    {
                        PBPMappingViewModel.PBPTableName = "pbpregions";
                        PBPDataMapDetails = GetPBPDataMapList(ViewModel.PBPImportQueueID, PBPMappingViewModel.PBPTableName, ViewModel.QID);
                        if (PBPDataMapDetails.JsonData != null)
                        {
                            Source = JObject.Parse(PBPDataMapDetails.JsonData);
                            Target.SelectToken(PBPMappingViewModel.ElementPath)[PBPMappingViewModel.FieldPath] = Source.SelectToken(PBPMappingViewModel.PBPFieldName);
                        }
                    }
                    else
                    {
                        PBPMappingViewModel.PBPTableName = "PLAN_AREAS";
                        PBPDataMapDetails = GetPBPDataMapList(ViewModel.PBPImportQueueID, PBPMappingViewModel.PBPTableName, ViewModel.QID);
                        if (PBPDataMapDetails.JsonData != null)
                        {
                            Source = JObject.Parse(PBPDataMapDetails.JsonData);
                            Target.SelectToken(PBPMappingViewModel.ElementPath)[PBPMappingViewModel.FieldPath] = Source.SelectToken(PBPMappingViewModel.PBPFieldName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "InsertCounties()", " Error In Method InsertCounties()", null, ViewModel.QID, ex);
            }
            return Target;

        }
    }
}
