using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.infrastructure.util;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using DocumentDesignTypes = tmg.equinox.domain.entities.Enums.DocumentDesignTypes;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.applicationservices.viewmodels.Settings;
using Newtonsoft.Json;
using tmg.equinox.applicationservices.viewmodels.EmailNotitication;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels.Report;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.config;
using System.Data;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.applicationservices.viewmodels.WCReport;

namespace tmg.equinox.applicationservices
{
    public partial class FolderVersionServices : IFolderVersionServices
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private CommentsData _commentsData { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IAutoSaveSettingsService _autoSaveSettingsService { get; set; }
        private IWorkFlowCategoryMappingService _workFlowCategoryMappingService { get; set; }
        private string _newProductIdSectionPath = string.Empty;
        private string _productIdFieldName = string.Empty;
        private string _isProductNewFieldName = string.Empty;
        private string _generateNewProductIdFieldName = string.Empty;
        private string _generalInformationSectionPath = string.Empty;
        private string _productNotesFieldName = string.Empty;
        private string _productNotesTitleFieldName = string.Empty;
        private string _benefitSet = string.Empty;
        private string _benefitSetName = string.Empty;
        private string _DEDEPfx = string.Empty;
        private string _LTLTPfx = string.Empty;
        private string _SEPYPfx = string.Empty;
        private string _CreateNewSEPY = string.Empty;
        private string _PDBCComponents = string.Empty;
        private string _PDBCType = string.Empty;
        private string _PDBCPrefix = string.Empty;
        private string _ProductReferenceFormInstanceID = string.Empty;
        private string _networkSectionPath = string.Empty;
        private string _createNewPrefixName = string.Empty;
        private string _isNewPrefixName = string.Empty;
        private static readonly ILog _logger = LogProvider.For<FolderVersionServices>();
        private JToken _formData { get; set; }
        private FormDesignVersionDetail _formDesignData { get; set; }
        private IReportingDataService _reportingDataService;
        private IResourceLockService _resourceLockService;
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        private IMDMSyncDataService _mDMSyncDataService;
        #endregion Private Members

        #region Public Properties

        // public string NewFolderVersionId { get; set; }

        #endregion Public Properties

        #region Constructor
        public FolderVersionServices(IUnitOfWorkAsync unitOfWork, IWorkFlowCategoryMappingService workFlowCategoryMappingService, IReportingDataService reportingDataService, IFormDesignService formDesignService = null, IAutoSaveSettingsService autoSaveSettingsService = null, IResourceLockService resourceLockService = null, IPlanTaskUserMappingService planTaskUserMappingService = null, IMDMSyncDataService mDMSyncDataService = null)
        {
            this._unitOfWork = unitOfWork;
            _commentsData = new CommentsData();
            this._formDesignService = formDesignService;
            this._autoSaveSettingsService = autoSaveSettingsService;
            this._newProductIdSectionPath = "ProductDefinition.FacetsProductInformation.NewProductID";
            this._productIdFieldName = "ProductID";
            this._isProductNewFieldName = "IsProductNew";
            this._generateNewProductIdFieldName = "GenerateNewProductID";
            this._generalInformationSectionPath = "ProductDefinition.GeneralInformation";
            this._productNotesFieldName = "Notes";
            this._productNotesTitleFieldName = "NotesTitle";
            this._benefitSet = "BenefitSetNetwork.NetworkList";
            this._benefitSetName = "BenefitSetName";
            this._SEPYPfx = "SEPYPFX";
            this._LTLTPfx = "LTLTPFX";
            this._DEDEPfx = "DEDEPFX";
            this._PDBCComponents = "ProductDefinition.FacetsProductInformation.FacetProductComponentsPDBC.PDBCPrefixList";
            this._PDBCType = "PDBCType";
            this._PDBCPrefix = "PDBCPrefix";
            this._CreateNewSEPY = "CreateNewSEPYPFX";
            this._ProductReferenceFormInstanceID = "Reference.FormInstanceID";
            this._networkSectionPath = "BenefitSetNetwork";
            this._createNewPrefixName = "CreateNewPrefix";
            this._isNewPrefixName = "IsPrefixNew";
            this._workFlowCategoryMappingService = workFlowCategoryMappingService;
            _reportingDataService = reportingDataService;
            _resourceLockService = resourceLockService;
            _planTaskUserMappingService = planTaskUserMappingService;
            _mDMSyncDataService = mDMSyncDataService;
        }
        #endregion Constructor

        #region Public Methods
        public int? GetMasterListFormDesignID(int FolderVersionID)
        {
            dynamic folderVersionList = null;
            try
            {
                folderVersionList = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderVersionID == FolderVersionID).Get()
                                     join f in this._unitOfWork.RepositoryAsync<Folder>().Query().Get() on fv.FolderID equals f.FolderID
                                     where fv.FolderVersionID == FolderVersionID
                                     select f.MasterListFormDesignID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderVersionList;
        }

        public FolderVersionModel GetVersionById(int folderVersionId)
        {
            FolderVersionModel folderVersion = null;
            try
            {
                folderVersion = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                 where fv.FolderVersionID == folderVersionId
                                 select new FolderVersionModel
                                 {
                                     FolderVersionID = fv.FolderVersionID,
                                     EffectiveDate = fv.EffectiveDate,
                                     VersionNumber = fv.FolderVersionNumber,
                                     WorkFlowStatus = fv.WorkFlowVersionState.WorkFlowState.WFStateName,
                                     Status = fv.FolderVersionState.FolderVersionStateName,
                                     Comments = fv.Comments
                                 }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }

            return folderVersion;
        }

        public List<FolderVersionModel> GetVersionsByFolder(int folderId)
        {
            List<FolderVersionModel> folderVersions = null;
            try
            {
                folderVersions = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  join f in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals f.FolderID
                                  where fv.FolderID == folderId
                                  select new FolderVersionModel
                                  {
                                      FolderVersionID = fv.FolderVersionID,
                                      EffectiveDate = fv.EffectiveDate,
                                      VersionNumber = fv.FolderVersionNumber,
                                      WorkFlowStatus = fv.WorkFlowVersionState.WorkFlowState.WFStateName,
                                      Status = fv.FolderVersionState.FolderVersionStateName,
                                      Comments = fv.Comments
                                  }).OrderBy(c => c.FolderVersionID).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }

            return folderVersions;
        }

        /// <summary>
        /// Gets the folder Name form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="IsPortfolio"></param>
        /// <returns></returns>
        public IEnumerable<FolderVersionViewModel> GetAllFolderList(int tenantId)
        {
            IList<FolderVersionViewModel> folderList = null;
            try
            {
                folderList = (from fld in this._unitOfWork.Repository<Folder>().Get()
                              join fI in this._unitOfWork.Repository<AccountFolderMap>().Get()
                                        on fld.FolderID equals fI.FolderID
                              join ac in this._unitOfWork.Repository<Account>().Get()
                                 on fI.AccountID equals ac.AccountID
                              where (fld.TenantID == tenantId && ac.IsActive == true)
                              select new FolderVersionViewModel
                              {
                                  AccountId = ac.AccountID,
                                  FolderId = fld.FolderID,
                                  FolderName = fld.Name,
                                  AccountName = ac.AccountName,
                                  PrimaryContact = fld.PrimaryContent,
                              }).OrderBy(ord => ord.FolderName).ToList();

                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;
        }

        public List<ReportQueueDetailsViewModel> UpdateReportQueueFolderDetails(List<ReportQueueDetailsViewModel> reportQueueDetails)
        {
            List<ReportQueueDetailsViewModel> result = (from rd in reportQueueDetails
                                                        join f in _unitOfWork.RepositoryAsync<Folder>().Get() on rd.FolderId equals f.FolderID
                                                        join fv in _unitOfWork.RepositoryAsync<FolderVersion>().Get() on rd.FolderVersionId equals fv.FolderVersionID
                                                        select new ReportQueueDetailsViewModel
                                                        {
                                                            ReportQueueId = rd.ReportQueueId,
                                                            FolderId = rd.FolderId,
                                                            FolderName = f.Name,
                                                            FolderVersionId = rd.FolderVersionId,
                                                            FolderVersionNumber = fv.FolderVersionNumber
                                                        }).ToList();

            return result;
        }
        public void UpdateFolderChange(int tenantId, string userName, int? folderId, int? folderVersionId)
        {
            try
            {
                Folder folderToUpdate = null;
                FolderVersion folderVersionToUpdate = null;
                if (folderId.HasValue == false)
                {
                    folderId = this._unitOfWork.Repository<FolderVersion>().Query().Filter(f => f.FolderVersionID == folderVersionId.Value).Get().FirstOrDefault().FolderID;
                }
                folderToUpdate = this._unitOfWork.RepositoryAsync<Folder>()
                                                  .Query()
                                                  .Filter(c => c.FolderID == folderId && c.TenantID == tenantId)
                                                  .Get()
                                                  .FirstOrDefault();
                if (folderToUpdate != null)
                {
                    folderToUpdate.UpdatedBy = userName;
                    folderToUpdate.UpdatedDate = DateTime.Now;
                    this._unitOfWork.Repository<Folder>().Update(folderToUpdate);

                    // Update folderVersion UpdatedBy/UpdatedDate
                    folderVersionToUpdate = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                        .Query()
                                        .Filter(c => c.FolderVersionID == folderVersionId.Value && c.TenantID == tenantId)
                                        .Get()
                                        .FirstOrDefault();
                    if (folderVersionToUpdate != null)
                    {
                        folderVersionToUpdate.UpdatedBy = userName;
                        folderVersionToUpdate.UpdatedDate = DateTime.Now;
                        this._unitOfWork.Repository<FolderVersion>().Update(folderVersionToUpdate);
                    }
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        public void UpdateFormInstanceDocID(int newFormInstanceID, string formName, int tenantId)
        {
            try
            {
                //update docid against forminsance
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@FormInstanceID", System.Data.SqlDbType.Int);
                param[0].Value = newFormInstanceID;
                param[1] = new SqlParameter("@ProductName", System.Data.SqlDbType.VarChar, 200);
                param[1].Value = formName;

                FormInstance formInstance = this._unitOfWork.Repository<FormInstance>().ExecuteSql("exec [dbo].[UpdateFormInstanceDocID] @FormInstanceID, @ProductName", param).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }
        /// <summary>
        /// Get anchor forminstanceID and formDesignVersionID
        /// </summary>
        /// <param name="anchorForminstanceId"></param>
        /// <returns></returns>
        public Dictionary<string, int?> getAnchorDetails(int viewForminstanceID)
        {
            Dictionary<string, int?> dictionary = new Dictionary<string, int?>();

            FormInstance anchorForminstanceDetails = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                      .Query()
                                                      .Filter(c => c.FormInstanceID == viewForminstanceID)
                                                      .Get()
                                                      .FirstOrDefault();

            int? anchorFormInstanceId = anchorForminstanceDetails.AnchorDocumentID;

            FormInstance forminstanceDetails = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                      .Query()
                                                      .Filter(c => c.FormInstanceID == anchorFormInstanceId)
                                                      .Get()
                                                      .FirstOrDefault();

            int anchorFormDesignVersionID = forminstanceDetails.FormDesignVersionID;

            dictionary.Add("anchorFormDesignVersionID", anchorFormDesignVersionID);
            dictionary.Add("anchorFormInstanceId", anchorFormInstanceId);

            return dictionary;
        }


        public IEnumerable<FolderVersionViewModel> GetFolderVersionList(int tenantId, int folderId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<FolderVersionViewModel> folderVersionList = null;
            try
            {
                folderVersionList = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                                        && c.FolderID == folderId)
                                              .Include(c => c.Folder)
                                              .Get()
                                     select new FolderVersionViewModel
                                     {

                                         FolderVersionId = c.FolderVersionID,
                                         FolderVersionNumber = c.FolderVersionNumber,
                                         IsPortfolio = c.Folder.IsPortfolio,
                                         FolderVersionStateID = c.FolderVersionStateID
                                     }).ToList();


                if (folderVersionList.Count() == 0)
                    folderVersionList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderVersionList;
        }

        public FolderVersionViewModel GetFolderVersion(int tenantId, string folderVersionNumber, int? folderId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            FolderVersionViewModel folderVersion = null;
            try
            {
                folderVersion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                                        && c.FolderVersionNumber == folderVersionNumber && c.FolderID == folderId)
                                              .Include(c => c.Folder)
                                              .Get()
                                 select new FolderVersionViewModel
                                 {
                                     FolderVersionId = c.FolderVersionID,
                                     FolderVersionNumber = c.FolderVersionNumber,
                                     FolderName = c.Folder.Name,
                                     WFStateID = (int)c.WFStateID,
                                     UpdatedBy = c.UpdatedBy,
                                     AddedBy = c.AddedBy
                                 }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderVersion;
        }

        public FolderVersionViewModel GetFolderVersion(int? CurrentUserId, string userName, int tenantId, int folderVersionId, int folderId)
        {
            FolderVersionViewModel folderVersionViewModel = null;
            AutoSaveSettingsViewModel autoSaveSettings = null;

            try
            {
                folderVersionViewModel = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                                        && c.FolderID == folderId && c.FolderVersionID == folderVersionId)
                                              .Include(c => c.Folder)
                                              .Include(c => c.WorkFlowVersionState)
                                              .Get()
                                          select new FolderVersionViewModel
                                          {
                                              FolderId = c.FolderID,
                                              FolderVersionId = c.FolderVersionID,
                                              TenantID = c.TenantID,
                                              FolderName = c.Folder.Name,
                                              EffectiveDate = c.EffectiveDate,
                                              FolderVersionNumber = c.FolderVersionNumber,
                                              WFStateID = c.WFStateID,
                                              IsActive = c.IsActive,
                                              AccountId = c.Folder.AccountFolderMaps.Select(d => d.AccountID).FirstOrDefault(),
                                              IsPortfolio = c.Folder.IsPortfolio,
                                              VersionTypeID = c.VersionTypeID,
                                              FolderVersionStateID = c.FolderVersionStateID,
                                              CategoryID = c.CategoryID,
                                              CatID = c.CatID,
                                              WFStateName = c.WorkFlowVersionState.WorkFlowState.WFStateName
                                          }).FirstOrDefault();
                autoSaveSettings = this._autoSaveSettingsService.GetAutoSaveDuration(tenantId);

                if (folderVersionViewModel != null)
                {
                    folderVersionViewModel.IsAutoSaveEnabled = autoSaveSettings.IsAutoSaveEnabled;
                    folderVersionViewModel.Duration = autoSaveSettings.Duration;
                    folderVersionViewModel.AutoSaveSettingsProperties = JsonConvert.SerializeObject(autoSaveSettings);
                    //   bool isMajorVersionLoaded = UpdateWithEffectiveFormDesinVersionID(userName, tenantId, folderVersionId);
                    //   folderVersionViewModel.IsNewLoadedVersionIsMajorVersion = isMajorVersionLoaded;
                    folderVersionViewModel.CategoryName = GetFolderVersionCategory(folderVersionViewModel.CategoryID);
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }


            return folderVersionViewModel;
        }

        public FolderVersionViewModel AddFolderVersion(int tenantId, DateTime folderEffectiveDate, string addedBy, int folderID,
                                               Nullable<int> workflowStateID, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, int? userId)
        {
            FolderVersion addVersion = null;
            try
            {
                var builder = new VersionNumberBuilder();

                var newVersionNumber = builder.GetNextMinorVersionNumber(null, folderEffectiveDate);

                addVersion = new FolderVersion();
                addVersion.FolderID = folderID;
                addVersion.EffectiveDate = folderEffectiveDate;
                addVersion.WFStateID = workflowStateID;
                addVersion.VersionTypeID = Convert.ToInt32(VersionType.New);
                addVersion.IsActive = true;
                addVersion.FolderVersionNumber = newVersionNumber;
                addVersion.FolderVersionStateID = Convert.ToInt32(FolderVersionState.INPROGRESS);
                addVersion.Comments = String.Format(CommentsData.InitialVersionComments, addedBy, newVersionNumber);
                addVersion.TenantID = tenantId;
                addVersion.ConsortiumID = consortiumID;
                addVersion.CategoryID = categoryID;
                addVersion.CatID = catID;
                addVersion.AddedBy = addedBy;
                addVersion.AddedDate = DateTime.Now;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FolderVersion>().Insert(addVersion);
                this._unitOfWork.Save();

                this.UpdateFolderChange(tenantId, addedBy, folderID, addVersion.FolderVersionID);

                var notApprovedApprovalStatus = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                                    .GetNotApprovedApprovalStatus(tenantId);

                if (notApprovedApprovalStatus != null && workflowStateID != null)
                {
                    var addFolderWorkflow = new FolderVersionWorkFlowState();
                    addFolderWorkflow.TenantID = tenantId;
                    addFolderWorkflow.IsActive = true;
                    addFolderWorkflow.AddedDate = DateTime.Now;
                    addFolderWorkflow.AddedBy = addedBy;
                    addFolderWorkflow.FolderVersionID = addVersion.FolderVersionID;
                    addFolderWorkflow.WFStateID = (int)workflowStateID;
                    addFolderWorkflow.ApprovalStatusID = Convert.ToInt32(notApprovedApprovalStatus.WorkFlowStateApprovalTypeID);
                    addFolderWorkflow.UserID = userId;
                    //Call to repository method to insert record.
                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(addFolderWorkflow);
                    this._unitOfWork.Save();
                }
                int FolderversionId = addVersion.FolderVersionID;
                if (_planTaskUserMappingService != null)
                    _planTaskUserMappingService.SavetaskPlanNewFolderVersion(FolderversionId, addedBy);



            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            if (addVersion != null)
                return new FolderVersionViewModel
                {
                    FolderId = addVersion.FolderID,
                    FolderVersionId = addVersion.FolderVersionID,
                    TenantID = addVersion.TenantID
                };
            else
                return null;
        }

        public List<FormInstanceViewModel> GetFormInstanceList(int tenantId, int folderVersionId, int folderId, int formDesignTypeId = 0, int[] instanceIds = null)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && (formDesignTypeId == 0 || c.FormDesign.DocumentDesignTypeID == formDesignTypeId) && c.IsActive == true)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         FormInstanceName = c.Name,
                                         UsesMasterListAliasDesign = c.FormDesign.UsesAliasDesignMasterList,
                                         AnchorDocumentID = c.AnchorDocumentID,
                                         DocID = c.DocID
                                     }).ToList();

                if (formInstances != null)
                {
                    if (instanceIds == null)
                        formInstanceList = formInstances.ToList();
                    else
                        formInstanceList = formInstances.Where(x => instanceIds.Contains(x.FormInstanceID)).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }


        public List<FormInstanceViewModel> GetFormInstanceListForAnchor(int tenantId, int folderVersionId, int anchorId, int formDesignTypeId = 0)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Include(c => c.FolderVersion)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.AnchorDocumentID == anchorId && (formDesignTypeId == 0 || c.FormDesign.DocumentDesignTypeID == formDesignTypeId) && c.IsActive == true)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FolderID = c.FolderVersion.FolderID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = c.FormDesign.FormName,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         FormInstanceName = c.Name,
                                         UsesMasterListAliasDesign = c.FormDesign.UsesAliasDesignMasterList,
                                         AnchorDocumentID = c.AnchorDocumentID,
                                         DocID = c.DocID,
                                         EffectiveDate = c.FolderVersion.EffectiveDate
                                     }).ToList();

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }


        public List<FormInstanceViewModel> GetSOTViewFormInstancesList(int tenantId, List<FolderViewModel> documentList)
        {
            List<FormInstanceViewModel> formInstances = new List<FormInstanceViewModel>();
            try
            {
                if (documentList != null && documentList.Count > 0)
                {
                    formInstances = (from doc in documentList
                                     join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     on doc.FolderVersionID equals fi.FolderVersionID
                                     join fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     on doc.FolderVersionID equals fldver.FolderVersionID
                                     join acc in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                                     on fi.FormInstanceID equals acc.FormInstanceID
                                     into temp
                                     from j in temp.DefaultIfEmpty()
                                     where fi.FolderVersionID == doc.FolderVersionID && fi.Name == doc.FormName
                                     && fi.DocID == doc.DocId && fi.IsActive
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = fi.FormInstanceID,
                                         FolderVersionID = fi.FolderVersionID,
                                         FormDesignID = fi.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(fi.Name) ? fi.FormDesign.FormName : fi.Name + " - " + (j == null ? string.Empty : j.ProductType),
                                         TenantID = fi.TenantID,
                                         FormDesignVersionID = fi.FormDesignVersionID,
                                         AnchorDocumentID = fi.AnchorDocumentID,
                                         DocID = fi.DocID,
                                         FolderVersionNumber = fldver.FolderVersionNumber,
                                         EffectiveDate = fldver.EffectiveDate,
                                         FolderVersionStateID = fldver.FolderVersionStateID
                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstances;
        }
        public List<FormInstanceViewModel> GetAnchorFormInstanceList(int tenantId, int folderVersionId)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId
                                                  && c.FolderVersionID == folderVersionId
                                                  && c.FormInstanceID == c.AnchorDocumentID
                                                  && c.IsActive == true)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                     });

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public List<OpenDocumentViewModel> GetAncherFormInstanceList(int folderVersionId, string folderViewMode, int? userId)
        {
            List<OpenDocumentViewModel> formInstanceList = null;

            var lockedDocuments = _resourceLockService.GetLockedDocuments(userId);
            int folderId = _unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(s => s.FolderVersionID == folderVersionId).Select(s => s.FolderID).FirstOrDefault();

            var folderVersions = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get() on fv.FolderVersionID equals fi.FolderVersionID
                                  join fld in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fld.FolderID
                                  where fv.IsActive == true
                                  && fv.FolderID == folderId
                                  select new FolderVersionViewModel
                                  {
                                      FolderVersionId = fv.FolderVersionID,
                                      FolderVersionNumber = fv.FolderVersionNumber,
                                      IsPortfolio = fld.IsPortfolio,
                                      FolderVersionStateID = fv.FolderVersionStateID,
                                      FormInstanceID = fi.FormInstanceID,
                                      DocId = fi.DocID
                                  }).ToList();


            var allDocuments = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fd.FormID
                                join e in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on fd.DocumentDesignTypeID equals e.DocumentDesignTypeID
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fld in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fld.FolderID
                                join map in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                                on fi.FormInstanceID equals map.FormInstanceID
                                into temp
                                from j in temp.DefaultIfEmpty()
                                where fi.IsActive == true && fv.FolderVersionID == folderVersionId
                                select new OpenDocumentVM
                                {
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocId = fi.DocID,
                                    AnchorDocumentID = fi.AnchorDocumentID,
                                    FormName = fd.FormName,
                                    FormDesignDisplayName = fd.DisplayText,
                                    FormDesignID = fi.FormDesignID,
                                    FormDesignVersionID = fi.FormDesignVersionID,
                                    ContractCode = j.ProductID ?? string.Empty,
                                    PlanNumber = j.ProductType ?? string.Empty,
                                    FolderVersionID = fi.FolderVersionID,
                                    FolderVersionNumber = fv.FolderVersionNumber,
                                    IsPortfolio = fld.IsPortfolio,
                                    FolderVersionStateID = fv.FolderVersionStateID,
                                    DocumentDesignTypeID = fd.DocumentDesignTypeID,
                                    DocumentDesignName = e.DocumentDesignName
                                }).ToList();

            if (allDocuments.Count > 0)
            {
                formInstanceList = (from doc in allDocuments
                                    where doc.FormInstanceID == doc.AnchorDocumentID
                                    select new OpenDocumentViewModel
                                    {
                                        AnchorFormInstanceId = doc.FormInstanceID,
                                        FormName = doc.FormInstanceName,
                                        FormDesignID = doc.FormDesignID,
                                        FormDesignVersionID = doc.FormDesignVersionID,
                                        ContractCode = doc.ContractCode,
                                        PlanNumber = doc.PlanNumber,
                                        DocId = doc.DocId,
                                    }).OrderByDescending(s => s.ContractCode).ToList();

                if (formInstanceList.Count > 0)
                {
                    if (folderViewMode == Convert.ToString(FolderViewMode.SOT))
                    {
                        formInstanceList.ForEach(doc =>
                        {
                            doc.DocumentViews = (from d in allDocuments
                                                 where d.AnchorDocumentID == doc.AnchorFormInstanceId
                                                 && d.DocumentDesignTypeID == 1
                                                 select new DocumentViewListViewModel
                                                 {
                                                     FormInstanceId = d.FormInstanceID,
                                                     FormInstanceName = d.FormInstanceName,
                                                     FormDesignID = d.FormDesignID,
                                                     FormDesignTypeID = d.DocumentDesignTypeID,
                                                     FormDesignName = d.FormName,
                                                     FormDesignDisplayName = d.FormDesignDisplayName,
                                                     FormDesignVersionID = d.FormDesignVersionID,
                                                     FormDesignTypeName = d.DocumentDesignName
                                                 }).ToList();
                            doc.FolderVersions = (from f in folderVersions
                                                  where f.DocId == doc.DocId
                                                  select new FolderVersionViewModel
                                                  {
                                                      FolderVersionId = f.FolderVersionId,
                                                      FolderVersionNumber = f.FolderVersionNumber,
                                                      IsPortfolio = f.IsPortfolio,
                                                      FolderVersionStateID = f.FolderVersionStateID,
                                                  }).Distinct().OrderByDescending(s => s.FolderVersionId).ToList();
                            if (lockedDocuments.Where(c => c.FormInstanceID == doc.AnchorFormInstanceId).Count() > 0)
                            {
                                doc.IsDocumentLocked = true;
                                doc.LockedBy = lockedDocuments.Where(c => c.FormInstanceID == doc.AnchorFormInstanceId).FirstOrDefault().LockedUserName;
                            }
                        });
                    }
                    else
                    {
                        formInstanceList.ForEach(doc =>
                        {
                            doc.DocumentViews = (from d in allDocuments
                                                 where d.AnchorDocumentID == doc.AnchorFormInstanceId
                                                 select new DocumentViewListViewModel
                                                 {
                                                     FormInstanceId = d.FormInstanceID,
                                                     FormDesignID = d.FormDesignID,
                                                     FormInstanceName = d.FormInstanceName,
                                                     FormDesignTypeID = d.DocumentDesignTypeID,
                                                     FormDesignName = d.FormName,
                                                     FormDesignDisplayName = d.FormDesignDisplayName,
                                                     FormDesignVersionID = d.FormDesignVersionID,
                                                     FormDesignTypeName = d.DocumentDesignName
                                                 }).OrderBy(s => s.FormInstanceId).ToList();
                            doc.FolderVersions = folderVersions;
                            if (lockedDocuments.Where(c => c.FormInstanceID == doc.AnchorFormInstanceId).Count() > 0)
                            {
                                doc.IsDocumentLocked = true;
                                doc.LockedBy = lockedDocuments.Where(c => c.FormInstanceID == doc.AnchorFormInstanceId).FirstOrDefault().LockedUserName;
                            }
                        });
                    }
                }
            }

            return formInstanceList;
        }

        public List<OpenDocumentViewModel> GetAncherFormInstanceList(int tenantId, int folderVersionId, string folderViewMode, int? userId)
        {
            List<OpenDocumentViewModel> formInstanceList = null;

            try
            {
                var lockedDocuments = _resourceLockService.GetLockedDocuments(userId);
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId
                                                  && c.FolderVersionID == folderVersionId
                                                  && c.FormInstanceID == c.AnchorDocumentID
                                                  && c.IsActive == true)
                                              .Get()
                                     join map in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                                     on c.FormInstanceID equals map.FormInstanceID
                                     into temp
                                     from j in temp.DefaultIfEmpty()
                                         //join resourceLock in lockedDocuments
                                         //on c.FormInstanceID equals resourceLock.FormInstanceID ?? 0
                                         //into temp2
                                         //from lockedDocument in temp2.DefaultIfEmpty()
                                     select new OpenDocumentViewModel
                                     {
                                         AnchorFormInstanceId = c.FormInstanceID,
                                         FormName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         ContractCode = j.ProductID ?? string.Empty,
                                         PlanNumber = j.ProductType ?? string.Empty,
                                         DocId = c.DocID,
                                         //LockedFormInstanceId = lockedDocument.FormInstanceID ?? 0
                                     }).OrderByDescending(s => s.ContractCode);

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                    foreach (var formInstance in formInstanceList)
                    {
                        if (folderViewMode == Convert.ToString(FolderViewMode.SOT))
                        {
                            formInstance.DocumentViews = GetSOTDocumentViewList(tenantId, formInstance.AnchorFormInstanceId);
                            formInstance.FolderVersions = GetDocumentFolderVersions(formInstance.DocId, formInstance.FormName, GetFolderVersionList(tenantId, GetFolderVersionById(folderVersionId).FolderId));
                        }
                        else
                        {
                            formInstance.DocumentViews = GetDocumentViewList(tenantId, formInstance.AnchorFormInstanceId);
                            formInstance.FolderVersions = GetDocumentFolderVersions(formInstance.DocId, formInstance.FormName, GetFolderVersionList(tenantId, GetFolderVersionById(folderVersionId).FolderId));
                        }
                        formInstance.FolderVersions = formInstance.FolderVersions.OrderByDescending(x => x.FolderVersionId);
                        if (lockedDocuments.Where(c => c.FormInstanceID == formInstance.AnchorFormInstanceId).Count() > 0)
                        {
                            //if (formInstance.AnchorFormInstanceId == formInstance.LockedFormInstanceId)
                            formInstance.IsDocumentLocked = true;
                            formInstance.LockedBy = lockedDocuments.Where(c => c.FormInstanceID == formInstance.AnchorFormInstanceId).FirstOrDefault().LockedUserName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return formInstanceList;
        }

        public List<DocumentViewListViewModel> GetDocumentViewList(int tenantId, int anchorFormInstanceId)
        {
            List<DocumentViewListViewModel> documentViewList = null;

            try
            {

                var AnchorformInstaceID = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get().Where(c => c.FormInstanceID == anchorFormInstanceId).Select(c => c.AnchorDocumentID).FirstOrDefault();
                if (AnchorformInstaceID == anchorFormInstanceId)
                {
                    CreateNewViewForExistingDocument(tenantId, anchorFormInstanceId);
                }
                if (AnchorformInstaceID != null)
                {
                    var documentViews = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Filter(c => c.TenantID == tenantId && c.IsActive == true && (c.AnchorDocumentID == AnchorformInstaceID))
                                                  .Get()
                                         join d in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                  .Query()
                                                  .Filter(c => c.IsActive == true)
                                                  .Get()
                                         on c.FormDesignID equals d.FormID
                                         join e in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get()
                                         on d.DocumentDesignTypeID equals e.DocumentDesignTypeID
                                         //join f in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                         //on d.FormID equals f.FormDesignID 
                                         orderby c.FormInstanceID
                                         select new DocumentViewListViewModel
                                         {
                                             FormInstanceId = c.FormInstanceID,
                                             FormDesignID = d.FormID,
                                             FormDesignTypeID = d.DocumentDesignTypeID,
                                             FormDesignTypeName = e.DocumentDesignName,
                                             FormDesignName = d.FormName,
                                             FormDesignDisplayName = d.DisplayText,
                                             FormInstanceName = c.Name,
                                             FormDesignVersionID = c.FormDesignVersionID,
                                             IsSectionLockEnabled = d.IsSectionLock
                                         });


                    if (documentViews != null)
                    {
                        documentViewList = documentViews.ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return documentViewList;
        }
        public List<DocumentViewListViewModel> GetSOTDocumentViewList(int tenantId, int anchorFormInstanceId)
        {
            List<DocumentViewListViewModel> documentViewList = null;

            try
            {

                var AnchorformInstaceID = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get().Where(c => c.FormInstanceID == anchorFormInstanceId).Select(c => c.AnchorDocumentID).FirstOrDefault();
                if (AnchorformInstaceID != null)
                {
                    var documentViews = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Filter(c => c.TenantID == tenantId && c.IsActive == true && (c.AnchorDocumentID == AnchorformInstaceID))
                                                  .Get()
                                         join d in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                         on c.FormDesignID equals d.FormID
                                         join e in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get()
                                         on d.DocumentDesignTypeID equals e.DocumentDesignTypeID
                                         where d.DocumentDesignTypeID == 1
                                         orderby c.FormInstanceID
                                         select new DocumentViewListViewModel
                                         {
                                             FormInstanceId = c.FormInstanceID,
                                             FormDesignID = d.FormID,
                                             FormDesignTypeID = d.DocumentDesignTypeID,
                                             FormDesignTypeName = e.DocumentDesignName,
                                             FormDesignName = d.FormName,
                                             FormDesignDisplayName = d.DisplayText,
                                             FormDesignVersionID = c.FormDesignVersionID
                                         });


                    if (documentViews != null)
                    {
                        documentViewList = documentViews.ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return documentViewList;
        }

        public List<FormInstanceViewModel> GetUpdatedDocumentList(int tenantId, int folderVersionId)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId
                                                  && c.FolderVersionID == folderVersionId
                                                  && c.FormInstanceID == c.AnchorDocumentID
                                                  && (c.UpdatedDate != null || c.DocID == c.FormInstanceID)
                                                  && c.IsActive == true)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         AnchorDocumentID = c.AnchorDocumentID
                                     });

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public void SaveCopyFromAuditTrail(int formInstanceId, int currentVersionId, int newFormInstanceId, string addedBy, bool isManualCopy)
        {
            try
            {
                FolderVersion folderversion = null;
                CopyFromAuditTrail copyValue = new CopyFromAuditTrail();
                bool isExist = this._unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Query().Filter(c => c.DestinationDocumentID == formInstanceId).Get().Any();
                if (isExist && !isManualCopy)
                {
                    CopyFromAuditTrail auditTrailData = this._unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Query().Filter(c => c.DestinationDocumentID == formInstanceId).Get().FirstOrDefault();

                    copyValue.SourceDocumentID = auditTrailData.SourceDocumentID;
                    copyValue.FolderID = auditTrailData.FolderID;
                    copyValue.AccountID = auditTrailData.AccountID;
                    copyValue.EffectiveDate = auditTrailData.EffectiveDate;
                    copyValue.DestinationFolderVersionID = currentVersionId;
                    copyValue.SourceFolderVersionID = auditTrailData.SourceFolderVersionID;
                    copyValue.DestinationDocumentID = newFormInstanceId;
                    copyValue.AddedBy = addedBy;
                    copyValue.AddedDate = DateTime.Now;
                    this._unitOfWork.Repository<CopyFromAuditTrail>().Insert(copyValue);
                    this._unitOfWork.Save();
                }
                else if (isManualCopy)
                {
                    FormInstance fromistance = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(c => c.FormInstanceID == formInstanceId && c.IsActive == true).Get().FirstOrDefault();

                    folderversion = this._unitOfWork.Repository<FolderVersion>().Query().Filter(f => f.FolderVersionID == fromistance.FolderVersionID && f.IsActive == true).Get().FirstOrDefault();
                    var accn = this._unitOfWork.RepositoryAsync<AccountFolderMap>()
                                                      .Query()
                                                      .Filter(c => c.FolderID == folderversion.FolderID).Get();
                    int accountId = 0;

                    //Resolved HNE-440. As portfolio won't have account , accountId is set to 0 when accn == null
                    if (accn != null && accn.Count() > 0)
                        accountId = accn.FirstOrDefault().AccountID;
                    copyValue.SourceDocumentID = formInstanceId;
                    copyValue.FolderID = folderversion.FolderID;
                    copyValue.AccountID = accountId;
                    copyValue.EffectiveDate = folderversion.EffectiveDate;
                    copyValue.DestinationFolderVersionID = currentVersionId;
                    copyValue.SourceFolderVersionID = fromistance.FolderVersionID;
                    copyValue.DestinationDocumentID = newFormInstanceId;
                    copyValue.AddedBy = addedBy;
                    copyValue.AddedDate = DateTime.Now;
                    this._unitOfWork.Repository<CopyFromAuditTrail>().Insert(copyValue);
                    this._unitOfWork.Save();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

        }

        public int GetAdminFormInstanceID(int tenantId, int formInstanceId)
        {
            int adminFormInstanceId = 0;
            try
            {
                var formInstance = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FormInstanceID == formInstanceId && c.IsActive == true)
                                              .Get()
                                    select new FormInstanceViewModel
                                    {
                                        FormInstanceID = c.FormInstanceID,
                                        FolderVersionID = c.FolderVersionID
                                    }).FirstOrDefault();


                if (formInstance != null)
                {
                    var adminForm = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Include(c => c.FormDesign)
                                                  .Filter(c => c.TenantID == tenantId && c.FolderVersionID == formInstance.FolderVersionID && c.FormDesignID == 11 && c.IsActive == true)
                                                  .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID
                                     }).FirstOrDefault();

                    if (adminForm != null)
                    {
                        adminFormInstanceId = adminForm.FormInstanceID;
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return adminFormInstanceId;
        }

        public void UpdateRetroChangesWhenReleased(int tenantId, int folderVersionId, string userName, int folderID)
        {
            var newfolderVersion = _unitOfWork.RepositoryAsync<FolderVersion>()
                                        .Query()
                                        .Filter(fil => fil.FolderID == folderID &&
                                            fil.FolderVersionStateID == (int)FolderVersionState.INPROGRESS_BLOCKED &&
                                            fil.TenantID == tenantId)
                                        .Get()
                                        .OrderBy(ord => ord.FolderVersionID)
                                        .FirstOrDefault();

            if (newfolderVersion != null)
            {
                //Set the newFolderVersion available Status = "In Progress"
                newfolderVersion.FolderVersionStateID = (int)FolderVersionState.INPROGRESS;
                newfolderVersion.UpdatedBy = userName;
                newfolderVersion.UpdatedDate = DateTime.Now;
                _unitOfWork.RepositoryAsync<FolderVersion>().Update(newfolderVersion);
                _unitOfWork.Save();
                this.UpdateFolderChange(tenantId, userName, folderID, folderVersionId);
            }
        }

        public List<FolderVersionHistoryViewModel> GetVersionHistory(int folderId, int tenantId, string versionType)
        {
            List<FolderVersionHistoryViewModel> folderVersionHistoryViewModels = null;
            try
            {
                var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderID == folderId).Get().FirstOrDefault();
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderversion.FolderVersionID);
                if (versionType.ToLower().Equals("major"))
                {
                    folderVersionHistoryViewModels = (from d in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                        .Query()
                                                        .Include(inc => inc.WorkFlowVersionState)
                                                        .Include(inc => inc.FolderVersionState)
                                                        .Filter(d => d.FolderID == folderId && d.TenantID == tenantId &&
                                                         d.FolderVersionState.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                                        .Get()
                                                      join category in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Query().Get()
                                                      on d.CategoryID equals category.FolderVersionCategoryID
                                                      into cat
                                                      from data in cat.DefaultIfEmpty()
                                                      select new FolderVersionHistoryViewModel
                                                      {
                                                          FolderId = d.FolderID,
                                                          FolderVersionId = d.FolderVersionID,
                                                          EffectiveDate = d.EffectiveDate,
                                                          FolderVersionNumber = d.FolderVersionNumber,
                                                          User = !String.IsNullOrEmpty(d.UpdatedBy) ? d.UpdatedBy : d.AddedBy,
                                                          AddedDate = d.AddedDate,
                                                          Comments = d.Comments,
                                                          WFStateName = d.WorkFlowVersionState.WorkFlowState.WFStateName,
                                                          VersionType = d.VersionTypeID == (int)VersionType.New ?
                                                          GlobalVariables.NEW : GlobalVariables.RETRO,
                                                          CategoryName = data != null ? data.FolderVersionCategoryName : "",
                                                          CategoryId = data != null ? data.FolderVersionCategoryID : 0,
                                                          CatID = d.CatID
                                                      }).OrderByDescending(ord => ord.FolderVersionId).ToList();
                }
                else
                {
                    folderVersionHistoryViewModels = (from d in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Include(inc => inc.WorkFlowVersionState)
                                                    .Include(inc => inc.FolderVersionState)
                                                    .Filter(d => d.FolderID == folderId &&
                                                        d.TenantID == tenantId &&
                                                        d.FolderVersionState.FolderVersionStateID !=
                                                        (int)FolderVersionState.RELEASED)
                                                    .Get()
                                                      join category in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Query().Get()
                                                          on d.CategoryID equals category.FolderVersionCategoryID
                                                          into cat
                                                      from data in cat.DefaultIfEmpty()
                                                      select new FolderVersionHistoryViewModel
                                                      {
                                                          FolderId = d.FolderID,
                                                          FolderVersionId = d.FolderVersionID,
                                                          EffectiveDate = d.EffectiveDate,
                                                          FolderVersionNumber = d.FolderVersionNumber,
                                                          User = !String.IsNullOrEmpty(d.UpdatedBy) ? d.UpdatedBy : d.AddedBy,
                                                          AddedDate = d.AddedDate,
                                                          Comments = d.Comments,
                                                          WFStateName = d.WorkFlowVersionState.WorkFlowState.WFStateName,
                                                          VersionType = d.VersionTypeID == (int)VersionType.New ?
                                                            GlobalVariables.NEW : GlobalVariables.RETRO,
                                                          FolderVersionStateName = d.FolderVersionState.FolderVersionStateName,
                                                          CategoryName = data != null ? data.FolderVersionCategoryName : "",
                                                          CategoryId = data != null ? data.FolderVersionCategoryID : 0,
                                                          CatID = d.CatID
                                                      }).OrderByDescending(ord => ord.FolderVersionId).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return folderVersionHistoryViewModels;
        }
        /// <summary>
        /// Get List of all Major versions of a folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="tenantId"></param>
        /// <param name="versionType"></param>
        /// <returns></returns>
        public List<FolderVersionHistoryViewModel> GetVersionHistoryML(int folderId, int tenantId, string versionType)
        {
            List<FolderVersionHistoryViewModel> folderVersionHistoryViewModels = null;
            try
            {
                if (versionType.ToLower().Equals("major"))
                {
                    folderVersionHistoryViewModels = (from d in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                        .Query()
                                                        .Include(inc => inc.FolderVersionState)
                                                        .Filter(d => d.FolderID == folderId && d.TenantID == tenantId &&
                                                         d.FolderVersionState.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                                        .Get()
                                                      join category in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Query().Get()
                                                      on d.CategoryID equals category.FolderVersionCategoryID
                                                      into cat
                                                      from data in cat.DefaultIfEmpty()
                                                      select new FolderVersionHistoryViewModel
                                                      {
                                                          FolderId = d.FolderID,
                                                          FolderVersionId = d.FolderVersionID,
                                                          EffectiveDate = d.EffectiveDate,
                                                          FolderVersionNumber = d.FolderVersionNumber,
                                                          User = d.AddedBy,
                                                          AddedDate = d.AddedDate,
                                                          Comments = d.Comments,
                                                          Status = "Released",
                                                          VersionType = d.VersionTypeID == (int)VersionType.New ?
                                                          GlobalVariables.NEW : GlobalVariables.RETRO,
                                                          CategoryName = data != null ? data.FolderVersionCategoryName : "",
                                                          CatID = d.CatID
                                                      }).OrderByDescending(ord => ord.FolderVersionId).ToList();
                }
                else
                {
                    folderVersionHistoryViewModels = (from d in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Include(inc => inc.FolderVersionState)
                                                    .Filter(d => d.FolderID == folderId &&
                                                        d.TenantID == tenantId &&
                                                        d.FolderVersionState.FolderVersionStateID !=
                                                        (int)FolderVersionState.RELEASED)
                                                    .Get()
                                                      join category in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Query().Get()
                                                          on d.CategoryID equals category.FolderVersionCategoryID
                                                          into cat
                                                      from data in cat.DefaultIfEmpty()
                                                      select new FolderVersionHistoryViewModel
                                                      {
                                                          FolderId = d.FolderID,
                                                          FolderVersionId = d.FolderVersionID,
                                                          EffectiveDate = d.EffectiveDate,
                                                          FolderVersionNumber = d.FolderVersionNumber,
                                                          User = d.AddedBy,
                                                          AddedDate = d.AddedDate,
                                                          Comments = d.Comments,
                                                          VersionType = d.VersionTypeID == (int)VersionType.New ?
                                                            GlobalVariables.NEW : GlobalVariables.RETRO,
                                                          FolderVersionStateName = d.FolderVersionState.FolderVersionStateName,
                                                          CategoryName = data != null ? data.FolderVersionCategoryName : "",
                                                          CatID = d.CatID
                                                      }).OrderByDescending(ord => ord.FolderVersionId).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return folderVersionHistoryViewModels;
        }

        /// <summary>
        /// Gets the form type list to create a new form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public List<FormTypeViewModel> GetFormTypeList(int tenantId, string folderType, DateTime effectiveDate, int folderId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            List<FormTypeViewModel> formTypes = null;
            List<FormTypeViewModel> distinctRecords = new List<FormTypeViewModel>();
            List<FormTypeViewModel> reversedList = new List<FormTypeViewModel>();
            List<FormTypeViewModel> FilterRecordByMasterList = new List<FormTypeViewModel>();
            List<FormTypeViewModel> anchordocumentlist = new List<FormTypeViewModel>();
            try
            {
                List<int> anchordoclist = this._unitOfWork.RepositoryAsync<FormDesignMapping>().Get().Select(c => c.TargetDesignID).Distinct().ToList();

                formTypes = (from frmgrpfoldertype in this._unitOfWork.RepositoryAsync<FormGroupFolderMap>().Get()
                             join fdgm in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get()
                             on frmgrpfoldertype.FormDesignGroupID equals fdgm.FormDesignGroupID
                             where frmgrpfoldertype.FolderType == folderType && frmgrpfoldertype.TenantID == tenantId
                             join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(x => !anchordoclist.Contains(x.FormID))
                             on fdgm.FormID equals fd.FormID
                             where fd.IsActive == true && fd.TenantID == tenantId
                             join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                             on fd.FormID equals fdv.FormDesignID
                             where fdv.EffectiveDate <= effectiveDate
                             //TODO: This is temporarily commented out, should be uncommented when Form Finalization code is complete
                             //TODO: Replace 3 by enum,
                             //TODO: also add effective date logic :
                             //      the finalized form design version with the latest effective date
                             //      before the effective date of this folder version is required
                             //where    fdv.StatusID == 3
                             select new FormTypeViewModel
                             {
                                 FormVersionDesignID = fdv.FormDesignVersionID,
                                 FormTypeName = fd.DisplayText,
                                 FormDesignID = fdv.FormDesignID
                             }).ToList();



                if (formTypes != null)
                {
                    //To move current version's FormDesignVersionID at the top of the list.
                    reversedList = formTypes.OrderByDescending(c => c.FormVersionDesignID).ToList();
                    foreach (var record in reversedList)
                    {
                        //To remove duplicate records in reversedList
                        if (distinctRecords.Where(p => p.FormDesignID == record.FormDesignID).FirstOrDefault() == null)
                        {
                            distinctRecords.Add(record);
                        }


                    }

                    // select Master List Applicable Form only
                    foreach (var item in distinctRecords)
                    {
                        FilterRecordByMasterList.Add(item);

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return FilterRecordByMasterList;
        }

        /// <summary>
        /// Gets the folder list form which the form instances will be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="IsPortfolio"></param>
        /// <returns></returns>
        public IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, bool IsPortfolio, int accountId, int categoryId, bool isFoundation = false)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<FolderVersionViewModel> folderList = null;
            try
            {
                if (IsPortfolio)
                {


                    folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                      //left join on folder version
                                  join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  on fld.FolderID equals fldvrsn.FolderID
                                  where (fld.TenantID == tenantId && fld.IsPortfolio == IsPortfolio && fld.IsFoundation == isFoundation)
                                  select new FolderVersionViewModel
                                  {
                                      FolderId = fldvrsn.FolderID,
                                      FolderName = fld.Name
                                  }).OrderBy(o => o.FolderName).Distinct().ToList();
                }
                else
                {
                    folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                  join fI in this._unitOfWork.Repository<AccountFolderMap>().Get()
                                               on fld.FolderID equals fI.FolderID
                                  //left join on folder version
                                  join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  on fld.FolderID equals fldvrsn.FolderID
                                  where (fld.TenantID == tenantId && fI.Account.IsActive == true && fI.AccountID == accountId)
                                  select new FolderVersionViewModel
                                  {
                                      FolderId = fldvrsn.FolderID,
                                      FolderName = fld.Name
                                  }).OrderBy(o => o.FolderName).Distinct().ToList();

                }

                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;

        }

        public GridPagingResponse<FolderVersionViewModel> GetAllFoldersList(GridPagingRequest gridPagingRequest)
        {
            IList<FolderVersionViewModel> folderList = null;
            int count = 0;
            try
            {
                //WITH MASTERLIST
                //folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                //                  //left join on folder version
                //              join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                //              on fld.FolderID equals fldvrsn.FolderID
                //              //where (fld.I)       //Exclude MasterList
                //              select new FolderVersionViewModel
                //              {
                //                  FolderId = fldvrsn.FolderID,
                //                  FolderName = fld.Name
                //              }).OrderBy(o => o.FolderName).Distinct().ToList();
                //WITHOUT MASTERLIST

                if (gridPagingRequest.rows != null)
                {
                    SearchCriteria criteria = new SearchCriteria();
                    criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                    folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                      //left join on folder version
                                  join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  on fld.FolderID equals fldvrsn.FolderID
                                  join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                  on fldvrsn.FolderVersionID equals fi.FolderVersionID
                                  join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  on fi.FormDesignID equals fd.FormID
                                  where (fd.IsMasterList == false)
                                  select new FolderVersionViewModel
                                  {
                                      FolderId = fldvrsn.FolderID,
                                      FolderName = fld.Name
                                  }).OrderBy(o => o.FolderName).Distinct().ToList().ApplySearchCriteria(criteria)
                                           .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                           .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                           .ToList();


                    if (folderList.Count() == 0)
                        folderList = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            // return folderList;
            return new GridPagingResponse<FolderVersionViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, folderList);

        }

        public IEnumerable<FolderVersionViewModel> GetAllFoldersList()
        {
            IList<FolderVersionViewModel> folderList = null;
            try
            {
                //WITH MASTERLIST
                //folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                //                  //left join on folder version
                //              join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                //              on fld.FolderID equals fldvrsn.FolderID
                //              //where (fld.I)       //Exclude MasterList
                //              select new FolderVersionViewModel
                //              {
                //                  FolderId = fldvrsn.FolderID,
                //                  FolderName = fld.Name
                //              }).OrderBy(o => o.FolderName).Distinct().ToList();
                //WITHOUT MASTERLIST
                folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                  //left join on folder version
                              join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                              on fld.FolderID equals fldvrsn.FolderID
                              where (fld.IsPortfolio == true)
                              select new FolderVersionViewModel
                              {
                                  FolderVersionId = fldvrsn.FolderVersionID,
                                  FolderVersionNumber = fldvrsn.FolderVersionNumber,
                                  FolderId = fld.FolderID,
                                  FolderName = fld.Name,
                              }).Distinct().OrderBy(o => o.FolderName).ToList();


                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;

        }


        public IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, bool IsPortfolio, int accountId, int categoryId, int? roleID, bool isFoundation = false)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<FolderVersionViewModel> folderList = null;
            try
            {
                if (IsPortfolio)
                {


                    folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                      //left join on folder version
                                  join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                  on fld.FolderID equals fldvrsn.FolderID
                                  where (fld.TenantID == tenantId && fld.IsPortfolio == IsPortfolio && fld.IsFoundation == isFoundation)
                                  select new FolderVersionViewModel
                                  {
                                      FolderId = fldvrsn.FolderID,
                                      FolderName = fld.Name
                                  }).OrderBy(o => o.FolderName).Distinct().ToList();
                }
                else
                {
                    var allfolderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                         join fI in this._unitOfWork.Repository<AccountFolderMap>().Get()
                                                      on fld.FolderID equals fI.FolderID
                                         //left join on folder version
                                         join fldvrsn in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                         on fld.FolderID equals fldvrsn.FolderID
                                         where (fld.TenantID == tenantId && fI.Account.IsActive == true && fI.AccountID == accountId)
                                         select new FolderVersionViewModel
                                         {
                                             FolderId = fldvrsn.FolderID,
                                             FolderName = fld.Name,
                                             CategoryID = fldvrsn.CategoryID,
                                             FolderVersionNumber = fldvrsn.FolderVersionNumber,
                                             FolderVersionId = fldvrsn.FolderVersionID
                                         }).OrderBy(o => o.FolderName).Distinct().ToList();
                    folderList = (from c in allfolderList
                                  group c by new
                                  {
                                      c.FolderId,
                                      c.FolderName
                                  } into grp
                                  select grp.First()).ToList();

                }
                //if (roleID != null && roleID == 22)
                //{
                //  folderList = folderList.Where(row => row.CategoryID == 3 || row.CategoryID == 4).ToList();
                //}
                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;

        }

        public IEnumerable<FolderVersionViewModel> GetFolderList(int tenantId, int accountId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            IList<FolderVersionViewModel> folderList = null;
            try
            {
                folderList = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                  //left join on folder version
                              join accfldmp in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get()
                              on fld.FolderID equals accfldmp.FolderID
                              where (fld.TenantID == tenantId && accfldmp.AccountID == accountId && fld.Name != "Master List")
                              select new FolderVersionViewModel
                              {
                                  FolderId = fld.FolderID,
                                  FolderName = fld.Name
                              }).OrderBy(o => o.FolderName).ToList();


                if (folderList.Count() == 0)
                    folderList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderList;
        }

        /// <summary>
        /// Gets the form list to be copied.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <returns></returns>
        public List<FormInstanceViewModel> GetFormList(int tenantId, int folderVersionId)
        {
            Contract.Requires(folderVersionId > 0, "Invalid Folder Version Id ");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            List<FormInstanceViewModel> formInstanceList = null;
            try
            {

                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.IsActive == true && c.FormDesign.DocumentDesignTypeID == 1)
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = c.Name != null ? c.Name : c.FormDesign.FormName,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         EffectiveDate = c.FormDesignVersion.EffectiveDate,
                                         FormInstanceName = c.Name
                                     });

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public ServiceResult CreateFormReference(int accountId, int folderId, int folderVersionId, int formInstanceId, int? consortiumId, int targetFormInstanceId, string userName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormReferenceMap objReference = new FormReferenceMap();
                objReference.ReferenceAccountID = accountId;
                objReference.ReferenceFolderID = folderId;
                objReference.ReferenceFolderVersionID = folderVersionId;
                objReference.ReferenceConsortiumID = consortiumId;
                objReference.ReferenceFormInstanceID = formInstanceId;
                objReference.TargetFormInstanceID = targetFormInstanceId;
                objReference.AddedDate = DateTime.Now;
                objReference.AddedBy = userName;

                this._unitOfWork.RepositoryAsync<FormReferenceMap>().Insert(objReference);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        public bool IsFormInstanceExist(string formName)
        {
            bool result = false;

            var formInstance = (from fi in _unitOfWork.RepositoryAsync<FormInstance>()
                                                          .Query()
                                                          .Filter(x => x.Name == formName)
                                                          .Get()
                                select fi).FirstOrDefault();

            if (formInstance != null)
                result = true;

            return result;
        }

        public bool IsFoundationFolder(int formInstanceID)
        {
            bool result = false;
            var folder = (from fl in _unitOfWork.RepositoryAsync<Folder>().Get()
                          join fv in _unitOfWork.RepositoryAsync<FolderVersion>().Get() on fl.FolderID equals fv.FolderID
                          join fi in _unitOfWork.RepositoryAsync<FormInstance>().Get() on fv.FolderVersionID equals fi.FolderVersionID
                          where fi.FormInstanceID == formInstanceID
                          select fl).FirstOrDefault();

            if (folder != null && folder.IsFoundation == true)
                result = true;

            return result;
        }

        /// <summary>
        /// Saves the form instance.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderVersionId">The folder version identifier.</param>
        /// <param name="formDesignVersionId">The form design version identifier.</param>
        /// <param name="formInstanceId">The form instance identifier.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        /// <param name="formName">Name of the form.</param>
        /// <param name="addedBy"></param>
        /// <returns></returns>
        public ServiceResult CreateFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formInstanceId, bool isCopy, string formName, string addedBy)
        {
            Contract.Requires(folderVersionId > 0, "Invalid Folder Version Id ");
            Contract.Requires(tenantId > 0, "Invalid tenantId");

            if (!isCopy)
                Contract.Requires(formDesignVersionId > 0, "Invalid FormDesign Version Id");
            else
                Contract.Requires(formInstanceId > 0, "Invalid Form Instance Id ");

            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                int newFormInstanceId = 0;

                //int folderID = (from fldr in this._unitOfWork.RepositoryAsync<FolderVersion>()
                //                                     .Query()
                //                                     .Filter(fdv => fdv.FolderVersionID == folderVersionId)
                //                                     .Get()
                //                select fldr.FolderID).FirstOrDefault();

                FolderVersionViewModel fv = (from fldver in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                     .Query()
                                                     .Filter(fdv => fdv.FolderVersionID == folderVersionId)
                                                     .Get()
                                             select new FolderVersionViewModel
                                             {

                                                 EffectiveDate = fldver.EffectiveDate,
                                                 FolderId = fldver.FolderID,
                                             }).FirstOrDefault();
                int folderID = fv.FolderId;
                DateTime? effectiveDate = fv.EffectiveDate;

                if (!isCopy)
                {
                    int formDesignId = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                     .Query()
                                                     .Filter(fdv => fdv.FormDesignID != null && fdv.FormDesignVersionID == formDesignVersionId)
                                                     .Get()
                                                     .Select(c => c.FormDesignID.Value).FirstOrDefault();


                    var isMasterList = IsMasterList(folderID);
                    if (!isMasterList)
                    {


                        //create new form instance. 
                        newFormInstanceId = AddFormInstance(tenantId, folderVersionId, formDesignVersionId, formDesignId, formName, addedBy);
                        var childFormDesignList = this._formDesignService.GetMappedDesignDocumentList(tenantId, formDesignId, effectiveDate);
                        if (childFormDesignList != null)
                        {
                            foreach (var childDesign in childFormDesignList)
                            {
                                string appName = config.Config.GetApplicationName();
                                if (!(appName.ToLower() == "emedicaresync" && childDesign.FormDesignID == 2409))
                                {
                                    string childFormName = childDesign.DisplayText;
                                    int childFormDesignId = childDesign.FormDesignID;
                                    AddFormInstance(tenantId, folderVersionId, childDesign.FormDesignVersionID, childDesign.FormDesignID, formName, addedBy, newFormInstanceId);
                                }
                            }
                        }
                    }
                    else
                    {
                        bool? usesAliasDesignMasterList = this._unitOfWork.RepositoryAsync<FormDesign>()
                                                     .Query()
                                                     .Filter(fd => fd.FormID == formDesignId)
                                                     .Get()
                                                     .Select(c => c.UsesAliasDesignMasterList).FirstOrDefault();
                        if (usesAliasDesignMasterList.HasValue == true && usesAliasDesignMasterList.Value == true)
                        {
                            newFormInstanceId = AddFormInstance(tenantId, folderVersionId, formDesignVersionId, formDesignId, formName, addedBy);
                        }
                    }
                }
                else
                {
                    //copy existing form instance.
                    newFormInstanceId = CopyFormInstance(tenantId, folderVersionId, formInstanceId, formName, false, true, addedBy);
                    var isMasterList = IsMasterList(folderID);
                    if (!isMasterList)
                    {
                        var childFormInstanceList = GetDocumentViewList(tenantId, formInstanceId);
                        if (childFormInstanceList != null)
                        {
                            foreach (var childInstance in childFormInstanceList)
                            {
                                if (childInstance.FormInstanceId != formInstanceId)
                                {
                                    string childFormName = formName + "@@" + childInstance.FormDesignName;
                                    if (childInstance.FormDesignID == 2409)
                                    {
                                        formName = childInstance.FormInstanceName;
                                    }
                                    int childFormInstanceId = childInstance.FormInstanceId;
                                    CopyFormInstance(tenantId, folderVersionId, childFormInstanceId, formName, false, true, addedBy, newFormInstanceId);
                                }

                            }
                        }
                    }
                }


                if (newFormInstanceId > 0)
                {
                    result.Result = ServiceResultStatus.Success;
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    //retrun newly created form instance id.
                    items.Add(new ServiceResultItem { Messages = new string[] { newFormInstanceId.ToString() } });
                    result.Items = items;
                    this.UpdateFolderChange(tenantId, addedBy, null, folderVersionId);
                    // this.SaveCopyFromAuditTrail(formInstanceId, folderVersionId, newFormInstanceId, addedBy, isCopy);                   
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        /// <summary>
        /// This method returns Name  from 'FormInstance' table
        /// The collection is filtered using 'tenantId'&'formInstanceID' which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public FormInstanceViewModel GetFormNameToCopy(int tenantId, int formInstanceID)
        {

            FormInstanceViewModel formName = null;
            try
            {
                formName = (from fn in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                      .Query()
                                                      .Filter(p => p.TenantID == tenantId && p.FormInstanceID == formInstanceID && p.IsActive == true)
                                                      .Get()
                            select new FormInstanceViewModel
                            {
                                Name = fn.Name
                            }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formName;
        }

        public FormInstanceViewModel GetFormInstance(int tenantId, int formInstanceID)
        {
            FormInstanceViewModel viewModel = null;
            try
            {
                viewModel = (from c in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                             join fldr in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                             on c.FolderVersionID equals fldr.FolderVersionID
                             where c.FormInstanceID == formInstanceID && c.TenantID == tenantId && c.IsActive == true
                             select new FormInstanceViewModel
                             {
                                 FormInstanceID = c.FormInstanceID,
                                 FolderVersionID = c.FolderVersionID,
                                 FormDesignID = c.FormDesignID,
                                 FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                 TenantID = c.TenantID,
                                 FormDesignVersionID = c.FormDesignVersionID,
                                 AnchorDocumentID = c.AnchorDocumentID,
                                 DocID = c.DocID,
                                 FolderVersionNumber = fldr.FolderVersionNumber,
                                 EffectiveDate = fldr.EffectiveDate,
                                 FolderID = fldr.FolderID
                             }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return viewModel;
        }

        public FormInstanceViewModel GetFormInstanceByAnchorInstanceIdandFormDesignversion(int formdesignVersionId, int formInstanceID)
        {
            FormInstanceViewModel viewModel = null;
            try
            {
                viewModel = (from c in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                             where c.AnchorDocumentID == formInstanceID && c.FormDesignVersionID == formdesignVersionId && c.IsActive == true
                             select new FormInstanceViewModel
                             {
                                 FormInstanceID = c.FormInstanceID,
                                 FolderVersionID = c.FolderVersionID,
                                 FormDesignID = c.FormDesignID,
                                 FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                             }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return viewModel;
        }

        public string GetFormInstanceData(int tenantId, int formInstanceID)
        {
            string data = "";
            FormInstanceDataMap formInstance = null;
            try
            {
                formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                   .Query()
                                                   .Filter(c => c.FormInstanceID == formInstanceID)
                                                   .Get()
                                                   .FirstOrDefault();

                if (formInstance != null)
                {
                    data = formInstance.FormData;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return data;
        }

        /// <summary>
        /// This method will get formInstance IDs which are not of Reference products
        /// </summary>
        /// <param name="formInstanceIDs"></param>
        /// <returns></returns>
        public List<int> GetProductFormInstanceIDList(List<int> formInstanceIDs)
        {
            List<int> formInstanceIDList = new List<int>();
            formInstanceIDList = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                   .Get()
                                  join fd in formInstanceIDs on c.FormInstanceID equals fd
                                  where c.FormDesignID == GlobalVariables.PRODUCTFORMDESIGNID
                                  select fd).ToList();
            return formInstanceIDList;
        }

        public string GetFormInstanceDataCompressed(int tenantId, int formInstanceID)
        {
            string data = "";
            FormInstanceDataMap formInstance = null;
            try
            {
                formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().GetFormInstanceDataDecompressed(formInstanceID);

                if (formInstance != null)
                {
                    data = formInstance.FormData;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return data;
        }

        public List<FormInstanceViewModel> GetMultipleFormInstancesData(int tenantId, List<int> formInstanceIDs)
        {
            List<FormInstanceViewModel> formInstances = null;
            try
            {
                List<FormInstanceDataMap> dataMaps = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().GetFormInstanceDataList(formInstanceIDs);

                formInstances = (from c in dataMaps
                                 join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                 on c.FormInstanceID equals fi.FormInstanceID
                                 where formInstanceIDs.Contains(c.FormInstanceID)
                                 select new FormInstanceViewModel
                                 {
                                     FormInstanceID = c.FormInstanceID,
                                     FolderVersionID = fi.FolderVersionID,
                                     FormDesignID = fi.FormDesignID,
                                     FormDesignName = String.IsNullOrEmpty(fi.Name) ? fi.FormDesign.FormName : fi.Name,
                                     TenantID = fi.TenantID,
                                     FormDesignVersionID = fi.FormDesignVersionID,
                                     FormInstanceDataMapID = c.FormInstanceDataMapID,
                                     ObjectInstanceID = c.ObjectInstanceID,
                                     FormData = c.FormData,
                                     AnchorDocumentID = fi.AnchorDocumentID
                                 }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstances;
        }

        public List<FormInstanceViewModel> GetFormInstancesList(int tenantId, List<int> formInstanceIDs)
        {
            List<FormInstanceViewModel> formInstances = null;
            List<FormInstanceViewModel> vbidFormInstances = null;
            try
            {
                formInstances = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                 where formInstanceIDs.Contains(fi.FormInstanceID) && fi.IsActive && fi.FormDesignID != 2409
                                 select new FormInstanceViewModel
                                 {
                                     FormInstanceID = fi.FormInstanceID,
                                     FolderVersionID = fi.FolderVersionID,
                                     FormDesignID = fi.FormDesignID,
                                     FormDesignName = String.IsNullOrEmpty(fi.Name) ? fi.FormDesign.FormName : fi.Name,
                                     TenantID = fi.TenantID,
                                     FormDesignVersionID = fi.FormDesignVersionID,
                                     AnchorDocumentID = fi.AnchorDocumentID
                                 }).ToList();

                vbidFormInstances = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     join fia in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     on fi.AnchorDocumentID equals fia.FormInstanceID
                                     where formInstanceIDs.Contains(fi.FormInstanceID) && fi.IsActive && fi.FormDesignID == 2409
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = fi.FormInstanceID,
                                         FolderVersionID = fi.FolderVersionID,
                                         FormDesignID = fi.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(fia.Name) ? fi.FormDesign.FormName : fia.Name,
                                         TenantID = fi.TenantID,
                                         FormDesignVersionID = fi.FormDesignVersionID,
                                         AnchorDocumentID = fi.AnchorDocumentID
                                     }).ToList();
                if (vbidFormInstances != null && vbidFormInstances.Count > 0)
                {
                    formInstances.AddRange(vbidFormInstances);
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstances;
        }
        public List<FormInstanceViewModel> getFormInstancDataList(List<int> formInstanceIDs)
        {
            List<FormInstanceViewModel> modelList = new List<FormInstanceViewModel>();
            List<FormInstanceDataMap> dataMaps = new List<FormInstanceDataMap>();
            dataMaps = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().GetFormInstanceDataList(formInstanceIDs);
            modelList = (from a in dataMaps
                         select new FormInstanceViewModel
                         {
                             FormInstanceDataMapID = a.FormInstanceDataMapID,
                             FormInstanceID = a.FormInstanceID,
                             ObjectInstanceID = a.ObjectInstanceID,
                             FormData = a.FormData
                         }).ToList();
            return modelList;
        }

        public List<FormInstanceViewModel> getFormInstanceDataList(List<int> formInstanceIDs)
        {
            List<FormInstanceViewModel> modelList = new List<FormInstanceViewModel>();
            modelList = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                       .Query()
                                       .Get()
                         join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get() on fv.FolderVersionID equals fi.FolderVersionID
                         where formInstanceIDs.Contains(fi.FormInstanceID)
                         select new FormInstanceViewModel
                         {
                             FolderID = fv.FolderID,
                             FolderVersionID = fv.FolderVersionID,
                             FormInstanceID = fi.FormInstanceID
                         }).ToList();
            return modelList;
        }

        /// <summary>
        /// Gets the JSON object from DB from which the PlaceHolder values for the report are parsed.
        /// When the formDesignID is not passed it's the Main Document otherwise it would be admin or other related Document.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="formDesignID"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetFormInstanceReportData(int tenantId, int formInstanceID, int folderVersionID, List<int> ApplicableDocumentIDsForReport, ref int CurrentFormDesignID, string ReportName)
        {
            Dictionary<int, string> ApplicableDocumentJSONsForReport = new Dictionary<int, string>();
            try
            {

                switch (ReportName)
                {

                    case GlobalVariables.BenefitMatrix:
                        CurrentFormDesignID = GlobalVariables.MedicalDesignID;
                        break;
                    case "SBC":
                        CurrentFormDesignID = GlobalVariables.MedicalDesignID;
                        break;
                    case "SPD":
                        CurrentFormDesignID = GlobalVariables.MedicalDesignID;
                        break;
                    case GlobalVariables.FaxBack:
                        CurrentFormDesignID = GlobalVariables.MedicalDesignID;
                        break;
                    case GlobalVariables.DentalMatrix:
                        CurrentFormDesignID = GlobalVariables.DentalDesignID;
                        break;
                    case GlobalVariables.DentalFaxBack:
                        CurrentFormDesignID = GlobalVariables.DentalDesignID;
                        break;
                    case GlobalVariables.VisionMatrix:
                        CurrentFormDesignID = GlobalVariables.VisionDesignID;
                        break;
                    case GlobalVariables.VisionFaxBack:
                        CurrentFormDesignID = GlobalVariables.VisionDesignID;
                        break;
                    case GlobalVariables.STDMatrix:
                        CurrentFormDesignID = GlobalVariables.STDDesignID;
                        break;
                    case GlobalVariables.BenAdminBenefitMatrix:
                        CurrentFormDesignID = GlobalVariables.BenAdminID;
                        break;

                    default:
                        break;
                }

                int CurrentFDesignId = CurrentFormDesignID;

                List<FormInstance> formInstanceForCCM = (from ins in this._unitOfWork.Repository<FormInstance>().Get()
                                                         where ins.FolderVersionID == folderVersionID
                                                         && ins.FormDesignID == CurrentFDesignId && ins.IsActive
                                                         select ins
                                                         ).ToList();

                if (formInstanceForCCM.Any())
                {
                    var currentformInstanceID = formInstanceForCCM.Where(i => i.FormInstanceID == formInstanceID).FirstOrDefault();
                    if (currentformInstanceID == null)
                    {
                        formInstanceID = formInstanceForCCM.FirstOrDefault().FormInstanceID;
                    }
                }



                FormInstance formInstanceForReportDoc = (from ins in this._unitOfWork.Repository<FormInstance>().Get()
                                                         where ins.FolderVersionID == folderVersionID
                                                         && ins.FormInstanceID == formInstanceID && ins.IsActive
                                                         select ins
                                                               ).FirstOrDefault();


                if (formInstanceForReportDoc != null)
                {
                    CurrentFormDesignID = formInstanceForReportDoc.FormDesignID;

                    foreach (int FormDesignId in ApplicableDocumentIDsForReport)
                    {
                        FormInstanceDataMap formInstanceData = (from ins in this._unitOfWork.Repository<FormInstance>().Get()
                                                                join insData in this._unitOfWork.Repository<FormInstanceDataMap>().Get() on ins.FormInstanceID equals insData.FormInstanceID
                                                                where ins.FolderVersionID == folderVersionID && ((CurrentFDesignId == FormDesignId && ins.FormInstanceID == formInstanceID) || CurrentFDesignId != FormDesignId)
                                                                && ins.FormDesignID == FormDesignId && ins.IsActive
                                                                select insData
                                                                ).FirstOrDefault();

                        if (formInstanceData != null)
                        {
                            if (string.IsNullOrEmpty(formInstanceData.FormData) && FormDesignId == CurrentFormDesignID)
                                throw new ReportErrorMessage("Please save the current Form to proceed with the Report Generation.");
                            else
                                ApplicableDocumentJSONsForReport.Add(FormDesignId, formInstanceData.FormData);
                        }
                    }
                }
                else
                {
                    throw new ReportErrorMessage("Folder does not contains all required documents!");
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return ApplicableDocumentJSONsForReport;
        }

        public int GetSourceFormInstanceID(int formInstanceID, int formDesignVersionID, int folderVersionID, int sourceFormDesignID)
        {
            //determine if source form is from the same folder
            //if so, get form instance id for the source form and return
            //if not, get the effective date of the current folder version
            //and get the latest finalized form instance id before that effective date
            int sourceFormInstanceID = 0;
            var sourceGroup = (from grpSource in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>()
                                        .Query()
                                        .Filter(c => c.FormID == sourceFormDesignID)
                                        .Get()
                               select grpSource).FirstOrDefault();

            var formGroup = (from grpSource in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                        .Query()
                                        .Include(c => c.FormDesign.FormDesignGroupMappings)
                                        .Filter(d => d.FormDesignVersionID == formDesignVersionID)
                                        .Get()
                             select grpSource.FormDesign.FormDesignGroupMappings).FirstOrDefault().FirstOrDefault();

            if (sourceGroup != null && formGroup != null)
            {
                if (sourceGroup.FormDesignGroupID == formGroup.FormDesignGroupID)
                {
                    //same folder
                    if ((bool)sourceGroup.AllowMultipleInstance)
                    {
                        List<FormDesignMapping> frmMapping = (from frm in this._unitOfWork.RepositoryAsync<FormDesignMapping>()
                                       .Query()
                                       .Filter(c => c.AnchorDesignID == sourceFormDesignID || c.TargetDesignID == sourceFormDesignID)
                                       .Get()
                                                              select frm).ToList();

                        if (frmMapping.Count > 0)
                        {
                            sourceFormInstanceID = this.GetAnchorViewFormInstanceId(formInstanceID, sourceFormDesignID, folderVersionID, frmMapping, formGroup.FormID);
                        }
                    }
                    else
                    {
                        var formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                        .Query()
                                        .Filter(c => c.FormDesignID == sourceFormDesignID && c.FolderVersionID == folderVersionID && c.IsActive == true)
                                        .Get()
                                        select frm.FormInstanceID).FirstOrDefault();


                        sourceFormInstanceID = formInst;
                    }
                }
                else
                {
                    bool isMasterList = (from frm in _unitOfWork.RepositoryAsync<FormDesign>()
                                         .Query()
                                         .Filter(c => c.FormID == sourceGroup.FormID && c.IsActive == true)
                                         .Get()
                                         select frm.IsMasterList).FirstOrDefault();

                    if (isMasterList)
                    {
                        sourceFormInstanceID = this.GetMasterListFormInstanceId(formInstanceID, sourceFormDesignID, folderVersionID);
                    }

                }
            }
            return sourceFormInstanceID;
        }

        public int GetSourceFormDesignVersionId(int FormInstanceId)
        {
            var designVerId = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                           .Query()
                                           .Filter(c => c.FormInstanceID == FormInstanceId)
                                           .Get()
                               select frm.FormDesignVersionID).FirstOrDefault();

            return designVerId;
        }

        public int GetMasterListFormInstanceId(int formInstanceID, int sourceFormDesignID, int folderVersionID)
        {
            int sourceFormInstanceID = 0;
            //TODO: effective date code to determine form instance from another folder
            var form = (from frmInst in this._unitOfWork.RepositoryAsync<FormInstance>()
                       .Query()
                       .Include(c => c.FolderVersion)
                       .Filter(d => d.FormInstanceID == formInstanceID && d.IsActive == true)
                       .Get()
                        select frmInst).FirstOrDefault();
            if (form != null)
            {
                //Check If Any FolderVersion is Released from given FormDesignId, If there are
                //multiple released versions pick the latest Released Version where EffectiveDate 
                //should be lesser than or equal to Folder Version EffectiveDate
                //else pick the latest Minor Version.

                //var releasedWorkflowState = _unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersionID);

                var listOfVersions = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                     .Query()
                                     .Include(c => c.FolderVersion)
                                     .Filter(h => h.FormDesignID == sourceFormDesignID &&
                                            h.FolderVersion.EffectiveDate <= form.FolderVersion.EffectiveDate &&
                                            //h.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID &&
                                            h.FolderVersion.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                                            h.IsActive == true)
                                     .Get()
                                      select frm.FolderVersion.FolderVersionNumber).ToList();
                var maxFolderVersionNumber = "";
                dynamic formInst = null;
                if (listOfVersions.Count() > 0)
                {
                    List<int> yearList = new List<int>();
                    yearList = (from version in listOfVersions select Convert.ToInt32(version.Split('_')[0])).ToList();

                    Double maxVersion = (from a in listOfVersions.Where(r => r.Contains(yearList.Max().ToString())).ToList()
                                         select Convert.ToDouble(a.Split('_')[1])).Max();


                    for (int i = 0; i < listOfVersions.Count; i++)
                    {
                        if (listOfVersions[i].Contains(maxVersion.ToString()) && listOfVersions[i].Contains(yearList.Max().ToString()))
                            maxFolderVersionNumber = listOfVersions[i];
                    }

                    formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                    .Query()
                                    .Include(c => c.FolderVersion)
                                    .Filter(h => h.FormDesignID == sourceFormDesignID &&
                                                h.FolderVersion.EffectiveDate <= form.FolderVersion.EffectiveDate &&
                                                //h.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID &&
                                                h.FolderVersion.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                                                h.IsActive == true &&
                                                h.FolderVersion.FolderVersionNumber == maxFolderVersionNumber)
                                    .Get()
                                select frm).FirstOrDefault();
                }

                if (formInst != null)
                {
                    sourceFormInstanceID = formInst.FormInstanceID;
                }
                else
                {
                    var latestformInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                               .Query()
                                               .Include(c => c.FolderVersion)
                                               .Filter(h => h.FormDesignID == sourceFormDesignID && h.FolderVersion.EffectiveDate <= form.FolderVersion.EffectiveDate && h.IsActive == true)
                                               .OrderBy(o => o.OrderByDescending(h => h.FolderVersion.EffectiveDate))
                                               .Get()
                                          select frm).FirstOrDefault();

                    if (latestformInst != null)
                        sourceFormInstanceID = latestformInst.FormInstanceID;
                }
            }
            return sourceFormInstanceID;
        }

        private int GetAnchorViewFormInstanceId(int formInstanceID, int sourceFormDesignID, int folderVersionID, List<FormDesignMapping> frmMapping, int targerFormId)
        {
            int sourceFormInstanceID = 0;
            FormDesignMapping sourceAsAnchorDesign = frmMapping.Where(c => c.AnchorDesignID == sourceFormDesignID).FirstOrDefault();
            //If Source is Anchor 
            if (sourceAsAnchorDesign != null)
            {
                int? formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                           .Query()
                           .Filter(c => c.FormInstanceID == formInstanceID && c.IsActive == true)
                           .Get()
                                 select frm.AnchorDocumentID).FirstOrDefault();


                sourceFormInstanceID = Convert.ToInt32(formInst);
            }
            else
            {
                FormDesignMapping isTargetAnchorDesign = frmMapping.Where(c => c.AnchorDesignID == targerFormId).FirstOrDefault();
                //Target is Anchor Document
                if (isTargetAnchorDesign != null)
                {
                    int? formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                              .Query()
                              .Filter(c => c.AnchorDocumentID == formInstanceID && c.FormDesignID == sourceFormDesignID)
                              .Get()
                                     select frm.FormInstanceID).FirstOrDefault();


                    sourceFormInstanceID = Convert.ToInt32(formInst);
                }
                else
                {
                    //If Source and Target both are Views
                    int? anchorId = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                  .Query()
                                  .Filter(c => c.FormInstanceID == formInstanceID)
                                  .Get()
                                     select frm.AnchorDocumentID).FirstOrDefault();

                    int? formInst = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                             .Query()
                             .Filter(c => c.AnchorDocumentID == anchorId && c.FormDesignID == sourceFormDesignID)
                             .Get()
                                     select frm.FormInstanceID).FirstOrDefault();

                    sourceFormInstanceID = Convert.ToInt32(formInst);
                }
            }
            return sourceFormInstanceID;
        }

        public int CopyFormInstance(int tenantId, int folderVersionId, int formInstanceId, string formName, bool isNewVersion, bool isManualCopy, string addedBy, int anchorFormInstanceID = 0, bool isAsyncCall = true, List<DocumentFilterResult> filterDocumentResults = null)
        {
            //copy form design version for selected form instance
            var formDesign = this._unitOfWork.RepositoryAsync<FormInstance>()
                                          .Query()
                                          .Filter(c => c.TenantID == tenantId && c.FormInstanceID == formInstanceId && c.IsActive == true)
                                          .Get().SingleOrDefault();


            int newformInstanceId = 0;
            if (formDesign != null)
            {
                formName = string.IsNullOrEmpty(formName) ? formDesign.Name : formName;

                // a new form instance with same form design version.
                newformInstanceId = AddFormInstance(tenantId, folderVersionId, formDesign.FormDesignVersionID, formDesign.FormDesignID, formName, addedBy, anchorFormInstanceID);

                if (newformInstanceId > 0)
                {
                    //using (var scope = new TransactionScope())
                    {
                        //Copy form design data for selected form instance
                        List<FormInstanceDataMap> formInstanceDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                .Query()
                                                                                .Filter(c => c.FormInstanceID == formInstanceId)
                                                                                .Get().ToList();
                        if (formInstanceDataMapList != null && formInstanceDataMapList.Count > 0)
                        {
                            //create data mapping for newly created form instance.
                            foreach (FormInstanceDataMap map in formInstanceDataMapList)
                            {
                                string newFormData = map.FormData;
                                if (!string.IsNullOrEmpty(newFormData) && isManualCopy)
                                {
                                    if (formDesign.FormDesignID == GlobalVariables.MedicareFormDesignID)
                                    {
                                        JObject source = JObject.Parse(newFormData);
                                        if (source != null)
                                        {
                                            source.SelectToken(GlobalVariables.ContractNumberSectionPath)[GlobalVariables.ContractNumber] = GetProxyNumber(newformInstanceId);
                                            source.SelectToken(GlobalVariables.MiscellaneousSectionPath)[GlobalVariables.IsPBPImport] = "No";
                                        }
                                        newFormData = JsonConvert.SerializeObject(source);
                                    }
                                }
                                if (formDesign.FormDesignID != GlobalVariables.MILESTONECHECKLISTFORMDESIGNID || !isNewVersion)
                                {
                                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().SaveFormInstanceDataCompressed(newformInstanceId, newFormData);
                                    this._unitOfWork.Save();

                                    //Update Reporting Database of new form instance
                                    //if (filterDocumentResults != null)
                                    //{
                                    //    DocumentFilterResult result = filterDocumentResults.Where(a => a.FormInstanceID == formInstanceId).FirstOrDefault();
                                    //    if (result == null)
                                    //    {
                                    //        UpdateReportingCenterDatabase(newformInstanceId, formInstanceId, isAsyncCall);
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    UpdateReportingCenterDatabase(newformInstanceId, formInstanceId, isAsyncCall);
                                    //}
                                }
                            }
                        }
                        //Copy AccountProduct related to old folderVersion  to newly created FolderVersion
                        CopyAccountProductMap(folderVersionId, addedBy, formInstanceId, newformInstanceId, isManualCopy);

                        //Copy AccountProduct related to old folderVersion  to newly created FolderVersion
                        CopyPBPDetailsMap(folderVersionId, addedBy, formInstanceId, newformInstanceId, isManualCopy);

                        //CopyFrom Audit trail
                        SaveCopyFromAuditTrail(formInstanceId, folderVersionId, newformInstanceId, addedBy, isManualCopy);


                        //scope.Complete();
                    }
                }
            }

            return newformInstanceId;
        }

        public void UpdateReportingCenterDatabase(int formInstanceId, int? oldFormInstanceID, bool isAsyncCall = true)
        {
            try
            {
                bool isValidFormInstance = true;  //oldFormInstanceID != null ? CheckOldFormInstanceExistsIntoReportingDatabase(oldFormInstanceID) : true;
                if (isValidFormInstance)
                {
                    FormInstance formInstance = _unitOfWork.RepositoryAsync<FormInstance>().Get().Where(x => x.FormInstanceID == formInstanceId).FirstOrDefault();
                    FormDesign formDesign = this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(x => x.FormID == formInstance.FormDesignID).FirstOrDefault();
                    FolderVersion folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(x => x.FolderVersionID == formInstance.FolderVersionID).FirstOrDefault();
                    FormInstanceDataMap formInstanceDataMap = _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get().Where(x => x.FormInstanceID == formInstanceId).FirstOrDefault();
                    if (formInstance != null && formDesign != null && folderVersion != null && formInstanceDataMap != null)
                    {
                        if (isAsyncCall)
                        {
                            _logger.Debug("Data Save Beign for FormInstanceId '" + formInstanceId + "' on ReportingCenter database.");
                            _reportingDataService.RunAsync(new JData { FormInstanceId = formInstanceId, FormInstanceName = formInstance.Name, FormDesignId = formInstance.FormDesignID, FormDesignVersionId = formInstance.FormDesignVersionID, FormData = formInstanceDataMap.FormData, FolderID = folderVersion.FolderID, FolderVersionID = formInstance.FolderVersionID, EffectiveDate = folderVersion.EffectiveDate, IsMasterList = formDesign.IsMasterList, AnchorDocumentID = formInstance.AnchorDocumentID });
                        }
                        else
                        {
                            _logger.Debug("Sync Data Save Beign for FormInstanceId '" + formInstanceId + "' on ReportingCenter database.");
                            _reportingDataService.Run(new JData { FormInstanceId = formInstanceId, FormInstanceName = formInstance.Name, FormDesignId = formInstance.FormDesignID, FormDesignVersionId = formInstance.FormDesignVersionID, FormData = formInstanceDataMap.FormData, FolderID = folderVersion.FolderID, FolderVersionID = formInstance.FolderVersionID, EffectiveDate = folderVersion.EffectiveDate, IsMasterList = formDesign.IsMasterList, AnchorDocumentID = formInstance.AnchorDocumentID });
                        }
                    }
                    else
                    {
                        var documentTracker = _mDMSyncDataService.GetDocumentUpdateTrackerStatusByFormInstanceId(formInstanceId);
                        if (documentTracker != null)
                        {
                            documentTracker.Status = 4;
                            documentTracker.UpdatedDate = DateTime.Now;
                            _mDMSyncDataService.UpdateDocumentUpdateTracker(documentTracker);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _mDMSyncDataService.AddLogForMDMProcess(new MDMLog() { ForminstanceID = formInstanceId, FormDesignID = 0, FormDesignVersionID = 0, AddedDate = DateTime.Now, Error = ex.Message, ErrorDescription = ex.StackTrace.ToString() });
                _logger.ErrorException("ReportingCenter database data update transaction fail In UpdateReportingCenterDatabase(). FormInstanceId='" + formInstanceId + "'", ex);
            }
        }

        private bool CheckOldFormInstanceExistsIntoReportingDatabase(int? formInstanceId)
        {
            DataSet ds = new DataSet();
            string sqlStatement = string.Format("SELECT TOP 1 * FROM [RPT].[FormInstanceDetail] WHERE FormInstanceId = {0} ", formInstanceId);
            using (SqlConnection connection = new SqlConnection(Config.GetReportingCenterConnectionString()))
            {
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlStatement, connection))
                {
                    adapter.SelectCommand.CommandTimeout = 3600;
                    adapter.Fill(ds);
                }
            }
            DataTable dt = ds.Tables.Count > 0 ? ds.Tables[0] : null;
            return dt != null && dt.Rows.Count > 0 ? true : false;
        }
        private string GetProxyNumber(int formInstanceId)
        {
            string proxyNumber = string.Empty;

            var objProxy = this._unitOfWork.RepositoryAsync<FormInstanceProxyNumber>().Get().Where(s => s.FormInstanceID == formInstanceId || s.IsUsed == false).FirstOrDefault();

            if (objProxy != null)
            {
                proxyNumber = objProxy.ProxyNumber;
                if (objProxy.FormInstanceID == null)
                {
                    objProxy.FormInstanceID = formInstanceId;
                    objProxy.IsUsed = true;
                    this._unitOfWork.RepositoryAsync<FormInstanceProxyNumber>().Update(objProxy);
                    this._unitOfWork.Save();
                }
            }

            return proxyNumber;
        }
        private void CopyPBPDetailsMap(int newfolderVersionID, string addedBy, int oldFormInstanceId, int newFormInstanceId, bool isManualCopy)
        {
            //Get All AccountProductMap based on oldFolderVersionId
            PBPImportDetails importdetails = this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                                                                     .Query()
                                                                     .Filter(c => c.FormInstanceID == oldFormInstanceId && c.IsActive == true)
                                                                     .Get()
                                                                     .FirstOrDefault();

            //create data mapping for newly created AccountProductMap.
            //Copy all data of AccountProductMap with newFolderVersionID 
            if (importdetails != null)
            {
                PBPImportDetails import = new PBPImportDetails();
                import.FolderVersionId = newfolderVersionID;
                import.CreatedBy = addedBy;
                import.CreatedDate = DateTime.Now;
                import.FolderId = this._unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == newfolderVersionID).Get().Select(c => c.FolderID).FirstOrDefault();
                import.ebsPlanName = importdetails.ebsPlanName;
                import.FormInstanceID = newFormInstanceId;
                import.DocId = this._unitOfWork.Repository<FormInstance>().Query().Filter(c => c.FormInstanceID == newFormInstanceId).Get().Select(c => c.DocID).FirstOrDefault(); ;
                import.IsActive = true;
                if (isManualCopy)
                {
                    import.QID = String.Empty;
                    import.ebsPlanNumber = GetProxyNumber(newFormInstanceId);
                    //assign QId as proxy plannumber for case of manul copy
                    import.QID = import.ebsPlanNumber;
                    import.PlanName = String.Empty;
                    import.PlanNumber = String.Empty;
                    import.PBPDatabase = String.Empty;
                    import.PBPDatabase1Up = 0;
                    import.IsIncludeInEbs = false;
                    import.Status = 0;
                    import.PBPImportQueueID = 0;
                    import.Year = 0;
                    import.UserAction = 0;
                }
                else
                {
                    import.ebsPlanName = importdetails.ebsPlanName;
                    import.ebsPlanNumber = importdetails.ebsPlanNumber;
                    import.PlanName = importdetails.PlanName;
                    import.PlanNumber = importdetails.PlanNumber;
                    import.PBPImportQueueID = importdetails.PBPImportQueueID;
                    import.QID = importdetails.QID;
                    import.Status = importdetails.Status;
                    import.Year = importdetails.Year;
                    import.PBPDatabase = importdetails.PBPDatabase;
                    import.UserAction = importdetails.UserAction;
                    import.PBPDatabase1Up = importdetails.PBPDatabase1Up;
                    import.IsIncludeInEbs = importdetails.IsIncludeInEbs;
                }
                this._unitOfWork.RepositoryAsync<PBPImportDetails>().Insert(import);
                this._unitOfWork.Save();

                if (!isManualCopy)
                {
                    importdetails.IsActive = false;
                    importdetails.UpdatedBy = addedBy;
                    importdetails.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Update(importdetails);
                    this._unitOfWork.Save();
                }
            }
        }

        public string EmptyAuditCheckList(JObject source)
        {
            string defaultObject = @"{
                        'IsAuditRequired': {
                            'IsAuditRequired': ''
                        },
                        'GeneralAuditInfo': {
                            'QID': '',
                            'GroupID': '',
                            'GroupName': '',
                            'AssignedTo': '',
                            'AuditType': '',
                            'SpecifyOther': ''
                        },
                        'CheckListDetails': {
                            'Product': '',
                            'ProductQCError': '',
                            'ProductPoints': '0',
                            'RX': '',
                            'RXQCError': '',
                            'RXPoints': '0',
                            'Dental': '',
                            'DentalQCError': '',
                            'DentalPoints': '0',
                            'Vision': '',
                            'VisionQCError': '',
                            'VisionPoints': '0',
                            'Stoploss': '',
                            'StoplossQCError': '',
                            'StoplossPoints': '0',
                            'DEDE': '',
                            'DEDEQCError': '',
                            'DEDEPoints': '0',
                            'LTLT': '',
                            'LTLTQCError': '',
                            'LTLTPoints': '0',
                            'EBCL': '',
                            'EBCLQCError': '',
                            'EBCLPoints': '0',
                            'SEPY1': '',
                            'SEPY1QCError': '',
                            'SEPY1Points': '0',
                            'SEPY2': '',
                            'SEPY2QCError': '',
                            'SEPY2Points': '0',
                            'SEPY3': '',
                            'SEPY3QCError': '',
                            'SEPY3Points': '0',
                            'SEPY4': '',
                            'SEPY4QCError': '',
                            'SEPY4Points': '0',
                            'SEPY5': '',
                            'SEPY5QCError': '',
                            'SEPY5Points': '0',
                            'SEPY6': '',
                            'SEPY6QCError': '',
                            'SEPY6Points': '0',
                            'BSBSBSDLDescription': '',
                            'VerifyprefixdescriptionmachesProductprefixdescription': '',
                            'Checkbenefitsummarytextforbenefitvariationscopaymentsandofpayment': '',
                            'Checkbenefitdetailandverifyamountslimitamountsandcoinsurancearecorrect': '',
                            'Blank': '',
                            'BSBSBSDL': '',
                            'BSBSBSDLQCError': '',
                            'BSBSBSDLPoints': '0',
                            'HRAAdminInfo': '',
                            'HRAAdminInfoQCError': '',
                            'HRAAdminInfoPoints': '0',
                            'HRAAllocationRules': '',
                            'HRAAllocationRulesQCError': '',
                            'HRAAllocationRulesPoints': '0',
                            'MARISDescription': '',
                            'VerifyProductIDHRAAdminPrefixanddates': '',
                            'MARIS': '',
                            'MARISQCError': '',
                            'MARISPoints': '0',
                            'TotalPoint': '0',
                            'BlankAudit': '',
                            'BlankAudit1': '',
                            'AuditScore': '0'
                        },
                        'GeneralComments': {
                            'GeneralComments': ''
                        },
                        'ApprovalDetails': {
                            'CompletedBy': '',
                            'DateCompleted': ''
                        }
                    }";

            source["AuditChecklist"] = JObject.Parse(defaultObject);
            return JsonConvert.SerializeObject(source);
        }

        public FolderVersionViewModel GetCurrentFolderVersionML(string formDesignName)
        {
            FolderVersionViewModel model = null;

            var folderVersionList = (from deg in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                     join ins in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     on deg.FormID equals ins.FormDesignID
                                     join fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     on ins.FolderVersionID equals fldver.FolderVersionID
                                     join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                     on fldver.FolderID equals fld.FolderID
                                     where ins.IsActive && fldver.IsActive && deg.IsMasterList && deg.IsActive
                                     && ((!string.IsNullOrEmpty(formDesignName) && ((deg.FormName.Replace(" ", "") == formDesignName) || (deg.DisplayText == formDesignName)))
                                      || (string.IsNullOrEmpty(formDesignName)))

                                     select new FolderVersionViewModel
                                     {
                                         AddedBy = fldver.AddedBy,
                                         AddedDate = fldver.AddedDate,
                                         Comments = fldver.Comments,
                                         EffectiveDate = fldver.EffectiveDate,
                                         FolderId = fldver.FolderID,
                                         FolderName = fld.Name,
                                         FolderVersionId = fldver.FolderVersionID,
                                         FolderVersionNumber = fldver.FolderVersionNumber,
                                         IsActive = fldver.IsActive,
                                         TenantID = fldver.TenantID,
                                         FolderVersionStateID = fldver.FolderVersionStateID,
                                         FolderType = deg.FormName,
                                         FormDesignDisplayText = deg.DisplayText
                                     }).OrderBy(h => h.FormDesignDisplayText).ThenByDescending(h => h.FolderVersionId)
                                       .ToList();

            if (folderVersionList != null)
            {
                foreach (var folderVersion in folderVersionList)
                {
                    if (folderVersion.FolderVersionStateID != (int)FolderVersionState.INPROGRESS_BLOCKED)
                    {
                        model = folderVersion;
                        break;
                    }
                }
            }
            return model;
        }

        //public FolderVersionViewModel GetFolderVersionML(int folderId, int folderVersionId)
        //{
        //    FolderVersionViewModel model = null;


        //    model = (from fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
        //                             join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
        //                             on fldver.FolderID equals fld.FolderID
        //                            where fldver.FolderID == folderId && fldver.FolderVersionID == folderVersionId
        //                             select new FolderVersionViewModel
        //                             {
        //                                 AddedBy = fldver.AddedBy,
        //                                 AddedDate = fldver.AddedDate,
        //                                 Comments = fldver.Comments,
        //                                 EffectiveDate = fldver.EffectiveDate,
        //                                 FolderId = fldver.FolderID,
        //                                 FolderName = fld.Name,
        //                                 FolderVersionId = fldver.FolderVersionID,
        //                                 FolderVersionNumber = fldver.FolderVersionNumber,
        //                                 IsActive = fldver.IsActive,
        //                                 TenantID = fldver.TenantID,
        //                                 FolderVersionStateID = fldver.FolderVersionStateID,
        //                             }).FirstOrDefault();

        //    return model;
        //}

        public FolderVersionViewModel GetCurrentFolderVersion(int folderId, int folderVersionId = 0)
        {
            FolderVersionViewModel model = null;


            var folderVersionList = (from fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                     on fldver.FolderID equals fld.FolderID
                                     where fldver.IsActive && fld.FolderID == folderId && fldver.FolderVersionID == folderVersionId
                                     select new FolderVersionViewModel
                                     {
                                         AddedBy = fldver.AddedBy,
                                         AddedDate = fldver.AddedDate,
                                         Comments = fldver.Comments,
                                         EffectiveDate = fldver.EffectiveDate,
                                         FolderId = fldver.FolderID,
                                         FolderName = fld.Name,
                                         FolderVersionId = fldver.FolderVersionID,
                                         FolderVersionNumber = fldver.FolderVersionNumber,
                                         IsActive = fldver.IsActive,
                                         TenantID = fldver.TenantID,
                                         FolderVersionStateID = fldver.FolderVersionStateID,

                                     }).OrderByDescending(h => h.FolderVersionId)
                                       .ToList();

            if (folderVersionList != null)
            {
                foreach (var folderVersion in folderVersionList)
                {
                    if (folderVersion.FolderVersionStateID != (int)FolderVersionState.INPROGRESS_BLOCKED)
                    {
                        model = folderVersion;
                        break;
                    }
                }
            }
            return model;
        }

        public List<string> GetProductIDListByFolderVersion(int folderVersionId)
        {
            List<string> productIds = new List<string>();
            try
            {
                productIds = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                          .Query()
                                                                          .Filter(c => c.FolderVersionID == folderVersionId)
                                                                          .Get()
                                                                          .Select(c => c.ProductID).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return productIds;
        }

        /// <summary>
        /// This method is added for soft delete of Form Instance.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public ServiceResult DeleteFormInstance(int folderId, int tenantId, int folderVersionId, int formInstanceId, string updatedBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                List<FormInstance> formInstances = new List<FormInstance>();
                if (this.IsFolderVersionInProgress(folderId, folderVersionId, tenantId))
                {
                    formInstances = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                        .Query()
                                                                        .Include(c => c.AccountProductMaps)
                                                                        .Filter(c => (c.FormInstanceID == formInstanceId || c.AnchorDocumentID == formInstanceId) && c.TenantID == tenantId)
                                                                        .Get()
                                                                        .ToList();
                    foreach (var formInstanceToDelete in formInstances)
                    {
                        if (formInstanceToDelete.FormDesignID == GlobalVariables.PRODUCTFORMDESIGNID)
                        {
                            //ProductReferenceDeleteFormInstance(formInstanceToDelete.FormInstanceID);
                        }
                        if (formInstanceToDelete != null)
                        {

                            //Soft Delete AccountProductMap related to this FormInstance
                            foreach (var accountProductMap in formInstanceToDelete.AccountProductMaps)
                            {
                                accountProductMap.IsActive = false;
                                this._unitOfWork.RepositoryAsync<AccountProductMap>().Update(accountProductMap);
                            }

                            if (formInstanceToDelete.AnchorDocumentID == formInstanceToDelete.FormInstanceID)
                            {
                                _planTaskUserMappingService.DeletePlanTaskUserMappingByFormInstanceId(folderVersionId, formInstanceId);
                            }
                            // Soft Delete the FormInstance.

                            formInstanceToDelete.UpdatedDate = DateTime.Now;
                            formInstanceToDelete.IsActive = false;
                            formInstanceToDelete.UpdatedBy = updatedBy;

                            this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstanceToDelete);
                            this._unitOfWork.Save();
                            result.Result = ServiceResultStatus.Success;

                            this.UpdateFolderChange(tenantId, updatedBy, folderId, folderVersionId);
                        }
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure; ;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Can not delete a document in Released Folder Version" } });
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }

        //Get the status of current user for Lock/unlock
        public FolderVersionViewModel GetFolderLockStatus(int tenantId, int? folderId, int? userId)
        {
            FolderVersionViewModel model = null;
            try
            {
                model = (from e in _unitOfWork.RepositoryAsync<FolderLock>()
                                  .Query()
                                  .Include(c => c.User)
                                  .Filter(fil => fil.TenantID == tenantId &&
                                          fil.FolderID == folderId)
                                  .Get()
                         select new FolderVersionViewModel
                         {
                             LockedBy = e.LockedBy,
                             IsLocked = e.IsLocked,
                             LockedByUser = e.User.UserName,
                             LockedDate = e.LockedDate,
                         }).FirstOrDefault();

                if (model != null && model.LockedBy != userId)
                {
                    DateTime DateEnd = DateTime.Now;
                    DateTime DateStart = DateEnd - new TimeSpan(0, 0, 3, 0);

                    if (model.LockedDate <= DateStart)
                    {
                        ServiceResult result = null;
                        result = ReleaseFolderLock(0, tenantId, folderId);

                        if (result.Result == ServiceResultStatus.Success)
                        {
                            model = null;
                        }
                    }
                }
                //if same user requests the locked folder.
                //if (model != null)
                //{
                //    if (model.LockedBy == userId)
                //    {
                //        model.IsLocked = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return model;
        }

        /// <summary>
        /// Add and update the lock for folder version
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ServiceResult UpdateFolderLockStatus(int? userId, int tenantId, int? folderId)
        {
            ServiceResult result = null;

            try
            {
                result = new ServiceResult();
                var folderLockStatus = _unitOfWork.RepositoryAsync<FolderLock>()
                                    .Query()
                                    .Filter(fil => fil.FolderID == folderId &&
                                         fil.LockedBy == userId)
                                    .Get()
                                    .OrderBy(ord => ord.FolderLockID)
                                    .FirstOrDefault();

                if (folderLockStatus != null)
                {
                    //for refresh
                    if (folderLockStatus.IsLocked == true)
                    {
                        result.Result = ServiceResultStatus.Success;
                    }
                }
                else
                {
                    //if folders entry is not in the folder lock table.
                    if (userId != null && userId != 0)
                    {
                        FolderLock folderLock = new FolderLock();
                        folderLock.IsLocked = true;
                        folderLock.LockedBy = userId;
                        folderLock.TenantID = tenantId;
                        folderLock.FolderID = folderId;
                        folderLock.LockedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FolderLock>().Insert(folderLock);
                        this._unitOfWork.Save();
                    }
                    result.Result = ServiceResultStatus.Success;

                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                // folder lock may fail with the current implementation, but not critical, so not rethrowing exception
                //if (reThrow)
                //    throw;
            }
            return result;
        }

        public ServiceResult ReleaseFolderLock(int? userId, int tenantId, int? folderId)
        {
            ServiceResult result = null;
            FolderLock folderlockToDelete = null;
            try
            {
                result = new ServiceResult();
                folderlockToDelete = _unitOfWork.RepositoryAsync<FolderLock>()
                                  .Query()
                                  .Filter(fil => fil.TenantID == tenantId &&
                                       fil.FolderID == folderId &&
                                       fil.LockedBy == userId)
                                  .Get()
                                  .OrderBy(ord => ord.FolderLockID)
                                  .FirstOrDefault();

                if (userId == 0)
                {
                    folderlockToDelete = _unitOfWork.RepositoryAsync<FolderLock>()
                                  .Query()
                                  .Filter(fil => fil.TenantID == tenantId &&
                                       fil.FolderID == folderId)
                                  .Get()
                                  .OrderBy(ord => ord.FolderLockID)
                                  .FirstOrDefault();

                }
                if (folderlockToDelete != null)
                {
                    this._unitOfWork.RepositoryAsync<FolderLock>().Delete(folderlockToDelete);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    //((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }

        public ServiceResult OverrideFolderLock(int? userId, int tenantId, int? folderId)
        {
            ServiceResult result = null;
            FolderLock folderlockToDelete = null;
            try
            {
                result = new ServiceResult();
                folderlockToDelete = _unitOfWork.RepositoryAsync<FolderLock>()
                                  .Query()
                                  .Filter(fil => fil.TenantID == tenantId &&
                                       fil.FolderID == folderId)
                                  .Get()
                                  .OrderBy(ord => ord.FolderLockID)
                                  .FirstOrDefault();

                if (folderlockToDelete != null)
                {
                    this._unitOfWork.RepositoryAsync<FolderLock>().Delete(folderlockToDelete);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder is already unlocked." } });
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }
        /// <summary>
        /// This method will update the locked date after every minute,
        /// untill the user is editing the folder version.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckFolderLockIsOverriden(int folderId, int? userId)
        {
            var folderLockIsOverriden = this._unitOfWork.Repository<FolderLock>()
                                                    .Query()
                                                    .Filter(c => c.FolderID == folderId
                                                        && c.LockedBy == userId)
                                                    .Get()
                                                    .ToList();
            if (folderLockIsOverriden.Count != 0)
            {
                //Update the folder lock date time with latest time.            
                FolderLock folderlck = folderLockIsOverriden.First();
                folderlck.LockedDate = DateTime.Now;
                _unitOfWork.RepositoryAsync<FolderLock>().Update(folderlck);
                _unitOfWork.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method returns true if document 
        /// is used as data source for other documents in the folder
        /// </summary>
        /// <param name="formDesignID"></param>
        /// <param name="formDesignVersionID"></param>
        public bool IsDataSource(int formDesignID, int formDesignVersionID)
        {
            bool isDataSource = false;
            try
            {
                isDataSource = (from ds in this._unitOfWork.Repository<DataSource>().Get()
                                join dsm in this._unitOfWork.Repository<DataSourceMapping>().Get()
                                on ds.DataSourceID equals dsm.DataSourceID
                                where (ds.FormDesignVersionID == formDesignVersionID &&
                                       ds.FormDesignVersionID != dsm.FormDesignVersionID)
                                select new { FormDesignVersionID = dsm.FormDesignVersionID }).Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isDataSource;
        }
        public ServiceResult DeleteFolderVersion(int tenantId, int folderId, int folderVersionId, string versionType, string userName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                if (versionType == VersionType.New.ToString())
                {
                    this.DeleteFolderVersion(tenantId, folderVersionId);

                    result.Result = ServiceResultStatus.Success;

                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                    {
                        Messages = new string[] { folderVersionId.ToString() }
                    });
                }
                else if (versionType == VersionType.Retro.ToString())
                {
                    result = this.DeleteFolderVersionRetroChanges(tenantId, folderId);
                    if (result.Result == ServiceResultStatus.Success)
                    {
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                        {
                            Messages = new string[] { "Folder version Cannot be Deleted" }
                        });

                        return result;
                    }
                }
                //using (var scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    this.UpdateFolderChange(tenantId, userName, folderId, null);
                    //scope.Complete();

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult DeleteFolder(int tenantId, int folderId)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                Folder folderToDelete = this._unitOfWork.RepositoryAsync<Folder>()
                                                                     .Query()
                                                                     .Include(c => c.FolderVersions)
                                                                     .Filter(c => c.FolderID == folderId && c.TenantID == tenantId)
                                                                     .Get()
                                                                     .SingleOrDefault();
                if (folderToDelete != null)
                {
                    //Delete Folder
                    this.DeleteFolder(folderToDelete, tenantId);

                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                    }

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public void UpdateFormInstanceWithEffectiveFormDesignVersion(string userName, int effectiveFormDesignVersionId, int formInstanceId)
        {
            try
            {
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                       .Query()
                                       .Filter(fil => fil.FormInstanceID == formInstanceId && fil.IsActive == true).Get().FirstOrDefault();

                if (formInstance != null)
                {
                    if (formInstance.FormDesignVersionID != effectiveFormDesignVersionId)
                    {
                        formInstance.FormDesignVersionID = effectiveFormDesignVersionId;
                        formInstance.UpdatedBy = userName;
                        formInstance.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstance);

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }

        public bool IsAnyFolderVersionInProgress(int folderId, int tenantId)
        {
            bool isReleased = false;
            try
            {
                isReleased = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                    .IsAnyFolderVersionInProgress(folderId, tenantId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isReleased;
        }

        #region Baselining Folder Version

        // Get all major version lsit for specific folderID  
        // List will include next generated major version number
        public IEnumerable<FolderVersionViewModel> GetMajorFolderVersionList(int tenantId, int folderId, string userName,
            string versionNumber, DateTime effectiveDate)
        {
            List<FolderVersionViewModel> majorVersionList = null;
            FolderVersionViewModel toCreateMajorVersion = null;
            VersionNumberBuilder builder = null;
            try
            {

                //List of all available major folder version for specific folderId
                var majorVersion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Include(inc => inc.FolderVersionState)
                                              .Filter(c => c.TenantID == tenantId && c.FolderID == folderId &&
                                                c.FolderVersionState.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                              .Get()
                                    select new FolderVersionViewModel
                                    {
                                        FolderVersionId = c.FolderVersionID,
                                        FolderVersionNumber = c.FolderVersionNumber,
                                        EffectiveDate = c.EffectiveDate,
                                        ReleaseDate = c.UpdatedDate,
                                        TenantID = c.TenantID,
                                        UserName = c.AddedBy,
                                        Comments = c.Comments
                                    }).OrderByDescending(c => c.FolderVersionId);

                builder = new VersionNumberBuilder();
                //Validate FolderVersionNumber
                var isValid = _unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(versionNumber);


                majorVersionList = majorVersion.ToList();
                if (isValid)
                {
                    var updatedVersionNumber = builder.GetNextMajorVersionNumber(versionNumber, effectiveDate);

                    // Generated next major folder version
                    toCreateMajorVersion = new FolderVersionViewModel();
                    toCreateMajorVersion.FolderVersionNumber = updatedVersionNumber;
                    toCreateMajorVersion.EffectiveDate = effectiveDate;
                    toCreateMajorVersion.ReleaseDate = DateTime.Now;
                    toCreateMajorVersion.TenantID = tenantId;
                    toCreateMajorVersion.UserName = userName;
                    majorVersionList.Add(toCreateMajorVersion);
                }
                else
                {
                    throw new ArgumentException("FolderVersion number is not in proper format");
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            // return major folder version list with descending order
            return majorVersionList.OrderByDescending(o => o.ReleaseDate);
        }

        // Get all minor version list for specific folderID  
        // List will include next generated minor version number
        public IEnumerable<FolderVersionViewModel> GetMinorFolderVersionList(int tenantId, int folderId, string ownerName, string userName, bool isBaseLine)
        {
            List<FolderVersionViewModel> minorVersionList = null;
            VersionNumberBuilder builder = null;

            try
            {
                builder = new VersionNumberBuilder();

                //List of all available minor folder versions for specific folderId
                var minorVersion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Include(inc => inc.FolderVersionState)
                                              .Filter(c => c.TenantID == tenantId &&
                                                c.FolderID == folderId &&
                                                c.FolderVersionState.FolderVersionStateID != (int)FolderVersionState.RELEASED)
                                              .Get()
                                    select new FolderVersionViewModel
                                    {
                                        FolderVersionId = c.FolderVersionID,
                                        FolderVersionNumber = c.FolderVersionNumber,
                                        EffectiveDate = c.EffectiveDate,
                                        BaseLineDate = c.AddedDate,
                                        UserName = c.AddedBy,
                                        Comments = c.Comments,
                                        FolderVersionStateID = c.FolderVersionStateID
                                    }).OrderBy(ord => ord.FolderVersionStateID).ThenByDescending(c => c.FolderVersionId);

                if (minorVersion != null)
                {
                    minorVersionList = minorVersion.ToList();
                }
                if (isBaseLine && minorVersionList.Count() > 0)
                {
                    var effectiveDate = minorVersionList.FirstOrDefault().EffectiveDate;
                    var versionNumber = minorVersionList.FirstOrDefault().FolderVersionNumber;

                    //Validate FolderVersionNumber
                    var isValid = this._unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(versionNumber);

                    if (isValid)
                    {
                        var updatedVersionNumber = builder.GetNextMinorVersionNumber(versionNumber, effectiveDate);
                        // Generated next minor folder version
                        FolderVersionViewModel toCreateMinorVersion = new FolderVersionViewModel();
                        toCreateMinorVersion.FolderVersionId = 0;
                        toCreateMinorVersion.FolderVersionNumber = updatedVersionNumber;
                        toCreateMinorVersion.BaseLineDate = DateTime.Now;
                        toCreateMinorVersion.TenantID = tenantId;
                        toCreateMinorVersion.UserName = userName;
                        minorVersionList.Add(toCreateMinorVersion);
                    }
                    else
                    {
                        throw new ArgumentException("FolderVersion number is not in proper format");
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            // return minor folder version list with descending order
            return minorVersionList.OrderByDescending(o => o.BaseLineDate);
        }

        public FolderVersionViewModel GetLatestFolderVersion(int tenantId, int folderId)
        {
            FolderVersionViewModel folderVersion = null;

            try
            {
                folderVersion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.FolderID == folderId)
                                              .Get()
                                              .OrderByDescending(f => f.FolderVersionID)
                                 select new FolderVersionViewModel
                                 {
                                     FolderVersionId = c.FolderVersionID,
                                     FolderVersionNumber = c.FolderVersionNumber,
                                     EffectiveDate = c.EffectiveDate,
                                     BaseLineDate = c.AddedDate,
                                     UserName = c.AddedBy,
                                     Comments = c.Comments,
                                     FolderVersionStateID = c.FolderVersionStateID
                                 }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersion;
        }

        public ServiceResult BaseLineFolderForCompareSync(int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId,
            string addedBy, string versionNumber, string comments, Nullable<int> consortiumID, DateTime? effectiveDate,
            bool isRelease, bool isNotApproved, bool isNewVersion, List<DocumentFilterResult> filterDocumentResults = null)
        {
            VersionNumberBuilder builder = new VersionNumberBuilder();
            string newVersionNumber = builder.GetNextMinorVersionNumber(versionNumber, effectiveDate.Value);
            return this.BaseLineFolder(tenantId, notApprovedWorkflowStateId, folderId, folderVersionId, userId,
                            addedBy, newVersionNumber, comments, consortiumID, effectiveDate, isRelease: false, isNotApproved: false, isNewVersion: false, filterDocumentResults: filterDocumentResults);
        }


        public ServiceResult BaseLineFolder(int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId,
            string addedBy, string versionNumber, string comments, Nullable<int> consortiumID, DateTime? effectiveDate,
            bool isRelease, bool isNotApproved, bool isNewVersion, bool isAsyncCall = true, List<DocumentFilterResult> filterDocumentResults = null)
        {
            ServiceResult result = null;

            try
            {

                result = new ServiceResult();

                //Get List FormInstance id related to specific folderVersion
                List<FormInstanceViewModel> formInstancesList;
                int newFolderVersionId;
                var isMasterList = IsMasterList(folderId);
                //check for multiple in Progress Folder Versions
                IEnumerable<FolderVersionViewModel> folderVersions = GetFolderVersionList(tenantId, folderId);
                if (folderVersions != null && folderVersions.Count() > 0)
                {
                    var ipProgFVs = (from folder in folderVersions
                                     where folder.FolderVersionStateID == (int)FolderVersionState.INPROGRESS
                                     select folder).ToList();
                    if (ipProgFVs != null && ipProgFVs.Count > 1)
                    {
                        string versions = string.Empty;
                        foreach (var item in ipProgFVs)
                        {
                            versions += item.FolderVersionId.ToString() + ",";
                        }
                        throw new Exception("This Folder has multiple (" + versions + ") In Progress Versions. This Folder Version cannot be Baselined." + ipProgFVs.Count.ToString());
                    }
                }

                if (isMasterList)
                {
                    formInstancesList = GetFormInstanceList(tenantId, folderVersionId, folderId, Convert.ToInt32(DocumentDesignTypes.MASTERLIST));
                }
                else
                {
                    formInstancesList = GetAnchorFormInstanceList(tenantId, folderVersionId);
                }


                //using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                {
                    //Copy all the record related olderFolderversion to New FolderVersion 
                    //Returns newly generated FolderVersion id
                    if (isMasterList)
                    {
                        newFolderVersionId = CopyMasterListFolderVersion(tenantId, notApprovedWorkflowStateId, folderVersionId, addedBy, versionNumber,
                                                                        comments, effectiveDate, isRelease, isNotApproved, isNewVersion, consortiumID);
                    }
                    else
                    {
                        newFolderVersionId = CopyFolderVersion(tenantId, notApprovedWorkflowStateId, folderVersionId, addedBy, versionNumber,
                                                    comments, effectiveDate, isRelease, isNotApproved, isNewVersion, consortiumID);
                    }

                    foreach (var forminstance in formInstancesList)
                    {
                        //Copy all form instances related to olderfolderVersion  to newly created FolderVersion
                        int newFormInstanceId = CopyFormInstance(tenantId, newFolderVersionId, forminstance.FormInstanceID, string.Empty, isNewVersion, false, addedBy, 0, isAsyncCall, filterDocumentResults: filterDocumentResults);
                        if (!isMasterList)
                        {
                            var childFormInstanceList = GetDocumentViewList(tenantId, forminstance.FormInstanceID);
                            if (childFormInstanceList != null)
                            {
                                foreach (var childFormInstance in childFormInstanceList)
                                {
                                    if (childFormInstance.FormInstanceId != forminstance.FormInstanceID)
                                    {
                                        CopyFormInstance(tenantId, newFolderVersionId, childFormInstance.FormInstanceId, string.Empty, isNewVersion, false, addedBy, newFormInstanceId, filterDocumentResults: filterDocumentResults);
                                    }
                                }
                            }
                        }

                    }
                    //  this.SaveCopyFromAuditByVersion(NewFolderVersionId, folderVersionId, folderId);
                    //Copy all workflowstates related to olderfolderVersion  to newly created FolderVersion
                    CopyFolderVersionWorkFlowState(tenantId, folderVersionId, newFolderVersionId, addedBy, userId, comments, isRelease, isNotApproved, notApprovedWorkflowStateId);

                    if (!isNewVersion)
                    {
                        //Copy all applicable teams related to olderfolderVersion  to newly created FolderVersion
                        CopyFolderVersionApplicableTeam(tenantId, folderVersionId, newFolderVersionId, addedBy, userId);
                    }

                    if (!isNewVersion)
                    {
                        //Copy all users related to workflowstates of olderfolderVersion to newly created FolderVersion's workflowstates
                        CopyWorkFlowStateUserMap(tenantId, folderVersionId, newFolderVersionId, addedBy, userId);
                    }

                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem { Messages = new string[] { newFolderVersionId.ToString(), "Baseline" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Success;
                    this.UpdateFolderChange(tenantId, addedBy, folderId, folderVersionId);
                    //Update the Tasks related to oldFolderversion
                    UpdateTaskMappingOnBaseliningtheFoldervresion(folderVersionId, newFolderVersionId, addedBy, versionNumber);
                    //scope.Complete();

                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }


        /// <summary>
        /// This method will update the tasks when the folderversion is baselined (All related tasks will be pointing to new minor folder version).
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="oldFolderVersionID"></param>
        /// <param name="newFolderVersionID"></param>
        /// <param name="addedBy"></param>
        /// <param name="versionNumber"></param>
        private void UpdateTaskMappingOnBaseliningtheFoldervresion(int oldFolderVersionID, int newFolderVersionID, string addedBy, string versionNumber)
        {
            try
            {
                var plantasksForOldFolderversion = (from planUserTask in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(row => row.FolderVersionID == oldFolderVersionID)
                                                    select planUserTask).ToList();

                foreach (var oldplantask in plantasksForOldFolderversion)
                {
                    if (oldplantask.PlanTaskUserMappingDetails != null)
                    {
                        var oldplanTaskUserMappingDetails = JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(oldplantask.PlanTaskUserMappingDetails);
                        var newTraverseDetails = oldplanTaskUserMappingDetails.TaskTraverseDetails;
                        var oldFormInstanceids = String.IsNullOrEmpty(oldplanTaskUserMappingDetails.FormInstanceId) ? null : oldplanTaskUserMappingDetails.FormInstanceId.Split(',').ToArray();
                        string newFormInstanceIds = string.Empty;
                        foreach (var oldFormInstanceid in oldFormInstanceids)
                        {
                            var forminstanceID = Convert.ToInt32(oldFormInstanceid);
                            var oldFormInstanceDesignVersion = ((from fInstanceOld in _unitOfWork.Repository<FormInstance>().Get().Where(row => row.FormInstanceID == forminstanceID) select fInstanceOld.FormDesignVersionID).FirstOrDefault());
                            var newFormInstanceId = (from formInstance in this._unitOfWork.Repository<FormInstance>().Get().Where(row => row.FolderVersionID == newFolderVersionID && row.FormDesignVersionID == oldFormInstanceDesignVersion)
                                                     select formInstance.FormInstanceID).FirstOrDefault();
                            newFormInstanceIds = newFormInstanceIds + newFormInstanceId.ToString() + ",";
                        }
                        oldplanTaskUserMappingDetails.FormInstanceId = newFormInstanceIds.Substring(0, newFormInstanceIds.Length - 1);
                        if (!String.IsNullOrEmpty(oldplanTaskUserMappingDetails.TaskTraverseDetails))
                        {
                            newTraverseDetails = string.Empty;
                            var oldTraverseDetails = oldplanTaskUserMappingDetails.TaskTraverseDetails.Split(',');
                            var oldformInstanceIdText = oldTraverseDetails[1].Split(':')[1];
                            int oldformInstanceId = Convert.ToInt32(oldformInstanceIdText);
                            var oldFormInstanceDesignVersion = ((from fInstanceOld in _unitOfWork.Repository<FormInstance>().Get().Where(row => row.FormInstanceID == oldformInstanceId) select fInstanceOld.FormDesignVersionID).FirstOrDefault());
                            var newFormInstanceId = (from formInstance in this._unitOfWork.Repository<FormInstance>().Get().Where(row => row.FolderVersionID == newFolderVersionID && row.FormDesignVersionID == oldFormInstanceDesignVersion)
                                                     select formInstance.FormInstanceID).FirstOrDefault();
                            newTraverseDetails = oldTraverseDetails[0] + "," + oldTraverseDetails[0].Split(':')[0] + ":" + newFormInstanceId.ToString() + "," + oldTraverseDetails[1];
                        }
                        oldplanTaskUserMappingDetails.TaskTraverseDetails = newTraverseDetails;
                        oldplantask.PlanTaskUserMappingDetails = JsonConvert.SerializeObject(oldplanTaskUserMappingDetails);
                    }
                    oldplantask.FolderVersionID = newFolderVersionID;
                    oldplantask.UpdatedBy = addedBy;
                    oldplantask.UpdatedDate = DateTime.Now;
                    oldplantask.Id = oldplantask.ID;
                    _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(oldplantask, true);

                    //Add the comment for updated task
                    //TaskComments taskComment = new TaskComments();
                    //taskComment.TaskID = oldplantask.ID;
                    //taskComment.Comments = "System has updated this task to latest folder version " + versionNumber;
                    //taskComment.AddedBy = addedBy;
                    //taskComment.AddedDate = DateTime.Now;
                    //taskComment.FolderVersionID = newFolderVersionID;
                    //this._unitOfWork.RepositoryAsync<TaskComments>().Insert(taskComment);
                    _unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }
        #endregion

        #region Saving FormInstance Data

        public ServiceResult SaveFormInstanceData(int tenantId, int folderVersionId, int formInstanceId, string formInstanceData,
            string enteredBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormInstanceDataMap formInstanceDataMap = null;
                var fiMap = (from c in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                                                    .Get()
                             select new
                             {
                                 FormInstanceDataMapID = c.FormInstanceDataMapID,
                                 FormInstanceID = c.FormInstanceID,
                                 ObjectInstanceID = c.ObjectInstanceID
                             }).FirstOrDefault();
                if (fiMap != null)
                {
                    //using (var scope = new TransactionScope())
                    {
                        formInstanceDataMap = new FormInstanceDataMap();
                        formInstanceDataMap.FormInstanceDataMapID = fiMap.FormInstanceDataMapID;
                        formInstanceDataMap.FormInstanceID = fiMap.FormInstanceID;
                        formInstanceDataMap.ObjectInstanceID = fiMap.FormInstanceID;
                        formInstanceDataMap.FormData = formInstanceData;
                        this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                        using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                        {
                            this._unitOfWork.Save();
                            scope.Complete();
                        }

                        FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                        .Query()
                                                                        .Filter(c => c.FormInstanceID == formInstanceId)
                                                                        .Get()
                                                                        .FirstOrDefault();
                        if (formInstance != null)
                        {
                            formInstance.UpdatedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstance);
                        }

                        SaveFormInstanceHistory(formInstanceId, formInstanceData, tenantId, enteredBy, "Update");
                        result.Result = ServiceResultStatus.Success;
                        //scope.Complete();
                    }
                }
                else
                {
                    //Since we are adding the data first time to the table. 
                    //this field needs to hold the value 0. 
                    int objectInstanceID = 0;

                    result = this.AddFormInstanceDataMap(formInstanceId, objectInstanceID, formInstanceData, tenantId, enteredBy);
                }

                this.UpdateFolderChange(tenantId, enteredBy, null, folderVersionId);
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }


        public bool SaveFormInstanceDataCompressed(int formInstanceID, string formData)
        {
            return this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().SaveFormInstanceDataCompressed(formInstanceID, formData);
        }

        public bool SaveFormInstanceDataCompressed(int formInstanceID, string formData, int folderVersionId, string userName)
        {
            bool result = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().SaveFormInstanceDataCompressed(formInstanceID, formData);
            this.UpdateFolderChange(1, userName, null, folderVersionId);
            return result;
        }

        public ServiceResult SaveMultipleFormInstancesData(int tenantId, int folderVersionId, List<FormInstanceViewModel> multipleFormInstances, List<JToken> formsData, string enteredBy, int userId)
        {
            ServiceResult result = null;
            try
            {
                foreach (var instance in multipleFormInstances)
                {
                    if (instance.FormDesignID != 3)
                    {
                        continue;
                    }
                    result = new ServiceResult();
                    FormInstanceDataMap formInstanceDataMap = null;
                    if (instance != null)
                    {
                        JToken form = formsData.FirstOrDefault(c => c["FormInsatnceID"].ToString() == Convert.ToString(instance.FormInstanceID));
                        //using (var scope = new TransactionScope())
                        {
                            formInstanceDataMap = new FormInstanceDataMap();
                            formInstanceDataMap.FormInstanceDataMapID = instance.FormInstanceDataMapID;
                            formInstanceDataMap.FormInstanceID = instance.FormInstanceID;
                            formInstanceDataMap.ObjectInstanceID = instance.ObjectInstanceID;
                            formInstanceDataMap.FormData = form["FormData"].ToString() == "" ? instance.FormData : form["FormData"].ToString();
                            this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                            {
                                this._unitOfWork.Save();
                                scope.Complete();
                            }

                            FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == instance.FormInstanceID)
                                                                            .Get()
                                                                            .FirstOrDefault();
                            if (formInstance != null)
                            {
                                formInstance.UpdatedDate = DateTime.Now;
                                this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstance);
                            }

                            SaveFormInstanceHistory(instance.FormInstanceID, instance.FormData, tenantId, enteredBy, "Update");
                            result.Result = ServiceResultStatus.Success;
                            //scope.Complete();
                        }
                    }
                    else
                    {
                        //Since we are adding the data first time to the table. 
                        //this field needs to hold the value 0. 
                        int objectInstanceID = 0;

                        result = this.AddFormInstanceDataMap(instance.FormInstanceID, objectInstanceID, instance.FormData, tenantId, enteredBy);
                    }
                }
                this.UpdateFolderChange(tenantId, enteredBy, null, folderVersionId);
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        /// <summary>
        /// This Save FormInstance Medthod used on Product Migration.
        /// </summary>
        /// <param name="formInstanceID">FormInstanceID</param>
        /// <param name="objectInstanceID"></param>
        /// <param name="formData">FormData</param>
        /// <param name="tenantId"></param>
        /// <param name="enteredBy"></param>
        /// <returns></returns>
        public ServiceResult SaveFacetFormInstanceDataMap(int formInstanceID, int objectInstanceID, string formInstanceData, int tenantId, string enteredBy, string compressedData)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                if (compressedData != null)
                {
                    FormInstanceDataMap newFormInstanceDataMap = new FormInstanceDataMap();
                    newFormInstanceDataMap.FormInstanceID = formInstanceID;
                    newFormInstanceDataMap.ObjectInstanceID = objectInstanceID;
                    newFormInstanceDataMap.CompressJsonData = compressedData;
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(newFormInstanceDataMap);
                    this._unitOfWork.Save();
                    this._unitOfWork.Clear<FormInstanceDataMap>(newFormInstanceDataMap);
                }
                else
                {
                    FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                   .Query()
                                                                   .Filter(c => c.FormInstanceID == formInstanceID)
                                                                   .Get()
                                                                   .FirstOrDefault();
                    if (formInstanceDataMap != null && formInstanceDataMap.FormData == null)
                    {
                        formInstanceDataMap.FormData = DataCompress.Decompress(formInstanceDataMap.CompressJsonData);
                        this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                        this._unitOfWork.Save();
                    }
                    this._unitOfWork.Clear<FormInstanceDataMap>(formInstanceDataMap);
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return result;
        }

        /// <summary>
        /// This method is used to insert only new user activity log data to database(Fldr.FormInstanceActivityLog)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="folderId"></param>
        /// <param name="folderVersionId"></param>
        /// <param name="formDesignId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="formInstanceActivityLogData"></param>
        /// <returns></returns>
        public ServiceResult SaveFormInstanceAvtivitylogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<ActivityLogModel> activityLogFormInstanceData)
        {
            ServiceResult result = null;

            result = new ServiceResult();
            try
            {
                if (activityLogFormInstanceData != null)
                {
                    var formInstance = (from fi in this._unitOfWork.Repository<FormInstance>().Query().Filter(c => c.FormInstanceID == formInstanceId).Get()
                                        select new
                                        {
                                            DocID = fi.DocID,
                                            FormDesignID = fi.FormDesignID
                                        }).FirstOrDefault();

                    var activityLogNewRecords = activityLogFormInstanceData.Where(c => c.IsNewRecord == true).ToList();
                    for (int i = 0; i < activityLogNewRecords.Count; i++)
                    {
                        FormInstanceActivityLog activityLog = new FormInstanceActivityLog();
                        activityLog.FormInstanceID = formInstanceId;
                        activityLog.FolderID = folderId;
                        activityLog.FolderVersionID = folderVersionId;
                        activityLog.FormDesignID = formDesignId == 0 ? formInstance.FormDesignID : formDesignId;
                        activityLog.FormDesignVersionID = formDesignVersionId;
                        //activityLog.ElementPath = activityLogNewRecords[i].SubSectionName;
                        activityLog.ElementPath = activityLogNewRecords[i].ElementPath;
                        activityLog.Field = activityLogNewRecords[i].Field;
                        activityLog.RowNumber = activityLogNewRecords[i].RowNum;
                        activityLog.Description = activityLogNewRecords[i].Description;
                        activityLog.AddedBy = activityLogNewRecords[i].UpdatedBy;
                        activityLog.AddedDate = DateTime.Now;
                        activityLog.UpdatedBy = activityLogNewRecords[i].UpdatedBy;
                        activityLog.UpdatedLast = DateTime.Now;
                        activityLog.IsNewRecord = true;
                        activityLog.DocID = formInstance.DocID;
                        this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Insert(activityLog);
                    }
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }

        public ServiceResult UpdateFormInstanceData(int formInstanceId, int objectInstanceID)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormInstanceDataMap formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                                                    .Get()
                                                                    .FirstOrDefault();

                if (formInstance != null)
                {
                    formInstance.ObjectInstanceID = objectInstanceID;
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstance);
                    this._unitOfWork.Save();

                    //UpdateReportingCenterDatabase(formInstanceId, null);

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        #endregion

        #region Version Management

        /// <summary>
        /// Create new minor folder version.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderId">The original folder identifier.</param>
        /// <param name="folderVersionId">The original folder version identifier.</param>
        /// <param name="versionNumber">versionNumber of the folder.</param>
        /// <param name="comments">comments</param>
        /// <param name="effectiveDate">effectiveDate</param>
        /// <param name="isRelease">isRelease</param>
        /// <param name="userId">userId</param>
        /// <param name="consortiumID">consortiumID</param>
        /// <param name="userName">userName</param>
        /// <returns>ServiceResult serviceResult</returns>
        public ServiceResult CreateNewMinorVersion(int tenantId, int folderId, int folderVersionId, string versionNumber,
                                                    string comments, DateTime? effectiveDate, bool isRelease,
                                                    int userId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string userName)
        {
            ServiceResult serviceResult = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            VersionNumberBuilder builder = new VersionNumberBuilder();
            try
            {
                //Validate FolderVersionNumber
                var isValid = _unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(versionNumber);
                if (isValid)
                {
                    var nextVersionNumber = builder.GetNextMinorVersionNumber(versionNumber, effectiveDate.Value);
                    comments = String.Format(CommentsData.NewVersionComments, userName, nextVersionNumber);
                    var folderVersion = CreateNewMinorVersion(tenantId, 0, folderId, folderVersionId, userId, userName, nextVersionNumber, comments, consortiumID, categoryID, catID, effectiveDate);
                    List<FormInstanceViewModel> formList = GetFormInstanceList(tenantId, folderVersion.FolderVersionID, folderId);

                    //List<CopyFromAuditTrail> copyFromAuditTrails = _unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Get().Where(x => x.FolderID == folderId && x.SourceFolderVersionID == folderVersionId && x.DestinationFolderVersionID == folderVersion.FolderVersionID).ToList();
                    //foreach (CopyFromAuditTrail copyFromAuditTrail in copyFromAuditTrails)
                    //UpdateReportingCenterDatabase(copyFromAuditTrail.DestinationDocumentID, copyFromAuditTrail.SourceDocumentID);

                    if (null != folderVersion)
                    {
                        items.Add(new ServiceResultItem { Messages = new string[] { folderVersion.FolderVersionID.ToString(), "Baseline" } });
                        serviceResult.Items = items;
                        serviceResult.Result = ServiceResultStatus.Success;
                    }
                }
                else
                {
                    throw new NotSupportedException("VersionNumber of previous Folder Version is not in proper format");
                }
            }
            catch (NotSupportedException ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
                if (reThrow)
                    throw;
            }
            return serviceResult;
        } //CreateNewMinorVersion


        /// <summary>
        /// </summary>
        /// <param name="tenantID">tenantID</param>
        /// <param name="notApprovedWorkflowStateId">notApprovedWorkflowStateId</param>
        /// <param name="originalFolderID">originalFolderID</param>
        /// <param name="originalFolderVersionID">originalFolderVersionID</param>
        /// <param name="userID">userID</param>
        /// <param name="currentUserName">currentUserName</param>
        /// <param name="newMiorVersionNumber">newMiorVersionNumber</param>
        /// <param name="comments">comments</param>
        /// <param name="userName">userName</param>
        /// <param name="consortiumID">consortiumID</param>
        /// <param name="consortiumID">categoryID</param>
        /// <param name="consortiumID">catID</param>
        /// <param name="effectiveDate">effectiveDate</param>
        /// <returns>FolderVersion</returns>
        private FolderVersion CreateNewMinorVersion(int tenantID, int? notApprovedWorkflowStateId, int originalFolderID, int originalFolderVersionID, int userID, string currentUserName, string newMiorVersionNumber,
            string comments, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, DateTime? effectiveDate)
        {
            FolderVersion folderVersion = null;
            SqlParameter paramConsortiumID = null;
            SqlParameter paramCategoryID = null;
            SqlParameter paramEffectiveDate = null;
            SqlParameter paramNotApprovedWorkflowStateId = null;
            try
            {
                SqlParameter paramTenantID = new SqlParameter("@TenantID", tenantID);
                SqlParameter paramOriginalFolderID = new SqlParameter("@OriginalFolderID", originalFolderID);
                SqlParameter paramOriginalFolderVersionID = new SqlParameter("@OriginalFolderVersionID", originalFolderVersionID);
                SqlParameter paramUserID = new SqlParameter("@UserID", userID);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", currentUserName);
                SqlParameter paramNewMiorVersionNumber = new SqlParameter("@NewMiorVersionNumber", newMiorVersionNumber);
                SqlParameter paramComments = new SqlParameter("@Comments", comments);

                //notApprovedWorkflowStateId
                if (null == notApprovedWorkflowStateId)
                    paramNotApprovedWorkflowStateId = new SqlParameter("@NotApprovedWorkflowStateId", DBNull.Value);
                else
                    paramNotApprovedWorkflowStateId = new SqlParameter("@NotApprovedWorkflowStateId", notApprovedWorkflowStateId);

                //paramConsortiumID
                if (null == consortiumID)
                    paramConsortiumID = new SqlParameter("@ConsortiumID", DBNull.Value);
                else
                    paramConsortiumID = new SqlParameter("@ConsortiumID", consortiumID);

                //paramCategoryID
                if (null == categoryID)
                    paramCategoryID = new SqlParameter("@CategoryID", DBNull.Value);
                else
                    paramCategoryID = new SqlParameter("@CategoryID", categoryID);

                SqlParameter paramCatID = new SqlParameter("@CatID", catID);

                //paramEffectiveDate
                if (null == effectiveDate)
                    paramEffectiveDate = new SqlParameter("@EffectiveDate", DBNull.Value);
                else
                    paramEffectiveDate = new SqlParameter("@EffectiveDate", effectiveDate);

                folderVersion = this._unitOfWork.Repository<FolderVersion>().ExecuteSql("exec [dbo].[USP_CreateNewMinorVersion] @TenantID ,@NotApprovedWorkflowStateId ,@OriginalFolderID ,@OriginalFolderVersionID ,@UserID ,@CurrentUserName ,@NewMiorVersionNumber ,@Comments ,@ConsortiumID, @CategoryID, @CatID, @EffectiveDate",
                                                                                    paramTenantID, paramNotApprovedWorkflowStateId, paramOriginalFolderID, paramOriginalFolderVersionID, paramUserID, paramCurrentUserName, paramNewMiorVersionNumber, paramComments, paramConsortiumID, paramCategoryID, paramCatID, paramEffectiveDate).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("CreateNewMinorVersion - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderVersion;
        } // CreateNewMinorVersion

        public IEnumerable<FolderVersionViewModel> GetVersionNumberForAccountRetroChanges(int tenantId, int folderId)
        {
            List<FolderVersionViewModel> FolderVersionList = null;

            try
            {
                FolderVersionList = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive
                                                         && c.FolderID == folderId && c.FolderVersionStateID == 3)
                                              .Get()
                                     select new FolderVersionViewModel
                                     {
                                         FolderVersionId = c.FolderVersionID,
                                         EffectiveDate = c.EffectiveDate,
                                         FolderVersionNumber = c.FolderVersionNumber,
                                         FolderId = c.FolderID,
                                         TenantID = c.TenantID,
                                         WFStateID = c.WFStateID,
                                         IsActive = c.IsActive,
                                         VersionTypeID = c.VersionTypeID,
                                         Comments = c.Comments,
                                         CategoryID = c.CategoryID
                                     }).OrderByDescending(ord => ord.FolderVersionNumber).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return FolderVersionList;
        }

        public string GetProductJsonHash(int tenantId, int formInstanceId, string productId)
        {
            string hash = string.Empty;

            var currentFolder = (from cf in this._unitOfWork.Repository<FolderVersion>().Query().Get()
                                 join fi in this._unitOfWork.Repository<FormInstance>().Query().Get()
                                 on cf.FolderVersionID equals fi.FolderVersionID
                                 where fi.FormInstanceID == formInstanceId
                                 select new { folderId = cf.FolderID, folderVersionId = cf.FolderVersionID }
                                 ).FirstOrDefault();

            FolderVersion folderVersion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                                        .Query()
                                                                        .Filter(c => c.FolderID == currentFolder.folderId && c.FolderVersionID < currentFolder.folderVersionId && c.IsActive == true)
                                                                        .Get()
                                           select c).OrderByDescending(c => c.FolderVersionID).FirstOrDefault();

            AccountProductMap accountProductMapId = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                        .Query()
                                                                        .Filter(c => c.ProductID == productId && c.FolderVersionID == folderVersion.FolderVersionID)
                                                                        .Get()
                                                                        .OrderByDescending(c => c.AccountProductMapID)
                                                                        .FirstOrDefault();
            if (accountProductMapId != null)
            {
                FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                        .Query()
                                                                        .Filter(c => c.FormInstanceID == accountProductMapId.FormInstanceID)
                                                                        .Get()
                                                                        .FirstOrDefault();
                if (formInstance != null)
                    hash = formInstance.ProductJsonHash;
            }

            return hash;
        }

        //Retro Changes
        public ServiceResult FolderVersionRetroChanges(List<RetroChangeViewModel> retroChangesList,
            IEnumerable<FolderVersionViewModel> folderVersionList, DateTime retroEffectiveDate, Nullable<int> categoryID, string catID, string userName, int userId)
        {
            ServiceResult serviceResult = null;
            VersionNumberBuilder builder = null;
            try
            {
                serviceResult = new ServiceResult();
                retroChangesList = retroChangesList.OrderBy(e => e.VersionNumber).ToList();
                folderVersionList = folderVersionList.Where(e =>
                                                      retroChangesList.Any(e1 => e1.FolderVersionId == e.FolderVersionId));

                if (folderVersionList != null && folderVersionList.Any())
                {
                    foreach (var retroChange in retroChangesList)
                    {
                        var folderVersion = folderVersionList.FirstOrDefault(e => e.FolderVersionId == retroChange.FolderVersionId);
                        if (folderVersion != null)
                        {
                            builder = new VersionNumberBuilder();

                            //retrieve Version number if both VersionNumbers having same year
                            var versionNumber = folderVersionList.Where(e =>
                                                    (builder.GetYearFromVersionNumber(e.FolderVersionNumber) ==
                                                    builder.GetYearFromVersionNumber(folderVersion.FolderVersionNumber)))
                                                            .OrderByDescending(ord => Convert.ToDouble(ord.FolderVersionNumber.Split('_')[1]))
                                                            .Select(sel => sel.FolderVersionNumber).FirstOrDefault();
                            //Validate FolderVersionNumber
                            var isValid = _unitOfWork.RepositoryAsync<FolderVersion>().IsValidFolderVersionNumber(versionNumber);

                            if (isValid)
                            {
                                var nextVersionNumber = builder.GetNextMinorVersionNumberForRetroChanges(versionNumber,
                                                                    folderVersion.EffectiveDate);
                                string comments = String.Format(CommentsData.RetroComments, userName, nextVersionNumber);

                                folderVersion.FolderVersionNumber = nextVersionNumber;
                                folderVersion.IsCopyRetro = retroChange.IsCopyRetro;
                                folderVersion.CategoryID = categoryID;
                                folderVersion.CatID = catID;
                                folderVersion.Comments = comments;

                                if (!retroChange.IsCopyRetro)
                                {
                                    folderVersion.EffectiveDate = retroChange.EffectiveDate;
                                }
                            }
                            else
                            {
                                throw new NotSupportedException("FolderVersion number is not in proper format");
                            }
                        }
                    }
                    //using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    //TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        FolderVersionBatch batch = new FolderVersionBatch();

                        batch.AddedBy = userName;
                        batch.AddedDate = DateTime.Now;
                        batch.EffectiveDate = retroEffectiveDate;

                        _unitOfWork.RepositoryAsync<FolderVersionBatch>().Insert(batch);
                        _unitOfWork.Save();

                        var effectiveFolderVersionList = folderVersionList.OrderBy(ord => ord.EffectiveDate).ToList();

                        serviceResult = this.FolderVersionRetro(effectiveFolderVersionList,
                                                batch.FolderVersionBatchID, userName, userId);

                        this._unitOfWork.Save();
                        //scope.Complete();
                    }
                }
            }
            catch (NotSupportedException ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            catch (Exception ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return serviceResult;
        }

        /// <summary>
        /// Get Impacted Folder Version List for given folderId
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<RetroChangeViewModel> GetImpactedFolderVersionList(int folderId, DateTime effectiveDate, int tenantId)
        {
            List<RetroChangeViewModel> FolderVersionList = null;

            try
            {
                FolderVersionList = new List<RetroChangeViewModel>();
                var firstfolderversion = _unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderID == folderId).Get().FirstOrDefault();
                var releasedWorkflowState = _unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, firstfolderversion.FolderVersionID);

                //Get Less than equal effective folder version
                var folderVersionList = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                        .Query()
                                        .Include(inc => inc.FolderVersionState)
                                            .Filter(fil => fil.TenantID == tenantId

                                                         && fil.FolderVersionStateID ==
                                                            (int)domain.entities.Enums.FolderVersionState.RELEASED
                                                        && fil.FolderID == folderId)
                                            .Get().ToList();

                List<RetroChangeViewModel> folderVersion = (from i in folderVersionList
                                                            where (i.EffectiveDate <= effectiveDate)
                                                            select new RetroChangeViewModel
                                                            {
                                                                FolderVersionId = i.FolderVersionID,
                                                                FolderVersionNumber = i.FolderVersionNumber,
                                                                EffectiveDate = i.EffectiveDate,
                                                                Comments = i.Comments,
                                                                VersionNumber = Convert.ToDouble(i.FolderVersionNumber.Split('_')[1])
                                                            }).OrderByDescending(e => e.EffectiveDate).ToList();

                if (folderVersion.Count() > 0)
                {
                    var maxFolderVersion = folderVersion.First();
                    //find same effiective date get highest folder version
                    var sameEffiectiveDateList = folderVersion.Where(e => e.EffectiveDate == maxFolderVersion.EffectiveDate).ToList();
                    if (sameEffiectiveDateList.Count() > 1)
                    {
                        var version = sameEffiectiveDateList.OrderByDescending(a => a.VersionNumber).FirstOrDefault();
                        FolderVersionList.Add(version);
                    }
                    else
                    {
                        var version = sameEffiectiveDateList.First();
                        FolderVersionList.Add(version);
                    }
                }

                List<RetroChangeViewModel> folderVersions = (from i in folderVersionList
                                                             where (i.EffectiveDate > effectiveDate)
                                                             select new RetroChangeViewModel
                                                             {
                                                                 FolderVersionId = i.FolderVersionID,
                                                                 FolderVersionNumber = i.FolderVersionNumber,
                                                                 EffectiveDate = i.EffectiveDate,
                                                                 Comments = i.Comments,
                                                                 VersionNumber = Convert.ToDouble(i.FolderVersionNumber.Split('_')[1])
                                                             }).ToList();

                //From folderVersions if I get set sameEffectiveDate for multiple folderVersions pick the latest order by verison number

                var effectiveFolderVersionsList = new List<RetroChangeViewModel>();
                var groupByEffiectiveDate = folderVersions.OrderBy(e => e.EffectiveDate).GroupBy(e => e.EffectiveDate).ToList();

                foreach (var eff in groupByEffiectiveDate)
                {
                    var varsion = folderVersions.Where(a => a.EffectiveDate == eff.Key).OrderByDescending(e => e.VersionNumber).ToList();
                    var highversion = varsion.FirstOrDefault();
                    effectiveFolderVersionsList.Add(highversion);
                }


                FolderVersionList.AddRange(effectiveFolderVersionsList);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            if (FolderVersionList != null)
                return FolderVersionList.OrderBy(ord => ord.FolderVersionNumber);
            else
                return null;
        }

        //Rollback FolderVersion
        public ServiceResult RollbackFolderVersion(int tenantId, int rollbackFolderVersionId, int folderId,
                                                   string rollbackFolderVersionNumber, FolderVersionViewModel inProgressFolderVersion,
                                                   int userId, string userName)
        {
            ServiceResult serviceResult = null;
            VersionNumberBuilder builder = null;
            try
            {
                serviceResult = new ServiceResult();
                builder = new VersionNumberBuilder();
                if (inProgressFolderVersion != null && inProgressFolderVersion.FolderVersionId > 0)
                {
                    serviceResult = CanRollbackFolderVersion(tenantId, rollbackFolderVersionNumber,
                                        inProgressFolderVersion.FolderVersionNumber);

                    var versionNumber = builder.GetNextMinorVersionNumber(inProgressFolderVersion.FolderVersionNumber,
                                                    inProgressFolderVersion.EffectiveDate);

                    if (serviceResult.Result == ServiceResultStatus.Failure)
                    {
                        return serviceResult;
                    }
                    else
                    {
                        //using (var scope = new TransactionScope(TransactionScopeOption.Required,
                        //TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                        {
                            ChangeFolderVersionStatus(tenantId, folderId, userName);

                            string comments = String.Format(CommentsData.RollbackComments, userName, rollbackFolderVersionNumber);

                            serviceResult = BaseLineFolder(tenantId, 0, folderId, rollbackFolderVersionId,
                                userId, userName, versionNumber, comments, 0, effectiveDate: null, isRelease: false, isNotApproved: true, isNewVersion: false);

                            _unitOfWork.Save();
                            //scope.Complete();

                            serviceResult.Result = ServiceResultStatus.Success;
                        }
                    }
                }
                else
                {
                    serviceResult = new ServiceResult();
                    ((IList<ServiceResultItem>)(serviceResult.Items)).Add(new ServiceResultItem()
                    {
                        Messages = new string[] { "InProgress minor version does not exist" }
                    });

                    return serviceResult;
                }
                serviceResult.Result = ServiceResultStatus.Success;
            }
            catch (NotSupportedException ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
            }
            catch (Exception ex)
            {
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return serviceResult;
        }

        public bool IsMasterListFolderVersionInProgress(int folderVersionId)
        {
            bool inProgress = false;
            try
            {
                if (folderVersionId > 0)
                {
                    inProgress = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Filter(fil => fil.FolderVersionID == folderVersionId &&
                                                        fil.FolderVersionStateID == (int)FolderVersionState.INPROGRESS)
                                                    .Get()
                                                    .Any();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return inProgress;
        }

        /// <summary>
        /// Set the latest FolderVersion Status from "BaseLined" to "In Progress"
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public ServiceResult ChangeFolderVersionStatus(int tenantId, int folderId, string userName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                var latestFolderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                 .Query()
                                                 .Filter(fil => fil.FolderID == folderId && fil.TenantID == tenantId)
                                                 .Get().OrderByDescending(ord => ord.FolderVersionID).FirstOrDefault();

                if (latestFolderVersion != null)
                {
                    if (latestFolderVersion.FolderVersionStateID == (int)FolderVersionState.BASELINED)
                    {
                        latestFolderVersion.FolderVersionStateID = (int)FolderVersionState.INPROGRESS;

                    }
                    else if (latestFolderVersion.FolderVersionStateID == (int)FolderVersionState.INPROGRESS)
                    {
                        latestFolderVersion.FolderVersionStateID = (int)FolderVersionState.BASELINED;
                    }
                    latestFolderVersion.UpdatedBy = userName;
                    latestFolderVersion.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(latestFolderVersion);
                    this._unitOfWork.Save();
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                throw;
            }
            return result;
        }

        public FolderVersionViewModel GetLatestMinorFolderVersion(int tenantId, int folderId)
        {
            FolderVersionViewModel inProgressMinorVersion;
            try
            {
                inProgressMinorVersion = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                   .Query()
                                                   .Filter(fil => fil.FolderID == folderId && fil.TenantID == tenantId &&
                                                              fil.FolderVersionStateID == (int)FolderVersionState.INPROGRESS)
                                                   .Get()
                                          select new FolderVersionViewModel
                                          {
                                              FolderVersionNumber = fv.FolderVersionNumber,
                                              FolderVersionId = fv.FolderVersionID,
                                              EffectiveDate = fv.EffectiveDate
                                          }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                throw;
            }
            return inProgressMinorVersion;
        }

        public ServiceResult IsValidRetroEffectiveDate(int folderID, int tenantID, DateTime retroEffectiveDate)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                var folderVersionList = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                             .Query()
                                             .Filter(fil => fil.FolderID == folderID &&
                                                            fil.TenantID == tenantID &&
                                                            fil.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                                                            fil.IsActive)
                                             .Get()
                                         select new FolderVersionViewModel()
                                         {
                                             FolderVersionId = fv.FolderVersionID,
                                             EffectiveDate = fv.EffectiveDate
                                         }).OrderBy(ord => ord.EffectiveDate).ToList();

                if (folderVersionList.Any())
                {
                    if (folderVersionList.FirstOrDefault().EffectiveDate <= retroEffectiveDate &&
                        retroEffectiveDate <= folderVersionList.LastOrDefault().EffectiveDate)
                    {
                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// This method returns true if state of FolderVersion is InProgress
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="folderVersionId"></param>
        public bool IsFolderVersionInProgress(int folderID, int folderVersionID, int tenantID)
        {
            bool isInProgress = false;
            try
            {
                isInProgress = this._unitOfWork.RepositoryAsync<FolderVersion>()
                              .IsFolderVersionInProgress(folderID, folderVersionID, tenantID);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isInProgress;
        }

        public string GetAccountName(int? accountId)
        {
            string accountName = this._unitOfWork.RepositoryAsync<Account>()
                                                  .Query()
                                                  .Filter(c => c.AccountID == accountId)
                                                  .Get()
                                                  .Select(sel => sel.AccountName).FirstOrDefault();
            return accountName;
        }
        /// <summary>
        /// Get Properties of all instances in current folderVersion using folderVersionID.
        /// </summary>
        /// <param name="folderVersionID"></param>
        /// <returns></returns>
        public List<FolderVersionPropertiesViewModel> GetFolderVersionProperties(int folderVersionID)
        {
            List<FolderVersionPropertiesViewModel> folderVersionProperties = null;
            folderVersionProperties = (from fd in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                  .Query()
                                                  .Get()
                                       join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                  .Query()
                                                                  .Get()
                                       on fd.FormID equals fdv.FormDesignID
                                       join fi in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                  .Query()
                                                                  .Get()
                                       on fdv.FormDesignVersionID equals fi.FormDesignVersionID
                                       where fi.FolderVersionID == folderVersionID && fi.IsActive == true && fi.AnchorDocumentID == fi.FormInstanceID
                                       select new FolderVersionPropertiesViewModel
                                       {
                                           FormInstanceName = fi.Name,
                                           FormDesignName = fd.FormName,
                                           DisplayText = fd.DisplayText,
                                           VersionNumber = fdv.VersionNumber
                                       }).ToList();

            return folderVersionProperties;
        }

        public List<CopyFromAuditTrailViewModel> GetCopyFromAuditTrailData(int folderVersionID)
        {
            List<CopyFromAuditTrailViewModel> copyData = null;
            var copiedFromDetails = from fd1 in this._unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Query().Filter(c => c.DestinationFolderVersionID == folderVersionID).Get()
                                        //Resolved HNE-440. As portfolio won't have account, apply left join on Account Table.
                                    join ac in this._unitOfWork.RepositoryAsync<Account>().Query().Get() on fd1.AccountID equals ac.AccountID
                                            into tmp
                                    from ac in tmp.DefaultIfEmpty()
                                    join fc in this._unitOfWork.RepositoryAsync<Folder>().Query().Get() on fd1.FolderID equals fc.FolderID
                                    join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Get() on fd1.SourceFolderVersionID equals fv.FolderVersionID
                                    join fd in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get() on fd1.DestinationDocumentID equals fd.FormInstanceID
                                    join fs in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(c => c.IsActive == true).Get() on fd1.SourceDocumentID equals fs.FormInstanceID
                                    where fs.AnchorDocumentID == fs.FormInstanceID
                                    select new
                                    {
                                        CopyFromAuditTrailID = fd1.CopyFromAuditTrailID,
                                        SourceDocumentName = fs.Name,
                                        DestinationDocumentName = fd.Name,
                                        AccountName = ac.AccountName,
                                        FolderName = fc.Name,
                                        FolderID = fc.FolderID,
                                        FolderVersionNumber = fv.FolderVersionNumber,
                                        FolderEffectiveDate = fd1.EffectiveDate,
                                        SourceFolderVersionID = fd1.SourceFolderVersionID,
                                        DestinationFolderVersionID = fd1.DestinationFolderVersionID,
                                        DestinationDocumentID = fd1.DestinationDocumentID,
                                        IsActive = fd.IsActive,
                                        AddedBy = fd1.AddedBy,
                                        AddedDate = fd1.AddedDate
                                    };
            if (copiedFromDetails != null)
            {
                copyData = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Query()
                                 .Filter(x => x.FolderVersionID == folderVersionID && x.IsActive == true && x.FormInstanceID == x.AnchorDocumentID)
                                  .Get()
                            join cd in copiedFromDetails.Where(c => c.IsActive == true) on fi.FormInstanceID equals cd.DestinationDocumentID into tempJoin
                            from t2 in tempJoin.DefaultIfEmpty()
                            select new CopyFromAuditTrailViewModel
                            {
                                CopyFromAuditTrailID = t2 != null ? t2.CopyFromAuditTrailID : 0,
                                SourceDocumentName = t2 != null ? t2.SourceDocumentName : GlobalVariables.NOTCOPYFROMDOCUMENT,
                                DestinationDocumentName = t2 != null ? t2.DestinationDocumentName : fi.Name,
                                AccountName = t2 != null ? t2.AccountName : GlobalVariables.NOTCOPYFROMDOCUMENT,
                                FolderName = t2 != null ? t2.FolderName : GlobalVariables.NOTCOPYFROMDOCUMENT,
                                FolderEffectiveDate = t2 != null ? t2.FolderEffectiveDate : null,
                                FolderVersionNumber = t2 != null ? t2.FolderVersionNumber : GlobalVariables.NOTCOPYFROMDOCUMENT,
                                FolderVersionID = t2 != null ? t2.SourceFolderVersionID : fi.FolderVersionID,
                                FolderID = t2 != null ? t2.FolderID : 0,
                                AddedBy = t2 != null ? t2.AddedBy : GlobalVariables.NOTCOPYFROMDOCUMENT,
                                AddedDate = t2 != null ? t2.AddedDate : null
                            }).ToList();
            }
            return copyData;

        }

        public int AddChildFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formDesignId, string formName, string addedBy, int anchorFormInstanceID = 0)
        {
            return AddFormInstance(tenantId, folderVersionId, formDesignVersionId, formDesignId, formName, addedBy, anchorFormInstanceID);
        }
        /// <summary>
        /// This method is used to check whether a folder is Master List or not.
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public bool IsMasterList(int folderID)
        {
            string folderName = string.Empty;
            bool isMasterList = false;
            try
            {
                var folder = (from fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                              join fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                              on fld.FolderID equals fldver.FolderID
                              join ins in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                              on fldver.FolderVersionID equals ins.FolderVersionID
                              join deg in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                              on ins.FormDesignID equals deg.FormID
                              where fld.FolderID == folderID && ins.IsActive && deg.IsActive && deg.IsMasterList
                              select fld).FirstOrDefault();

                if (folder != null)
                {
                    isMasterList = true;
                    //if (folderName == GlobalVariables.MASTERLIST_CBC || folderName == GlobalVariables.MASTERLIST_HSB)
                    //{
                    //    isMasterList = true;
                    //}
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isMasterList;
        }

        //public bool IsMasterList(int folderID)
        //{

        //    string folderName = string.Empty;
        //    bool isMasterList = false;
        //    try
        //    {
        //        folderName = this._unitOfWork.RepositoryAsync<Folder>()
        //                        .Query()
        //                        .Filter(c => c.FolderID == folderID)
        //                        .Get()
        //                        .Select(p => p.Name).FirstOrDefault();
        //        if (folderName == GlobalVariables.MASTERLIST_CBC || folderName == GlobalVariables.MASTERLIST_HSB)
        //        {
        //            isMasterList = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
        //        if (reThrow)
        //            throw;
        //    }
        //    return isMasterList;
        //}

        public bool HasFolderLockByCurrentUser(int? userId, int folderId)
        {
            bool isLocked = false;
            try
            {
                isLocked = this._unitOfWork.RepositoryAsync<FolderLock>()
                                .Query()
                                .Filter(c => c.LockedBy == userId && c.FolderID == folderId)
                                .Get()
                                .Select(p => p.IsLocked).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isLocked;
        }

        public bool IsTeamManager(int? userId)
        {
            bool isTeamManager = false;
            var applicableTeamUserMapList = (from applicableTeamUserMap in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>()
                            .Query()
                            .Filter(c => c.UserID == userId && c.IsDeleted == false && c.User.IsActive == true)
                            .Get()
                                             select applicableTeamUserMap).ToList();
            int count = applicableTeamUserMapList.Where(c => c.IsTeamManager == true).ToList().Count;

            if (count > 0)
            {
                isTeamManager = true;
            }
            return isTeamManager;
        }

        /// <summary>
        /// This method is used to retrieve activity log data from Fldr.FormInstanceActivityLog table.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        public GridPagingResponse<ActivityLogModel> GetActivityLogData(int formInstanceID, GridPagingRequest gridPagingRequest)
        {

            List<ActivityLogModel> activitylogList = null;
            int count = 0;
            string productId = string.Empty;
            //bool isProductIDValid = false;
            SearchCriteria criteria = new SearchCriteria();

            try
            {
                var formInstance = this._unitOfWork.Repository<FormInstance>().Query().Filter(c => c.FormInstanceID == formInstanceID).Get().FirstOrDefault();

                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                //productId = this.GetProductId(formInstanceID);
                //isProductIDValid = !string.IsNullOrWhiteSpace(productId);
                activitylogList = (from c in this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>()
                                  .Query()
                                  .Get()
                                  .OrderByDescending(x => x.UpdatedLast).ThenByDescending(c => c.FormInstanceActivityLogID)

                                       //join pm in this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                       //.Query()
                                       //.Get() on c.FormInstanceID equals pm.FormInstanceID

                                   join am in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                   .Query()
                                   .Get() on c.FolderVersionID equals am.FolderVersionID

                                   where (formInstance.DocID > 0 && c.DocID == formInstance.DocID)
                                      || (formInstance.DocID == 0 && c.FormInstanceID == formInstanceID)

                                   //where (isProductIDValid && pm.ProductID == productId)
                                   //     || (!isProductIDValid && c.FormInstanceID == formInstanceID)

                                   select new ActivityLogModel
                                   {
                                       Description = c.Description,
                                       SubSectionName = c.ElementPath,
                                       ElementPath = c.ElementPath,
                                       Field = c.Field,
                                       RowNum = c.RowNumber,
                                       UpdatedBy = c.UpdatedBy,
                                       UpdatedLast = c.UpdatedLast,
                                       IsNewRecord = false,
                                       FormInstanceID = c.FormInstanceID,
                                       FolderVersionName = am.FolderVersionNumber
                                   }).ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                      .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<ActivityLogModel>(gridPagingRequest.page, count, gridPagingRequest.rows, activitylogList);
        }

        /// <summary>
        /// This method is used to retrieve activity log data from Fldr.FormInstanceActivityLog table.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <returns></returns>
        public DataTable GetActivityLogData(int formInstanceID)
        {

            List<ActivityLogModel> activitylogList = null;

            string productId = string.Empty;
            DataTable ExportDataTable = null;


            try
            {
                var formInstance = this._unitOfWork.Repository<FormInstance>().Query().Filter(c => c.FormInstanceID == formInstanceID).Get().FirstOrDefault();

                //productId = this.GetProductId(formInstanceID);
                //isProductIDValid = !string.IsNullOrWhiteSpace(productId);
                activitylogList = (from c in this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>()
                                  .Query()
                                  .Get()
                                  .OrderByDescending(x => x.UpdatedLast).ThenByDescending(c => c.FormInstanceActivityLogID)

                                       //join pm in this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                       //.Query()
                                       //.Get() on c.FormInstanceID equals pm.FormInstanceID

                                   join am in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                   .Query()
                                   .Get() on c.FolderVersionID equals am.FolderVersionID

                                   where (formInstance.DocID > 0 && c.DocID == formInstance.DocID)
                                      || (formInstance.DocID == 0 && c.FormInstanceID == formInstanceID)

                                   //where (isProductIDValid && pm.ProductID == productId)
                                   //     || (!isProductIDValid && c.FormInstanceID == formInstanceID)

                                   select new ActivityLogModel
                                   {
                                       Description = c.Description,
                                       SubSectionName = c.ElementPath,
                                       ElementPath = c.ElementPath,
                                       Field = c.Field,
                                       RowNum = c.RowNumber,
                                       UpdatedBy = c.UpdatedBy,
                                       UpdatedLast = c.UpdatedLast,
                                       IsNewRecord = false,
                                       FormInstanceID = c.FormInstanceID,
                                       FolderVersionName = am.FolderVersionNumber
                                   }).ToList();

                if (activitylogList != null)
                {
                    ExportDataTable = new DataTable();
                    ExportDataTable.Columns.Add("ElementPath");
                    ExportDataTable.Columns.Add("Field");
                    ExportDataTable.Columns.Add("RowNum");
                    ExportDataTable.Columns.Add("Description");
                    ExportDataTable.Columns.Add("FolderVersionName");
                    //ExportDataTable.Columns.Add("Year");
                    ExportDataTable.Columns.Add("UpdatedBy");
                    ExportDataTable.Columns.Add("UpdatedLast");

                    foreach (var item in activitylogList)
                    {
                        DataRow row = ExportDataTable.NewRow();
                        row["ElementPath"] = item.ElementPath;
                        row["Field"] = item.Field;
                        row["RowNum"] = item.RowNum;
                        row["Description"] = item.Description.Replace("</b>", "").Replace("<b>", "");
                        row["FolderVersionName"] = item.FolderVersionName;
                        //row["Year"] = item.Year;
                        row["UpdatedBy"] = item.UpdatedBy.Replace("</b>", "").Replace("<b>", "");
                        row["UpdatedLast"] = item.UpdatedLast;
                        ExportDataTable.Rows.Add(row);

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ExportDataTable;
        }

        public List<int> GetFolderVersions(int folderId)
        {
            List<int> folderVersions = (from j in this._unitOfWork.Repository<FolderVersion>().Query()
                                            .Filter(fil => fil.FolderID == folderId && fil.IsActive == true)
                                            .Get()
                                        select j.FolderVersionID).ToList();

            return folderVersions;
        }

        #endregion Public Methods

        #region Private Methods

        private ServiceResult AddFormInstanceDataMap(int formInstanceID, int objectInstanceID, string formData, int tenantId, string enteredBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                FormInstanceDataMap formInstanceDataMap = new FormInstanceDataMap();
                formInstanceDataMap.FormInstanceID = formInstanceID;
                formInstanceDataMap.ObjectInstanceID = objectInstanceID;
                formInstanceDataMap.FormData = formData;

                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(formInstanceDataMap);
                this._unitOfWork.Save();

                SaveFormInstanceHistory(formInstanceID, formData, tenantId, enteredBy, "Add");

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        private void SaveFormInstanceHistory(int formInstanceID, string formData, int tenantId, string enteredBy, string action)
        {
            ICompressionBase compressionBase = CompressionFactory.GetCompressionFactory(CompressionType.Memory);
            byte[] jsonDataCompressed = (byte[])compressionBase.Compress(formData);

            /*byte[] jsonDataCompressed = Compression.CompressDataThroughByteArray(formData);*/

            //CheckFormInstanceHistoryOriginalJsonData(jsonDataCompressed);

            FormInstanceHistory formInstanceHistory = new FormInstanceHistory();
            formInstanceHistory.FormInstanceID = formInstanceID;
            formInstanceHistory.FormData = jsonDataCompressed;
            formInstanceHistory.TenantID = tenantId;
            formInstanceHistory.EnteredBy = enteredBy;
            formInstanceHistory.EnteredDate = DateTime.Now;
            formInstanceHistory.Action = action;
            this._unitOfWork.RepositoryAsync<FormInstanceHistory>().Insert(formInstanceHistory);
            this._unitOfWork.Save();
        }

        private int CopyFolderVersion(int tenantId, int? notApprovedWorkflowStateId, int folderVersionId, string addedBy, string versionNumber,
           string comments, DateTime? effectiveDate, bool isRelease, bool isNotApproved, bool isNewVersion, Nullable<int> consortiumID)
        {

            //Get All FolderVersion Data related to FolderVersionID           
            FolderVersion folderVersionData = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();

            FolderVersion version = null;
            if (folderVersionData != null)
            {
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderVersionData.CategoryID, folderVersionData.Folder.IsPortfolio);

                version = new FolderVersion();
                //Generate the new FolderVersionId & use FolderVersion TYpe Minor
                version.TenantID = folderVersionData.TenantID;
                if (isRelease)
                {
                    version.WFStateID = workflowState.WorkFlowVersionStateID;
                }
                else if (isNotApproved)
                {
                    version.WFStateID = (notApprovedWorkflowStateId == 0) ? workflowState.WorkFlowVersionStateID : Convert.ToInt32(notApprovedWorkflowStateId.ToString());
                    comments = String.Format(CommentsData.NewVersionComments, addedBy, versionNumber);
                    folderVersionData.FolderVersionStateID = Convert.ToInt32(FolderVersionState.BASELINED);
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);
                }
                else
                {
                    //Update FolderVErsionState to Baselined

                    //As per request WEL-1146 comment and updatedby is change
                    if (!String.IsNullOrEmpty(comments))
                    {
                        folderVersionData.Comments = comments;
                    }
                    if (!String.IsNullOrEmpty(addedBy))
                    {
                        folderVersionData.UpdatedBy = addedBy;
                    }
                    folderVersionData.FolderVersionStateID = Convert.ToInt32(tmg.equinox.domain.entities.Enums.FolderVersionState.BASELINED);
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);

                    version.WFStateID = folderVersionData.WFStateID;
                }
                version.FolderVersionNumber = versionNumber;
                version.FolderID = folderVersionData.FolderID;
                version.EffectiveDate = effectiveDate == null ? folderVersionData.EffectiveDate : effectiveDate.Value;
                version.AddedBy = addedBy;
                version.AddedDate = DateTime.Now;
                //version.Comments = comments;
                version.IsActive = folderVersionData.IsActive;
                version.CategoryID = folderVersionData.CategoryID;
                version.CatID = folderVersionData.CatID;
                //If consortiumID is 0,take consortiumID of folder version to be copied, else take value of consortiumID from parameter.
                if (consortiumID == 0)
                    version.ConsortiumID = folderVersionData.ConsortiumID;
                else
                    version.ConsortiumID = consortiumID;
                if (isNewVersion)
                {
                    version.VersionTypeID = (int)VersionType.New;
                }
                else
                {
                    version.VersionTypeID = folderVersionData.VersionTypeID;
                    version.FolderVersionBatchID = folderVersionData.FolderVersionBatchID;
                }
                version.FolderVersionStateID = Convert.ToInt32(domain.entities.Enums.FolderVersionState.INPROGRESS);

                this._unitOfWork.RepositoryAsync<FolderVersion>().Insert(version);
                this._unitOfWork.Save();
            }
            return version.FolderVersionID;


        }


        private int CopyMasterListFolderVersion(int tenantId, int? notApprovedWorkflowStateId, int folderVersionId, string addedBy, string versionNumber,
           string comments, DateTime? effectiveDate, bool isRelease, bool isNotApproved, bool isNewVersion, Nullable<int> consortiumID)
        {

            //Get All FolderVersion Data related to FolderVersionID           
            FolderVersion folderVersionData = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();

            FolderVersion version = null;
            if (folderVersionData != null)
            {
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, 0, folderVersionData.Folder.IsPortfolio);

                version = new FolderVersion();
                //Generate the new FolderVersionId & use FolderVersion TYpe Minor
                version.TenantID = folderVersionData.TenantID;
                //if (isRelease)
                //{
                //    version.WFStateID = workflowState.WorkFlowVersionStateID;
                //}
                //else if (isNotApproved)
                //{
                //    version.WFStateID = (notApprovedWorkflowStateId == 0) ? workflowState.WorkFlowVersionStateID : Convert.ToInt32(notApprovedWorkflowStateId.ToString());
                //    comments = String.Format(CommentsData.NewVersionComments, addedBy, versionNumber);
                //    folderVersionData.FolderVersionStateID = Convert.ToInt32(FolderVersionState.BASELINED);
                //    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);
                //}
                //else
                //{
                //    //Update FolderVErsionState to Baselined
                //    folderVersionData.FolderVersionStateID = Convert.ToInt32(tmg.equinox.domain.entities.Enums.FolderVersionState.BASELINED);
                //    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);

                //    version.WFStateID = folderVersionData.WFStateID;
                //}

                //if (isRelease)
                //{
                if (!isRelease)
                {
                    folderVersionData.FolderVersionStateID = Convert.ToInt32(tmg.equinox.domain.entities.Enums.FolderVersionState.BASELINED);
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);
                }
                //version.WFStateID = folderVersionData.WFStateID;
                //}

                version.FolderVersionNumber = versionNumber;
                version.FolderID = folderVersionData.FolderID;
                version.EffectiveDate = effectiveDate == null ? folderVersionData.EffectiveDate : effectiveDate.Value;
                version.AddedBy = addedBy;
                version.AddedDate = DateTime.Now;
                if (!String.IsNullOrEmpty(comments))
                {
                    version.Comments = comments;
                }
                version.IsActive = folderVersionData.IsActive;
                version.CategoryID = folderVersionData.CategoryID;
                version.CatID = folderVersionData.CatID;
                //If consortiumID is 0,take consortiumID of folder version to be copied, else take value of consortiumID from parameter.
                if (consortiumID == 0)
                    version.ConsortiumID = folderVersionData.ConsortiumID;
                else
                    version.ConsortiumID = consortiumID;
                if (isNewVersion)
                {
                    version.VersionTypeID = (int)VersionType.New;
                }
                else
                {
                    version.VersionTypeID = folderVersionData.VersionTypeID;
                    version.FolderVersionBatchID = folderVersionData.FolderVersionBatchID;
                }
                version.FolderVersionStateID = Convert.ToInt32(domain.entities.Enums.FolderVersionState.INPROGRESS);

                this._unitOfWork.RepositoryAsync<FolderVersion>().Insert(version);
                this._unitOfWork.Save();
            }
            return version.FolderVersionID;


        }




        private void CopyFolderVersionWorkFlowState(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId, string comment, bool isRelease, bool isNotApproved, int? notApprovedWorkflowStateId)
        {
            if (isNotApproved)
            {
                var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == newFolderVersionID).Get().FirstOrDefault();
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);

                if (notApprovedWorkflowStateId == 0 || notApprovedWorkflowStateId == workflowState.WorkFlowVersionStateID)
                {
                    FolderVersionWorkFlowState addFolderWorkflow = new FolderVersionWorkFlowState();
                    addFolderWorkflow.TenantID = tenantId;
                    addFolderWorkflow.IsActive = true;
                    addFolderWorkflow.AddedDate = DateTime.Now;
                    addFolderWorkflow.AddedBy = addedBy;
                    addFolderWorkflow.FolderVersionID = newFolderVersionID;
                    addFolderWorkflow.WFStateID = workflowState.WorkFlowVersionStateID;
                    addFolderWorkflow.UserID = userId;
                    addFolderWorkflow.ApprovalStatusID = Convert.ToInt32(ApprovalStatus.NOTAPPROVED);

                    //Call to repository method to insert record.
                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(addFolderWorkflow);
                    this._unitOfWork.Save();
                }
                else
                {
                    WorkFlowVersionState nextWorkflowState = null;
                    int workflowStateID = 0; int approvalStatusID = 0;
                    while (nextWorkflowState == null || nextWorkflowState.WorkFlowVersionStateID != notApprovedWorkflowStateId)
                    {
                        workflowStateID = (nextWorkflowState == null) ? workflowState.WorkFlowVersionStateID : nextWorkflowState.WorkFlowVersionStateID;

                        //Approval status of old folder version to copy.
                        approvalStatusID = this._unitOfWork.Repository<FolderVersionWorkFlowState>()
                                                               .Query()
                                                               .Filter(c => c.FolderVersionID == oldFolderVersionID && c.WFStateID == workflowStateID)
                                                               .Get()
                                                               .Select(c => c.ApprovalStatusID).FirstOrDefault();
                        if (approvalStatusID == 0)
                            approvalStatusID = Convert.ToInt32(ApprovalStatus.COMPLETED);
                        FolderVersionWorkFlowState addFolderWorkflow = new FolderVersionWorkFlowState();
                        addFolderWorkflow.TenantID = tenantId;
                        addFolderWorkflow.IsActive = true;
                        addFolderWorkflow.AddedDate = DateTime.Now;
                        addFolderWorkflow.AddedBy = addedBy;
                        addFolderWorkflow.FolderVersionID = newFolderVersionID;
                        addFolderWorkflow.WFStateID = workflowStateID;
                        addFolderWorkflow.UserID = userId;
                        addFolderWorkflow.ApprovalStatusID = approvalStatusID;

                        //Call to repository method to insert record.
                        this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(addFolderWorkflow);
                        this._unitOfWork.Save();

                        //approvalStatusID = Convert.ToInt32(ApprovalStatus.APPROVED);
                        if (nextWorkflowState == null)
                        {
                            nextWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, newFolderVersionID, workflowStateID, approvalStatusID, true);
                        }
                        else
                        {
                            int nextWorkflowStateId = nextWorkflowState.WorkFlowVersionStateID;
                            nextWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, newFolderVersionID, nextWorkflowStateId, approvalStatusID, true);
                        }
                    }

                    FolderVersionWorkFlowState addNotApprovedFolderWorkflow = new FolderVersionWorkFlowState();
                    addNotApprovedFolderWorkflow.TenantID = tenantId;
                    addNotApprovedFolderWorkflow.IsActive = true;
                    addNotApprovedFolderWorkflow.AddedDate = DateTime.Now;
                    addNotApprovedFolderWorkflow.AddedBy = addedBy;
                    addNotApprovedFolderWorkflow.FolderVersionID = newFolderVersionID;
                    addNotApprovedFolderWorkflow.WFStateID = nextWorkflowState.WorkFlowVersionStateID;
                    addNotApprovedFolderWorkflow.UserID = userId;
                    addNotApprovedFolderWorkflow.ApprovalStatusID = Convert.ToInt32(ApprovalStatus.NOTAPPROVED);

                    //Call to repository method to insert record.
                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(addNotApprovedFolderWorkflow);
                    this._unitOfWork.Save();
                }
            }
            else
            {
                //Get All FolderVersionWorkFlowState based on the oldFolderVersionID
                List<FolderVersionWorkFlowState> folderVersionWorkFlowStateList = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                                        .Query()
                                                                        .Filter(c => c.FolderVersionID == oldFolderVersionID)
                                                                        .Get().ToList();

                //create data mapping for newly created FolderVersion WorkFlow.
                //Copy all data of FolderVersionWorkFlow with newFolderVersionID 
                if (!isRelease)
                {
                    foreach (var folderVersionWorkFlow in folderVersionWorkFlowStateList)
                    {
                        FolderVersionWorkFlowState workflowState = new FolderVersionWorkFlowState();
                        workflowState.TenantID = folderVersionWorkFlow.TenantID;
                        workflowState.UserID = folderVersionWorkFlow.UserID;
                        workflowState.WFStateID = folderVersionWorkFlow.WFStateID;
                        workflowState.FolderVersionID = newFolderVersionID;
                        workflowState.ApprovalStatusID = folderVersionWorkFlow.ApprovalStatusID;
                        workflowState.AddedBy = addedBy;
                        workflowState.AddedDate = DateTime.Now;
                        workflowState.Comments = folderVersionWorkFlow.Comments;
                        workflowState.IsActive = folderVersionWorkFlow.IsActive;
                        this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(workflowState);
                    }
                }
                this._unitOfWork.Save();
            }
        }

        private void CopyFolderVersionApplicableTeam(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId)
        {
            //Get All WorkFlowStateFolderVersionMap based on the oldFolderVersionID
            List<WorkFlowStateFolderVersionMap> workFlowStateFolderVersionMapList = this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FolderVersionID == oldFolderVersionID)
                                                                    .Get().ToList();

            //create data mapping for newly created FolderVersion WorkFlow.
            //Copy all data of WorkFlowStateFolderVersionMap with newFolderVersionID
            foreach (var workFlowStateFolderVersionMap in workFlowStateFolderVersionMapList)
            {
                WorkFlowStateFolderVersionMap workflowStateFolderVersion = new WorkFlowStateFolderVersionMap();
                workflowStateFolderVersion.ApplicableTeamID = workFlowStateFolderVersionMap.ApplicableTeamID;
                workflowStateFolderVersion.FolderID = workFlowStateFolderVersionMap.FolderID;
                workflowStateFolderVersion.FolderVersionID = newFolderVersionID;
                workflowStateFolderVersion.AddedBy = addedBy;
                workflowStateFolderVersion.AddedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Insert(workflowStateFolderVersion);
            }

            this._unitOfWork.Save();

        }

        private void CopyWorkFlowStateUserMap(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId)
        {
            //Get All WorkFlowStateUserMap based on the oldFolderVersionID
            List<WorkFlowStateUserMap> workFlowStateUserMapList = this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FolderVersionID == oldFolderVersionID)
                                                                    .Get().ToList();

            //create data mapping for newly created FolderVersion WorkFlow.
            //Copy all data of WorkFlowStateUserMap with newFolderVersionID
            foreach (var workFlowStateUserMap in workFlowStateUserMapList)
            {
                WorkFlowStateUserMap workFlowStateUser = new WorkFlowStateUserMap();
                workFlowStateUser.UserID = workFlowStateUserMap.UserID;
                workFlowStateUser.WFStateID = workFlowStateUserMap.WFStateID;
                workFlowStateUser.FolderID = workFlowStateUserMap.FolderID;
                workFlowStateUser.FolderVersionID = newFolderVersionID;
                workFlowStateUser.AddedBy = addedBy;
                workFlowStateUser.AddedDate = DateTime.Now;
                workFlowStateUser.IsActive = true;
                workFlowStateUser.TenantID = workFlowStateUserMap.TenantID;
                workFlowStateUser.ApprovedWFStateID = workFlowStateUserMap.ApprovedWFStateID;
                workFlowStateUser.ApplicableTeamID = workFlowStateUserMap.ApplicableTeamID;
                this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Insert(workFlowStateUser);
            }

            this._unitOfWork.Save();
        }

        private void CopyAccountProductMap(int newfolderVersionID, string addedBy, int oldFormInstanceId, int newFormInstanceId, bool isManualCopy)
        {
            //Get All AccountProductMap based on oldFolderVersionId
            AccountProductMap accountProductMap = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                     .Query()
                                                                     .Filter(c => c.FormInstanceID == oldFormInstanceId)
                                                                     .Get()
                                                                     .FirstOrDefault();

            //create data mapping for newly created AccountProductMap.
            //Copy all data of AccountProductMap with newFolderVersionID 
            if (accountProductMap != null)
            {
                AccountProductMap accountProductMapToAdd = new AccountProductMap();
                accountProductMapToAdd.FolderVersionID = newfolderVersionID;
                accountProductMapToAdd.AddedBy = addedBy;
                accountProductMapToAdd.AddedDate = DateTime.Now;
                accountProductMapToAdd.FolderID = this._unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderVersionID == newfolderVersionID).Get().Select(c => c.FolderID).FirstOrDefault();
                if (isManualCopy)
                {
                    accountProductMapToAdd.ProductID = GetProxyNumber(newFormInstanceId); // For copied new form, ProductID should be blank.
                }
                else
                {
                    accountProductMapToAdd.ProductID = accountProductMap.ProductID;
                }
                accountProductMapToAdd.PlanCode = accountProductMap.PlanCode;
                accountProductMapToAdd.ProductType = accountProductMap.ProductType;
                accountProductMapToAdd.TenantID = accountProductMap.TenantID;
                accountProductMapToAdd.FormInstanceID = newFormInstanceId;
                accountProductMapToAdd.IsActive = true;
                accountProductMapToAdd.ServiceGroup = accountProductMap.ServiceGroup;
                this._unitOfWork.RepositoryAsync<AccountProductMap>().Insert(accountProductMapToAdd);
                this._unitOfWork.Save();
            }
        }


        private int AddFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formDesignId, string formName, string addedBy, int anchorFormInstanceID = 0)
        {
            string uniqueformname = GetUniqueFormName(folderVersionId, formDesignVersionId, formDesignId, formName, 0);
            formName = uniqueformname;

            if (!this._unitOfWork.RepositoryAsync<FormInstance>().IsFormInstanceExists(tenantId, folderVersionId, formDesignId, formDesignVersionId))
            {
                var designRecord = this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(i => i.FormID == formDesignId).FirstOrDefault();
                if (designRecord != null)
                    anchorFormInstanceID = designRecord.IsMasterList ? 0 : anchorFormInstanceID;// i.IsMasterList

                FormInstance frmInstance = new FormInstance();
                frmInstance.FolderVersionID = folderVersionId;
                frmInstance.FormDesignVersionID = formDesignVersionId;
                frmInstance.FormDesignID = formDesignId;
                frmInstance.TenantID = tenantId;
                frmInstance.AddedBy = addedBy;
                frmInstance.AddedDate = DateTime.Now;
                frmInstance.Name = formName;
                frmInstance.IsActive = true;
                frmInstance.AnchorDocumentID = anchorFormInstanceID;


                this._unitOfWork.RepositoryAsync<FormInstance>().Insert(frmInstance);
                this._unitOfWork.Save();
                if (frmInstance.AnchorDocumentID == 0 && !designRecord.IsMasterList)
                {
                    frmInstance.AnchorDocumentID = frmInstance.FormInstanceID;
                    this._unitOfWork.RepositoryAsync<FormInstance>().Update(frmInstance);
                    this._unitOfWork.Save();
                }
                this.UpdateFormInstanceDocID(frmInstance.FormInstanceID, formName, tenantId);

                return frmInstance.FormInstanceID;
            }
            else
            {
                return 0;
            }
        }

        private string GetUniqueFormName(int folderVersionId, int formDesignVersionId, int formDesignId, string OrgformName, int numberforcopy)
        {
            string formname = OrgformName;

            if (numberforcopy != 0)
                formname = OrgformName + "(" + numberforcopy.ToString() + ")";

            if (!this._unitOfWork.RepositoryAsync<FormInstance>().IsFormInstanceNameExist(1, folderVersionId, formDesignId, formDesignVersionId, formname))
                return formname;
            else
                return GetUniqueFormName(folderVersionId, formDesignVersionId, formDesignId, OrgformName, numberforcopy + 1);
        }

        private ServiceResult DeleteFormInstance(FormInstance formInstance)
        {
            ServiceResult result = new ServiceResult();
            if (formInstance != null)
            {
                if (formInstance.FormDesignID == GlobalVariables.PRODUCTFORMDESIGNID)
                {
                    //ProductReferenceDeleteFormInstance(formInstance.FormInstanceID);
                }
                ////Delete from FormInstanceDataMap table
                List<int> formInstanceDataMapIDList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                        .Query()
                                                                                        .Filter(c => c.FormInstanceID == formInstance.FormInstanceID)
                                                                                        .Get()
                                                                                        .Select(s => s.FormInstanceDataMapID)
                                                                                        .ToList();
                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().DeleteSql(formInstanceDataMapIDList.ToArray());


                ////Delete from FormInstanceDataMap table
                List<int> formInstanceRepeaterDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>()
                                                                                        .Query()
                                                                                        .Filter(c => c.FormInstanceID == formInstance.FormInstanceID)
                                                                                        .Get()
                                                                                        .Select(s => s.FormInstanceDataMapID)
                                                                                        .ToList();
                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().DeleteSql(formInstanceRepeaterDataMapList.ToArray());


                List<int> journalIDList = this._unitOfWork.RepositoryAsync<Journal>()
                                                       .Query()
                                                       .Filter(x => x.FormInstanceID == formInstance.FormInstanceID)
                                                       .Get()
                                                       .Select(s => s.JournalID)
                                                       .ToList();
                this._unitOfWork.RepositoryAsync<JournalResponse>().DeleteRange(c => journalIDList.Contains(c.JournalID));
                this._unitOfWork.RepositoryAsync<Journal>().DeleteRange(c => c.FormInstanceID == formInstance.FormInstanceID);
                this._unitOfWork.RepositoryAsync<AccountProductMap>().DeleteRange(c => c.FormInstanceID == formInstance.FormInstanceID);
                this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().DeleteRange(c => c.FormInstanceID == formInstance.FormInstanceID);
                this._unitOfWork.RepositoryAsync<FormInstanceViewImpactLog>().DeleteRange(c => c.FormInstanceID == formInstance.FormInstanceID);
                this._unitOfWork.RepositoryAsync<FormInstance>().Delete(formInstance);

                result.Result = ServiceResultStatus.Success;
            }
            return result;
        }

        //Delete referenced Product Forms Instances from Fldr.FormInstance & related tables if current Main Product's FormInstance is deleted
        private void ProductReferenceDeleteFormInstance(int formInstanceId)
        {
            if (formInstanceId != 0)
            {
                List<FormInstanceDataMap> formInstanceDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                        .Query()
                                                                                        .Include(f => f.FormInstance)
                                                                                        .Filter(fil => fil.FormInstance.FormDesignID == GlobalVariables.DOCUMENTREFERENCEFORMDESIGNID)
                                                                                        .Get()
                                                                                        .Select(s => s)
                                                                                        .ToList();
                foreach (FormInstanceDataMap map in formInstanceDataMapList)
                {
                    if (map.FormData != "")
                    {
                        JObject source = JObject.Parse(map.FormData);
                        JToken ProductReferenceFormInstanceIDToken = source.SelectToken(this._ProductReferenceFormInstanceID);
                        if (Convert.ToString(ProductReferenceFormInstanceIDToken) != "" && Convert.ToInt32(ProductReferenceFormInstanceIDToken) == formInstanceId)
                        {
                            //Delete from FormInstanceDataMap table
                            List<int> formInstanceDataMapIDList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                                    .Query()
                                                                                                    .Filter(c => c.FormInstanceID == map.FormInstanceID)
                                                                                                    .Get()
                                                                                                    .Select(S => S.FormInstanceDataMapID)
                                                                                                    .ToList();
                            foreach (var formInstanceDataMapID in formInstanceDataMapIDList)
                            {
                                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Delete(formInstanceDataMapID);
                            }

                            List<int> journalIDList = this._unitOfWork.RepositoryAsync<Journal>()
                                                                   .Query()
                                                                   .Filter(x => x.FormInstanceID == map.FormInstanceID)
                                                                   .Get()
                                                                   .Select(s => s.JournalID)
                                                                   .ToList();
                            List<int> journalResponseIDList = null;
                            //Delete from JournalResponse table
                            foreach (var journalID in journalIDList)
                            {
                                journalResponseIDList = this._unitOfWork.RepositoryAsync<JournalResponse>()
                                                                   .Query()
                                                                   .Filter(x => x.JournalID == journalID)
                                                                   .Get()
                                                                   .Select(s => s.JournalResponseID)
                                                                   .ToList();
                                foreach (var responseID in journalResponseIDList)
                                {
                                    this._unitOfWork.RepositoryAsync<JournalResponse>().Delete(responseID);
                                }
                            }

                            //Delete from Journal table
                            foreach (var journalID in journalIDList)
                            {
                                this._unitOfWork.RepositoryAsync<Journal>().Delete(journalID);
                            }

                            //Delete from AccountProductMap table
                            List<int> accountProductMapIDList = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == map.FormInstanceID)
                                                                            .Get()
                                                                            .Select(S => S.AccountProductMapID)
                                                                            .ToList();
                            foreach (var accountProductMapID in accountProductMapIDList)
                            {
                                this._unitOfWork.RepositoryAsync<AccountProductMap>().Delete(accountProductMapID);
                            }

                            //Delete from FormInstanceActivityLog table
                            List<int> formInstanceActivityLogIDList = this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == map.FormInstanceID)
                                                                            .Get()
                                                                            .Select(S => S.FormInstanceActivityLogID)
                                                                            .ToList();
                            foreach (var formInstanceActivityLogID in formInstanceActivityLogIDList)
                            {
                                this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Delete(formInstanceActivityLogID);
                            }

                            this._unitOfWork.RepositoryAsync<FormInstance>().Delete(map.FormInstanceID);
                        }
                    }
                }
            }
        }

        private ServiceResult DeleteFolderVersion(int tenantId, int folderVersionId)
        {
            var result = new ServiceResult();
            DateTime date = DateTime.Now;
            try
            {

                var folderVersionToDelete = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId &&
                                                             c.TenantID == tenantId)
                                                .Get().SingleOrDefault();

                if (folderVersionToDelete != null)
                {
                    //Delete the FormInstances associated with FolderVersion
                    foreach (var item in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId &&
                                                             c.TenantID == tenantId)
                                                .Get().ToList())
                    {
                        this.DeleteFormInstance(item);
                    }


                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().DeleteRange(c => c.FolderVersionID == folderVersionId);


                    this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().DeleteRange(c => c.FolderVersionID == folderVersionId);


                    this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().DeleteRange(c => c.FolderVersionID == folderVersionId);

                    this._unitOfWork.RepositoryAsync<EmailLog>().DeleteRange(c => c.FolderVersionID == folderVersionId);

                    _planTaskUserMappingService.DeletePlanTaskUserMappingByFolderversionId(folderVersionId);
                    //this._unitOfWork.RepositoryAsync<PlanTaskUserMappingDetails>().DeleteRange(c => c. == folderVersionId);

                    //Delete the FolderVersion
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Delete(folderVersionToDelete);

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            var timeinminutes = DateTime.Now.Subtract(date).TotalMinutes;

            return result;
        }

        private ServiceResult DeleteFolderVersionRetroChanges(int tenantId, int folderId)
        {
            var result = new ServiceResult();

            var folderVersionToDelete = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                            .Query()
                                            .Include(c => c.AccountProductMaps)
                                            .Include(c => c.FormInstances)
                                            .Include(c => c.FolderVersionWorkFlowStates)
                                            .Include(c => c.WorkFlowStateFolderVersionMaps)
                                            .Include(c => c.WorkFlowStateUserMaps)
                                            .Include(c => c.EmailLogs)
                                            .Filter(c => c.FolderID == folderId &&
                                                         c.TenantID == tenantId &&
                                                         c.VersionTypeID == (int)VersionType.Retro)
                                            .Get().ToList();

            if (folderVersionToDelete.Any())
            {
                var inProgressRetro = folderVersionToDelete
                                        .FirstOrDefault(fv => fv.FolderVersionStateID == (int)FolderVersionState.INPROGRESS);

                if (inProgressRetro != null)
                {
                    var batchID = inProgressRetro.FolderVersionBatchID;

                    if (folderVersionToDelete.Any(
                        fv => fv.FolderVersionStateID == (int)FolderVersionState.RELEASED &&
                            fv.FolderVersionBatchID == batchID))
                    {
                        result.Result = ServiceResultStatus.Failure;
                    }
                    else
                    {
                        foreach (var folderVersion in folderVersionToDelete
                                                .Where(fv => fv.FolderVersionBatchID == batchID))
                        {

                            //Delete the AccountProductMaps associated with FolderVersion
                            foreach (var item in folderVersion.AccountProductMaps.ToList())
                            {
                                this._unitOfWork.RepositoryAsync<AccountProductMap>().Delete(item);
                            }

                            //Delete the FormInstances associated with FolderVersion
                            foreach (var item in folderVersion.FormInstances.ToList())
                            {
                                this.DeleteFormInstance(item);
                            }

                            //Delete the FolderVersionWorkFlowStates associated with FolderVersion
                            foreach (var item in folderVersion.FolderVersionWorkFlowStates.ToList())
                            {
                                this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Delete(item);
                            }

                            //Delete the WorkFlowStateFolderVersionMaps associated with FolderVersion
                            foreach (var item in folderVersion.WorkFlowStateFolderVersionMaps.ToList())
                            {
                                this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Delete(item);
                            }

                            //Delete the WorkFlowStateUserMaps associated with FolderVersion
                            foreach (var item in folderVersion.WorkFlowStateUserMaps.ToList())
                            {
                                this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Delete(item);
                            }

                            //Delete the EmailLogs associated with FolderVersion
                            foreach (var item in folderVersion.EmailLogs.ToList())
                            {
                                this._unitOfWork.RepositoryAsync<EmailLog>().Delete(item);
                            }

                            //Delete the FolderVersion
                            this._unitOfWork.RepositoryAsync<FolderVersion>().Delete(folderVersion);

                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                            {
                                Messages = new string[] { folderVersion.FolderVersionID.ToString() }
                            });
                        }
                        result.Result = ServiceResultStatus.Success;
                    }
                }

            }
            return result;
        }

        private ServiceResult DeleteFolder(Folder folder, int tenantId)
        {
            ServiceResult result = new ServiceResult();
            if (folder != null)
            {

                var folderVersionList = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                                     .Query()
                                                                     .Filter(c => c.FolderID == folder.FolderID)
                                                                     .Get()
                                         select new FolderVersionViewModel()
                                         {
                                             FolderVersionId = fv.FolderVersionID,
                                             VersionType = fv.VersionTypeID == (int)VersionType.New ?
                                                                GlobalVariables.NEW : GlobalVariables.RETRO
                                         }).ToList();
                //Delete Folder Version
                foreach (var folderVersion in folderVersionList)
                {
                    this.DeleteFolderVersion(tenantId, folder.FolderID, folderVersion.FolderVersionId, folderVersion.VersionType, "");
                }

                //Delete the Folder
                this._unitOfWork.RepositoryAsync<Folder>().Delete(folder);

                result.Result = ServiceResultStatus.Success;
            }
            return result;
        }

        /// <summary>
        /// FormData is stored in byte format in Arc.FormInstanceHistory table
        /// So if we want to check original JsonData we need to pass byte array data stored in table
        /// to this method to get original json data.
        /// </summary>
        /// <param name="jsonDataCompressed"></param>
        private static void CheckFormInstanceHistoryOriginalJsonData(byte[] jsonDataCompressed)
        {
            ICompressionBase compressionBase = CompressionFactory.GetCompressionFactory(CompressionType.Memory, jsonDataCompressed);
            byte[] decompress = (byte[])compressionBase.Decompress();

            string jsonStr = Encoding.UTF8.GetString(decompress);
        }

        public ServiceResult CanRollbackFolderVersion(int tenantId, string rollbackFolderVersionNumber, string inProgressMinorVersionNumber)
        {
            ServiceResult result = null;
            VersionNumberBuilder builder = null;
            try
            {
                result = new ServiceResult();
                builder = new VersionNumberBuilder();

                //return 'X' value from versionNumber in the format given YYYY_X.YZ 

                if (builder.GetIntegerPartFromVersionNumber(rollbackFolderVersionNumber) ==
                    builder.GetIntegerPartFromVersionNumber(inProgressMinorVersionNumber))
                {
                    result.Result = ServiceResultStatus.Success;
                    return result;
                }
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                {
                    Messages = new string[] { "minor version previous to latest major Version cannot Rollback" }
                });
                result.Result = ServiceResultStatus.Failure;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                throw;
            }
            return result;
        }

        private ServiceResult FolderVersionRetro(IEnumerable<FolderVersionViewModel> folderVersions, int folderVersionBatchId,
            string userName, int userId)
        {
            var result = new ServiceResult();
            int newFolderVersionId = 0;
            foreach (var folderVersion in folderVersions)
            {
                //Get List FormInstance id related to specific folderVersion
                List<FormInstanceViewModel> formInstancesList;
                var isMasterList = IsMasterList(folderVersion.FolderId);
                if (isMasterList)
                {
                    formInstancesList = GetFormInstanceList(folderVersion.TenantID, folderVersion.FolderVersionId, folderVersion.FolderId, Convert.ToInt32(DocumentDesignTypes.MASTERLIST));
                }
                else
                {
                    formInstancesList = GetAnchorFormInstanceList(folderVersion.TenantID, folderVersion.FolderVersionId);
                }


                newFolderVersionId = CopyFolderVersionRetroChanges(folderVersion, folderVersionBatchId, userName);
                int anchorFormInstanceId = 0;
                //Changes to copy every versions formInstances with old data to preserve old changes before Retro in Forms.
                foreach (var forminstance in formInstancesList)
                {
                    //Copy all form instances related to olderfolderVersion  to newly created FolderVersion
                    int newFormInstanceID = CopyFormInstanceRetroChanges(folderVersion.TenantID, newFolderVersionId, forminstance.FormInstanceID,
                         string.Empty, userName, anchorFormInstanceId);
                    if (!isMasterList)
                    {
                        var childFormInstanceList = GetDocumentViewList(folderVersion.TenantID, forminstance.FormInstanceID);
                        if (childFormInstanceList != null)
                        {
                            foreach (var childFormInstance in childFormInstanceList)
                            {
                                if (childFormInstance.FormInstanceId != forminstance.FormInstanceID)
                                {
                                    CopyFormInstanceRetroChanges(folderVersion.TenantID, newFolderVersionId, childFormInstance.FormInstanceId,
                                 string.Empty, userName, newFormInstanceID);
                                }
                            }
                        }
                    }
                }
                //Copy all workflowStates related to olderfolderVersion  to newly created FolderVersion
                CreateFolderVersionWorkFlowStateRetroChanges(newFolderVersionId, userName, userId, folderVersion.TenantID);


                //Copy all applicable teams related to olderfolderVersion  to newly created FolderVersion
                CopyFolderVersionApplicableTeam(folderVersion.TenantID, folderVersion.FolderVersionId, newFolderVersionId, userName, userId);

                //Copied 'CopyFromAuditTrail' details of previous version(if exist) 
                if (!folderVersion.IsCopyRetro)
                {
                    // SaveCopyFromAuditByVersion(newFolderVersionId, folderVersion.FolderVersionId, folderVersion.FolderId);
                }

            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        private int CopyFolderVersionRetroChanges(FolderVersionViewModel folderVersion, int folderVersionBatchId, string userName)
        {
            var isPortFolio = this._unitOfWork.RepositoryAsync<Folder>().Query().Filter(c => c.FolderID == folderVersion.FolderId).Get().FirstOrDefault().IsPortfolio;
            WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderVersion.CategoryID, isPortFolio);

            var newVersion = new FolderVersion();
            newVersion.FolderVersionNumber = folderVersion.FolderVersionNumber;
            newVersion.FolderID = folderVersion.FolderId;
            newVersion.EffectiveDate = folderVersion.EffectiveDate;
            newVersion.AddedBy = userName;
            newVersion.AddedDate = DateTime.Now;
            newVersion.Comments = folderVersion.Comments;
            newVersion.IsActive = folderVersion.IsActive;
            newVersion.WFStateID = workflowState.WorkFlowVersionStateID;
            newVersion.VersionTypeID = (int)VersionType.Retro;
            newVersion.TenantID = folderVersion.TenantID;
            newVersion.FolderVersionStateID = folderVersion.IsCopyRetro ?
                                            (int)FolderVersionState.INPROGRESS_BLOCKED : (int)FolderVersionState.INPROGRESS;
            newVersion.FolderVersionBatchID = folderVersionBatchId;
            newVersion.ConsortiumID = folderVersion.ConsortiumID;
            newVersion.CategoryID = folderVersion.CategoryID;
            newVersion.CatID = folderVersion.CatID;
            this._unitOfWork.RepositoryAsync<FolderVersion>().Insert(newVersion);
            this._unitOfWork.Save();
            return newVersion.FolderVersionID;
        }

        private void CreateFolderVersionWorkFlowStateRetroChanges(int newFolderVersionID, string addedBy, int userId, int tenantId)
        {

            //Retrive sequence of Setup State           
            //WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(tenantId);
            var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == newFolderVersionID).Get().FirstOrDefault();
            WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);
            var newworkflowState = new FolderVersionWorkFlowState();
            newworkflowState.TenantID = tenantId;
            newworkflowState.IsActive = true;
            newworkflowState.AddedDate = DateTime.Now;
            newworkflowState.AddedBy = addedBy;
            newworkflowState.FolderVersionID = newFolderVersionID;
            newworkflowState.WFStateID = workflowState.WorkFlowVersionStateID;
            newworkflowState.UserID = userId;
            newworkflowState.ApprovalStatusID = Convert.ToInt32(ApprovalStatus.NOTAPPROVED);
            this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(newworkflowState);

            this._unitOfWork.Save();
        }

        private int CopyFormInstanceRetroChanges(int tenantId, int folderVersionId, int formInstanceId, string formName, string addedBy, int anchorFormInstanceId)
        {
            //copy form design version for selected form instance
            var formDesign = this._unitOfWork.RepositoryAsync<FormInstance>()
                                          .Query()
                                          .Filter(c => c.TenantID == tenantId && c.FormInstanceID == formInstanceId && c.IsActive == true)
                                          .Get().SingleOrDefault();
            if (string.IsNullOrEmpty(formName))
            {
                formName = this._unitOfWork.RepositoryAsync<FormInstance>()
                                  .Query()
                                  .Filter(c => c.TenantID == tenantId && c.FormInstanceID == formInstanceId && c.IsActive == true)
                                  .Get().Select(c => c.Name).SingleOrDefault();

            }
            int newformInstanceId = 0;
            if (formDesign != null)
            {
                // a new form instance with same form design version.
                newformInstanceId = AddFormInstance(tenantId, folderVersionId, formDesign.FormDesignVersionID, formDesign.FormDesignID, formName, addedBy, anchorFormInstanceId);

                if (newformInstanceId > 0)
                {
                    //Copy form design data for selected form instance
                    List<FormInstanceDataMap> formInstanceDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == formInstanceId)
                                                                            .Get().ToList();
                    if (formInstanceDataMapList != null && formInstanceDataMapList.Count > 0)
                    {

                        //create data mapping for newly created form instance.
                        foreach (FormInstanceDataMap map in formInstanceDataMapList)
                        {
                            string newFormData = map.FormData;
                            //if (formDesign.FormDesignID == GlobalVariables.PRODUCTFORMDESIGNID)
                            //{
                            //    JObject source = JObject.Parse(map.FormData);
                            //    source.SelectToken(this._newProductIdSectionPath)[_isProductNewFieldName] = string.Empty;
                            //    source.SelectToken(this._newProductIdSectionPath)[_generateNewProductIdFieldName] = string.Empty;
                            //    source.SelectToken(this._generalInformationSectionPath)[_productNotesFieldName] = string.Empty;
                            //    source.SelectToken(this._generalInformationSectionPath)[_productNotesTitleFieldName] = string.Empty;
                            //    //newFormData = source.ToString();
                            //    newFormData = this.EmptyAuditCheckList(source);
                            //    newFormData = SetDefaultCreateNewPrefix(newFormData);
                            //}
                            if (formDesign.FormDesignID != GlobalVariables.MILESTONECHECKLISTFORMDESIGNID)
                            {
                                var formInstanceDataMap = new FormInstanceDataMap();
                                formInstanceDataMap.FormInstanceID = newformInstanceId;
                                formInstanceDataMap.ObjectInstanceID = map.ObjectInstanceID;
                                formInstanceDataMap.FormData = newFormData;
                                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(formInstanceDataMap);
                                this._unitOfWork.Save();
                            }
                        }
                    }

                    //Copy formInstance Repeater data for selected form instance
                    List<FormInstanceRepeaterDataMap> formInstanceRepeaterDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>()
                                                                            .Query()
                                                                            .Filter(c => c.FormInstanceID == formInstanceId)
                                                                            .Get().ToList();
                    if (formInstanceRepeaterDataMapList != null && formInstanceRepeaterDataMapList.Count > 0)
                    {
                        //create data mapping for newly created form instance.
                        foreach (FormInstanceRepeaterDataMap map in formInstanceRepeaterDataMapList)
                        {
                            if (formDesign.FormDesignID != GlobalVariables.MILESTONECHECKLISTFORMDESIGNID)
                            {
                                FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = new FormInstanceRepeaterDataMap();
                                formInstanceRepeaterDataMap.FormInstanceDataMapID = map.FormInstanceDataMapID;
                                formInstanceRepeaterDataMap.FormInstanceID = newformInstanceId;
                                formInstanceRepeaterDataMap.SectionID = map.SectionID;
                                formInstanceRepeaterDataMap.RepeaterUIElementID = map.RepeaterUIElementID;
                                formInstanceRepeaterDataMap.FullName = map.FullName;
                                formInstanceRepeaterDataMap.RepeaterData = map.RepeaterData;
                                formInstanceRepeaterDataMap.AddedBy = addedBy;
                                formInstanceRepeaterDataMap.AddedDate = DateTime.Now;
                                formInstanceRepeaterDataMap.IsActive = true;
                                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Insert(formInstanceRepeaterDataMap);
                                this._unitOfWork.Save();
                            }
                        }

                    }
                    //Copy AccountProduct related to old folderVersion  to newly created FolderVersion
                    CopyAccountProductMap(folderVersionId, addedBy, formInstanceId, newformInstanceId, false);

                    CopyPBPDetailsMap(folderVersionId, addedBy, formInstanceId, newformInstanceId, false);
                    //Copy From Audit Trail
                    SaveCopyFromAuditTrail(formInstanceId, folderVersionId, newformInstanceId, addedBy, false);

                }
            }

            return newformInstanceId;
        }


        public bool UpdateWithEffectiveFormDesinVersionID(string userName, int tenantId, int folderVersionId)
        {
            bool isNewVersionIsMajorVersion = false;
            int latestFormDesignVersionId = 0;
            var formInstances = (from form in this._unitOfWork.RepositoryAsync<FormInstance>()
                .Query().Include(e => e.FolderVersion)
                .Filter(fil => fil.FolderVersionID == folderVersionId && fil.IsActive == true)
                .Get()
                                 select new FormInstanceViewModel
                                 {
                                     FormInstanceID = form.FormInstanceID,
                                     FormDesignID = form.FormDesignID,
                                     EffectiveDate = form.FolderVersion.EffectiveDate,
                                     FormDesignVersionID = form.FormDesignVersionID
                                 });
            foreach (var forminstance in formInstances)
            {

                //find latest formDesignVersionID
                latestFormDesignVersionId = _formDesignService.GetEffectiveFormDesignVersion(userName, tenantId,
                    forminstance.FormInstanceID, forminstance.FormDesignVersionID, folderVersionId);

                if (latestFormDesignVersionId > 0)
                {
                    //Update FormInstance using latestFormDesignVersionId
                    UpdateFormInstanceWithEffectiveFormDesignVersion(userName,
                    latestFormDesignVersionId, forminstance.FormInstanceID);
                    //Check new version is major version or not
                    isNewVersionIsMajorVersion = _formDesignService.IsMajorFormDesingVersion(latestFormDesignVersionId, tenantId);

                    if (isNewVersionIsMajorVersion)
                    {
                        //Reset WorkFlowState
                        ResetWorkFlowStateForFolderVersion(folderVersionId, tenantId, userName);
                    }

                }
            }

            this._unitOfWork.Save();

            return isNewVersionIsMajorVersion;
        }


        private ServiceResult ResetWorkFlowStateForFolderVersion(int folderVersionId, int tenantId, string userName)
        {
            var result = new ServiceResult();
            var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == folderVersionId).Get().FirstOrDefault();
            if (folderversion.CategoryID != null)
            {
                var firstWorkFlowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);

                List<int> folderVersionWorkFlowStateList = this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId)
                                                .Get()
                                                .Select(sel => sel.FVWFStateID).ToList();

                if (folderVersionWorkFlowStateList != null)
                {
                    foreach (var folderVersionWorkFlowStateID in folderVersionWorkFlowStateList)
                    {
                        this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Delete(folderVersionWorkFlowStateID);
                    }
                }

                var notApprovedApprovalStatus = this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                                .GetNotApprovedApprovalStatus(tenantId);

                if (notApprovedApprovalStatus != null && firstWorkFlowState != null)
                {
                    var addFolderWorkflow = new FolderVersionWorkFlowState();
                    addFolderWorkflow.TenantID = tenantId;
                    addFolderWorkflow.IsActive = true;
                    addFolderWorkflow.AddedBy = userName;
                    addFolderWorkflow.AddedDate = DateTime.Now;
                    addFolderWorkflow.FolderVersionID = folderVersionId;
                    addFolderWorkflow.WFStateID = firstWorkFlowState.WorkFlowVersionStateID;

                    addFolderWorkflow.ApprovalStatusID = Convert.ToInt32(notApprovedApprovalStatus.WorkFlowStateApprovalTypeID);
                    //Call to repository method to insert record.
                    this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(addFolderWorkflow);

                }
            }
            result.Result = ServiceResultStatus.Success;

            return result;
        }

        private IEnumerable<FolderVersionViewModel> GetDocumentFolderVersions(int docID, string formName, IEnumerable<FolderVersionViewModel> foldeVersionList)
        {
            IList<FolderVersionViewModel> documentFolderVersionList = new List<FolderVersionViewModel>();
            try
            {
                if (foldeVersionList != null && foldeVersionList.Count() > 0)
                {
                    foreach (var version in foldeVersionList)
                    {
                        var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                 .Query()
                                                 .Filter(c => c.Name == formName && c.FolderVersionID == version.FolderVersionId && c.DocID == docID)
                                                 .Get()
                                                 .ToList();
                        if (formInstance != null && formInstance.Count > 0)
                        {
                            documentFolderVersionList.Add(version);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return documentFolderVersionList;
        }
        #endregion Private Methods

        #region FolderVersionCategory

        /// <summary>
        /// Gets the list of FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<FolderVersionCategoryViewModel> GetFolderVersionCategoryList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<FolderVersionCategoryViewModel> consortiumList = new List<FolderVersionCategoryViewModel>();
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                consortiumList = (from cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()
                                  join grp in this._unitOfWork.RepositoryAsync<FolderVersionGroup>().Get()
                                  on cat.FolderVersionGroupID equals grp.FolderVersionGroupID
                                  where cat.TenantID == tenantID && cat.IsActive == true
                                  select new FolderVersionCategoryViewModel
                                  {
                                      FolderVersionCategoryID = cat.FolderVersionCategoryID,
                                      FolderVersionCategoryName = cat.FolderVersionCategoryName,
                                      IsActive = cat.IsActive,
                                      FolderVersionGroupID = cat.FolderVersionGroupID,
                                      FolderVersionGroupName = grp.FolderVersionGroupName
                                  })
                                 .ApplySearchCriteria(criteria)
                                 .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                   .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            //return consortiumList;
            return new GridPagingResponse<FolderVersionCategoryViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, consortiumList);
        }


        /// <summary>
        /// Adds the FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the FolderVersionCategory.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">FolderVersionCategory Name already exists</exception>
        public ServiceResult AddFolderVersionCategory(int tenantID, string folderVersionCategoryName, int folderVersionGroupID, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                FolderVersionCategory category = this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                .Query().Filter(a => a.FolderVersionCategoryName.ToUpper() == folderVersionCategoryName.ToUpper()).Get().FirstOrDefault(); //&& a.FolderVersionGroupID == folderVersionGroupID  -- AD: Removed this condition(EQN-2008)

                if (category == null)
                {

                    FolderVersionCategory folderVersionCategoryToAdd = new FolderVersionCategory();
                    folderVersionCategoryToAdd.FolderVersionCategoryName = folderVersionCategoryName;
                    folderVersionCategoryToAdd.FolderVersionGroupID = folderVersionGroupID;
                    folderVersionCategoryToAdd.TenantID = tenantID;
                    folderVersionCategoryToAdd.AddedBy = addedBy;
                    folderVersionCategoryToAdd.AddedDate = DateTime.Now;
                    folderVersionCategoryToAdd.UpdatedBy = null;
                    folderVersionCategoryToAdd.UpdatedDate = null;
                    folderVersionCategoryToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Insert(folderVersionCategoryToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category name already exists" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

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
        public ServiceResult UpdateFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName, int folderVersionGroupID, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                               .Query().Filter(a => a.FolderVersionCategoryID == folderVersionCategoryID).Get().FirstOrDefault();
                if (workFlowCategoryMapping != null && workFlowCategoryMapping.IsFinalized == true)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category workflow is finalized so can not be edited." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {
                    FolderVersionCategory existCategory = this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                    .Query().Filter(a => a.FolderVersionCategoryName.ToUpper() == folderVersionCategoryName.ToUpper() && a.FolderVersionGroupID == folderVersionGroupID).Get().FirstOrDefault();

                    if (existCategory == null)
                    {


                        FolderVersionCategory category = this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                                        .Find(folderVersionCategoryID);

                        category.FolderVersionCategoryName = folderVersionCategoryName;
                        category.FolderVersionGroupID = folderVersionGroupID;
                        this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Update(category);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Category name already exists" } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }


        /// <summary>
        /// Delete the FolderVersionCategory.
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
        public ServiceResult DeleteFolderVersionCategory(int tenantID, int folderVersionCategoryID, string folderVersionCategoryName, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                int folderVersionsCount = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                .Query().Filter(a => a.CategoryID == folderVersionCategoryID).Get().Count();
                if (folderVersionsCount > 0)
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Category is used with Folder versions so can not be deleted." } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
                else
                {

                    //Delete Work Flow Associated with the category
                    WorkFlowCategoryMapping workFlowCategoryMapping = this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                .Query().Filter(a => a.FolderVersionCategoryID == folderVersionCategoryID).Get().FirstOrDefault();
                    if (workFlowCategoryMapping != null && workFlowCategoryMapping.IsFinalized == true)
                    {
                        items.Add(new ServiceResultItem() { Messages = new string[] { "Category workflow is finalized so can not be deleted." } });
                        result.Items = items;
                        result.Result = ServiceResultStatus.Failure;
                    }
                    else
                    {
                        if (workFlowCategoryMapping != null)
                        {
                            //_workFlowCategoryMappingService.DeleteWorkFlowCategoryMapping(tenantID, workFlowCategoryMapping.WorkFlowVersionID);
                            List<WorkFlowVersionState> workFlowVersionStates = this._unitOfWork.Repository<WorkFlowVersionState>()
                                        .Query().Filter(a => a.WorkFlowVersionID == workFlowCategoryMapping.WorkFlowVersionID).Include(e => e.WorkFlowVersionStatesAccess).Include(e => e.WFVersionStatesApprovalType).Include(e => e.WFVersionStatesApprovalType.Select(s => s.WFStatesApprovalTypeActions)).Get().ToList();
                            if (workFlowVersionStates != null && workFlowVersionStates.Count() > 0)
                            {
                                foreach (var state in workFlowVersionStates)
                                {
                                    foreach (var userAccess in state.WorkFlowVersionStatesAccess.ToList())
                                    {
                                        this._unitOfWork.Repository<WorkFlowVersionStatesAccess>().Delete(userAccess);
                                    }
                                    foreach (var approvalType in state.WFVersionStatesApprovalType.ToList())
                                    {
                                        foreach (var approvalTypeActions in approvalType.WFStatesApprovalTypeActions.ToList())
                                        {
                                            this._unitOfWork.Repository<WFStatesApprovalTypeAction>().Delete(approvalTypeActions);
                                        }
                                        this._unitOfWork.Repository<WFVersionStatesApprovalType>().Delete(approvalType);
                                    }
                                    this._unitOfWork.Repository<WorkFlowVersionState>().Delete(state);
                                }
                            }
                            this._unitOfWork.Repository<WorkFlowCategoryMapping>().Delete(workFlowCategoryMapping);
                        }
                        using (var scope = new TransactionScope())
                        {
                            FolderVersionCategory category = this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                                        .Find(folderVersionCategoryID);
                            this._unitOfWork.Repository<FolderVersionCategory>().Delete(category);
                            this._unitOfWork.Save();
                            result.Result = ServiceResultStatus.Success;
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Gets the list of FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<FolderVersionCategoryViewModel> GetFolderVersionCategoryForDropdown(int tenantID, bool? isPortfolio, int folderVersionID, bool? isFinalized = null)
        {
            IEnumerable<FolderVersionCategoryViewModel> categoryList = null;
            try
            {
                int accountType = 0;
                if (isFinalized == null)
                {

                    categoryList = (from con in this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                      .Query()
                                      .Filter(con => con.TenantID == tenantID && con.IsActive == true)
                                      .Get()
                                    select new FolderVersionCategoryViewModel
                                    {
                                        FolderVersionCategoryID = con.FolderVersionCategoryID,
                                        FolderVersionCategoryName = con.FolderVersionCategoryName,
                                        FolderVersionGroupID = con.FolderVersionGroupID,
                                        IsActive = con.IsActive
                                    });

                }
                else
                    if (isPortfolio == null && folderVersionID == 0)
                {
                    List<int> finalizedCategories = (from con in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                       .Query()
                                       .Filter(con => con.IsFinalized == isFinalized || isFinalized == null)
                                       .Get()
                                                     select con.FolderVersionCategoryID).ToList();

                    categoryList = (from con in this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                           .Query()
                                           .Filter(con => con.TenantID == tenantID && con.IsActive == true && finalizedCategories.Contains(con.FolderVersionCategoryID))
                                           .Get()
                                    select new FolderVersionCategoryViewModel
                                    {
                                        FolderVersionCategoryID = con.FolderVersionCategoryID,
                                        FolderVersionCategoryName = con.FolderVersionCategoryName,
                                        FolderVersionGroupID = con.FolderVersionGroupID,
                                        IsActive = con.IsActive
                                    });

                }
                else
                {
                    accountType = 0;
                    if (folderVersionID > 0)
                    {
                        //From folderversionId get the Account type
                        var folderversion = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                  .Query()
                                                  .Include(inc => inc.Folder)
                                                  .Filter(c => c.FolderVersionID == folderVersionID)
                                                  .Get()
                                             select c).ToList().FirstOrDefault();
                        if (folderversion != null)
                        {
                            accountType = folderversion.Folder.IsPortfolio ? 1 : 2;
                        }
                    }
                    else
                    {
                        accountType = isPortfolio == true ? (int)AccountType.PORTFOLIO : (int)AccountType.ACCOUNT;
                    }
                    List<int> accountSpecificCategories = (from con in this._unitOfWork.RepositoryAsync<WorkFlowCategoryMapping>()
                                       .Query()
                                       .Filter(con => con.AccountType == accountType && con.IsFinalized == true)
                                       .Get()
                                                           select con.FolderVersionCategoryID).ToList();

                    categoryList = (from con in this._unitOfWork.RepositoryAsync<FolderVersionCategory>()
                                           .Query()
                                           .Filter(con => con.TenantID == tenantID && con.IsActive == true && accountSpecificCategories.Contains(con.FolderVersionCategoryID))
                                           .Get()
                                    select new FolderVersionCategoryViewModel
                                    {
                                        FolderVersionCategoryID = con.FolderVersionCategoryID,
                                        FolderVersionCategoryName = con.FolderVersionCategoryName,
                                        FolderVersionGroupID = con.FolderVersionGroupID,
                                        IsActive = con.IsActive
                                    });
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return categoryList;
        }

        /// <summary>
        /// Gets details of a FolderVersionCategory.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public string GetFolderVersionCategory(int? CategoryID)
        {
            string categoryName = "";
            if (CategoryID != 0 && CategoryID != null)
            {
                categoryName = this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Find(CategoryID).FolderVersionCategoryName;
            }
            return categoryName;
        }
        #endregion FolderVersionCategory

        #region FolderVersionGroup
        /// <summary>
        /// Gets the list of FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<FolderVersionGroupViewModel> GetFolderVersionGroupList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<FolderVersionGroupViewModel> groupList = new List<FolderVersionGroupViewModel>();
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                groupList = (from cat in this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                       .Query()
                                       .Filter(con => con.TenantID == tenantID && con.IsActive == true)
                                       .Get()
                             select new FolderVersionGroupViewModel
                             {
                                 FolderVersionGroupID = cat.FolderVersionGroupID,
                                 FolderVersionGroupName = cat.FolderVersionGroupName,
                                 IsActive = cat.IsActive
                             })
                                 .ApplySearchCriteria(criteria)
                                 .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                   .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            //return consortiumList;
            return new GridPagingResponse<FolderVersionGroupViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, groupList);
        }


        /// <summary>
        /// Adds the FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="folderVersionCategoryName">Name of the FolderVersionCategory.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">FolderVersionCategory Name already exists</exception>
        public ServiceResult AddFolderVersionGroup(int tenantID, string folderVersionGroupName, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                FolderVersionGroup group = this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                .Query().Filter(a => a.FolderVersionGroupName.ToUpper() == folderVersionGroupName.ToUpper()).Get().FirstOrDefault();

                if (group == null)
                {

                    FolderVersionGroup folderVersionGroupToAdd = new FolderVersionGroup();
                    folderVersionGroupToAdd.FolderVersionGroupName = folderVersionGroupName;
                    folderVersionGroupToAdd.TenantID = tenantID;
                    folderVersionGroupToAdd.AddedBy = addedBy;
                    folderVersionGroupToAdd.AddedDate = DateTime.Now;
                    folderVersionGroupToAdd.UpdatedBy = null;
                    folderVersionGroupToAdd.UpdatedDate = null;
                    folderVersionGroupToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<FolderVersionGroup>().Insert(folderVersionGroupToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Group name already exists" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }

            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Updates the FolderVersionGroup
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
        public ServiceResult UpdateFolderVersionGroup(int tenantID, int folderVersionGroupID, string folderVersionGroupName, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                FolderVersionGroup existGroup = this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                .Query().Filter(a => a.FolderVersionGroupName.ToUpper() == folderVersionGroupName.ToUpper()).Get().FirstOrDefault();

                if (existGroup == null)
                {

                    FolderVersionGroup group = this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                                    .Find(folderVersionGroupID);

                    group.FolderVersionGroupName = folderVersionGroupName;
                    this._unitOfWork.RepositoryAsync<FolderVersionGroup>().Update(group);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    items.Add(new ServiceResultItem() { Messages = new string[] { "Group name already exists" } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }


        /// <summary>
        /// Delete the FolderVersionGroup.
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
        public ServiceResult DeleteFolderVersionGroup(int folderVersionGroupID, string folderVersionGroupName, string updatedBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                FolderVersionGroup group = this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                .Find(folderVersionGroupID);

                if (group != null)
                {
                    group.UpdatedBy = updatedBy;
                    group.UpdatedDate = DateTime.Now;
                    group.IsActive = false;

                    this._unitOfWork.RepositoryAsync<FolderVersionGroup>().Update(group);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    throw new Exception("Group Does Not exists");
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }


        /// <summary>
        /// Gets the list of FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<FolderVersionGroupViewModel> GetFolderVersionGroupForDropdown(int tenantID)
        {
            IEnumerable<FolderVersionGroupViewModel> consortiumList = null;

            try
            {
                consortiumList = (from con in this._unitOfWork.RepositoryAsync<FolderVersionGroup>()
                                       .Query()
                                       .Filter(con => con.TenantID == tenantID && con.IsActive == true)
                                       .Get()
                                  select new FolderVersionGroupViewModel
                                  {
                                      FolderVersionGroupID = con.FolderVersionGroupID,
                                      FolderVersionGroupName = con.FolderVersionGroupName,
                                      IsActive = con.IsActive
                                  });

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return consortiumList;
        }

        /// <summary>
        /// Gets details of a FolderVersionGroup.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public string GetFolderVersionGroup(int? GroupID)
        {
            string groupName = "";
            if (GroupID != 0 && GroupID != null)
            {
                groupName = this._unitOfWork.RepositoryAsync<FolderVersionGroup>().Find(GroupID).FolderVersionGroupName;
            }
            return groupName;
        }
        #endregion FolderVersionGroup

        private class CommentsData
        {
            public const string InitialVersionComments = "{0} created Initial minor version '{1}'";
            public const string RollbackComments = "{0} rolling back the Folder Version '{1}' ";
            public const string RetroComments = "{0} created new retro FolderVersion '{1}' ";
            public const string NewVersionComments = "{0} created new FolderVersion '{1}' ";
        }

        /// <summary>
        /// This method is used to update formInstance data by formInsatnceId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="formInstanceData"></param>
        /// <returns></returns>
        public ServiceResult UpdateFormInstanceData(int tenantId, int formInstanceId, string formInstanceData)
        {

            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormInstanceDataMap formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                                                    .Get()
                                                                    .FirstOrDefault();

                if (formInstance != null)
                {
                    formInstance.FormData = formInstanceData;
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstance);
                    this._unitOfWork.Save();

                    //UpdateReportingCenterDatabase(formInstanceId, null);

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        public void SaveCopyFromAuditByVersion(int currentfolderVersionId, int folderVersionId, int folderId)
        {
            try
            {
                CopyFromAuditTrail copyValue = new CopyFromAuditTrail();
                List<CopyFromAuditTrail> getcopyValue = this._unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Query().Filter(c => c.DestinationFolderVersionID == folderVersionId).Get().ToList();
                if (getcopyValue.Count > 0)
                {
                    foreach (var value in getcopyValue)
                    {
                        bool isActive = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(c => c.FormInstanceID == value.DestinationDocumentID).Get().Where(c => c.IsActive == true).Any();
                        if (isActive)
                        {
                            copyValue.SourceDocumentID = value.SourceDocumentID;
                            copyValue.FolderID = folderId;
                            copyValue.AccountID = value.AccountID;
                            copyValue.EffectiveDate = value.EffectiveDate;
                            copyValue.DestinationFolderVersionID = currentfolderVersionId;
                            copyValue.SourceFolderVersionID = value.SourceFolderVersionID;
                            copyValue.DestinationDocumentID = value.DestinationDocumentID;
                            copyValue.AddedBy = value.AddedBy;
                            copyValue.AddedDate = DateTime.Now;
                            this._unitOfWork.Repository<CopyFromAuditTrail>().Insert(copyValue);
                            this._unitOfWork.Save();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        /// <summary>
        /// Updates document name with newly generated product Id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="newDocName"></param>
        public void UpdateDocumentName(int tenantId, int formInstanceID, string newDocName)
        {
            try
            {
                FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                                   .Query()
                                                                                   .Filter(c => c.FormInstanceID == formInstanceID && c.TenantID == tenantId && c.IsActive == true)
                                                                                   .Get().FirstOrDefault();

                formInstance.Name = newDocName;
                this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstance);
                this._unitOfWork.Save();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        /// <summary>
        /// Updates Product JSON hash
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceID"></param>
        /// <param name="newDocName"></param>
        public void UpdateProductJsonHash(int tenantId, int formInstanceID, string hash)
        {
            try
            {
                FormInstance formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                                   .Query()
                                                                                   .Filter(c => c.FormInstanceID == formInstanceID && c.TenantID == tenantId && c.IsActive == true)
                                                                                   .Get().FirstOrDefault();

                formInstance.ProductJsonHash = hash;
                this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstance);
                this._unitOfWork.Save();
                this._unitOfWork.Clear<FormInstance>(formInstance);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        /// <summary>
        /// Get product ID from Account ProductMap Table
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        public string GetProductId(int formInstanceID)
        {
            string productId = null;

            List<AccountProductMap> accountProductMap = this._unitOfWork.Repository<AccountProductMap>()
                .Query()
                .Filter(c => c.FormInstanceID == formInstanceID)
                .Get()
                .ToList();
            if (accountProductMap.Count > 0)
            {
                productId = accountProductMap.FirstOrDefault().ProductID;
            }

            return productId;
        }

        /// <summary>
        /// Updates product Id field with newly generated product Id
        /// </summary>
        /// <param name="newProductId"></param>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        public void UpdateJSONProductId(string newProductId, int formInstanceID)
        {
            int benefitSetPosition = -1;
            try
            {
                FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                    .Query()
                                                                                    .Filter(c => c.FormInstanceID == formInstanceID)
                                                                                    .Get().FirstOrDefault();
                if (formInstanceDataMap != null)
                {
                    JObject source = JObject.Parse(formInstanceDataMap.FormData);
                    if (source.SelectToken(this._newProductIdSectionPath) != null)
                    {
                        source.SelectToken(this._newProductIdSectionPath)[this._productIdFieldName] = newProductId;
                        source.SelectToken(this._newProductIdSectionPath)[this._generateNewProductIdFieldName] = "False";

                        //Writing back blank SEPY PFX when the generate new product method is called
                        var benifitSets = source.SelectToken(this._benefitSet);
                        foreach (var bSet in benifitSets)
                        {
                            benefitSetPosition++;
                            source.SelectToken(this._benefitSet)[benefitSetPosition][this._SEPYPfx] = "";
                            if (source.SelectToken(this._benefitSet)[benefitSetPosition][this._CreateNewSEPY] != null)
                                source.SelectToken(this._benefitSet)[benefitSetPosition][this._CreateNewSEPY] = "Yes";
                        }
                        formInstanceDataMap.FormData = JsonConvert.SerializeObject(source);
                        this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                        this._unitOfWork.Save();

                        //Update Product ID to AccountProductMap
                        AccountProductMap accountProductMap = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                                    .Query()
                                                                                    .Filter(c => c.FormInstanceID == formInstanceID)
                                                                                    .Get().FirstOrDefault();
                        if (accountProductMap != null)
                        {
                            accountProductMap.ProductID = newProductId;
                            this._unitOfWork.RepositoryAsync<AccountProductMap>().Update(accountProductMap);
                            this._unitOfWork.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        public ReferenceDocumentViewModel GetReferenceDocumentModel(int tenantId, int accountId, int folderVersionId, int folderId, int formInstanceId)
        {
            ReferenceDocumentViewModel refData = null;
            refData = (from ac in this._unitOfWork.RepositoryAsync<Account>().Query().Filter(ac => ac.AccountID == accountId).Get()
                       join am in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Query().Get() on ac.AccountID equals am.AccountID
                       join fc in this._unitOfWork.RepositoryAsync<Folder>().Query().Filter(fc => fc.FolderID == folderId).Get() on am.FolderID equals fc.FolderID
                       join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(fv => fv.FolderVersionID == folderVersionId).Get() on fc.FolderID equals fv.FolderID
                       join fd in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Filter(fd => fd.FormInstanceID == formInstanceId).Get() on fv.FolderVersionID equals fd.FolderVersionID
                       select new ReferenceDocumentViewModel
                       {
                           AccountName = ac.AccountName,
                           DocumentName = fd.Name,
                           FolderName = fc.Name,
                           FolderVersionEffectiveDate = fv.EffectiveDate,
                           FolderVersionNumber = fv.FolderVersionNumber,
                           ConsortiumID = fv.ConsortiumID
                       }).FirstOrDefault();

            return refData;
        }

        /// Updates prrfixes in JSON.
        /// </summary>
        /// <param name="newProductId"></param>
        /// <param name="formInstanceID"></param>
        /// <returns></returns>
        public bool UpdatePrefixesInJSON(int formInstanceID, string SEPYPFXs, string DEDEPFXs, string LTLTPFXs, string benefitSetNames, string isNewSEPYFlags, string isNewLTLTFlags, string isNewDEDEFlags)
        {
            int benefitSetPosition = -1;
            int pfxLoc = -1;
            bool isSuccess = false;
            try
            {
                FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                    .Query()
                                                                                    .Filter(c => c.FormInstanceID == formInstanceID)
                                                                                    .Get().FirstOrDefault();
                if (formInstanceDataMap != null)
                {
                    string[] benefitSets = benefitSetNames.Split('|');
                    string[] SEPYs = SEPYPFXs.Split('|');
                    string[] LTLTs = LTLTPFXs.Split('|');
                    string[] DEDEs = DEDEPFXs.Split('|');
                    string[] isNewSEPYs = isNewSEPYFlags.Split('|');
                    string[] isnewDEDEs = isNewDEDEFlags.Split('|');
                    string[] isnewLTLTs = isNewLTLTFlags.Split('|');

                    JObject source = JObject.Parse(formInstanceDataMap.FormData);
                    var benifitSets = source.SelectToken(this._benefitSet);
                    foreach (var bSet in benifitSets)
                    {
                        benefitSetPosition++;//this will be benefitSet number for which prefixes to be updated.
                        pfxLoc = -1;
                        foreach (string bSetName in benefitSets)
                        {
                            pfxLoc++;
                            if (bSet.SelectToken(_benefitSetName).ToString().Trim() == bSetName.Trim())
                            {
                                //Prfix to be updated only if IsNew flag >=1
                                if (int.Parse(isNewSEPYs[pfxLoc]) >= 1)
                                    source.SelectToken(this._benefitSet)[benefitSetPosition][this._SEPYPfx] = SEPYs[pfxLoc];
                                if (int.Parse(isnewLTLTs[pfxLoc]) >= 1)
                                    source.SelectToken(this._benefitSet)[benefitSetPosition][this._LTLTPfx] = LTLTs[pfxLoc];
                                if (int.Parse(isnewDEDEs[pfxLoc]) >= 1)
                                    source.SelectToken(this._benefitSet)[benefitSetPosition][this._DEDEPfx] = DEDEs[pfxLoc];
                                break;
                            }
                        }
                    }
                    formInstanceDataMap.FormData = JsonConvert.SerializeObject(source);
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                    this._unitOfWork.Save();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isSuccess;
        }

        public List<int> GetFormInstanceIds(int folderVersionId)
        {
            List<int> formInstanceIds = null;
            try
            {
                formInstanceIds = this._unitOfWork.RepositoryAsync<FormInstance>().Get().Where(c => c.FolderVersionID == folderVersionId && c.IsActive == true).Select(p => p.FormInstanceID).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formInstanceIds;
        }

        public Dictionary<int, ProductState> GetProductStateList(int folderVersionId, string folderType)
        {
            Dictionary<int, ProductState> productStateList = new Dictionary<int, ProductState>();
            try
            {
                List<int> formInstances = GetFormInstanceIds(folderVersionId);

                if (formInstances.Count > 0)
                {
                    foreach (int id in formInstances)
                    {

                        ProductState productState = new ProductState();
                        productState.IsFolderVersionReleased = false;
                        productState.IsFolderVersionBaselined = false;
                        productState.FormInstanceID = id;
                        productState.IsFolderVersionReleased = IsFolderVersionReleased(folderVersionId);
                        productState.IsFolderVersionBaselined = IsFolderVersionBaselined(folderVersionId);
                        productState.IsProductInMigration = IsFolderInMigration(id);
                        productStateList.Add(id, productState);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return productStateList;
        }

        public bool UpdatePDBCSection(int formInstanceID, string EBCLPFX, string BSBSPFX, int isNewEBCL, int isNewBSBS, bool? IsUsingNewBSBS)
        {
            int PDBCSectionPosition = -1;
            bool isSuccess = false;
            if (IsUsingNewBSBS == null)
                IsUsingNewBSBS = false;
            try
            {
                FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                    .Query()
                                                                                    .Filter(c => c.FormInstanceID == formInstanceID)
                                                                                    .Get().FirstOrDefault();
                if (formInstanceDataMap != null)
                {
                    JObject source = JObject.Parse(formInstanceDataMap.FormData);
                    var pdbcPFXList = source.SelectToken(this._PDBCComponents);
                    foreach (var lst in pdbcPFXList)
                    {
                        PDBCSectionPosition++;
                        if (lst.SelectToken(this._PDBCType).ToString() == "EBCL")
                        {
                            if (isNewEBCL >= 1)
                                source.SelectToken(this._PDBCComponents)[PDBCSectionPosition][this._PDBCPrefix] = EBCLPFX;
                        }
                        if (lst.SelectToken(this._PDBCType).ToString() == "BSBS")
                        {
                            source.SelectToken(this._PDBCComponents)[PDBCSectionPosition][this._PDBCPrefix] = BSBSPFX;
                            if (IsUsingNewBSBS == true)
                            {
                                source.SelectToken(this._PDBCComponents)[PDBCSectionPosition]["IsPrefixNew"] = "True";
                            }
                            if (isNewBSBS == 1)
                            {
                                source.SelectToken(this._PDBCComponents)[PDBCSectionPosition]["CreateNewPrefix"] = "False";
                            }
                        }
                    }
                    formInstanceDataMap.FormData = JsonConvert.SerializeObject(source);
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Update(formInstanceDataMap);
                    this._unitOfWork.Save();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isSuccess;
        }

        /// <summary>
        /// VersionTypeID  ,VersionType 
        /// 1              ,New
        /// 2              ,Retro
        /// If versionTypeID = 2 then folder version is retro.
        /// </summary>
        /// <returns></returns>
        public bool GetFolderVersionType(int formInstanceID)
        {
            bool isRetro = false;
            try
            {
                int versionTypeID = (from fldrVer in this._unitOfWork.Repository<FolderVersion>().Query().Get()
                                     join frmInst in this._unitOfWork.Repository<FormInstance>().Query().Get()
                                     on fldrVer.FolderVersionID equals frmInst.FolderVersionID
                                     where frmInst.FormInstanceID == formInstanceID
                                     select fldrVer.VersionTypeID).FirstOrDefault();

                isRetro = versionTypeID == 2 ? true : false;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isRetro;
        }

        /// <summary>
        /// Generating product json hash
        /// </summary>
        /// <param name="productJSON"></param>
        /// <returns></returns>
        public string ComputeProductHash(string productJSON)
        {
            MD5 md5Hash = MD5.Create();
            string pattern = @",""RowIDProperty"":[0-9]*";
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);
            JObject source = JObject.Parse(productJSON);
            source["AuditChecklist"] = string.Empty;
            source.SelectToken("ProductDefinition.GeneralInformation")["Notes"] = string.Empty;
            source.SelectToken("ProductDefinition.GeneralInformation")["NotesTitle"] = string.Empty;
            source.SelectToken("ProductDefinition.FacetsProductInformation.NewProductID")["GenerateNewProductID"] = string.Empty;
            source.SelectToken("ProductDefinition.FacetsProductInformation.NewProductID")["IsProductNew"] = string.Empty;
            source["ShadowBenefitReview"] = string.Empty;
            source["CustomRulesSettings"] = string.Empty;
            JObject sourceSection = (JObject)source["ProductDefinition"]["FacetsProductInformation"];
            try
            {

                if (sourceSection.Property("Blank") != null)
                    sourceSection.Property("Blank").Remove();

                var PDBCPrefixList = source["ProductDefinition"]["FacetsProductInformation"]["FacetProductComponentsPDBC"]["PDBCPrefixList"];

                foreach (var item in PDBCPrefixList)
                {
                    JObject t = (JObject)item;
                    if (t.Property("CreateNewPrefix") != null)
                        t.Property("CreateNewPrefix").Remove();

                    if (t.Property("IsPrefixNew") != null)
                        t.Property("IsPrefixNew").Remove();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                //if (reThrow)
                //    throw ex;
            }
            productJSON = JsonConvert.SerializeObject(source);
            productJSON = rgx.Replace(productJSON, "");

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(productJSON));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public bool IsActivityPerformed(int formInstanceID)
        {
            bool isActivityPerformend = false;
            var activityList = this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Get().Where(c => c.FormInstanceID == formInstanceID).ToList();
            if (activityList.Count > 0)
            {
                isActivityPerformend = true;
            }
            return isActivityPerformend;
        }
        public bool IsMasterListDesign(int formDesignID)
        {
            bool IsMasterListDesign = false;
            if (this._unitOfWork.RepositoryAsync<FormDesign>().Get().Any(c => c.FormID == formDesignID && c.IsMasterList))
            {
                IsMasterListDesign = true;
            }
            return IsMasterListDesign;
        }
        public FolderVersionViewModel GetFolderVersionById(int folderVersionID)
        {
            FolderVersionViewModel folderVersion = null;
            try
            {
                folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                .Get()
                                .Where(s => s.FolderVersionID == folderVersionID)
                                .Select(f => new FolderVersionViewModel
                                {
                                    FolderId = f.FolderID,
                                    FolderVersionId = f.FolderVersionID,
                                    FolderVersionStateID = f.FolderVersionStateID,
                                    EffectiveDate = f.EffectiveDate,
                                    Comments = f.Comments
                                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersion;
        }
        public IList<FolderVersions> GetFolderVersionByFolderId(int folderID)
        {
            IList<FolderVersions> folderVersion = null;
            try
            {
                folderVersion = (from fldr in this._unitOfWork.Repository<Folder>().Query().Filter(x => x.FolderID == folderID).Get()
                                 join fldrVer in this._unitOfWork.Repository<FolderVersion>().Query().Filter(x => x.FolderID == folderID && x.IsActive == true).Get() on fldr.FolderID equals fldrVer.FolderID
                                 //join frmInst in this._unitOfWork.Repository<FormInstance>().Query().Filter(a => a.IsActive == true).Get() on fldrVer.FolderVersionID equals frmInst.FolderVersionID
                                 //join frmDesig in this._unitOfWork.Repository<FormDesign>().Query().Filter(a => a.IsActive == true).Get() on frmInst.FormDesignID equals frmDesig.FormID
                                 where fldr.FolderID == folderID
                                 select new FolderVersions
                                 {
                                     FolderVersionID = fldrVer.FolderVersionID,
                                     //FolderName = fldr.Name,
                                     //FolderVersionNumber = fldrVer.FolderVersionNumber,
                                     //EffectiveDate = fldrVer.EffectiveDate.Month + "/" + fldrVer.EffectiveDate.Day + "/" + fldrVer.EffectiveDate.Year,
                                     //Status = fldrVer.FolderVersionStateID == 1 ? "In Progress" : fldrVer.FolderVersionStateID == 2 ? "Baselined" : fldrVer.FolderVersionStateID == 3 ? "Released" : fldrVer.FolderVersionStateID == 4 ? "In Progress-Blocked" : "",
                                     //Document = (new[] { 
                                     //    new Documents 
                                     //    { 
                                     //        DocumentID = frmInst.FormInstanceID.ToString(), 
                                     //        DocumentName = frmInst.Name, 
                                     //        DesignTemplate=frmDesig.FormName, 
                                     //        DesignTemplateVersion = frmDesig.FormDesignVersions.FirstOrDefault().VersionNumber,
                                     //        Links = ( new[] { new url { rowTemplate ="/api/data/v1.0/Documents/" + frmInst.FormInstanceID.ToString() }})
                                     //    }
                                     //})
                                 }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersion;
        }
        public IList<FolderVersions> GetFolderVersionByFolderVersionId(int folderVersionID)
        {
            IList<FolderVersions> folderVersion = null;
            try
            {
                folderVersion = (from fldrVer in this._unitOfWork.Repository<FolderVersion>().Query().Get()
                                 join fldr in this._unitOfWork.Repository<Folder>().Query().Get() on fldrVer.FolderID equals fldr.FolderID
                                 join frmInst in this._unitOfWork.Repository<FormInstance>().Get() on fldrVer.FolderVersionID equals frmInst.FolderVersionID
                                 where fldrVer.FolderVersionID == folderVersionID && fldrVer.IsActive == true
                                 select new FolderVersions
                                 {
                                     FolderVersionID = fldrVer.FolderVersionID,
                                     FolderName = fldr.Name,
                                     FolderVersionNumber = fldrVer.FolderVersionNumber,
                                     EffectiveDate = fldrVer.EffectiveDate.Month + "/" + fldrVer.EffectiveDate.Day + "/" + fldrVer.EffectiveDate.Year,
                                     Status = fldrVer.FolderVersionStateID == 1 ? "In Progress" : fldrVer.FolderVersionStateID == 2 ? "Baselined" : fldrVer.FolderVersionStateID == 3 ? "Released" : fldrVer.FolderVersionStateID == 4 ? "In Progress-Blocked" : "",
                                 }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersion;
        }

        public IList<Documents> GetFolderVersionDocuments(int folderVersionID)
        {
            IList<Documents> documents = null;
            try
            {
                documents = (from frmInst in this._unitOfWork.Repository<FormInstance>().Query().Filter(a => a.IsActive == true).Get() //on fldrVer.FolderVersionID equals frmInst.FolderVersionID
                             join frmDesig in this._unitOfWork.Repository<FormDesign>().Query().Filter(a => a.IsActive == true).Get() on frmInst.FormDesignID equals frmDesig.FormID
                             where frmInst.FolderVersionID == folderVersionID
                             select new Documents
                             {
                                 DocumentID = frmInst.FormInstanceID.ToString(),
                                 DocumentName = frmInst.Name,
                                 DesignTemplate = frmDesig.FormName,
                                 DesignTemplateVersion = frmDesig.FormDesignVersions.FirstOrDefault().VersionNumber,
                                 Links = (new[] { new url { rowTemplate = "/api/data/v1.0/Documents/" + frmInst.FormInstanceID.ToString() } })
                             }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return documents;
        }
        public string GetFolderVersionState(int FolderVersionStateID)
        {
            string state = string.Empty;
            switch (FolderVersionStateID)
            {
                case 1:
                    state = "In Progress";
                    break;
                case 2:
                    state = "Baselined";
                    break;
                case 3:
                    state = "Released";
                    break;
                case 4:
                    state = "In Progress-Blocked";
                    break;
                default:
                    break;
            }
            return state;
        }
        public bool IsFolderVersionReleased(int folderVersionID)
        {
            bool isReleased = false;
            try
            {
                isReleased = this._unitOfWork.Repository<FolderVersion>()
                                            .Query()
                                            .Filter(x => x.FolderVersionID == folderVersionID && x.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                            .Get()
                                            .Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isReleased;
        }

        public bool IsFolderVersionBaselined(int folderVersionID)
        {
            bool isBaselined = false;
            try
            {
                isBaselined = this._unitOfWork.Repository<FolderVersion>()
                                            .Query()
                                            .Filter(x => x.FolderVersionID == folderVersionID && x.FolderVersionStateID == (int)FolderVersionState.BASELINED)//.Where(x => x.FolderVersionID == folderVersionID).Select(s => s.FolderVersionStateID).FirstOrDefault();
                                            .Get()
                                            .Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isBaselined;
        }

        public bool IsFolderInMigration(int formInstanceId)
        {
            bool isMigration = false;
            try
            {
                isMigration = (from mp in this._unitOfWork.Repository<MigrationPlans>().Get()
                               join mb in this._unitOfWork.Repository<MigrationBatchs>().Get()
                               on mp.BatchId equals mb.BatchId
                               where mp.FormInstanceId == formInstanceId && mb.IsActive == true
                               select mb.Status == "Completed" ? true : false).Any();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isMigration;
        }

        /// <summary>
        /// This method is used to set default value(true) to 'Create New Prefix' checkbox from PDBC Prefix list.
        /// </summary>
        /// <param name="formInstnaceId"></param>
        public string SetDefaultCreateNewPrefix(string newFormData)
        {
            int PDBCSectionPosition = -1;
            string FormData = null;
            try
            {
                if (newFormData != null)
                {
                    JObject source = JObject.Parse(newFormData);
                    var pdbcPFXList = source.SelectToken(this._PDBCComponents);
                    foreach (var lst in pdbcPFXList)
                    {
                        PDBCSectionPosition++;

                        if (lst.SelectToken(this._PDBCType).ToString() == "BSBS")
                        {
                            source.SelectToken(this._PDBCComponents)[PDBCSectionPosition][this._createNewPrefixName] = "Yes";
                            source.SelectToken(this._PDBCComponents)[PDBCSectionPosition][this._isNewPrefixName] = "No";
                            break;
                        }
                    }
                    FormData = JsonConvert.SerializeObject(source);
                }


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return FormData;
        }

        public int GetFolderVersionByFormInstance(int formInstanceId)
        {
            int folderVersionId = 0;
            try
            {
                var formInstance = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.FormInstanceID == formInstanceId)
                                              .Get()
                                    select new FormInstanceViewModel
                                    {
                                        FormInstanceID = c.FormInstanceID,
                                        FolderVersionID = c.FolderVersionID
                                    }).FirstOrDefault();


                if (formInstance != null)
                {
                    folderVersionId = formInstance.FolderVersionID;
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return folderVersionId;
        }

        public FolderVersionViewModel GetFolderLockStatusForSync(int tenantId, int? folderId, int? userId)
        {
            FolderVersionViewModel model = null;
            try
            {
                model = (from e in _unitOfWork.RepositoryAsync<FolderLock>()
                                  .Query()
                                  .Include(c => c.User)
                                  .Filter(fil => fil.TenantID == tenantId &&
                                          fil.FolderID == folderId)
                                  .Get()
                         select new FolderVersionViewModel
                         {
                             LockedBy = e.LockedBy,
                             IsLocked = e.IsLocked,
                             LockedByUser = e.User.UserName,
                             LockedDate = e.LockedDate,
                         }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return model;
        }

        public string applDesignRule(FormDesignVersionDetail detail)
        {
            this._formData = JObject.Parse(detail.JSONData);
            this._formDesignData = detail;

            IterateThroughAllElementsInFormDesign(detail.Sections);

            return JsonConvert.SerializeObject(this._formData);
        }

        public void IterateThroughAllElementsInFormDesign(List<SectionDesign> sections)
        {
            List<string> False = new List<string> { "FALSE", "False" };
            List<string> True = new List<string> { "True", "TRUE" };
            JObject formData = (JObject)this._formData;
            //sections[0].CustomHtml = "ggggggggggggggg";
            foreach (SectionDesign sec in sections)
            {
                if (sec.FullName.ToUpper().Contains("CENTERSOFEXCELLENCE"))
                {
                }
                ExecuteRuleToSetVisibilityofElement(sec);
                foreach (ElementDesign ele in sec.Elements)
                {
                    try
                    {
                        if (ele.FullName.ToUpper().Contains("COBRAQUESTIONS"))
                        {
                        }

                        if (ele.Type == "section")
                        {
                            IterateThroughAllElementsInFormDesign(new List<SectionDesign>() { ele.Section });
                        }
                        else
                        {
                            if (ele.FullName.ToUpper().Contains("IFYESWHATISTHEEFFECTIVEDATE"))
                            {
                            }

                            ExecuteRuleToSetVisibilityofElement(ele);
                            if (ele.DataType == "bool" || ele.Type == "checkbox")
                            {

                                var IsValueFalse = False.Contains((string)formData.SelectToken(ele.FullName) ?? "");
                                var IsValueTrue = True.Contains((string)formData.SelectToken(ele.FullName) ?? "");
                                if (IsValueFalse)
                                    UpdateJsonAtSpecifiedPath(ele.FullName, formData, "false");
                                else if (IsValueTrue)
                                    UpdateJsonAtSpecifiedPath(ele.FullName, formData, "true");
                                //ReplaceFalseWithBlankForCheckboxesInJSON(ele.FullName, formData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void ExecuteRuleToSetVisibilityofElement(dynamic Element)
        {
            var Rules = _formDesignData.Rules.ToList();
            List<string> FalseVals = new List<string> { "false", "no", "", "null", null };
            List<string> TrueVals = new List<string> { "true", "yes" };
            bool Truthy = false;

            var IsRuleApplicableForSection = Rules.Where(i => i.UIElementFullName == Element.FullName).FirstOrDefault();
            if (IsRuleApplicableForSection != null)
            {
                List<ExpressionDesign> Expressions = IsRuleApplicableForSection.Expressions.ToList();

                List<ExpressionDesign> ExpressionList = Expressions[0].Expressions;
                if (ExpressionList == null)
                    ExpressionList = Expressions[1].Expressions;

                if (ExpressionList == null)
                    ExpressionList = Expressions[2].Expressions;

                var IsLogicalAND = Expressions[0].LogicalOperatorTypeId == 1 ? true : false;
                var IsPropertyVisibility = IsRuleApplicableForSection.TargetPropertyTypeId == 4 ? true : false;
                if (IsPropertyVisibility)
                {
                    Truthy = IsLogicalAND ? true : false;
                    #region Evaluate Truthness
                    foreach (ExpressionDesign exp in ExpressionList)
                    {
                        try
                        {
                            if ((IsLogicalAND && !Truthy) || (!IsLogicalAND && Truthy))
                                break;
                            var RightOperand = exp.RightOperand;        //  Yes No  True False   Static value
                            var LeftOperand = exp.LeftOperand;
                            bool IsLeftOperandBoolean = (LeftOperand ?? "").ToLower().Contains("radio") || (LeftOperand ?? "").ToLower().Contains("checkbox");
                            var LeftOperandName = exp.LeftOperandName;

                            string LeftOperandValue = string.Empty;
                            if (!string.IsNullOrEmpty(LeftOperandName))         //ADM11Radio5702
                                LeftOperandValue = Convert.ToString(_formData.SelectToken(LeftOperandName)) ?? "";
                            else
                            {
                                if (Element.FullName.Contains("CentersofExcellence.ProgramBasics.ProcedureInformation"))
                                    Truthy = false;
                                else
                                    Truthy = true;
                                continue;
                            }

                            //var LeftOperandValue = Convert.ToString(formData.SelectToken(LeftOperandName)) ?? "";
                            var RightOperandName = exp.RightOperandName;
                            var IsRightOperandElement = exp.IsRightOperandElement;
                            if (IsRightOperandElement)
                                RightOperand = Convert.ToString(_formData.SelectToken(RightOperandName)) ?? "";
                            var OperatorTypeId = exp.OperatorTypeId;
                            LeftOperandValue = (LeftOperandValue ?? "").ToLower();
                            RightOperand = (RightOperand ?? "").ToLower();

                            #region Logical AND or OR
                            if (IsLogicalAND)
                            {
                                if (OperatorTypeId == 1)     // Equals
                                {
                                    if (!IsLeftOperandBoolean)
                                        Truthy = Truthy && LeftOperandValue == RightOperand;
                                    else
                                        Truthy = Truthy && ((FalseVals.Contains(LeftOperandValue) && FalseVals.Contains(RightOperand))
                                                 || (TrueVals.Contains(LeftOperandValue) && TrueVals.Contains(RightOperand)));
                                }
                                else if (OperatorTypeId == 5)   // Not Equals
                                {
                                    if (!IsLeftOperandBoolean)
                                        Truthy = Truthy && LeftOperandValue != RightOperand;
                                    else
                                        Truthy = Truthy && ((FalseVals.Contains(LeftOperandValue) && !FalseVals.Contains(RightOperand))
                                                 || (TrueVals.Contains(LeftOperandValue) && !TrueVals.Contains(RightOperand)));
                                }
                            }
                            else
                            {
                                if (OperatorTypeId == 1)     // Equals
                                {
                                    if (!IsLeftOperandBoolean)
                                        Truthy = Truthy || LeftOperandValue == RightOperand;
                                    else
                                        Truthy = Truthy || ((FalseVals.Contains(LeftOperandValue) && FalseVals.Contains(RightOperand))
                                                 || (TrueVals.Contains(LeftOperandValue) && TrueVals.Contains(RightOperand)));
                                }
                                else if (OperatorTypeId == 5)   // Not Equals
                                {
                                    if (!IsLeftOperandBoolean)
                                        Truthy = Truthy || LeftOperandValue != RightOperand;
                                    else
                                        Truthy = Truthy || ((FalseVals.Contains(LeftOperandValue) && !FalseVals.Contains(RightOperand))
                                                 || (TrueVals.Contains(LeftOperandValue) && !TrueVals.Contains(RightOperand)));
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    #endregion

                    if (Truthy)
                        Element.Visible = true;
                    else
                        Element.Visible = false;
                }
            }
        }

        public static void UpdateJsonAtSpecifiedPath(string path, JToken formData, string UpdatedValue)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string[] pathArr = path.Split('.');
                int ItemCount = pathArr.Count();
                try
                {
                    switch (ItemCount)
                    {
                        case 1:
                            formData[pathArr[0]] = UpdatedValue;
                            break;
                        case 2:
                            formData[pathArr[0]][pathArr[1]] = UpdatedValue;
                            break;
                        case 3:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]] = UpdatedValue;
                            break;
                        case 4:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]] = UpdatedValue;
                            break;
                        case 5:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]] = UpdatedValue;
                            break;
                        case 6:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]] = UpdatedValue;
                            break;
                        case 7:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]] = UpdatedValue;
                            break;
                        case 8:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]][pathArr[7]] = UpdatedValue;
                            break;
                        case 9:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]][pathArr[7]][pathArr[8]] = UpdatedValue;
                            break;
                        case 10:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]][pathArr[7]][pathArr[8]][pathArr[9]] = UpdatedValue;
                            break;
                        case 11:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]][pathArr[7]][pathArr[8]][pathArr[9]][pathArr[10]] = UpdatedValue;
                            break;
                        case 12:
                            formData[pathArr[0]][pathArr[1]][pathArr[2]][pathArr[3]][pathArr[4]][pathArr[5]][pathArr[6]][pathArr[7]][pathArr[8]][pathArr[9]][pathArr[10]][pathArr[11]] = UpdatedValue;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ee)
                {
                }
            }
        }

        public List<FormInstanceViewModel> GetProductById(int formInstanceID)
        {
            List<FormInstanceViewModel> modelList = new List<FormInstanceViewModel>();
            FormInstanceViewModel model = new FormInstanceViewModel();
            try
            {
                var data = (from a in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Query().Filter(c => c.FormInstanceID == formInstanceID).Get()
                            join fIn in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                            on a.FormInstanceID equals fIn.FormInstanceID
                            join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                            on fIn.FormDesignID equals fd.FormID
                            join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                            on fIn.FormDesignVersionID equals fdv.FormDesignVersionID
                            where (a.FormInstanceID == formInstanceID && fIn.IsActive == true)
                            select new FormInstanceViewModel
                            {
                                FormInstanceDataMapID = a.FormInstanceDataMapID,
                                FormData = a.FormData,
                                FormDesignName = fd.FormName,
                                FormDesignVersionNumber = fdv.VersionNumber,
                                FormInstanceName = fIn.Name
                            }).FirstOrDefault();
                if (data == null)
                    return null;
                model.FormData = data.FormData;//.ToString().Replace("\r\n", string.Empty).Replace("\"", "");
                model.FormDesignName = data.FormDesignName;
                model.FormDesignVersionNumber = data.FormDesignVersionNumber;
                model.FormInstanceName = data.FormInstanceName;
                modelList.Add(model);
                if (modelList.Count() == 0)
                {
                    modelList = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return modelList;
        }

        public List<FormInstanceViewModel> GetProductByProductId(string productId)
        {
            List<FormInstanceViewModel> modelList = new List<FormInstanceViewModel>();
            try
            {
                var formInstanceId = (from fId in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Query().Filter(c => c.FormInstanceID == Int32.Parse(productId)).Get()
                                      join fI in this._unitOfWork.Repository<FormInstance>().Get()
                                                    on fId.FormInstanceID equals fI.FormInstanceID
                                      where (fI.Name == productId && fI.IsActive == true)
                                      select new FormInstanceViewModel
                                      {
                                          FormInstanceID = fId.FormInstanceID
                                      }).FirstOrDefault().FormInstanceID;
                if (formInstanceId == null)
                    return null;
                modelList = (from a in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get().Where(c => c.FormInstanceID == formInstanceId)
                             select new FormInstanceViewModel
                             {
                                 FormInstanceDataMapID = a.FormInstanceDataMapID,
                                 FormData = a.FormData
                             }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                return null;
            }
            return modelList;
        }

        public List<FormInstanceViewModel> GetProductDataByElementNames(int formInstanceId, string elementList, string elementType)
        {
            List<FormInstanceViewModel> formData = new List<FormInstanceViewModel>();

            string[] elements = (elementList ?? "").Split(',');

            FormInstanceViewModel data = null;

            data = (from fID in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Query().Filter(c => c.FormInstanceID == formInstanceId).Get()
                    join fI in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                    on fID.FormInstanceID equals fI.FormInstanceID
                    join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                    on fI.FormDesignID equals fd.FormID
                    join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                    on fI.FormDesignVersionID equals fdv.FormDesignVersionID
                    where (fI.FormInstanceID == formInstanceId && fI.IsActive == true)
                    select new FormInstanceViewModel
                    {
                        FormData = fID.FormData,
                        FormDesignVersionID = fdv.FormDesignVersionID,
                        FormDesignVersionNumber = fdv.VersionNumber,
                        FormDesignName = fd.FormName,
                        FormInstanceName = fI.Name
                    }).FirstOrDefault();
            if (data != null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    var elementName = elements[i].Trim();
                    if (!string.IsNullOrEmpty(elementName))
                    {
                        FormInstanceViewModel productData = new FormInstanceViewModel();
                        try
                        {
                            UIElement uiElement = null;
                            switch (elementType)
                            {
                                case "section":
                                    uiElement = (from fdvUiMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Query().Filter(c => c.FormDesignVersionID == data.FormDesignVersionID).Get()
                                                 join ui in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(c => c.GeneratedName == elementName).Get()
                                                  on fdvUiMap.UIElementID equals ui.UIElementID
                                                 join secui in this._unitOfWork.RepositoryAsync<SectionUIElement>().Get()
                                                 on fdvUiMap.UIElementID equals secui.UIElementID
                                                 select ui).FirstOrDefault();
                                    if (uiElement != null)
                                    {
                                        data.UIElementID = uiElement.UIElementID;
                                    }

                                    break;
                                case "repeater":
                                    uiElement = (from fdvUiMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Query().Filter(c => c.FormDesignVersionID == data.FormDesignVersionID).Get()
                                                 join ui in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(c => c.GeneratedName == elementName).Get()
                                                  on fdvUiMap.UIElementID equals ui.UIElementID
                                                 join secui in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                                                 on fdvUiMap.UIElementID equals secui.UIElementID
                                                 select ui).FirstOrDefault();
                                    if (uiElement != null)
                                    {
                                        data.UIElementID = uiElement.UIElementID;
                                    }
                                    break;

                            }

                            if (uiElement != null)
                            {
                                JObject source = JObject.Parse(data.FormData);
                                JObject sectionData = new JObject();
                                string path = GetUIElementFullPath(data.UIElementID, data.FormDesignVersionID);
                                sectionData.Add(elementName, source.SelectToken(path));
                                productData.FormData = sectionData.ToString();
                                productData.path = path;
                                productData.FormInstanceName = data.FormInstanceName;
                                productData.FormDesignVersionNumber = data.FormDesignVersionNumber;
                                productData.FormDesignName = data.FormDesignName;
                            }
                            else
                                continue;
                        }

                        catch (Exception ex)
                        {

                            bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                            return null;
                        }
                        formData.Add(productData);
                    }
                }
            }
            return formData;
        }

        public string GetUIElementFullPath(int uielementID, int formDesignVersionID)
        {
            string fullName = "";
            try
            {
                IList<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                                    join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                            .Query()
                                                                            .Get()
                                                    on u.UIElementID equals fd.UIElementID
                                                    where fd.FormDesignVersionID == formDesignVersionID
                                                    select u).ToList();

                UIElement element = (from elem in formElementList
                                     where elem.UIElementID == uielementID
                                     select elem).FirstOrDefault();
                if (element != null)
                {
                    fullName = GetFullPath(element, formElementList);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return fullName;
        }

        public string GetFullPath(UIElement element, IEnumerable<UIElement> formElementList)
        {
            string fullName = "";
            try
            {
                int currentElementID = element.UIElementID;
                int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                fullName = element.GeneratedName;
                while (parentUIElementID > 0)
                {
                    element = (from elem in formElementList
                               where elem.UIElementID == parentUIElementID
                               select elem).FirstOrDefault();
                    parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = element.GeneratedName + "." + fullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        public FolderVersionViewModel GetFolderVersion(int folderVersionID)
        {
            FolderVersionViewModel folderVersion = null;
            try
            {
                folderVersion = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                 join con in this._unitOfWork.RepositoryAsync<Consortium>().Get()
                                 on fldv.ConsortiumID equals con.ConsortiumID
                                 where (con.IsActive == true && fldv.FolderVersionID == folderVersionID)
                                 select new FolderVersionViewModel
                                 {
                                     CategoryID = fldv.CategoryID,
                                     CatID = fldv.CatID,
                                     ConsortiumID = con.ConsortiumID,
                                 }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersion;
        }

        public int GetFolderIdByFolderName(string Name)
        {
            int folderID = this._unitOfWork.Repository<Folder>().Get()
                                        .Where(s => s.Name.Equals(Name)).OrderByDescending(a => a.AddedDate)
                                        .Select(s => s.FolderID).FirstOrDefault();
            return folderID;
        }

        public MasterListDocuments GetMasterListDocuments()
        {
            MasterListDocuments documents = new MasterListDocuments();

            var masterListDocuments = (from deg in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                                       join degVer in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                       on deg.FormID equals degVer.FormDesignID
                                       where deg.IsActive && deg.IsMasterList
                                       orderby deg.DisplayText
                                       select deg).ToList();


            if (masterListDocuments != null && masterListDocuments.Any())
            {
                documents.Sections = new List<Section>();
                foreach (var doc in masterListDocuments)
                {
                    if (!documents.Sections.Any())
                        documents.Sections.Add(new Section { Enabled = true, Visible = true, Name = doc.FormName, Label = doc.DisplayText });
                    else if (!documents.Sections.Select(j => j.Name).ToList().Contains(doc.FormName))
                        documents.Sections.Add(new Section { Enabled = true, Visible = true, Name = doc.FormName, Label = doc.DisplayText });
                }
            }

            return documents;
        }


        public FolderVersionViewModel LoadFolderVersionViewModel(int folderId, int folderVersionId)
        {
            FolderVersionViewModel model = null;


            model = (from fldver in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                     join fld in this._unitOfWork.RepositoryAsync<Folder>().Get()
                     on fldver.FolderID equals fld.FolderID
                     where fldver.FolderID == folderId && fldver.FolderVersionID == folderVersionId
                     select new FolderVersionViewModel
                     {
                         AddedBy = fldver.AddedBy,
                         AddedDate = fldver.AddedDate,
                         Comments = fldver.Comments,
                         EffectiveDate = fldver.EffectiveDate,
                         FolderId = fldver.FolderID,
                         FolderName = fld.Name,
                         FolderVersionId = fldver.FolderVersionID,
                         FolderVersionNumber = fldver.FolderVersionNumber,
                         IsActive = fldver.IsActive,
                         TenantID = fldver.TenantID,
                         FolderVersionStateID = fldver.FolderVersionStateID,
                         CategoryID = fldver.CategoryID
                     }).FirstOrDefault();

            return model;
        }


        public ServiceResult ReleaseMLVersion(int tenantId, int folderId, int folderVersionId, string userName, string majorFolderVersionNumber, string commentText)
        {
            ServiceResult result = null;
            string commenttext = commentText;
            var folderversionToUpdate = this._unitOfWork.RepositoryAsync<FolderVersion>().FindById(folderVersionId);

            try
            {
                result = new ServiceResult();
                List<ServiceResultItem> items = new List<ServiceResultItem>();
                int designStatusID = this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                    .Filter(s => s.FolderVersionID == folderVersionId)
                    .Include(f => f.FormDesignVersion)
                    .Get()
                    .FirstOrDefault().FormDesignVersion.StatusID;

                if (designStatusID == 3)
                {
                    VersionNumberBuilder builder = new VersionNumberBuilder();
                    var folderVersion = this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == folderVersionId).FirstOrDefault();
                    string updatedVersionNumber = builder.GetNextMajorVersionNumber(folderVersion.FolderVersionNumber, folderVersion.EffectiveDate);
                    folderversionToUpdate.FolderVersionNumber = updatedVersionNumber;
                    folderversionToUpdate.FolderVersionStateID = Convert.ToInt32(FolderVersionState.RELEASED);
                    folderversionToUpdate.WFStateID = null;
                    folderversionToUpdate.Comments = commentText;

                    folderversionToUpdate.AddedBy = userName;
                    folderversionToUpdate.UpdatedBy = userName;
                    folderversionToUpdate.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderversionToUpdate);
                    this._unitOfWork.Save();

                    items.Add(new ServiceResultItem { Messages = new string[] { folderVersionId.ToString(), string.IsNullOrEmpty(commentText) ? "Released" : commentText } });
                    result.Items = items;
                    result.Result = ServiceResultStatus.Success;

                    this.UpdateFolderChange(tenantId, userName, folderId, folderVersionId);
                    this.UpdateRetroChangesWhenReleased(tenantId, folderVersionId, userName, folderversionToUpdate.FolderID);
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure; ;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder Version can not be Released, Document Design Version needs to be Finalized first!" } });
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }



        public bool GetUserFolderVersiontCreationPermission(int? userId, bool isPortfolioSearch)
        {
            if (userId == null) throw new ArgumentNullException();
            int accountType = 0;
            accountType = isPortfolioSearch == true ? 1 : 2;
            bool isUserPermittedToCreateFolderVersion = false;
            try
            {
                int roleId = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()
                              join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                              on userMap.UserID equals userRoleMap.UserId
                              where userMap.UserID == userId
                              select new List<int>
                              {
                                  userRoleMap.RoleId
                              }).FirstOrDefault().First<int>();
                AccountFolderCreationPermission accountFolderCreationPermission = this._unitOfWork.RepositoryAsync<AccountFolderCreationPermission>()
                    .Query()
                    .Filter(row => row.UserRoleID == roleId && row.HasAccountCreationPermission == true && row.AccountType == accountType).Get().FirstOrDefault();
                if (accountFolderCreationPermission != null)
                {
                    isUserPermittedToCreateFolderVersion = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isUserPermittedToCreateFolderVersion;
        }

        public List<FormInstanceViewModel> GetProductList(int tenantId, int folderVersionId)
        {
            Contract.Requires(folderVersionId > 0, "Invalid Folder Version Id ");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            List<int> formDesignIdsNeedstobeQueuedFortranslation = new List<int>() { 2405, 2447, 2448, 2449, 2450, 2458 };
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                //Check if folder is Foundation Template
                //var isfoundationFolder = (from d in this._unitOfWork.RepositoryAsync<FolderVersion>()
                //                                        .Query()
                //                                        .Include(inc => inc.Folder)
                //                                        .Filter(d => d.FolderVersionID == folderVersionId)
                //                                        .Get()

                // select d.Folder.IsFoundation).FirstOrDefault();
                //if (!isfoundationFolder)
                {
                    var formInstances = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                         join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                         join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                         join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                         join apm in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get() on fi.FormInstanceID equals apm.FormInstanceID
                                         into wt5
                                         from wt6 in wt5.DefaultIfEmpty()
                                         join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get() on fv.ConsortiumID equals cons.ConsortiumID
                                         into wt1
                                         from wt in wt1.DefaultIfEmpty()
                                         join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID
                                         into wt2
                                         from afm in wt2.DefaultIfEmpty()
                                         join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on afm.AccountID equals accn.AccountID
                                    into wt3
                                         from ac in wt3.DefaultIfEmpty()
                                         where (fv.FolderVersionID == folderVersionId && fi.IsActive == true && formDesignIdsNeedstobeQueuedFortranslation.Contains(fi.FormDesignID))
                                         select new FormInstanceViewModel
                                         {
                                             FolderID = fldr.FolderID,
                                             FolderName = fldr.Name,
                                             FolderVersionNumber = fv.FolderVersionNumber,
                                             FolderVersionID = fv.FolderVersionID,
                                             AccountID = ac == null ? 0 : ac.AccountID,
                                             AccountName = ac == null ? "" : ac.AccountName,
                                             ConsortiumID = wt == null ? 0 : (int)wt.ConsortiumID,
                                             ConsortiumName = wt == null ? "" : wt.ConsortiumName,
                                             FormInstanceName = fi.Name,
                                             FormInstanceID = fi.FormInstanceID,
                                             IsTranslated = false,
                                             EffectiveDate = fv.EffectiveDate,
                                             FormDesignID = fi.FormDesignID,
                                             FormDesignVersionID = fi.FormDesignVersionID,
                                             ProductName = wt6.ProductName,
                                             ProductID = wt6.ProductID,
                                             ProductType = wt6.ProductType

                                         });

                    if (formInstances != null)
                    {
                        formInstanceList = formInstances.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public List<FormInstanceViewModel> GetChildElementsOfFormInstance(int tenantId, int formInstanceID)
        {
            List<FormInstanceViewModel> formInstanceList = null;
            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                              .Query()
                              .Include(c => c.FormDesign)
                              .Filter(c => c.TenantID == tenantId && c.AnchorDocumentID == formInstanceID && c.FormInstanceID != formInstanceID)
                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = c.Name != null ? c.Name : c.FormDesign.FormName,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                         EffectiveDate = c.FormDesignVersion.EffectiveDate

                                     });

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return formInstanceList;
        }

        public DateTime GetFolderVersionEffectiveDate(int folderVersionID)
        {
            DateTime folderVersionEffectiveDate = DateTime.Now;
            try
            {
                folderVersionEffectiveDate = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Filter(fil => fil.FolderVersionID == folderVersionID && fil.FolderVersionStateID != (int)FolderVersionState.RELEASED && fil.FolderVersionStateID != (int)FolderVersionState.BASELINED && fil.FolderVersionStateID != (int)FolderVersionState.INPROGRESS_BLOCKED)
                                                    .Get()
                                                    .Select(sel => sel.EffectiveDate)
                                                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersionEffectiveDate;
        }


        public DateTime GetFolderVersionEffectiveDate(int tenantId, int folderVersionID)
        {
            DateTime folderVersionEffectiveDate = DateTime.Now;
            try
            {
                folderVersionEffectiveDate = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Filter(fil => fil.FolderVersionID == folderVersionID)
                                                    .Get()
                                                    .Select(sel => sel.EffectiveDate)
                                                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersionEffectiveDate;
        }

        /// <summary>
        /// Delete select Folder
        /// </summary>
        /// <param name="tenantId">TenantID</param>
        /// <param name="folderID">FolderID</param>
        /// <returns>retuen selectd folder if failure result</returns>
        public ServiceResult DeleteNonPortfolioBasedFolder(int tenantId, int folderID)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                SqlParameter paramTenantID = new SqlParameter("@TenantID", tenantId);
                SqlParameter paramFolderID = new SqlParameter("@CurrentFolderID", folderID);

                if (null != paramFolderID)
                {
                    var deleteFolder = this._unitOfWork.Repository<Folder>().ExecuteSql("exec [dbo].[USP_Delete_Folders] @TenantID ,@CurrentFolderID ", paramTenantID, paramFolderID).FirstOrDefault();
                    if (deleteFolder != null && deleteFolder.FolderID == folderID)
                        result.Result = ServiceResultStatus.Failure;
                    else
                        result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }

            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        }

        /// <summary>
        /// Get values of Platform and PlanDesign fields for provided forminstance
        /// </summary>
        /// <param name="forminstanceId"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetPltformPlnDesign(int forminstanceId)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            FormInstanceDataMap formInstanceDataMapList = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                                      .Query()
                                                                                      .Include(f => f.FormInstance)
                                                                                      .Filter(fil => fil.FormInstanceID == forminstanceId)
                                                                                      .Get()
                                                                                      .Select(s => s)
                                                                                      .FirstOrDefault();

            string formdata = formInstanceDataMapList.FormData;
            string platformValue = JObject.Parse(formdata).SelectToken("ProductRules").SelectToken("Platform").ToString();
            string planDesign = JObject.Parse(formdata).SelectToken("ProductRules").SelectToken("PlanDesign").ToString();
            dict.Add(1, platformValue);
            dict.Add(2, planDesign);
            return dict;
        }

        public IQueryable<FolderViewModel> GetFolderList(int tenantId, int? userId)
        {
            IQueryable<FolderViewModel> folderList = null;
            try
            {
                folderList = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Query().Filter(c => c.IsPortfolio == true).Get().OrderBy(x => x.Name)
                              select new FolderViewModel
                              {
                                  DocId = folder.FolderID,
                                  FormName = folder.Name
                              }).AsQueryable();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return folderList;
        }

        public IQueryable<FolderVersionViewModel> GetFolderVersionsList(int tenantId, int? userId, int folderId)
        {
            IQueryable<FolderVersionViewModel> folderVersionList = null;
            try
            {
                folderVersionList = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                               .Query()
                               .Filter(c => c.TenantID == tenantId && c.IsActive == true
                                         && c.FolderID == folderId && c.FolderVersionStateID == (int)FolderVersionState.INPROGRESS)
                               .Include(c => c.Folder)
                               .Include(c => c.WorkFlowVersionState)
                               .Get()
                                     select new FolderVersionViewModel
                                     {
                                         FolderId = c.FolderID,
                                         FolderVersionId = c.FolderVersionID,
                                         TenantID = c.TenantID,
                                         FolderName = c.Folder.Name,
                                         EffectiveDate = c.EffectiveDate,
                                         FolderVersionNumber = c.FolderVersionNumber,
                                         WFStateID = c.WFStateID,
                                         IsActive = c.IsActive,
                                         AccountId = c.Folder.AccountFolderMaps.Select(d => d.AccountID).FirstOrDefault(),
                                         IsPortfolio = c.Folder.IsPortfolio,
                                         VersionTypeID = c.VersionTypeID,
                                         FolderVersionStateID = c.FolderVersionStateID,
                                         CategoryID = c.CategoryID,
                                         CatID = c.CatID,
                                         WFStateName = c.WorkFlowVersionState.WorkFlowState.WFStateName
                                     }).AsQueryable();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersionList;
        }

        public List<FormInstanceViewModel> GetFormInstanceListForFolderVersion(int tenantId, int folderVersionId, int folderId, int formDesignTypeId = 0)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Include(c => c.FormDesign)
                                              .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.AnchorDocumentID == c.FormInstanceID && (formDesignTypeId == 0 || c.FormDesign.DocumentDesignTypeID == formDesignTypeId) && c.IsActive == true
                                              )
                                              .Get()
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = c.FormInstanceID,
                                         FolderVersionID = c.FolderVersionID,
                                         FormDesignID = c.FormDesignID,
                                         FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                         TenantID = c.TenantID,
                                         FormDesignVersionID = c.FormDesignVersionID,
                                     }).ToList();

                if (formInstances != null)
                {
                    formInstanceList = formInstances.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public List<FormInstanceViewModel> GetAncillaryProductList(int folderVersionId, string sectionName)
        {
            List<FormInstanceViewModel> productList = new List<FormInstanceViewModel>();
            try
            {
                int formDesignId = this._unitOfWork.RepositoryAsync<FormDesign>().Query().Filter(c => c.FormName.ToString() == sectionName).Get().Select(a => a.FormID).FirstOrDefault();

                DateTime effectiveDate = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderVersionID == folderVersionId).Get().Select(a => a.EffectiveDate).FirstOrDefault();

                List<int> folderVersionIDs = (from p in _unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                              where p.EffectiveDate <= effectiveDate && p.FolderVersionStateID == 3
                                              group p by p.FolderID into g
                                              select g.Select(m => m.FolderVersionID).Max()).ToList();

                productList = (from fl in this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.EffectiveDate <= effectiveDate && c.FolderVersionStateID == 3).Get()
                               join ap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                               on fl.FolderVersionID equals ap.FolderVersionID
                               join fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               on ap.FormInstanceID equals fi.FormInstanceID
                               join f in this._unitOfWork.RepositoryAsync<Folder>().Get()
                               on fl.FolderID equals f.FolderID
                               where (fi.FormDesignID == formDesignId) && folderVersionIDs.Contains(fl.FolderVersionID)
                               select new FormInstanceViewModel
                               {
                                   FormInstanceID = ap.FormInstanceID,
                                   Name = fi.Name,
                                   FormDesignVersionID = fi.FormDesignVersionID,
                                   FolderEffectiveDate = fl.EffectiveDate,
                                   FolderVersionNumber = fl.FolderVersionNumber,
                                   FolderName = f.Name,
                               }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return productList;
        }

        private void CreateNewViewForExistingDocument(int tenantId, int anchorFormInstanceId)
        {
            var folderVersionId = this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get().Where(c => c.FormInstanceID == anchorFormInstanceId).Select(c => c.FolderVersionID).FirstOrDefault();
            FormInstanceViewModel fi = (from frmIns in this._unitOfWork.RepositoryAsync<FormInstance>()
                                        .Query()
                                        .Filter(c => c.FormInstanceID == anchorFormInstanceId)
                                        .Get()
                                        select new FormInstanceViewModel
                                        {
                                            FolderVersionID = frmIns.FolderVersionID,
                                            FormDesignID = frmIns.FormDesignID,
                                            AnchorDocumentID = frmIns.AnchorDocumentID,
                                            Name = frmIns.Name,
                                            AddedBy = frmIns.AddedBy
                                        }
                                        ).FirstOrDefault();

            FolderVersionViewModel fv = (from fldver in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                 .Query()
                                                 .Filter(fdv => fdv.FolderVersionID == fi.FolderVersionID)
                                                 .Get()
                                         select new FolderVersionViewModel
                                         {
                                             EffectiveDate = fldver.EffectiveDate,
                                             FolderId = fldver.FolderID,
                                         }).FirstOrDefault();
            int folderID = fv.FolderId;
            DateTime? effectiveDate = fv.EffectiveDate;
            var childFormDesignList = this._formDesignService.GetMappedDesignDocumentList(tenantId, fi.FormDesignID, effectiveDate);
            var existingViewList = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive == true && (c.AnchorDocumentID == fi.AnchorDocumentID) && (c.FormInstanceID != fi.AnchorDocumentID))
                                              .Get()
                                    join d in this._unitOfWork.RepositoryAsync<FormDesign>()
                                               .Query()
                                              .Filter(c => c.IsActive == true)
                                              .Get()
                                    on c.FormDesignID equals d.FormID
                                    join e in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get()
                                    on d.DocumentDesignTypeID equals e.DocumentDesignTypeID
                                    orderby c.FormInstanceID
                                    select new DocumentViewListViewModel
                                    {
                                        FormInstanceId = c.FormInstanceID,
                                        FormDesignID = d.FormID,
                                        FormDesignTypeID = d.DocumentDesignTypeID,
                                        FormDesignTypeName = e.DocumentDesignName,
                                        FormDesignName = d.FormName,
                                        FormDesignVersionID = c.FormDesignVersionID
                                    }).ToList();

            if (existingViewList.Count() < (childFormDesignList != null ? childFormDesignList.Count() : 0))
            {
                foreach (var childDesign in childFormDesignList)
                {
                    string childFormName = fi.Name + "@@" + childDesign.FormDesignName;
                    int childFormDesignId = childDesign.FormDesignID;
                    int currentFormDesignId = existingViewList.Where(c => c.FormDesignID == childFormDesignId).Select(c => c.FormDesignID).FirstOrDefault();
                    if (currentFormDesignId == 0)
                    {
                        string appName = config.Config.GetApplicationName();
                        if (!(appName.ToLower() == "emedicaresync" && childDesign.FormDesignID == 2409))
                        {
                            AddFormInstance(tenantId, folderVersionId, childDesign.FormDesignVersionID, childDesign.FormDesignID, childFormName, fi.AddedBy, anchorFormInstanceId);
                        }
                    }
                }
            }
        }

        public bool IsDataForFormInstanceChanged(int formInstanceId)
        {
            bool isdataChanged = false;
            string currentDataHash = string.Empty;
            //Check the JSON Hash chnage
            var formData = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                    .Query()
                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                    .Get().FirstOrDefault();
            if (formData != null)
            {
                currentDataHash = HashGenerator.ToMD5(formData.FormData.ToString());
            }
            var formInstanceData = this._unitOfWork.RepositoryAsync<FormInstance>()
                                    .Query()
                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                    .Get().FirstOrDefault();
            if (formInstanceData.ProductJsonHash != currentDataHash)
            {
                isdataChanged = true;
                //Update formInstance JSOn Hash
                formInstanceData.ProductJsonHash = currentDataHash;
                this._unitOfWork.RepositoryAsync<FormInstance>().Update(formInstanceData);
                this._unitOfWork.Save();
            }
            return isdataChanged;
        }
        public int GetFormDesignIDByFormName(string FormName)
        {
            FormInstanceViewModel fi = (from frmIns in this._unitOfWork.RepositoryAsync<FormDesign>()
                                        .Query()
                                        .Filter(c => c.FormName == FormName)
                                        .Get()
                                        select new FormInstanceViewModel
                                        {
                                            FormDesignID = frmIns.FormID
                                        }
                                        ).FirstOrDefault();
            return fi.FormDesignID;
        }
    }
}