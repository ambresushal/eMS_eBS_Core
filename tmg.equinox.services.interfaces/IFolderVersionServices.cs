using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.applicationservices.viewmodels.Report;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.applicationservices.viewmodels.WCReport;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFolderVersionServices
    {
        FormInstanceViewModel GetFormInstanceByAnchorInstanceIdandFormDesignversion(int formdesignVersionId, int formInstanceID);
        List<FolderVersionModel> GetVersionsByFolder(int folderId);
        FolderVersionModel GetVersionById(int folderVersionId);
        IList<FolderVersions> GetFolderVersionByFolderVersionId(int folderVersionID);
        IList<Documents> GetFolderVersionDocuments(int folderVersionID);
        int? GetMasterListFormDesignID(int formDesignVersionId);
        bool IsMasterListDesign(int formDesignID);
        IList<FolderVersions> GetFolderVersionByFolderId(int folderID);
        List<string> GetProductIDListByFolderVersion(int folderVersionId);
        ServiceResult CreateFormReference(int accountId, int folderId, int folderVersionId, int formInstanceId, int? consortiumId, int targetFormInstanceId, string userName);

        FolderVersionViewModel GetFolderVersion(int? CurrentUserId, string Username, int tenantId, int folderVersionId, int folderId);

        GridPagingResponse<ActivityLogModel> GetActivityLogData(int formInstanceId, GridPagingRequest gridPagingRequest);

        DataTable GetActivityLogData(int formInstanceId);
        FolderVersionViewModel GetLatestFolderVersion(int tenantId, int folderId);

        FolderVersionViewModel AddFolderVersion(int tenantId, DateTime folderEffectiveDate, string addedBy, int folderID,
                                                Nullable<int> workflowStateID, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, int? userId);

        List<FormInstanceViewModel> GetFormInstanceList(int tenantId, int folderVersionId, int folderId, int formDesignType = 0, int[] instanceIds = null);
        //List<FormInstanceViewModel> GetFormInstanceList(int tenantId, int folderVersionId, int formDesignType);
        List<FormInstanceViewModel> GetAnchorFormInstanceList(int tenantId, int folderVersionId);
        List<OpenDocumentViewModel> GetAncherFormInstanceList(int folderVersionId, string folderViewMode, int? userId);
        List<OpenDocumentViewModel> GetAncherFormInstanceList(int tenantId, int folderVersionId, string folderViewMode, int? userId);

        List<DocumentViewListViewModel> GetDocumentViewList(int tenantId, int anchorFormInstanceId);

        List<FormInstanceViewModel> GetUpdatedDocumentList(int tenantId, int folderVersionId);

        List<FormInstanceViewModel> getFormInstanceDataList(List<int> formInstanceIDs);

        int GetAdminFormInstanceID(int tenantId, int folderVersionId);

        ServiceResult BaseLineFolder(int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId, string addedBy,
            string versionNumber, string comments, Nullable<int> ConsortiumID, DateTime? effectiveDate,
            bool isRelease, bool isNotApproved, bool isNewVersion, bool isAsyncCall = true, List<DocumentFilterResult> filterDocumentResults = null);

        ServiceResult BaseLineFolderForCompareSync(int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId, string addedBy,
        string versionNumber, string comments, Nullable<int> ConsortiumID, DateTime? effectiveDate,
        bool isRelease, bool isNotApproved, bool isNewVersion, List<DocumentFilterResult> filterDocumentResults = null);

        bool IsFormInstanceExist(string formName);
        bool IsFoundationFolder(int formInstanceID);
        ServiceResult SaveFormInstanceData(int tenantId, int folderVersionId, int formInstanceId, string formInstanceData, string enteredBy);

        void UpdateReportingCenterDatabase(int formInstanceId, int? oldFormInstanceID, bool isAsyncCall = true);
        bool SaveFormInstanceDataCompressed(int formInstanceID, string formData);

        bool SaveFormInstanceDataCompressed(int formInstanceID, string formData, int folderVersionId, string userName);

        ServiceResult SaveMultipleFormInstancesData(int tenantId, int folderVersionId, List<FormInstanceViewModel> multipleFormInstances, List<JToken> formsData, string enteredBy, int userId);

        ServiceResult UpdateFormInstanceData(int formInstanceId, int objectInstanceID);

        List<FolderVersionHistoryViewModel> GetVersionHistory(int folderId, int tenantId, string versionType);

        List<FolderVersionHistoryViewModel> GetVersionHistoryML(int folderId, int tenantId, string versionType);

        /// <summary>
        /// Gets the form type list to create a new form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        List<FormTypeViewModel> GetFormTypeList(int tenantId, string folderType, DateTime effectiveDate, int folderId);

        /// <summary>
        /// Gets the form list to be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        List<FormInstanceViewModel> GetFormList(int tenantId, int folderVersionId);

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountType">Type of the account.</param>
        /// <returns></returns>
        IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, bool IsPortfolio, int accountId, int categoryId, bool isFoundation = false);
        IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, bool IsPortfolio, int accountId, int categoryId, int? roleID, bool isFoundation = false);
        IEnumerable<FolderVersionViewModel> GetAllFolderList(int tenantId);
        /// <summary>
        /// Saves the form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <param name="formDesignVersionId">The form design version identifier.</param>
        /// <param name="formInstanceId">The form instance identifier.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        /// <param name="formName">Name of the form.</param>
        /// <returns></returns>
        ServiceResult CreateFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formInstanceId, bool isCopy, string formName, string addedBy);

        FormInstanceViewModel GetFormInstance(int tenantId, int formInstanceID);

        /// <summary>
        /// Gets the form name.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        FormInstanceViewModel GetFormNameToCopy(int tenantId, int formInstanceID);

        string GetFormInstanceData(int tenantId, int formInstanceID);
        string GetFormInstanceDataCompressed(int tenantId, int formInstanceID);
        List<FormInstanceViewModel> GetMultipleFormInstancesData(int tenantId, List<int> forms);
        List<FormInstanceViewModel> GetFormInstancesList(int tenantId, List<int> formInstanceIDs);
        List<FormInstanceViewModel> GetSOTViewFormInstancesList(int tenantId, List<FolderViewModel> documentList);

        /// <summary>
        /// Gets the JSON object from DB from which the PlaceHolder values for the report are parsed.
        /// When the formDesignID is not passed it's the Main Document otherwise it would be admin or other related Document.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="formDesignID"></param>
        /// <returns></returns>
        // string GetFormInstanceReportData(int tenantId, int formInstanceID, int folderVersionID, ref int formDesignID);
        Dictionary<int, string> GetFormInstanceReportData(int tenantId, int formInstanceID, int folderVersionID, List<int> ApplicableDocumentIDsForReport, ref int CurrentFormDesignID, string ReportName);

        int GetSourceFormInstanceID(int formInstanceID, int formDesignVersionID, int folderVersionID, int sourceFormDesignID);

        int GetSourceFormDesignVersionId(int FormInstanceId);

        //Copy Form Instance With Data
        int CopyFormInstance(int tenantId, int folderVersionId, int formInstanceId, string formName, bool isNewVersion, bool isManualCopy, string addedBy, int anchorFormInstanceID = 0, bool isAsyncCall = true, List<DocumentFilterResult> filterDocumentResults = null);

        IEnumerable<FolderVersionViewModel> GetMajorFolderVersionList(int tenantId, int folderId, string ownerName, string versionNumber,
                                            DateTime effectiveDate);

        IEnumerable<FolderVersionViewModel> GetMinorFolderVersionList(int tenantId, int folderId, string ownerName, string userName, bool isBaseLine);

        FolderVersionViewModel GetCurrentFolderVersionML(string formDesignName);

        FolderVersionViewModel GetCurrentFolderVersion(int folderId, int folderVersionId = 0);

        ServiceResult DeleteFormInstance(int folderID, int tenantId, int folderVersionId, int formInstanceId, string updatedBy);

        /// <summary>
        /// This method returns true if document 
        /// is used as data source for other documents in the folder
        /// </summary>
        /// <param name="formDesignID"></param>
        /// <param name="formDesignVersionID"></param>
        bool IsDataSource(int formDesignID, int formDesignVersionID);

        ServiceResult DeleteFolderVersion(int tenantId, int folderId, int folderVersionId, string versionType, string userName);

        ServiceResult DeleteFolder(int tenantId, int folderId);

        void UpdateFormInstanceWithEffectiveFormDesignVersion(string userName, int effectiveFormDesignVersionId, int formInstanceId);

        IEnumerable<FolderVersionViewModel> GetVersionNumberForAccountRetroChanges(int tenantId, int folderId);

        /// <summary>
        /// This method returns true if state of FolderVersion is InProgress
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        bool IsAnyFolderVersionInProgress(int folderId, int tenantId);

        ServiceResult CreateNewMinorVersion(int tenantId, int folderId, int folderVersionId, string versionNumber,
                                            string comments, DateTime? effectiveDate, bool isRelease,
                                            int userId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string userName);

        IEnumerable<RetroChangeViewModel> GetImpactedFolderVersionList(int folderId, DateTime effectiveDate,
                                                                         int tenantId);

        ServiceResult FolderVersionRetroChanges(List<RetroChangeViewModel> retroChangesList,
                                                IEnumerable<FolderVersionViewModel> folderVersionList,
                                                DateTime retroEffectiveDate, Nullable<int> categoryID, string catID, string userName, int userId);

        ServiceResult ChangeFolderVersionStatus(int tenantId, int folderId, string userName);

        FolderVersionViewModel GetLatestMinorFolderVersion(int tenantId, int folderId);

        void UpdateRetroChangesWhenReleased(int tenantId, int folderVersionId, string userName, int folderID);

        ServiceResult RollbackFolderVersion(int tenantId, int rollbackFolderVersionId, int folderId, string rollbackFolderVersionNumber,
                                            FolderVersionViewModel inProgressFolderVersion, int userId, string userName);

        bool IsMasterListFolderVersionInProgress(int folderVersionId);

        ServiceResult CanRollbackFolderVersion(int tenantId, string rollbackFolderVersionNumber,
                                                 string inProgressMinorVersionNumber);

        ServiceResult IsValidRetroEffectiveDate(int folderID, int tenantID, DateTime retroEffectiveDate);

        ServiceResult UpdateFolderLockStatus(int? userId, int tenantId, int? folderId);

        ServiceResult ReleaseFolderLock(int? userId, int tenantId, int? folderId);

        ServiceResult OverrideFolderLock(int? userId, int tenantId, int? folderId);

        bool CheckFolderLockIsOverriden(int folderId, int? userId);

        FolderVersionViewModel GetFolderLockStatus(int tenantId, int? folderId, int? userId);

        bool UpdateWithEffectiveFormDesinVersionID(string userName, int tenantId, int folderVersionId);

        string GetAccountName(int? accountId);

        List<FolderVersionPropertiesViewModel> GetFolderVersionProperties(int folderVersionID);

        /// <summary>
        /// Checks whether a folder is Master List
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        bool IsMasterList(int folderID);
        ServiceResult UpdateFormInstanceData(int tenantId, int formInstanceId, string formInstanceData);
        bool HasFolderLockByCurrentUser(int? userId, int folderId);
        bool IsTeamManager(int? userId);
        ServiceResult SaveFormInstanceAvtivitylogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<ActivityLogModel> formInstanceActivityLogData);
        void UpdateFolderChange(int tenantId, string userName, int? folderId, int? folderVersionId);
        void UpdateDocumentName(int tenantId, int formInstanceID, string newDocName);
        void UpdateJSONProductId(string newProductId, int formInstanceID);
        List<CopyFromAuditTrailViewModel> GetCopyFromAuditTrailData(int folderVersionId);
        void SaveCopyFromAuditByVersion(int currentfolderVersionId, int folderVersionId, int folderId);

        IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, int accountId);

        IEnumerable<FolderVersionViewModel> GetAllFoldersList();

        GridPagingResponse<FolderVersionViewModel> GetAllFoldersList(GridPagingRequest gridPagingRequest);

        IEnumerable<FolderVersionViewModel> GetFolderVersionList(int tenantId, int folderId);

        ReferenceDocumentViewModel GetReferenceDocumentModel(int tenantId, int accountId, int folderVersionId, int folderId, int formInstanceId);

        List<int> GetFolderVersions(int folderId);
        bool UpdatePrefixesInJSON(int formInstanceID, string SEPYPFX, string DEDEPFX, string LTLTPFX, string benefitSetName, string isNewSEPY, string isNewLTLT, string isNewDEDE);
        //bool UpdatePDBCSection(int formInstanceID, string EBCLPFX, string BSBSPFX, int isNewEBCL);
        bool UpdatePDBCSection(int formInstanceID, string EBCLPFX, string BSBSPFX, int isNewEBCL, int isNewBSBS, bool? IsUsingNewBSBS);
        FolderVersionViewModel GetFolderVersion(int tenantId, string folderVersionNumber, int? folderId);
        FolderVersionViewModel GetFolderVersion(int folderVersionID);
        int GetFolderIdByFolderName(string Name);

        /// <summary>
        /// This method used in Product Migration
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="objectInstanceID"></param>
        /// <param name="formData"></param>
        /// <param name="tenantId"></param>
        /// <param name="enteredBy"></param>
        /// <returns></returns>
        ServiceResult SaveFacetFormInstanceDataMap(int formInstanceID, int objectInstanceID, string formData, int tenantId, string enteredBy, string compresseJsonData);

        bool GetFolderVersionType(int formInstanceID);

        string GetProductId(int formInstanceID);
        void UpdateProductJsonHash(int tenantId, int formInstanceID, string hash);
        string GetProductJsonHash(int tenantId, int FormInstance, string productId);
        string ComputeProductHash(string productJSON);
        bool IsActivityPerformed(int formInstanceID);

        FolderVersionViewModel GetFolderVersionById(int folderVersionID);
        bool IsFolderVersionReleased(int folderVersionID);
        bool IsFolderVersionBaselined(int folderVersionID);
        string SetDefaultCreateNewPrefix(string newFormData);
        List<FormInstanceViewModel> getFormInstancDataList(List<int> formInstanceIDs);
        int GetFolderVersionByFormInstance(int formInstanceId);
        FolderVersionViewModel GetFolderLockStatusForSync(int tenantId, int? folderId, int? userId);
        #region FolderVersionCategory

        /// <summary>
        /// Gets the list of FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        GridPagingResponse<FolderVersionCategoryViewModel> GetFolderVersionCategoryList(int tenantID, GridPagingRequest gridPagingRequest);

        /// <summary>
        /// Adds the FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the FolderVersionCategory.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">FolderVersionCategory Name already exists</exception>
        ServiceResult AddFolderVersionCategory(int tenantID, string folderVersionCategoryName, int folderVersionGroupID, string addedBy);

        /// <summary>
        /// Updates the FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryID">folderVersionCategory identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the folderVersionCategory.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// FolderVersionCategory Name Alraedy exists
        /// or
        /// FolderVersionCategory Does Not exists
        /// </exception>
        ServiceResult UpdateFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName, int folderVersionGroupID, string updatedBy);

        /// <summary>
        /// Delete the FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryID">folderVersionCategory identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the folderVersionCategory.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// FolderVersionCategory Does Not exists
        /// </exception>
        ServiceResult DeleteFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName, string updatedBy);


        /// <summary>
        /// Gets the list of FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<FolderVersionCategoryViewModel> GetFolderVersionCategoryForDropdown(int tenantID, bool? isPortfolio, int folderVersionID, bool? isFinalized);

        /// <summary>
        /// Gets details of a FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        //FolderVersionCategoryViewModel GetFolderVersionCategory(int folderVersionID);
        #endregion FolderVersionCategory

        #region FolderVersionGroup

        /// <summary>
        /// Gets the list of FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        GridPagingResponse<FolderVersionGroupViewModel> GetFolderVersionGroupList(int tenantID, GridPagingRequest gridPagingRequest);

        /// <summary>
        /// Adds the FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the FolderVersionCategory.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">FolderVersionCategory Name already exists</exception>
        ServiceResult AddFolderVersionGroup(int tenantID, string folderVersionGroupName, string addedBy);

        /// <summary>
        /// Updates the FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryID">folderVersionCategory identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the folderVersionCategory.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// FolderVersionCategory Name Alraedy exists
        /// or
        /// FolderVersionCategory Does Not exists
        /// </exception>
        ServiceResult UpdateFolderVersionGroup(int tenantID, int folderVersionGroupID, string folderVersionGroupName, string updatedBy);

        /// <summary>
        /// Delete the FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryID">folderVersionCategory identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the folderVersionCategory.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// FolderVersionCategory Does Not exists
        /// </exception>
        ServiceResult DeleteFolderVersionGroup(int folderVersionGroupID, string folderVersionGroupName, string updatedBy);

        /// <summary>
        /// Gets the list of FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<FolderVersionGroupViewModel> GetFolderVersionGroupForDropdown(int tenantID);

        #endregion FolderVersionGroup

        string applDesignRule(viewmodels.FormDesignVersion.FormDesignVersionDetail detail);

        #region Product
        List<FormInstanceViewModel> GetProductById(int formInstanceID);
        List<FormInstanceViewModel> GetProductByProductId(string productId);
        List<FormInstanceViewModel> GetProductDataByElementNames(int formInstanceId, string elementList, string elementType);
        #endregion Product

        MasterListDocuments GetMasterListDocuments();

        FolderVersionViewModel LoadFolderVersionViewModel(int folderId, int folderVersionId);

        ServiceResult ReleaseMLVersion(int tenantId, int folderId, int folderVersionId, string CurrentUserName, string versionNumber, string commentText);

        /// <summary>
        /// check if the user has permission to create the account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool GetUserFolderVersiontCreationPermission(int? userId, bool isPortfolioSearch);

        List<FormInstanceViewModel> GetProductList(int tenantId, int folderVersionId);

        Dictionary<int, ProductState> GetProductStateList(int folderVersionId, string folderType);

        Dictionary<string, int?> getAnchorDetails(int viewForminstanceID);

        List<FormInstanceViewModel> GetChildElementsOfFormInstance(int tenantId, int formInstanceID);

        DateTime GetFolderVersionEffectiveDate(int folderVersionID);
        DateTime GetFolderVersionEffectiveDate(int tenantId, int folderVersionID);
        ServiceResult DeleteNonPortfolioBasedFolder(int tenantId, int folderID);
        /// <summary>
        /// Get values of Platform and PlanDesign fields for provided forminstance
        /// </summary>
        /// <param name="forminstanceId"></param>
        /// <returns></returns>
        Dictionary<int, string> GetPltformPlnDesign(int forminstanceId);

        IQueryable<FolderViewModel> GetFolderList(int tenantId, int? userId);

        IQueryable<FolderVersionViewModel> GetFolderVersionsList(int tenantId, int? userId, int folderId);

        List<FormInstanceViewModel> GetFormInstanceListForFolderVersion(int tenantId, int folderVersionId, int folderId, int formDesignType = 0);

        int GetMasterListFormInstanceId(int formInstanceID, int sourceFormDesignID, int folderVersionID);

        List<FormInstanceViewModel> GetFormInstanceListForAnchor(int tenantId, int folderVersionId, int anchorId, int formDesignTypeId = 0);

        int AddChildFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formDesignId, string formName, string addedBy, int anchorFormInstanceID = 0);

        List<FormInstanceViewModel> GetAncillaryProductList(int folderVersionId, string sectionName);

        bool IsDataForFormInstanceChanged(int formInstanceId);
        List<ReportQueueDetailsViewModel> UpdateReportQueueFolderDetails(List<ReportQueueDetailsViewModel> reportQueueDetails);
        int GetFormDesignIDByFormName(string FormName);
    }
}
