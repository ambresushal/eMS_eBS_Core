using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdateViewModels;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IGlobalUpdateService
    {
        List<FormDesignVersionRowModel> GetFormVersions(DateTime effectiveDateFrom, DateTime effectiveDateTo);
        IEnumerable<DocumentVersionUIElementRowModel> GetUIElementListForGuFormDesignVersion(int tenantId, int formDesignVersionId, int globalUpdateId, bool isOnlySelectedElements);
        GridPagingResponse<GlobalUpdateViewModel> GetExistingGlobalUpdatesList(int tenantId,GridPagingRequest gridPagingRequest);
        List<IASWizardStepViewModel> GetIASWizardList(int tenantId);
        ServiceResult SaveGlobalUpdate(int tenantId, int globalUpdateId, string GlobalUpdateName, DateTime EffectiveDateFrom, DateTime EffectiveDateTo, string addedBy);
        ServiceResult SaveFormDesignVersionUIElements(int tenantId, int formDesignId, int formDesignVersionId, int globalUpdateId, List<int> selectedUIElementList, string addedBy);
        List<FormDesignVersionRowModel> GetUpdatedDocumentDesignVersion(int globalUpdatedId);
        //IEnumerable<DocumentVersionUIElementRowModel> GetSelectedDocumentDesignVersion(int ,int globalUpdatedId, int formDesignVersionId);
        IEnumerable<IASFolderDataModel> GetGlobalUpdateImpactedFolderVersionList(int GlobalUpdateID, DateTime effectiveDateFrom, DateTime effectiveDateTo, int tenantId);
        List<IASFolderDataModel> GetGlobalUpdatesFolderDataList(int GlobalUpdateID);
        ServiceResult SaveIASFolderDataValues(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy);
        List<GlobalUpdateViewModel> GetSelectedRowGlobalUpdateData(int? GlobalUpdateId);
        List<FormDesignElementValueVeiwModel> GetFormDesignVersionUIElements(int GlobalUpdateID);
        ServiceResult SaveIASElementExportDataValues(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy);
        List<FormDesignElementValueVeiwModel> GetSelectedUIElementsList(int globalUpdateId, int formDesignVersionId);
        ServiceResult AddIASTemplate(IASFileUploadViewModel viewModel);
        ServiceResult UpdateValue(int tenantId, string userName, string elementHeader, int globalUpdateId, int formDesignVersionId, int uiElementId, int uiElementDataTypeId, bool modifyRules, IEnumerable<GuRuleRowModel> rules, bool IsPropertyGridModified);
        IEnumerable<GuRuleRowModel> GetRulesForUIElement(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId);
        List<BatchViewModel> GetExistingBatchesList(int tenantId);
        List<BatchExecutionViewModel> GetExecutedBatchesList(int tenantId, int rollBackHrs);
        List<IASElementImportViewModel> listImportedNotAddedIAS();
        List<BatchIASMapViewModel> getGUIdsFromBatchMap(Guid batchId);
        List<IASElementImportViewModel> editBatchIASListGrid(Guid batchID);
        List<IASElementImportViewModel> viewBatchIASListGrid(Guid batchID);
        ElementHeaderViewModel ConfirmedUpdateValueNotification(int uiElmentId, int globalUpdateId, int formDesignVersionId);
        ServiceResult SaveBatch(string batchName, string executionType, DateTime? scheduleDate, TimeSpan? scheduledTime, DateTime addedDate, string addedBy, List<int> globalUpdateIDArray, int thresholdLimit);
        IEnumerable<DocumentVersionUIElementRowModel> GetUpdateSectionUIElements(int tenantId, int formDesignVersionId);
        ServiceResult ApproveBatch(string batchName, string currentUser);
        ServiceResult UpdateBatch(Guid batchId, string batchName, string executionType, DateTime? scheduleDate, TimeSpan? scheduledTime, DateTime updatedDate, string updatedBy, List<int> globalUpdateIDArray, int thresholdLimit);
        ServiceResult GlobalUpdateBaseLineFolder(Guid BatchID, int GlobalUpdateID, int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId, string addedBy, string versionNumber, string comments, DateTime? effectiveDate,
          bool isRelease, out int newFldrVrsionId, bool isNotApproved, bool isNewVersion);
        ServiceResult GenerateAuditReport(Guid BatchId);
        void DeleteGlobalUpdateData(int globalUpdateId);     
        List<GlobalUpateExecutionLogViewModel> GlobalUpdateBaseLineFolderList(Guid batchId, List<BaselineDataViewModel> baselineData, int userID, string userName);
        List<GlobalUpateExecutionLogViewModel> getFolderVersionsBaselined(Guid batchId, int userId, string userName);
        ServiceResult ExecuteBatch(Guid BatchID, string BatchName, int tenantId, string userName, int userId);
        ServiceResult ScheduleGlobalUpdate(int GlobalUpdateID, bool flag, string addedBy);
        bool IsValidIASUpload(int GlobalUpdateID);
        ServiceResult ScheduleIASUpload(int GlobalUpdateID, bool flag, string addedBy);
        ServiceResult rollBackBatch(Guid batchID, string rollbackComment);
        List<GlobalUpdateComputedStatus> GetLatestGlobalUpdateStatus(List<int> globalUpdateIds);
        ServiceResult DeleteBatch(Guid batchId, string deletedBy);
        ServiceResult CheckDuplicateFolderVersionExistsInSelectedBatchIAS(List<int> guIdList);
    }

}
