using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.applicationservices.viewmodels;
using System.Data;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPBPImportService
    {
       
        #region PBPImport Services
        GridPagingResponse<PBPImportQueueViewModel> GetPBPImportQueueList(int tenantID, GridPagingRequest gridPagingRequest);
        ServiceResult AddPBPImportQueue(int tenantId, PBPImportQueueViewModel pBPImportQueueViewModel, out int PBPImportQueueID);
        List<PBPTableViewModel> GetPlanDetailsFromAccessDataBase(string fileName, int PBPImportQueueID);
        PlanConfigurationViewModel ProcessPlanConfiguration(List<PBPTableViewModel> PBPTableData, string CreateBy, int PBPImportQueueID, int PBPDataBase1Up);
        ServiceResult SavePBPPlanDetails(int ImportID, string createdBy);
        ServiceResult ValidateQID(string fileName1, string fileName2);
        ServiceResult ValidateFileScheme(string pbpFileName, string pBPAreaPlanFileName);
        PlanConfigurationViewModel CreateMatchAndMisMatchPlanLists(int PBPImportQueueID);
        ServiceResult UpdatePlanConfig(int tenantId, IList<PBPMatchConfigViewModel> PBPMatchConfigList, string UpdateBy);
        ServiceResult UpdateMatchPlanConfig(IList<PBPMatchConfigViewModel> PBPMatchConfigList, string UpdateBy);
        DataTable GetExportQueueDataTable();
        List<PBPImportQueueViewModel> GetQueuedPBPImportList();
        PBPImportQueueViewModel GetQueuedOrProcessingPBPImport();
        List<PBPPlanConfigViewModel> GetPBPPlanDetailsForProcess(int pBPImportQueueID);
        ServiceResult UpdateImportQueueStatus(int PBPImportQueueID, domain.entities.Enums.ProcessStatusMasterCode status);
        //void UpdateJobId(int PBPImportQueueID, int JobId,string JobLocation);
        void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync);
        ServiceResult UpdateImportQueue(int PBPImportQueueID, PBPImportQueueViewModel model);
        ServiceResult UpdatePBPDetailAfterPerformUserAction(PBPPlanConfigViewModel ViewModel);
        ServiceResult DiscardImportChanges(int pbpImportId, string UpdateBy);
        DataTable GetExportPreviewGridDataTable(int pbpImportId, int PlanType);
        List<PBPImportQueueViewModel> GetPBPImportQueueListForEmailNotification();
        ServiceResult UpdatePBPPlanDetails(PBPPlanConfigViewModel ViewModel, int userAction);
        ServiceResult UpdateAccountProductMap(PBPPlanConfigViewModel ViewModel);
        void SendPBPImportEmail();
        int GetContractYear(string fileName1);
        #endregion

        #region PBPDatabase Services
        GridPagingResponse<PBPDatabaseViewModel> GetPBPDatabaseList(int tenantID, GridPagingRequest gridPagingRequest);
        ServiceResult AddPBPDatabase(AddPBPDBNameViewModel list, string addedBy);
        List<PBPDatabaseViewModel> GetPBPDatabaseNameList(int tenantID);
        ServiceResult UpdatePBPDatabase(UpdatePBPDBNameViewModel update, string updatedby);
        #endregion

        #region ProcessStatusMaster Services

        #endregion

        #region PBPUserAction Services
        IList<UserActionViewModel> GetUserActionList();
        #endregion

        #region PBPImportDetails Services

        #endregion

        #region PBPMatchConfig Services

        #endregion

        IList<PBPPlanConfigViewModel> GetPBPPlanDetailsList();
        //#region PBPImportOperation
        //IEnumerable<PBPImportTablesViewModel> GetPBPImportTablesList();
        ////void InitImportOperation();
        //void AddPBPImportActivityLog(int queueID, int batchID, string fileName, string tableName, string message, string username);
        //void PerformImportOperationWithSequence(int batchId, IEnumerable<PBPImportTablesViewModel> collPBPImportTablesList, string username);
        //void StartPBPImportOperation(IFolderVersionServices _folderVersionService, ILoggingService _loggingService, IDomainModelService _domainModelService);


        //#endregion

    }
}
