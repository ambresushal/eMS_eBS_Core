using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;

using tmg.equinox.pbpimport.Interfaces;
using System.Data.SqlClient;
using tmg.equinox.core.logging.Logging;
using Newtonsoft.Json;

namespace tmg.equinox.pbpimport
{
    public class PerformUserAction
    {
        IPBPImportService _pBPImportService = null;
        IUnitOfWorkAsync _unitOfWorkAsync = null;
        List<PBPBaseLineFolderDetailViewModel> PBPBaseLineFolderDetailList = new List<PBPBaseLineFolderDetailViewModel>();
        IFolderVersionServices _folderVersionService = null;
        IFormDesignService _formDesignService = null;
        IPBPImportHelperServices _PBPImportHelperServices;
        IPBPImportActivityLogServices _PBPImportActivityLogServices;
        IPBPMappingServices _PBPMappingServices;
        int MEDICAREFORMDESIGNVERSIONID = 0;
        private static ILog _logger = LogProvider.For<PerformUserAction>();
        int _year = 0;
        public PerformUserAction(IPBPImportService _pBPImportService, IUnitOfWorkAsync _unitOfWorkAsync,
                                        IFolderVersionServices _folderVersionService, IFormDesignService _formDesignService, IPBPImportHelperServices PBPImportHelperServices, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPMappingServices PBPMappingServices, int year)
        {
            this._pBPImportService = _pBPImportService;
            this._unitOfWorkAsync = _unitOfWorkAsync;
            this._folderVersionService = _folderVersionService;
            this._formDesignService = _formDesignService;
            _PBPImportHelperServices = PBPImportHelperServices;
            _PBPImportActivityLogServices = PBPImportActivityLogServices;
            _PBPMappingServices = PBPMappingServices;
            _PBPImportHelperServices.InitializeVariables(this._unitOfWorkAsync);
            _PBPImportActivityLogServices.InitializeVariables(this._unitOfWorkAsync);
            _year = year;
            MEDICAREFORMDESIGNVERSIONID = _PBPImportHelperServices.GetFormDesignVersionID(DocumentName.MEDICARE,_year);
        }

        public ServiceResult PerformUserActionOnPBPPlan(int pBPImportQueueID, bool IsFullMigration)
        {
            ServiceResult Result = new ServiceResult();
            string Message = string.Empty;
            int TotalQIDs = 0, FailedCount = 0, SuccessCount = 0, PbpImportQueueId = 0;
            List<PBPPlanConfigViewModel> PlanDetailList = _pBPImportService.GetPBPPlanDetailsForProcess(pBPImportQueueID);
            Common CommonServiceObj = new Common(this._unitOfWorkAsync);
            IEnumerable<PBPImportTablesViewModel> PBPImportTablesList = CommonServiceObj.GetPBPImportTablesList();
            bool IsNeedToCreateBaseLineFolder = false;
            if (PlanDetailList != null)
            {
                _pBPImportService.UpdateImportQueueStatus(pBPImportQueueID, ProcessStatusMasterCode.InProgress);
                _logger.Debug(string.Format("Started QId{0}", pBPImportQueueID.ToString()));
                TotalQIDs = PlanDetailList.Count();
                foreach (var item in PlanDetailList)
                {
                    int formInstanceId = 0;
                    PbpImportQueueId = item.PBPImportQueueID;
                    IsNeedToCreateBaseLineFolder = this.IsNeedToCreateBaseLineFolder(PBPBaseLineFolderDetailList, item.FolderId);


                    string baseline = JsonConvert.SerializeObject(
                        PBPBaseLineFolderDetailList.Select(ll => new { ll.FolderId, ll.AfterBaseLineFolderVersionId, ll.BeforeBaseLineFolderVersionId}),
                            Formatting.Indented);

                    _logger.Debug(string.Format("InstanceId {0} QUID {1} IsNeedToCreateBaseLineFolder {2} BaseLineList {3}", item.FormInstanceId, item.QID,  IsNeedToCreateBaseLineFolder, baseline));
                    switch (item.UserAction)
                    {
                        case (int)PBPUserActionList.AddPlanIneBS:
                            Result = AddPlanIneBS(item, PBPImportTablesList, IsFullMigration, IsNeedToCreateBaseLineFolder,ref formInstanceId);
                            if (Result.Result.Equals(ServiceResultStatus.Success))
                            {
                                UpdateAccountMapPBPDetailTable(item);
                            }
                            break;

                        case (int)PBPUserActionList.MapItWithAnothereBSPlan:
                            Result = MapItWithAnothereBSPlan(item, PBPImportTablesList, IsFullMigration, IsNeedToCreateBaseLineFolder,ref formInstanceId);
                            if (Result.Result.Equals(ServiceResultStatus.Success))
                            {
                                UpdateAccountMapPBPDetailTable(item);
                            }
                            break;

                        case (int)PBPUserActionList.TerminatePlanFromeBS:
                            Result = TerminatePlanFromeBS(item, IsNeedToCreateBaseLineFolder, PlanDetailList);
                            if (Result.Result.Equals(ServiceResultStatus.Success))
                            {
                                UpdateAccountMapPBPDetailTable(item);
                            }
                            break;

                        case (int)PBPUserActionList.UpdatePlan:
                            if (item.IsIncludeInEbs)
                            {
                                Result = UpdatePlanIneBS(item, PBPImportTablesList, IsFullMigration, IsNeedToCreateBaseLineFolder,ref formInstanceId);
                                if (Result.Result.Equals(ServiceResultStatus.Success))
                                {
                                    UpdateAccountMapPBPDetailTable(item);
                                }
                            }
                            else
                            {
                                Result.Result = ServiceResultStatus.Success;
                            }
                            break;
                        case (int)PBPUserActionList.NoActionRequired:
                            Result.Result = ServiceResultStatus.Success;
                            break;
                    }
                    if (Result.Result == ServiceResultStatus.Failure)
                    {
                        FailedCount = FailedCount + 1;
                        Message += "" + item.QID + "       Failed\n" + "";
                    }
                    else
                    {
                        SuccessCount = SuccessCount + 1;
                        Message += item.QID + "       Success\n" + "";
                        //if(formInstanceId > 0)
                        //{
                        //    _folderVersionService.UpdateReportingCenterDatabase(formInstanceId, null, false);
                        //}
                    }
                }

                if (SuccessCount == TotalQIDs)
                {
                    _pBPImportService.UpdateImportQueueStatus(pBPImportQueueID, ProcessStatusMasterCode.Complete);
                }
                else
                {
                    _pBPImportService.UpdateImportQueueStatus(pBPImportQueueID, ProcessStatusMasterCode.Errored);
                }

                string MessageStr = String.Concat("PBPImportQueueID = ", pBPImportQueueID, "\n Total QID = ", TotalQIDs
                                                  , "\n Successfull = ", SuccessCount, "\n Failed = ", FailedCount
                                                  , "\n QID       Status\n", Message);
                //PBPImportActivityLogServices PBPImportActivityLogServicesObj = new PBPImportActivityLogServices(this._unitOfWorkAsync);
                _PBPImportActivityLogServices.AddPBPImportActivityLog(pBPImportQueueID, null, MessageStr, null, null, new Exception());

            }

            return Result;
        }

        private ServiceResult AddPlanIneBS(PBPPlanConfigViewModel ViewModel, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList, bool IsFullMigration, bool IsNeedToCreateBaseLineFolder,ref int formInstanceId)
        {
            ServiceResult Result = new ServiceResult();
            ServiceResult AddPlanResult = new ServiceResult();
            ServiceResult BaseLineResult = new ServiceResult();
            try
            {
                //check QID is exist in folder or not 
                if (IsQIDExist(ViewModel))
                {
                    _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "AddPlanIneBS()", " " + ViewModel.QID + " is already Exist in selected Folder( " + ViewModel.FolderId + ") FolderVesion ( " + ViewModel.FolderVersionId + ") ", null, ViewModel.QID, new Exception());
                    AddPlanResult.Result = ServiceResultStatus.Success;
                }
                else
                {
                    //Create BaseLine 
                    if (IsNeedToCreateBaseLineFolder)
                    {
                        BaseLineResult = CreateBaseLineFolderBeforeUpdate(ViewModel);
                        int BeforeBaseLineFolderVersionId = ViewModel.FolderVersionId;
                        //assign new folderversion id after base line 
                        ViewModel.FolderVersionId = Convert.ToInt32(BaseLineResult.Items.FirstOrDefault().Messages.FirstOrDefault());
                        //append PBPBaseLineList with new baseline
                        _logger.Debug(string.Format( "ViewModel.QID{0} AddPlanIneBS", ViewModel.QID));
                        AddInPBPBaseLineList(this.PBPBaseLineFolderDetailList, ViewModel.FolderId, BeforeBaseLineFolderVersionId, ViewModel.FolderVersionId);
                        if (BaseLineResult.Result.Equals(ServiceResultStatus.Failure))
                        {
                            _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "AddPlanIneBS()", "Error unable to create base line for QID", null, ViewModel.QID, null);
                        }
                    }
                    else
                    {
                        ViewModel.FolderVersionId = this.PBPBaseLineFolderDetailList
                                                    .Where(s => s.FolderId.Equals(ViewModel.FolderId))
                                                    .Select(t => t.AfterBaseLineFolderVersionId).FirstOrDefault();
                        BaseLineResult.Result = ServiceResultStatus.Success;
                    }
                    if (BaseLineResult.Result == ServiceResultStatus.Success)
                    {
                        //check duplicate fromname anmd append import id
                        ViewModel = GenerateFormInstance(ViewModel, MEDICAREFORMDESIGNVERSIONID);

                        if (ViewModel.FormInstanceId > 0)
                        {
                            Result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            Result.Result = ServiceResultStatus.Failure;
                            _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "AddPlanIneBS()", " unable to create forminstance", null, ViewModel.QID, null);
                        }

                        if (Result.Result == ServiceResultStatus.Success)
                        {
                            Mapping MappingServiceObj = new Mapping(this._folderVersionService, this._formDesignService, this._unitOfWorkAsync, _PBPImportActivityLogServices, _PBPImportHelperServices, _PBPMappingServices, _pBPImportService);
                            if (IsFullMigration)
                            {
                                AddPlanResult = MappingServiceObj.ProcessMedicareMapping(ViewModel, MEDICAREFORMDESIGNVERSIONID, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                            }
                            else
                            {
                                AddPlanResult = MappingServiceObj.ProcessPBPViewMapping(ViewModel, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                            }
                            if(AddPlanResult.Result == ServiceResultStatus.Success)
                            {
                                formInstanceId = ViewModel.FormInstanceId;
                            }
                        }
                    }
                }
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "AddPlanIneBS()", "Error for QID", null, ViewModel.QID, ex);
                AddPlanResult = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        private ServiceResult MapItWithAnothereBSPlan(PBPPlanConfigViewModel ViewModel, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList, bool IsFullMigration, bool IsNeedToCreateBaseLineFolder,ref int formInstanceId)
        {
            ServiceResult Result = new ServiceResult();
            ServiceResult AddPlanResult = new ServiceResult();
            ServiceResult BaseLineResult = new ServiceResult();
            try
            {
                if (ViewModel.FormInstanceId > 0 && ViewModel.FolderId > 0 && ViewModel.FolderVersionId > 0)
                {
                    //Create BaseLine 
                    if (IsNeedToCreateBaseLineFolder)
                    {
                        BaseLineResult = CreateBaseLineFolderBeforeUpdate(ViewModel);
                        int BeforeBaseLineFolderVersionId = ViewModel.FolderVersionId;
                        //assign new folderversion id after base line 
                        ViewModel.FolderVersionId = Convert.ToInt32(BaseLineResult.Items.FirstOrDefault().Messages.FirstOrDefault());
                        _logger.Debug(string.Format("ViewModel.QID{0} MapItWithAnothereBSPlan", ViewModel.QID));
                        //append PBPBaseLineList with new baseline
                        AddInPBPBaseLineList(this.PBPBaseLineFolderDetailList, ViewModel.FolderId, BeforeBaseLineFolderVersionId, ViewModel.FolderVersionId);

                        if (BaseLineResult.Result.Equals(ServiceResultStatus.Failure))
                        {
                            _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "MapItWithAnothereBSPlan()", "Error unable to create base line for QID", null, ViewModel.QID, null);
                        }
                    }
                    else
                    {
                        ViewModel.FolderVersionId = this.PBPBaseLineFolderDetailList
                                                    .Where(s => s.FolderId.Equals(ViewModel.FolderId))
                                                    .Select(t => t.AfterBaseLineFolderVersionId).FirstOrDefault();
                        BaseLineResult.Result = ServiceResultStatus.Success;
                    }
                    if (BaseLineResult.Result == ServiceResultStatus.Success)
                    {
                        ViewModel.FormInstanceId = _PBPImportHelperServices.GetMedicareFormInstanceID(ViewModel.FolderVersionId, MEDICAREFORMDESIGNVERSIONID, ViewModel.DocumentId);
                        Mapping MappingServiceObj = new Mapping(this._folderVersionService, this._formDesignService, this._unitOfWorkAsync, _PBPImportActivityLogServices, _PBPImportHelperServices, _PBPMappingServices, _pBPImportService);
                        if (IsFullMigration)
                        {
                            AddPlanResult = MappingServiceObj.ProcessMedicareMapping(ViewModel, MEDICAREFORMDESIGNVERSIONID, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                        }

                        else
                        {
                            AddPlanResult = MappingServiceObj.ProcessPBPViewMapping(ViewModel, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                        }
                        if (AddPlanResult.Result == ServiceResultStatus.Success)
                        {
                            Result.Result = ServiceResultStatus.Success;
                            formInstanceId = ViewModel.FormInstanceId;
                        }
                        else
                        {
                            Result.Result = ServiceResultStatus.Failure;
                        }
                    }
                }
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "MapItWithAnothereBSPlan()", "Error In Update Json From Instance  for QID", null, ViewModel.QID, ex);
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        private ServiceResult UpdatePlanIneBS(PBPPlanConfigViewModel ViewModel, IEnumerable<PBPImportTablesViewModel> PBPImportTablesList, bool IsFullMigration, bool IsNeedToCreateBaseLineFolder,ref int formInstanceId)
        {
            ServiceResult Result = new ServiceResult();
            ServiceResult AddPlanResult = new ServiceResult();
            ServiceResult BaseLineResult = new ServiceResult();
            try
            {
                if (ViewModel.FolderId > 0 && ViewModel.FolderVersionId > 0)
                {
                    if (IsNeedToCreateBaseLineFolder)
                    {
                        //create base line
                        BaseLineResult = CreateBaseLineFolderBeforeUpdate(ViewModel);
                        int BeforeBaseLineFolderVersionId = ViewModel.FolderVersionId;
                        //assign new folderversion id after base line 
                        ViewModel.FolderVersionId = Convert.ToInt32(BaseLineResult.Items.FirstOrDefault().Messages.FirstOrDefault());
                        //append PBPBaseLineList with new baseline
                        _logger.Debug(string.Format("ViewModel.QID{0} UpdatePlanIneBS", ViewModel.QID));
                        AddInPBPBaseLineList(this.PBPBaseLineFolderDetailList, ViewModel.FolderId, BeforeBaseLineFolderVersionId, ViewModel.FolderVersionId);

                        if (BaseLineResult.Result.Equals(ServiceResultStatus.Failure))
                        {
                            _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "UpdatePlanIneBS()", "Error unable to create base line for QID", null, ViewModel.QID, null);
                        }

                    }
                    else
                    {
                        ViewModel.FolderVersionId = this.PBPBaseLineFolderDetailList
                                                    .Where(s => s.FolderId.Equals(ViewModel.FolderId))
                                                    .Select(t => t.AfterBaseLineFolderVersionId).FirstOrDefault();
                        BaseLineResult.Result = ServiceResultStatus.Success;
                    }
                    if (BaseLineResult.Result == ServiceResultStatus.Success)
                    {
                        ViewModel.FormInstanceId = _PBPImportHelperServices.GetMedicareDocumentIDByName(MEDICAREFORMDESIGNVERSIONID, ViewModel.DocumentId, ViewModel.FolderVersionId);
                        Mapping MappingServiceObj = new Mapping(this._folderVersionService, this._formDesignService, this._unitOfWorkAsync, _PBPImportActivityLogServices, _PBPImportHelperServices, _PBPMappingServices, _pBPImportService);
                        if (IsFullMigration)
                        {
                            AddPlanResult = MappingServiceObj.ProcessMedicareMapping(ViewModel, MEDICAREFORMDESIGNVERSIONID, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                        }
                        else
                        {
                            AddPlanResult = MappingServiceObj.ProcessPBPViewMapping(ViewModel, IsFullMigration, PBPImportTablesList, ViewModel.Year);
                        }
                        //_pBPImportService.UpdateAccountProductMap(ViewModel);
                        if (AddPlanResult.Result == ServiceResultStatus.Success)
                        {
                            Result.Result = ServiceResultStatus.Success;
                            formInstanceId = ViewModel.FormInstanceId;
                        }
                        else
                        {
                            Result.Result = ServiceResultStatus.Failure;
                        }
                    }
                }
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                else
                {
                    Result.Result = ServiceResultStatus.Success;
                }
            }

            catch (Exception ex)
            {
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure || AddPlanResult.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "UpdatePlanIneBS()", "Error In Update Json From Instance  for QID", null, ViewModel.QID, ex);
                Result = ex.ExceptionMessages();
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        private ServiceResult TerminatePlanFromeBS(PBPPlanConfigViewModel ViewModel, bool IsNeedToCreateBaseLineFolder, List<PBPPlanConfigViewModel> planList)
        {
            ServiceResult Result = new ServiceResult();
            ServiceResult BaseLineResult = new ServiceResult();
            try
            {
                if (IsNeedToCreateBaseLineFolder)
                {
                    BaseLineResult = CreateBaseLineFolderBeforeUpdate(ViewModel);
                    int BeforeBaseLineFolderVersionId = ViewModel.FolderVersionId;
                    //assign new folderversion id after base line 
                    ViewModel.FolderVersionId = Convert.ToInt32(BaseLineResult.Items.FirstOrDefault().Messages.FirstOrDefault());
                    //append PBPBaseLineList with new baseline
                    _logger.Debug(string.Format("ViewModel.QID{0} TerminatePlanFromeBS", ViewModel.QID));
                    AddInPBPBaseLineList(this.PBPBaseLineFolderDetailList, ViewModel.FolderId, BeforeBaseLineFolderVersionId, ViewModel.FolderVersionId);
                    if (BaseLineResult.Result.Equals(ServiceResultStatus.Failure))
                    {
                        _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "TerminatePlanFromeBS()", "Error unable to create base line for QID", null, ViewModel.QID, null);
                    }
                }
                else
                {
                    ViewModel.FolderVersionId = this.PBPBaseLineFolderDetailList
                                                .Where(s => s.FolderId.Equals(ViewModel.FolderId))
                                                .Select(t => t.AfterBaseLineFolderVersionId).FirstOrDefault();
                    BaseLineResult.Result = ServiceResultStatus.Success;
                }
                if (BaseLineResult.Result == ServiceResultStatus.Success)
                {

                    /* delete 2018 proxy plan from 2019 PBP mport*/

                    var FolderVersionDetail = this._unitOfWorkAsync.RepositoryAsync<FolderVersion>().Get()
                                                    .Where(s => s.FolderVersionID.Equals(ViewModel.FolderVersionId)
                                                    && s.FolderVersionStateID.Equals(1)
                                                    )
                                                    .FirstOrDefault();
                    int MedicareFormDesignVersionId = _PBPImportHelperServices.GetFormDesignVersionID(DocumentName.MEDICARE, FolderVersionDetail.EffectiveDate.Year);
                    ViewModel = _PBPImportHelperServices.GetFormInstanceIdForDelete(MedicareFormDesignVersionId, ViewModel);
                    Result = _folderVersionService.DeleteFormInstance(ViewModel.FolderId, 1, ViewModel.FolderVersionId, ViewModel.FormInstanceId, "PBP Super User");
                }
            }
            catch (Exception ex)
            {
                if (BaseLineResult.Result == ServiceResultStatus.Failure || Result.Result == ServiceResultStatus.Failure)
                {
                    Result.Result = ServiceResultStatus.Failure;
                    _PBPImportActivityLogServices.SaveUserErrorMessage(ViewModel.PBPImportQueueID, ViewModel.QID, "Failed");
                }
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "TerminatePlanFromeBS()", " Error for QID", null, ViewModel.QID, ex);
                Result.Result = ServiceResultStatus.Failure;
            }
            return Result;
        }

        private bool IsNeedToCreateBaseLineFolder(List<PBPBaseLineFolderDetailViewModel> PBPBaseLineFolderDetailList, int folderId)
        {
            bool Result = false;

            if (folderId > 0)
            {
                if (PBPBaseLineFolderDetailList.Where(s => s.FolderId.Equals(folderId)).Any() == false)
                {
                    Result = true;
                }
                else
                {
                    Result = false;
                }
            }
            return Result;
        }

        private ServiceResult CreateBaseLineFolderBeforeUpdate(PBPPlanConfigViewModel ViewModel)
        {
            ServiceResult Result = new ServiceResult();
            //VersionNumberBuilder builder = null;
            //builder = new VersionNumberBuilder();

            var LatestFolderVersion = this._unitOfWorkAsync.RepositoryAsync<FolderVersion>()
                                             .Get()
                                             .Where(s => s.FolderVersionID == ViewModel.FolderVersionId
                                             && s.IsActive == true)
                                             .OrderByDescending(s => s.FolderVersionID)
                                             .FirstOrDefault();

            if (LatestFolderVersion.FolderVersionStateID.Equals(3))
            {
                int UserId = this._unitOfWorkAsync.RepositoryAsync<User>().Get()
                             .Where(s => s.UserName.Equals(ViewModel.CreatedBy))
                             .Select(s => s.UserID).FirstOrDefault();
                Result = _folderVersionService.CreateNewMinorVersion(1, ViewModel.FolderId,
                    LatestFolderVersion.FolderVersionID
                    , LatestFolderVersion.FolderVersionNumber
                    , ViewModel.CreatedBy + " Create new minor version for PBP Import"
                    , LatestFolderVersion.EffectiveDate
                    , true
                   , UserId
                   , null,
                    7,
                    ""
                    , ViewModel.CreatedBy
                   );
                return Result;

            }
            else
            {

                var FolderVersion = this._unitOfWorkAsync.RepositoryAsync<FolderVersion>()
                                          .Get()
                                          .Where(s => s.FolderVersionID == ViewModel.FolderVersionId
                                                 && s.IsActive == true).FirstOrDefault();
                string CurrentVersionNumber = FolderVersion.FolderVersionNumber;
                DateTime FolderVersionEffectiveDate = FolderVersion.EffectiveDate;

                string NextFolderVersionName = this.GetNextMinorVersionNumber(CurrentVersionNumber, FolderVersionEffectiveDate);
                Result = _folderVersionService.BaseLineFolder(1, 0, ViewModel.FolderId, ViewModel.FolderVersionId,
                0, ViewModel.CreatedBy, NextFolderVersionName, "Baseline Created For PBP Import By '" + ViewModel.CreatedBy + "'", 0, null, false, false, false);
                return Result;
            }
        }

        private void AddInPBPBaseLineList(List<PBPBaseLineFolderDetailViewModel> PBPBaseLineFolderDetailList, int folderId, int beforeBaseLineFolderVersionId, int afterBaseLineFolderVersionId)
        {
            _logger.Debug(string.Format("folderId:{0},afterBaseLineFolderVersionId:{1},beforeBaseLineFolderVersionId:{2}", folderId, afterBaseLineFolderVersionId, beforeBaseLineFolderVersionId));
            if (folderId > 0 && afterBaseLineFolderVersionId > 0 && beforeBaseLineFolderVersionId > 0)
            {
                PBPBaseLineFolderDetailList.Add(new PBPBaseLineFolderDetailViewModel
                {
                    FolderId = folderId,
                    AfterBaseLineFolderVersionId = afterBaseLineFolderVersionId,
                    BeforeBaseLineFolderVersionId = beforeBaseLineFolderVersionId
                });
            }
        }

        private string GetNextMinorVersionNumber(string folderVersionNumber, DateTime effectiveDate)
        {
            int _year = effectiveDate.Year;
            string _versionNumber = folderVersionNumber;
            bool _isMinor = true;

            const string UNDERSCORE = "_";
            const string NULLSTRING = "";
            const string FIRSTMINORVERSION = "0.01";
            const string MAJORVERSION = ".0";

            string nextVersionNumber = null;
            double versionNumeric = 0;
            if (!string.IsNullOrEmpty(_versionNumber))
            {

                if (_isMinor)
                {
                    if (_versionNumber.Split('_')[0] != _year.ToString())
                        nextVersionNumber = NULLSTRING + _year + UNDERSCORE + FIRSTMINORVERSION;
                    else
                    {
                        versionNumeric = Convert.ToDouble(_versionNumber.Split('_')[1]) + 0.01;
                        nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric;
                    }
                }
                else
                {
                    string[] versionData = _versionNumber.Split('_');
                    versionNumeric = Convert.ToDouble(versionData[1].Split('.')[0]) + 1;
                    nextVersionNumber = _year.ToString() + UNDERSCORE + versionNumeric + MAJORVERSION;
                }
            }
            else
            {
                if (_year > 0)
                    nextVersionNumber = NULLSTRING + _year + UNDERSCORE + FIRSTMINORVERSION;
                else
                    nextVersionNumber = NULLSTRING + DateTime.Now.Year + UNDERSCORE + FIRSTMINORVERSION;
            }
            return nextVersionNumber;
        }

        private bool IsFormInstanceNameExist(PBPPlanConfigViewModel ViewModel)
        {
            bool IsExist = this._unitOfWorkAsync.RepositoryAsync<FormInstance>().Get()
                          .Where(s => s.Name.Equals(ViewModel.QID)
                                && s.IsActive.Equals(true)
                                && s.FolderVersionID.Equals(ViewModel.FolderVersionId)
                          ).Any();
            return IsExist;
        }

        private PBPPlanConfigViewModel GenerateFormInstance(PBPPlanConfigViewModel ViewModel, int MedicareFormDesignVersionID)
        {
            int NewFromInstanceId = 0;
            ServiceResult Result = new ServiceResult();
            bool DocumentNameExist = false;
            string DocName = string.Empty;
            DocumentNameExist = IsFormInstanceNameExist(ViewModel);
            if (DocumentNameExist)
            {
                DocName = string.Concat(ViewModel.QID, "_", ViewModel.PBPDataBaseName);
                Result = this._folderVersionService.CreateFormInstance(1, ViewModel.FolderVersionId, MedicareFormDesignVersionID, 0,
                                           false, DocName, ViewModel.CreatedBy);
            }
            else
            {
                DocName = ViewModel.QID;
                Result = this._folderVersionService.CreateFormInstance(1, ViewModel.FolderVersionId, MedicareFormDesignVersionID, 0,
                           false, DocName, ViewModel.CreatedBy);
            }
            if (Result.Result == ServiceResultStatus.Success)
            {
                NewFromInstanceId = Convert.ToInt32(Result.Items.FirstOrDefault().Messages.FirstOrDefault());
            }
            ViewModel.FormInstanceId = NewFromInstanceId;
            ViewModel.FormInstanceName = DocName;
            return ViewModel;
        }

        private bool IsQIDExist(PBPPlanConfigViewModel ViewModel)
        {
            bool IsExist = false;
            IsExist = this._unitOfWorkAsync.RepositoryAsync<PBPImportDetails>().Get()
                      .Where(s => s.FolderVersionId.Equals(ViewModel.FolderVersionId)
                      && s.FolderId.Equals(ViewModel.FolderId)
                      && s.QID.Equals(ViewModel.QID)
                      && s.IsActive.Equals(true)
                      ).Any();
            return IsExist;
        }

        private ServiceResult UpdateAccountMapPBPDetailTable(PBPPlanConfigViewModel ViewModel)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ViewModel.PlanName = ViewModel.PlanName == null ? "" : ViewModel.PlanName;
                ViewModel.PlanNumber = ViewModel.PlanNumber == null ? "" : ViewModel.PlanNumber;

                ViewModel.ebsPlanName = ViewModel.ebsPlanName == null ? "" : ViewModel.ebsPlanName;
                ViewModel.eBsPlanNumber = ViewModel.eBsPlanNumber == null ? "" : ViewModel.eBsPlanNumber;
                ViewModel.QID = ViewModel.QID == null ? "" : ViewModel.QID;

                SqlParameter PBPDatabase1Up = new SqlParameter("@PBPDatabase1Up", ViewModel.PBPDataBase1Up);
                SqlParameter PBPImportQueueID = new SqlParameter("@PBPImportQueueID", ViewModel.PBPImportQueueID);
                SqlParameter PlanName = new SqlParameter("@PlanName", ViewModel.PlanName);
                SqlParameter PlanNumber = new SqlParameter("@PlanNumber", ViewModel.PlanNumber);
                SqlParameter ebsPlanName = new SqlParameter("@ebsPlanName", ViewModel.ebsPlanName);
                SqlParameter ebsPlanNumber = new SqlParameter("@ebsPlanNumber", ViewModel.eBsPlanNumber);

                SqlParameter QID = new SqlParameter("@QID", ViewModel.QID);
                SqlParameter FolderId = new SqlParameter("@FolderId", ViewModel.FolderId);
                SqlParameter FolderVersionId = new SqlParameter("@FolderVersionId", ViewModel.FolderVersionId);
                SqlParameter FormInstanceID = new SqlParameter("@FormInstanceID", ViewModel.FormInstanceId);
                SqlParameter DocId = new SqlParameter("@DocId", ViewModel.DocumentId);
                SqlParameter Status = new SqlParameter("@Status", (int)ProcessStatusMasterCode.Complete);
                SqlParameter UserAction = new SqlParameter("@UserAction", ViewModel.UserAction);
                SqlParameter PlanYear = new SqlParameter("@PlanYear", ViewModel.Year);

                var log = this._unitOfWorkAsync.Repository<PBPImportDetails>()
                      .ExecuteSql("exec [dbo].[UpdatePBPImportTables] @PBPDatabase1Up,@PBPImportQueueID,@PlanName,@PlanNumber,@ebsPlanName,@ebsPlanNumber,@QID,@FolderId,@FolderVersionId,@FormInstanceID,@DocId,@Status,@UserAction,@PlanYear",
                      PBPDatabase1Up, PBPImportQueueID, PlanName, PlanNumber, ebsPlanName, ebsPlanNumber, QID,
                      FolderId, FolderVersionId, FormInstanceID, DocId, Status, UserAction, PlanYear
                      ).ToList().FirstOrDefault();
                result.Result = ServiceResultStatus.Success;

                FolderVersion folderVersionData = this._unitOfWorkAsync.RepositoryAsync<FolderVersion>()
                                        .Query().Include(c => c.Folder)
                                        .Filter(c => c.FolderVersionID == ViewModel.FolderVersionId)
                                        .Get().FirstOrDefault();
                folderVersionData.UpdatedBy = ViewModel.CreatedBy;
                folderVersionData.UpdatedDate = DateTime.Now;
                this._unitOfWorkAsync.RepositoryAsync<FolderVersion>().Update(folderVersionData);
            }
            catch (Exception ex)
            {
                _PBPImportActivityLogServices.AddPBPImportActivityLog(ViewModel.PBPImportQueueID, "AddPlanIneBS()", " Error for QID", null, ViewModel.QID, ex);
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }
    }
}

