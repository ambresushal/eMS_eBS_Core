using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Diagnostics.Contracts;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.infrastructure.util;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.FormDesignVersionCompare;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.domain.entities;
using System.Configuration;
using tmg.equinox.setting;
using tmg.equinox.setting.Interface;

namespace tmg.equinox.applicationservices
{
    public partial class FormDesignService : IFormDesignService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IDomainModelService _domainModelService { get; set; }

        private IReportingDataService _reportingDataService;

        private IGenerateSchemaService _generateSchemaService;
        private ISettingManager _settingManager;
        ISectionLockService _sectionLockService;

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public FormDesignService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService, IDomainModelService domainModelService, IReportingDataService reportingDataService, IGenerateSchemaService generateSchemaService, ISectionLockService sectionLockService, ISettingManager settingManager)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
            this._domainModelService = domainModelService;
            _reportingDataService = reportingDataService;
            _generateSchemaService = generateSchemaService;
            _sectionLockService = sectionLockService;
            _settingManager = settingManager;
        }
		//Need to verify following constructor required or not
		//Introduced from Jamir Code for hangfire
		public FormDesignService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// This method returns a collection of Data from 'FormDesign' table
        /// The collection is filtered using tenantId which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<FormDesignRowModel> GetFormDesignList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<FormDesignRowModel> formDesignList = null;
            try
            {
                formDesignList = (from c in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantId && c.IsActive == true)
                                                                        .Get().OrderBy(c => c.DisplayText)
                                  select new FormDesignRowModel
                                  {
                                      FormDesignId = c.FormID,
                                      FormDesignName = c.FormName,
                                      DisplayText = c.DisplayText,
                                      //IsActive = c.IsActive,
                                      //Abbreviation = c.Abbreviation,
                                      TenantID = c.TenantID,
                                      //AddedBy = c.AddedBy,
                                      //AddedDate = c.AddedDate,
                                      //UpdatedBy = c.UpdatedBy,
                                      //UpdatedDate = c.UpdatedDate,
                                      IsMasterList = c.IsMasterList,
                                      DocumentDesignTypeID = c.DocumentDesignTypeID,
                                      DocumentLocationID = c.DocumentLocationID,
                                      IsAliasDesignMasterList = c.IsAliasDesignMasterList,
                                      UsesAliasDesignMasterList = c.UsesAliasDesignMasterList
                                  }).ToList();

                if (formDesignList.Count() == 0)
                    formDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }

        public List<FormDesignGroupMapModel> GetFormDesignGroupMap()
        {
            List<FormDesignGroupMapModel> formGroups = new List<FormDesignGroupMapModel>();
            try
            {
                formGroups = (from f in _unitOfWork.RepositoryAsync<FormDesign>().Get()
                              join map in this._unitOfWork.RepositoryAsync<FormDesignMapping>().Get() on f.FormID equals map.TargetDesignID into gj
                              from subset in gj.DefaultIfEmpty()
                              join grpmap in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get() on f.FormID equals grpmap.FormID
                              join grp in this._unitOfWork.RepositoryAsync<FormDesignGroup>().Get() on grpmap.FormDesignGroupID equals grp.FormDesignGroupID
                              select new FormDesignGroupMapModel
                              {
                                  FormDesignID = f.FormID,
                                  FormName = f.FormName,
                                  FolderType = grp.GroupName,
                                  ParentFormDesignID = subset == null ? 0 : subset.AnchorDesignID,
                                  AllowMultiple = grpmap.AllowMultipleInstance ?? false,
                                  IsMasterList = f.IsMasterList
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formGroups;
        }

        public IEnumerable<FormDesignRowModel> GetAnchorDesignList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<FormDesignRowModel> formDesignList = null;
            try
            {
                var childwithAnchor = (from ac in this._unitOfWork.RepositoryAsync<FormDesignMapping>()
                                           .Query()
                                           .Get()
                                       select ac.TargetDesignID).ToList();



                formDesignList = (from c in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantId && c.IsActive == true && c.DocumentDesignTypeID == 1 && !childwithAnchor.Contains(c.FormID))
                                                                        .Get().OrderBy(c => c.DisplayText)
                                  select new FormDesignRowModel
                                  {
                                      FormDesignId = c.FormID,
                                      FormDesignName = c.FormName,
                                      DisplayText = c.DisplayText,
                                      TenantID = c.TenantID,
                                      IsMasterList = c.IsMasterList,
                                      DocumentDesignTypeID = c.DocumentDesignTypeID,
                                      DocumentLocationID = c.DocumentLocationID,
                                      IsAliasDesignMasterList = c.IsAliasDesignMasterList,
                                      UsesAliasDesignMasterList = c.UsesAliasDesignMasterList
                                  }).ToList();

                if (formDesignList.Count() == 0)
                    formDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }

        public IEnumerable<FormDesignRowModel> GetFormDesignListByDocType(int tenantId, int docType)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<FormDesignRowModel> formDesignList = null;
            try
            {
                formDesignList = (from c in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  join p in this._unitOfWork.RepositoryAsync<FormDesignMapping>().Get()
                                  on c.FormID equals p.TargetDesignID into result
                                  from data in result.DefaultIfEmpty()
                                  where (c.TenantID == tenantId && c.IsActive == true && c.DocumentDesignTypeID == docType)

                                  select new FormDesignRowModel
                                  {
                                      FormDesignId = c.FormID,
                                      FormDesignName = c.FormName,
                                      DisplayText = c.DisplayText,
                                      TenantID = c.TenantID,
                                      IsMasterList = c.IsMasterList,
                                      SourceDesign = data == null ? 0 : data.AnchorDesignID,
                                      DocumentDesignTypeID = c.DocumentDesignTypeID,
                                      DocumentLocationID = c.DocumentLocationID,
                                      IsAliasDesignMasterList = c.IsAliasDesignMasterList,
                                      UsesAliasDesignMasterList = c.UsesAliasDesignMasterList,
                                      IsSectionLock = c.IsSectionLock

                                  }).ToList();
                if (formDesignList.Count() == 0)
                    formDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }

        public IEnumerable<DocumentDesignTypeRowModel> GetDocumentDesignType()
        {
            IList<DocumentDesignTypeRowModel> DocDesignType = null;
            try
            {
                DocDesignType = (from c in this._unitOfWork.RepositoryAsync<DocumentDesignType>()
                                                                        .Query()
                                                                        .Get()
                                 select new DocumentDesignTypeRowModel
                                 {
                                     DocumentDesignTypeID = c.DocumentDesignTypeID,
                                     DocumentDesignName = c.DocumentDesignName
                                 }).ToList();

                if (DocDesignType.Count() == 0)
                    DocDesignType = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return DocDesignType;
        }

        public ServiceResult AddFormDesign(string userName, int tenantId, string formName, string displayText, string abbreviation, bool isMasterList, int docType, int srcDesign, bool isAliasDesign, bool usesAliasDesign, bool IsSectionLock)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(!string.IsNullOrEmpty(formName), "Formname cannot be empty");
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesign itemToAdd = new FormDesign();
                itemToAdd.FormName = formName;
                itemToAdd.DisplayText = displayText;
                itemToAdd.Abbreviation = abbreviation;
                itemToAdd.IsMasterList = isMasterList;
                itemToAdd.IsActive = true;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.TenantID = tenantId;
                itemToAdd.DocumentDesignTypeID = docType;
                itemToAdd.IsAliasDesignMasterList = isAliasDesign;
                itemToAdd.UsesAliasDesignMasterList = usesAliasDesign;
                itemToAdd.IsSectionLock = IsSectionLock;
                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormDesign>().Insert(itemToAdd);
                this._unitOfWork.Save();

                //after Save, generate abbreviation using new ID
                itemToAdd.Abbreviation = (itemToAdd.FormName.Length > 3 ? itemToAdd.FormName.Substring(0, 3).ToUpper() : itemToAdd.FormName.ToUpper()) + itemToAdd.FormID;
                this._unitOfWork.RepositoryAsync<FormDesign>().Update(itemToAdd);
                this._unitOfWork.Save();

                //save parent and child design link
                if (srcDesign > 0)
                {
                    FormDesignMapping addMapping = new FormDesignMapping();
                    addMapping.AnchorDesignID = srcDesign;
                    addMapping.TargetDesignID = itemToAdd.FormID;

                    this._unitOfWork.RepositoryAsync<FormDesignMapping>().Insert(addMapping);
                    this._unitOfWork.Save();
                }

                _sectionLockService.RefeshFormDesignLock();
                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateFormDesign(string userName, int tenantId, int formDesignId, string formName, string displayText, int srcDesign, bool IsSectionLock)
        {
            Contract.Requires(!string.IsNullOrEmpty(userName), "Username cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(!string.IsNullOrEmpty(formName), "Display Text cannot be empty");
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesign itemToUpdate = this._unitOfWork.RepositoryAsync<FormDesign>()
                                                               .FindById(formDesignId);

                UIElement itemToUpdateTabName = this._unitOfWork.RepositoryAsync<UIElement>()
                                                .Query()
                                                .Filter(frm => frm.FormID == formDesignId && frm.UIElementName.Contains("TAB"))
                                                .Get().FirstOrDefault();


                if (itemToUpdate != null)
                {
                    //itemToUpdate.FormName = formName;
                    itemToUpdate.DisplayText = displayText;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    itemToUpdate.IsSectionLock = IsSectionLock;
                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<FormDesign>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    if (itemToUpdateTabName != null)
                    {
                        itemToUpdateTabName.Label = displayText;
                        itemToUpdateTabName.UpdatedBy = userName;
                        itemToUpdateTabName.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<UIElement>().Update(itemToUpdateTabName);
                        this._unitOfWork.Save();
                    }
                    //Delete from mapping table any data related to child parent mapping
                    FormDesignMapping el = this._unitOfWork.RepositoryAsync<FormDesignMapping>().Query().Filter(c => c.TargetDesignID == formDesignId).Get().FirstOrDefault();
                    if (el != null)
                    {
                        this._unitOfWork.RepositoryAsync<FormDesignMapping>().Delete(el);
                        this._unitOfWork.Save();
                    }
                    // Check if source present and add in the table
                    if (srcDesign > 0)
                    {
                        FormDesignMapping addMapping = new FormDesignMapping();
                        addMapping.AnchorDesignID = srcDesign;
                        addMapping.TargetDesignID = formDesignId;

                        this._unitOfWork.RepositoryAsync<FormDesignMapping>().Insert(addMapping);
                        this._unitOfWork.Save();
                    }
                    _sectionLockService.RefeshFormDesignLock();
                    //Return success result
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;

        }

        public ServiceResult DeleteFormDesign(string userName, int tenantId, int formDesignId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //check if this form design has finalized form version
                List<FormDesignGroupMapping> formDesignGroupMappingList = this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>()
                                                                                                .Query()
                                                                                                .Filter(c => c.FormID == formDesignId)
                                                                                                .Get()
                                                                                                .ToList();
                //delete all version in Form Design                  
                if (formDesignGroupMappingList.Any())
                {
                    //delete all FormDesignGroupMappings
                    foreach (var item in formDesignGroupMappingList)
                    {
                        this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Delete(item);
                    }
                }
                this._unitOfWork.RepositoryAsync<FormDesign>().Delete(formDesignId);

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                    scope.Complete();
                }

                _sectionLockService.RefeshFormDesignLock();
            }


            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        /// <summary>
        /// This method returns a collection of Data from 'FormDesignVersion' table
        /// The collection is filtered using 'tenantId' and 'FormDesignId' which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignId"></param>
        /// <returns></returns>
        public IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int formDesignId)
        {
            IList<FormDesignVersionRowModel> formDesignVersionList = null;
            try
            {
                formDesignVersionList = (from c in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                                 .Query()
                                                                                 .Filter(c => c.FormDesignID == formDesignId /*&& c.IsActive == true*/)
                                                                                 .OrderBy(c => c.OrderByDescending(d => d.AddedDate))
                                                                                 .Get()
                                         select new FormDesignVersionRowModel
                                         {
                                             FormDesignVersionId = c.FormDesignVersionID,
                                             FormDesignId = c.FormDesignID,
                                             Version = c.VersionNumber,
                                             StatusId = c.StatusID,
                                             StatusText = c.Status.Status1,
                                             EffectiveDate = c.EffectiveDate,
                                             //AddedBy = c.AddedBy,
                                             //AddedDate = c.AddedDate,
                                             //UpdatedBy = c.UpdatedBy,
                                             //UpdatedDate = c.UpdatedDate
                                         }).ToList();
                if (formDesignVersionList.Count() == 0)
                    formDesignVersionList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersionList;
        }

        public ServiceResult AddFormDesignVersion(string userName, int tenantId, int formDesignId, DateTime effectiveDate, string versionNumber, string formDesignVersionData)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesignVersion itemToAdd = new FormDesignVersion();
                int status = Convert.ToInt32(tmg.equinox.domain.entities.Status.InProgress);

                itemToAdd.FormDesignID = formDesignId;
                itemToAdd.VersionNumber = GetNextVersionNumber(versionNumber, isMajorVersion: false, isAddNewVersion: true);
                itemToAdd.FormDesignVersionData = formDesignVersionData;
                itemToAdd.StatusID = status;
                itemToAdd.EffectiveDate = effectiveDate;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.TenantID = tenantId;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormDesignVersion>().Insert(itemToAdd);
                this._unitOfWork.Save();

                FormDesignRowModel FormDesignRowViewModel = GetFormDesignList(1).Where(s => s.FormDesignId.Equals(formDesignId)).FirstOrDefault();

                if (FormDesignRowViewModel.IsMasterList.Equals(true) && !this._unitOfWork.RepositoryAsync<Folder>().IsFolderNameExists(tenantId, 0, FormDesignRowViewModel.FormDesignName))
                {
                    //Added New Design Group for New Master List Design
                    FormDesignGroupService formDesignGroupSer = new FormDesignGroupService(_unitOfWork);
                    ServiceResult formGroupAddResult = formDesignGroupSer.AddFormGroup("SuperUser", tenantId, FormDesignRowViewModel.FormDesignName, FormDesignRowViewModel.IsMasterList);
                    if (formGroupAddResult.Result == ServiceResultStatus.Success)
                    {
                        int formGroupId = 0;
                        if (formGroupAddResult.Items != null && formGroupAddResult.Items.Count() > 0)
                        {
                            string[] messages = formGroupAddResult.Items.First().Messages;
                            if (messages != null && messages.Length > 0)
                            {
                                bool hasFormGroup = int.TryParse(messages[0], out formGroupId);
                                if (hasFormGroup == true && formGroupId > 0)
                                {
                                    List<FormGroupFormRowModel> mapModels = new List<FormGroupFormRowModel>();
                                    FormGroupFormRowModel mapModel = new FormGroupFormRowModel();
                                    mapModel.FormDesignId = formDesignId;
                                    mapModel.Sequence = 1;
                                    mapModel.IsIncluded = true;
                                    if (FormDesignRowViewModel.UsesAliasDesignMasterList == true)
                                    {
                                        mapModel.AllowMultipleInstance = true;
                                    }
                                    else
                                    {
                                        mapModel.AllowMultipleInstance = false;
                                    }
                                    mapModels.Add(mapModel);
                                    formDesignGroupSer.UpdateFormGroupMapping("superUser", tenantId, formGroupId, mapModels).Wait();
                                }
                            }
                        }
                    }
                    IPlanTaskUserMappingService _planTaskUserMappingService = null;
                    ILoggingService loggingService = null;
                    IWorkFlowStateServices workFlowStateServices = null;
                    IFolderVersionServices folderVersionService = new FolderVersionServices(_unitOfWork, null, _reportingDataService, this);
                    ConsumerAccountService ConsumerAccountServiceObj = new ConsumerAccountService(_unitOfWork, loggingService, folderVersionService, workFlowStateServices, _planTaskUserMappingService);
                    ConsumerAccountServiceObj.AddFolder(tenantId, null, FormDesignRowViewModel.DisplayText, Convert.ToDateTime(itemToAdd.EffectiveDate), false, 1228, "", 1, null, null, "", "superUser");
                    int formDesignVersionId = GetFormDesignVersionList(1, formDesignId).OrderBy(s => s.FormDesignVersionId).Select(s => s.FormDesignVersionId).FirstOrDefault();
                    int folderID = folderVersionService.GetFolderIdByFolderName(FormDesignRowViewModel.DisplayText);
                    int folderVersionId = folderVersionService.GetFolderVersionList(tenantId, folderID).OrderBy(s => s.FolderVersionNumber).Select(s => s.FolderVersionId).LastOrDefault();
                    folderVersionService.CreateFormInstance(tenantId, folderVersionId, formDesignVersionId, 0, false, FormDesignRowViewModel.DisplayText, "superUser");

                }


                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateFormDesignVersion(string userName, int tenantId, int formDesignVersionId, DateTime effectiveDate, string versionNumber)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesignVersion itemToUpdate = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                               .FindById(formDesignVersionId);
                DateTime previuosEffectiveDate = DateTime.Now;

                if (itemToUpdate != null)
                {
                    previuosEffectiveDate = itemToUpdate.EffectiveDate.Value;
                    //itemToUpdate.VersionNumber = versionNumber;
                    itemToUpdate.EffectiveDate = effectiveDate;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<FormDesignVersion>().Update(itemToUpdate);

                    //Perform this only when the new  and the previous dates do not match
                    if (!effectiveDate.ToShortDateString().Equals(previuosEffectiveDate.ToShortDateString()))
                    {
                        //Update  the effective dates  of the  form design version uielement mapping also 
                        IList<FormDesignVersionUIElementMap> allmappingsToUpdate =
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                        .Query()
                                                        .Include(inc => inc.FormDesignVersion)
                                                        .Filter(c => c.FormDesignVersion.FormDesignID == itemToUpdate.FormDesignID)
                                                        .Get().ToList();
                        IList<FormDesignVersionUIElementMap> mappingsToUpdate =
                            allmappingsToUpdate.Where(c => c.FormDesignVersionID == formDesignVersionId).ToList();
                        if (mappingsToUpdate != null && mappingsToUpdate.Count() > 0)
                        {
                            foreach (FormDesignVersionUIElementMap formdesignuielementMap in mappingsToUpdate)
                            {
                                if (!formdesignuielementMap.EffectiveDate.Value.ToShortDateString().Equals(effectiveDate.ToShortDateString()))
                                {
                                    formdesignuielementMap.EffectiveDate = effectiveDate;
                                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(formdesignuielementMap);
                                }
                            }
                        }

                        //Update the Removal Effective Date of all the uielement mappings for all the prior form design version mappings(if effective date of removal matches the updating form design versions effective date -1
                        //to the current effective date -1
                        IList<FormDesignVersionUIElementMap> toUpdateRemovalEffectiveDate =
                            allmappingsToUpdate.Where(c =>
                                                c.FormDesignVersionID != formDesignVersionId &&
                                                c.FormDesignVersion.FormDesignID == itemToUpdate.FormDesignID).ToList();
                        foreach (FormDesignVersionUIElementMap mappingtoupdate in toUpdateRemovalEffectiveDate)
                        {
                            if (mappingtoupdate.EffectiveDateOfRemoval.HasValue == true)
                            {
                                if (mappingtoupdate.EffectiveDateOfRemoval.Value.ToShortDateString().Equals(previuosEffectiveDate.AddDays(-1).ToShortDateString()))
                                {
                                    mappingtoupdate.EffectiveDateOfRemoval = effectiveDate.AddDays(-1);
                                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(mappingtoupdate);
                                }
                            }
                        }
                    }
                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                        result.Result = ServiceResultStatus.Success;
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult FinalizeFormDesignVersion(string userName, int tenantId, int formDesignVersionId, string comments)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                FormDesignVersion itemToUpdate = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                               .FindById(formDesignVersionId);

                VersionTypeEvaluator evaluator = new VersionTypeEvaluator(itemToUpdate.FormDesignID.Value, formDesignVersionId, _unitOfWork);
                bool isMajorVersion = evaluator.IsMajorVersion();
                if (isMajorVersion)
                {
                    itemToUpdate.FormDesignTypeID = Convert.ToInt32(FormDesignVersionType.MAJOR);

                }
                else
                {
                    itemToUpdate.FormDesignTypeID = Convert.ToInt32(FormDesignVersionType.MINOR);
                }

                if (itemToUpdate != null)
                {
                    itemToUpdate.VersionNumber = GetNextVersionNumber(itemToUpdate.VersionNumber, isMajorVersion, isAddNewVersion: false);
                    itemToUpdate.StatusID = Convert.ToInt32(domain.entities.Status.Finalized);
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    itemToUpdate.Comments = comments;

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<FormDesignVersion>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    //TODO:Saving to DomainModel tables
                    //This will be used for creating java services where services can be used for importing and exporting data,maintaing relationships after finalizing form.
                    //So this will use later .
                    //Task.Factory.StartNew(() => this._domainModelService.Create(tenantId, formDesignVersionId));

                    // get the instance of UnityConfig.Resolve<IRptUnitOfWorkAsync>()
                    //var _generateSchemaService = new GenerateSchemaService(UnityConfig.Resolve<IRptUnitOfWorkAsync>());
                    var jsonDesigns = GetFormDesignInformation(tenantId, itemToUpdate.FormDesignID.Value, formDesignVersionId);
                    //Task.Run(() => _generateSchemaService.Run(jsonDesigns));
                    //_generateSchemaService.Run(jsonDesigns);

                    //Return success result
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;

        }
        public int GetPreviousFormDesignVersion(int tenantId, int formDesignId, int formDesignVersionId)
        {
            var previousFormDesignVersionId = 0;
            try
            {
                var formDesignVersionList = (from c in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                                 .Query()
                                                                                 .Filter(c => (c.FormDesignID == formDesignId && c.FormDesignVersionID < formDesignVersionId) /*&& c.IsActive == true*/)
                                                                                 .OrderBy(c => c.OrderByDescending(d => d.AddedDate))
                                                                                 .Get()
                                             select new FormDesignVersionRowModel
                                             {
                                                 FormDesignVersionId = c.FormDesignVersionID,
                                                 FormDesignId = c.FormDesignID,
                                                 Version = c.VersionNumber,
                                                 StatusId = c.StatusID,
                                                 StatusText = c.Status.Status1,
                                                 EffectiveDate = c.EffectiveDate,
                                                 //AddedBy = c.AddedBy,
                                                 //AddedDate = c.AddedDate,
                                                 //UpdatedBy = c.UpdatedBy,
                                                 //UpdatedDate = c.UpdatedDate
                                             }).ToList().FirstOrDefault();

                if (formDesignVersionList != null)
                {
                    previousFormDesignVersionId = formDesignVersionList.FormDesignVersionId;
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return previousFormDesignVersionId;
        }

        public ServiceResult CopyFormDesignVersion(string userName, int tenantId, int formDesignVersionId, DateTime effectiveDate, string versionNumber, string formDesignVersionData)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //Add the form design version
                FormDesignVersion itemToAdd = new FormDesignVersion();

                int status = Convert.ToInt32(tmg.equinox.domain.entities.Status.InProgress);

                int formdesignId = (from frm in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Query().
                                        Filter(frm => frm.FormDesignVersionID == formDesignVersionId).Get()
                                    select frm.FormDesignID).FirstOrDefault().Value;

                itemToAdd.FormDesignID = formdesignId;
                itemToAdd.VersionNumber = GetNextVersionNumber(versionNumber, isMajorVersion: false, isAddNewVersion: true);
                itemToAdd.FormDesignVersionData = formDesignVersionData;
                itemToAdd.EffectiveDate = effectiveDate;
                itemToAdd.AddedBy = userName;
                itemToAdd.StatusID = status;
                itemToAdd.TenantID = tenantId;
                itemToAdd.AddedDate = DateTime.Now;

                //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                //{
                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormDesignVersion>().Insert(itemToAdd);
                this._unitOfWork.Save();

                //Copy all alternative Labels from previous version to new version
                IList<AlternateUIElementLabel> altLabels = null;
                altLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>()
                                                            .Query()
                                                            .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                            .Get()
                                                            .ToList();
                if (altLabels != null && altLabels.Count() > 0)
                {
                    foreach (AlternateUIElementLabel item in altLabels)
                    {
                        AlternateUIElementLabel atlLabel = new AlternateUIElementLabel
                        {
                            FormDesignID = formdesignId,
                            FormDesignVersionID = itemToAdd.FormDesignVersionID,
                            AlternateLabel = item.AlternateLabel,
                            UIElementID = item.UIElementID
                        };

                        this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Insert(atlLabel);
                    }
                }


                //Get all the UI Elements for the form old form design version and map it against new form design version
                IList<FormDesignVersionUIElementMap> formuiElementMaplist = null;
                formuiElementMaplist = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                            .Query()
                                                            .Filter(c => c.FormDesignVersionID == formDesignVersionId /*&& c.IsActive == true*/)
                                                            .Get()
                                                            .ToList();


                if (formuiElementMaplist != null && formuiElementMaplist.Count() > 0)
                {
                    List<FormDesignVersionUIElementMap> lstFormDesignVersionUIElementMap = new List<FormDesignVersionUIElementMap>();
                    foreach (FormDesignVersionUIElementMap formuielement in formuiElementMaplist)
                    {
                        //insert the Form Design UI Element Map with effective date as effective 
                        FormDesignVersionUIElementMap uielementmaptoinsert = new FormDesignVersionUIElementMap
                        {
                            EffectiveDate = formuielement.EffectiveDate,
                            EffectiveDateOfRemoval = formuielement.EffectiveDateOfRemoval,
                            FormDesignVersionID = itemToAdd.FormDesignVersionID,
                            UIElementID = formuielement.UIElementID
                        };
                        lstFormDesignVersionUIElementMap.Add(uielementmaptoinsert);
                        //this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(uielementmaptoinsert);
                    }
                    if (lstFormDesignVersionUIElementMap.Count > 0)
                    {
                        this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().InsertRange(lstFormDesignVersionUIElementMap);
                    }
                    // Copy All DataSourceMapping to new version

                    IList<DataSourceMapping> formDesignVersionDataSourceMappingList = null;

                    formDesignVersionDataSourceMappingList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                    .Query()
                                                    .Filter(c => c.FormDesignID == formdesignId
                                                                 && c.FormDesignVersionID == formDesignVersionId)
                                                    .Get()
                                                    .ToList();
                    if (formDesignVersionDataSourceMappingList != null && formDesignVersionDataSourceMappingList.Count() > 0)
                    {
                        foreach (DataSourceMapping formDesignVersionDataSourceMapping in formDesignVersionDataSourceMappingList)
                        {
                            //insert the Form Design UI Element Map with effective date as effective 
                            DataSourceMapping newDataSourceMappingToAdd = new DataSourceMapping
                            {
                                UIElementID = formDesignVersionDataSourceMapping.UIElementID,
                                DataSourceID = formDesignVersionDataSourceMapping.DataSourceID,
                                MappedUIElementID = formDesignVersionDataSourceMapping.MappedUIElementID,
                                IsPrimary = formDesignVersionDataSourceMapping.IsPrimary,
                                DataSourceElementDisplayModeID = formDesignVersionDataSourceMapping.DataSourceElementDisplayModeID,
                                DataSourceFilter = formDesignVersionDataSourceMapping.DataSourceFilter,
                                DataCopyModeID = formDesignVersionDataSourceMapping.DataCopyModeID,
                                FormDesignVersionID = itemToAdd.FormDesignVersionID,
                                FormDesignID = formdesignId,
                                IsKey = formDesignVersionDataSourceMapping.IsKey,
                                DataSourceOperatorID = formDesignVersionDataSourceMapping.DataSourceOperatorID,
                                Value = formDesignVersionDataSourceMapping.Value,
                                DataSourceModeID = formDesignVersionDataSourceMapping.DataSourceModeID
                            };
                            this._unitOfWork.RepositoryAsync<DataSourceMapping>().Insert(newDataSourceMappingToAdd);
                        }
                    }

                    //Copy all Document Rule from previous version to new version
                    IList<DocumentRule> docRules = null;
                    docRules = this._unitOfWork.RepositoryAsync<DocumentRule>()
                                                                .Query()
                                                                .Filter(c => c.FormDesignVersionID == formDesignVersionId
                                                                                && c.FormDesignID == formdesignId && c.IsActive == true)
                                                                .Get()
                                                                .ToList();
                    if (docRules != null && docRules.Count() > 0)
                    {
                        IList<DocumentRuleEventMap> docEventMapAll = null;
                        docEventMapAll = this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Get().ToList();

                        foreach (DocumentRule dr in docRules)
                        {
                            DocumentRule docRule = new DocumentRule
                            {
                                DisplayText = dr.DisplayText,
                                Description = dr.Description,
                                AddedBy = userName,
                                AddedDate = DateTime.Now,
                                IsActive = true,
                                DocumentRuleTypeID = dr.DocumentRuleTypeID,
                                DocumentRuleTargetTypeID = dr.DocumentRuleTargetTypeID,
                                RuleJSON = dr.RuleJSON,
                                FormDesignID = dr.FormDesignID,
                                FormDesignVersionID = itemToAdd.FormDesignVersionID,
                                TargetUIElementID = dr.TargetUIElementID,
                                TargetElementPath = dr.TargetElementPath
                            };

                            this._unitOfWork.RepositoryAsync<DocumentRule>().Insert(docRule);
                            this._unitOfWork.Save();

                            IList<DocumentRuleEventMap> docEventMap = null;
                            docEventMap = docEventMapAll.Where(row => row.DocumentRuleID == dr.DocumentRuleID).ToList();
                            //.Filter(c => c.DocumentRuleID ==  dr.DocumentRuleID).Get().ToList();
                            if (docEventMap != null && docEventMap.Count() > 0)
                            {
                                foreach (DocumentRuleEventMap dem in docEventMap)
                                {
                                    DocumentRuleEventMap docEventMaps = new DocumentRuleEventMap
                                    {
                                        DocumentRuleID = docRule.DocumentRuleID,
                                        DocumentRuleEventTypeID = dem.DocumentRuleEventTypeID
                                    };
                                    this._unitOfWork.RepositoryAsync<DocumentRuleEventMap>().Insert(docEventMaps);
                                }
                            }
                        }
                    }

                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                    //scope.Complete();
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
                //}
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult DeleteFormDesignVersion(string userName, int tenantId, int formDesignVersionId, int formDesignId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId))
                {
                    if (!this._unitOfWork.RepositoryAsync<FormInstance>().IsFormDesignVersionUsedInFormInstance(formDesignId, formDesignVersionId))
                    {
                        FormDesignVersion version = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .Query()
                                                                        .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                        .Get()
                                                                        .FirstOrDefault();

                        if (version != null)
                        {
                            //Retrive all mapping related to formdesign version
                            List<int> uiElementMapList = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                              .Query()
                                                                                              .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                                              .Get()
                                                                                              .Select(c => c.UIElementID)
                                                                                              .ToList();

                            var deleteFormDesign = true;
                            List<int> dataSourceIds = null;
                            List<DataSource> dataSourceList = this._unitOfWork.RepositoryAsync<DataSource>()
                                                                       .Query()
                                                                       .Filter(c => c.FormDesignVersionID == formDesignVersionId
                                                                                    && c.FormDesignID == formDesignId)
                                                                       .Get()
                                                                       .ToList();
                            if (dataSourceList != null && dataSourceList.Count() > 0)
                            {
                                dataSourceIds = dataSourceList.Select(c => c.DataSourceID).ToList();

                                List<int> uiElementList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                      .Query()
                                                                                      .Filter(c => dataSourceIds.Any(id => c.DataSourceID == id))
                                                                                      .Get()
                                                                                      .Select(c => c.UIElementID)
                                                                                      .ToList();
                                foreach (var ui in uiElementList)
                                {
                                    if (!uiElementMapList.Contains(ui))
                                    {
                                        deleteFormDesign = false;
                                        break;
                                    }

                                }
                            }

                            if (deleteFormDesign)
                            {
                                //Delete All Datasource Mapping
                                List<DataSourceMapping> dataSourceMappingList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                    .Query()
                                                                                    .Filter(f => f.FormDesignID == formDesignId
                                                                                                && f.FormDesignVersionID == formDesignVersionId)
                                                                                    .Get()
                                                                                    .ToList();
                                if (dataSourceMappingList != null)
                                {
                                    foreach (var dataSourceMap in dataSourceMappingList)
                                    {
                                        this._unitOfWork.RepositoryAsync<DataSourceMapping>().Delete(dataSourceMap);
                                    }
                                }


                                //Delete All formInstance
                                List<FormInstance> formInstancesList = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                                              .Query()
                                                                              .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                              .Get()
                                                                              .ToList();



                                if (formInstancesList.Any())
                                {
                                    DeleteFormInstance(formInstancesList);
                                }

                                //Delete record form FormVersionObjectVersionMap
                                FormVersionObjectVersionMap formVersionObjectVersionMap = this._unitOfWork.RepositoryAsync<FormVersionObjectVersionMap>()
                                                                                                    .Query()
                                                                                                    .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                                                    .Get()
                                                                                                     .FirstOrDefault();
                                if (formVersionObjectVersionMap != null)
                                {
                                    this._unitOfWork.RepositoryAsync<FormVersionObjectVersionMap>().Delete(formVersionObjectVersionMap);
                                }

                                foreach (var uiElementId in uiElementMapList)
                                {
                                    //retrieve all mappings for UIElement other than the current form Version
                                    List<FormDesignVersionUIElementMap> uiElementMapListForOtherFormVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                                                    .Query()
                                                                                                                    .Filter(c => c.UIElementID == uiElementId && c.FormDesignVersionID != formDesignVersionId)
                                                                                                                    .Get()
                                                                                                                    .ToList();
                                    foreach (var map in uiElementMapListForOtherFormVersion)
                                    {
                                        if (version.EffectiveDate.HasValue)
                                        {
                                            //check if EffectiveDateOfRemoval is equal to form version effective date - 1
                                            if (map.EffectiveDateOfRemoval == version.EffectiveDate.Value.AddDays(-1))
                                            {
                                                //set it to null, as deleting form version nullifies all the previous updates made to released form version 
                                                //if any control is deleted/modified etc.
                                                map.EffectiveDateOfRemoval = null;
                                                this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(map);
                                            }
                                        }
                                    }



                                    //delete the element
                                    this._unitOfWork.RepositoryAsync<UIElement>().Delete(this._unitOfWork, tenantId, uiElementId, formDesignId, formDesignVersionId);
                                }

                                if (dataSourceList != null)
                                {
                                    foreach (var deleteDataSource in dataSourceList)
                                    {
                                        this._unitOfWork.RepositoryAsync<DataSource>().Delete(deleteDataSource);
                                    }
                                }

                                //delete form version
                                this._unitOfWork.RepositoryAsync<FormDesignVersion>().Delete(version);

                                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                                {
                                    this._unitOfWork.Save();
                                    result.Result = ServiceResultStatus.Success;
                                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { formDesignVersionId.ToString() } });
                                    scope.Complete();
                                }
                            }
                            else
                            {
                                result.Result = ServiceResultStatus.Failure;
                                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Form Design Version is having Data sources used in other forms. Please remove those mapping prior deleting." } });

                            }
                        }

                    }
                    else
                    {
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Can Not Delete Form Design Version as it is uesd in Folder Version.." } });
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
                else
                {
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Can Not Delete a Finalized (Released) Form Design Version.." } });
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public string GetFormDesignVersionData(int tenantId, int formDesignVersionId)
        {
            string data = string.Empty;
            try
            {
                data = JsonHelper.JsonSerializer<FormDesignVersionDetail>(this.GetFormDesignVersionDetail(tenantId, formDesignVersionId));
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return data;
        }

        public string GetFormDesignVersionDataJson(int tenantId, int formDesignVersionId)
        {
            string formDesignVersiondata = string.Empty;
            try
            {
                formDesignVersiondata = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(c => c.TenantID == tenantId && c.FormDesignVersionID == formDesignVersionId)
                                                                       .Get()
                                                                       .FirstOrDefault().FormDesignVersionData;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersiondata;
        }

        public ServiceResult CheckDataSourceMappings(string username, int tenantId, int formDesignVersionId)
        {
            var result = new ServiceResult();
            try
            {
                result = this._domainModelService.CheckDataSourceMappings(username, tenantId, formDesignVersionId);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result.Result = ServiceResultStatus.Failure;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public int GetLatestFormDesignVersion(int formDesignVersionId)
        {
            int latestFormDesignVesionId = formDesignVersionId;
            var formDesignId = this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).Select(s => s.FormDesignID).FirstOrDefault();
            if (formDesignId != null)
            {
                latestFormDesignVesionId = (from ver in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                            where ver.FormDesignID == formDesignId
                                            orderby ver.FormDesignVersionID descending
                                            select ver.FormDesignVersionID
                               ).FirstOrDefault();
            }
            return latestFormDesignVesionId;
        }

        public int GetEffectiveFormDesignVersion(string userName, int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId)
        {
            int latestFormDesignVesionId = 0;
            FormDesignVersionRowModel formDesignVersion = null;
            bool isDebugMode = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDebugMode"]);

            try
            {
                var formDesignId =
                    this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                        .Query()
                        .Filter(fil => fil.FormDesignVersionID == formDesignVersionId)
                        .Get().Select(sel => sel.FormDesignID).FirstOrDefault();

                var folderVersionEffectiveDate = this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Filter(fil => fil.FolderVersionID == folderVersionId && fil.FolderVersionStateID != (int)FolderVersionState.RELEASED && fil.FolderVersionStateID != (int)FolderVersionState.BASELINED && fil.FolderVersionStateID != (int)FolderVersionState.INPROGRESS_BLOCKED)
                                                    .Get()
                                                    .Select(sel => sel.EffectiveDate)
                                                    .FirstOrDefault();

                if (!isDebugMode)
                {
                    var isFinalizedVersionExists = IsFinalizedFormDesignVersionExists(formDesignId);

                    if (isFinalizedVersionExists)
                    {
                        formDesignVersion = (from designVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(
                                                                           fil =>
                                                                           fil.FormDesignID == formDesignId &&
                                                                           fil.StatusID ==
                                                                           (int)domain.entities.Status.Finalized)
                                                                       .Get()
                                             select new FormDesignVersionRowModel
                                             {
                                                 FormDesignVersionId = designVersion.FormDesignVersionID,
                                                 EffectiveDate = designVersion.EffectiveDate
                                             }).OrderByDescending(ord => ord.EffectiveDate)
                                                     .ThenByDescending(ord => ord.FormDesignVersionId)
                                                   .FirstOrDefault(e => e.EffectiveDate <= folderVersionEffectiveDate);
                    }
                    else
                    {
                        formDesignVersion = (from designVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(
                                                                           fil =>
                                                                           fil.FormDesignID == formDesignId)
                                                                       .Get()
                                             select new FormDesignVersionRowModel
                                             {
                                                 FormDesignVersionId = designVersion.FormDesignVersionID,
                                                 EffectiveDate = designVersion.EffectiveDate
                                             })
                                                 .FirstOrDefault(e => e.EffectiveDate < folderVersionEffectiveDate);
                    }
                }
                else
                {
                    formDesignVersion = (from designVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(
                                                                           fil =>
                                                                           fil.FormDesignID == formDesignId)
                                                                       .Get()
                                         select new FormDesignVersionRowModel
                                         {
                                             FormDesignVersionId = designVersion.FormDesignVersionID,
                                             EffectiveDate = designVersion.EffectiveDate
                                         }).OrderByDescending(ord => ord.FormDesignVersionId)
                                                   .FirstOrDefault(e => e.EffectiveDate <= folderVersionEffectiveDate);
                }
                if (formDesignVersion != null && formDesignVersionId != formDesignVersion.FormDesignVersionId)
                {

                    latestFormDesignVesionId = formDesignVersion.FormDesignVersionId;
                }
                return latestFormDesignVesionId;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return latestFormDesignVesionId;
        }

        public bool IsFinalizedFormDesignVersionExists(int? formDesignId)
        {
            var isFinalizedVersionExists = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                               .Query()
                                               .Filter(
                                                   fil =>
                                                   fil.FormDesignID == formDesignId &&
                                                   fil.StatusID == (int)domain.entities.Status.Finalized)
                                               .Get()
                                               .Any();
            return isFinalizedVersionExists;
        }

        public bool IsMajorFormDesingVersion(int formDesignVersionId, int tenantId)
        {
            bool isMajorVersion = false;
            try
            {
                int majorVersion = Convert.ToInt32(FormDesignVersionType.MAJOR);
                isMajorVersion = this._unitOfWork.Repository<FormDesignVersion>()
                                           .Query()
                                           .Filter(c => c.FormDesignTypeID == majorVersion && c.FormDesignVersionID == formDesignVersionId)
                                           .Get()
                                           .Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isMajorVersion;
        }

        public string GetVersionNumber(int formDesignVersionId, int tenantId)
        {
            string version = null;
            version = this._unitOfWork.Repository<FormDesignVersion>()
                               .Query()
                               .Filter(x => x.FormDesignVersionID == formDesignVersionId)
                               .Get().FirstOrDefault().VersionNumber;
            return version;
        }

        public IEnumerable<FormDesignRowModel> GetMasterListFormDesignList(int tenantId)
        {
            IEnumerable<FormDesignRowModel> masterFormDesignList = GetFormDesignList(tenantId).Where(s => s.IsMasterList == true);
            return masterFormDesignList;
        }

        /// <summary>
        /// This method is used to check whether Form is Master List or not.
        /// </summary>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        public bool IsMasterList(int formDesignVersionId)
        {

            bool isMasterList = false;
            isMasterList = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                        .Query().Include(c => c.FormDesign)
                        .Get()
                        .Where(c => c.FormDesignVersionID == formDesignVersionId && c.FormDesign.FormName == "MasterList").Any();

            return isMasterList;
        }

        public ServiceResult AddDesignType(string displayText)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DocumentDesignType itemToAdd = new DocumentDesignType();
                itemToAdd.DocumentDesignName = displayText;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<DocumentDesignType>().Insert(itemToAdd);
                this._unitOfWork.Save();

                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public List<DesignDocumentMapViewModel> GetMappedDesignDocumentList(int tenantId, int formDesignID, DateTime? effectiveDate)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(formDesignID > 0, "Invalid formDesignID");
            List<DesignDocumentMapViewModel> formDesignList = null;
            try
            {
                var childwithAnchor = (from ac in this._unitOfWork.RepositoryAsync<FormDesignMapping>()
                                           .Query()
                                           .Filter(x => x.AnchorDesignID == formDesignID)
                                           .Get()
                                       select ac.TargetDesignID).ToList();
                formDesignList = (from form in this._unitOfWork.RepositoryAsync<FormDesign>()
                                            .Query()
                                            .Filter(c => c.TenantID == tenantId && c.IsActive == true && childwithAnchor.Contains(c.FormID))
                                            .Get().OrderBy(c => c.DisplayText)

                                  join latestVersion in
                                      (from lv in _unitOfWork.RepositoryAsync<FormDesignVersion>().Query().Filter(c => c.EffectiveDate <= effectiveDate).Get()
                                       group lv by new { lv.FormDesign.FormName, lv.FormDesignID } into g
                                       select g.OrderByDescending(a => a.StatusID).ThenByDescending(b => b.FormDesignVersionID).FirstOrDefault()).AsQueryable()
                                       //group lv by new { lv.FormDesignID, lv.StatusID }
                                       //    into g 
                                       //    select new
                                       //    {
                                       //        groupFormDesignId = g.Key.FormDesignID,
                                       //        groupStatusId = g.Key.StatusID,
                                       //        groupFormDesignVersionID = g.Max(x => x.FormDesignVersionID)
                                       //    }).AsQueryable()

                                       on form.FormID equals latestVersion.FormDesign.FormID
                                  where form.TenantID == tenantId// && latestVersion.groupStatusId == (int)domain.entities.Status.Finalized
                                  orderby latestVersion.FormDesignVersionID descending
                                  select new DesignDocumentMapViewModel
                                  {
                                      FormDesignID = form.FormID,
                                      FormDesignVersionID = latestVersion.FormDesignVersionID,
                                      FormDesignName = form.FormName,
                                      DisplayText = form.DisplayText,
                                      TenantID = form.TenantID,
                                      IsMasterList = form.IsMasterList
                                  }).ToList();


                if (formDesignList.Count() == 0)
                    formDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }
        public bool isMasterList(string formName)
        {
            FormDesign masterListDesign = _unitOfWork.Repository<FormDesign>().Query().Filter(whr => whr.FormName == formName && whr.IsMasterList == true).Get().FirstOrDefault();
            return masterListDesign != null;
        }

        public List<JsonDesign> GetFormDesignInformation()
        {
            List<JsonDesign> jsonDesign = new List<JsonDesign>();

            var eff2018 = DateTime.Parse("2020-01-01 00:00:00.000");
            var effectiveDate = _settingManager.GetSettingValue("formDesignEffeciveDateForSchemaGeneration");
            if (!String.IsNullOrEmpty(effectiveDate))
            {
                eff2018 = DateTime.Parse(effectiveDate);
            }
            var formDesignIdsSettings = _settingManager.GetSettingValue("formDesignIdsForSchemaGeneration");
            if (!String.IsNullOrEmpty(formDesignIdsSettings))
            {
                var formDesignIds = formDesignIdsSettings.Split(',').Select(x => int.Parse(x));
                jsonDesign = (from x in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                              join y in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on x.FormDesignID equals y.FormID
                              join z in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on y.DocumentDesignTypeID equals z.DocumentDesignTypeID
                              where (formDesignIds.Contains(y.FormID)
                              && x.EffectiveDate == eff2018)
                              select new JsonDesign
                              {
                                  JsonDesignId = y.FormID,
                                  JsonDesignVersionId = x.FormDesignVersionID,
                                  JsonDesignData = x.FormDesignVersionData,
                                  TableSchemaName = y.Abbreviation.Substring(0, 3),
                                  TableLabel = y.DisplayText,
                                  TableDescription = y.DisplayText,
                                  TableDesignType = z.DocumentDesignName,
                                  VersionNumber = x.VersionNumber,
                              }).ToList();
            }else
                jsonDesign = (from x in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                          join y in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on x.FormDesignID equals y.FormID
                          join z in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on y.DocumentDesignTypeID equals z.DocumentDesignTypeID
                          where ((y.IsMasterList == true && y.FormID <= 2382) || (y.IsMasterList == false && (y.FormID == GlobalVariables.PBPDesignID || y.FormID == GlobalVariables.MedicalDesignID || y.FormID == GlobalVariables.VBIDDesignID)))
                          && x.EffectiveDate == eff2018
                          select new JsonDesign
                          {
                              JsonDesignId = y.FormID,
                              JsonDesignVersionId = x.FormDesignVersionID,
                              JsonDesignData = x.FormDesignVersionData,
                              TableSchemaName = y.Abbreviation.Substring(0, 3),
                              TableLabel = y.DisplayText,
                              TableDescription = y.DisplayText,
                              TableDesignType = z.DocumentDesignName,
                              VersionNumber = x.VersionNumber,
                          }).ToList();


            if (jsonDesign.Count <= 0)
                jsonDesign = null;

            return jsonDesign;
        }

        public List<JsonDesign> GetFormDesignInformation(int formDesignId, int formDesignVersionId)
        {
            List<JsonDesign> jsonDesign = new List<JsonDesign>();
            string[] sysName = { "sys" };
            DateTime? eff2018 = _unitOfWork.RepositoryAsync<FormDesignVersion>().Get().Where(a => a.FormDesignVersionID == formDesignVersionId).Select(a => a.EffectiveDate).FirstOrDefault();

            //var eff2018 = DateTime.Parse("2020-01-01 00:00:00.000");

            //var effectiveDate = _settingManager.GetSettingValue("formDesignEffeciveDateForSchemaGeneration");
            //if (!String.IsNullOrEmpty(effectiveDate))
            //{
            //    eff2018 = DateTime.Parse(effectiveDate);
            //}
            var formDesignIdsSettings = formDesignId.ToString();
            if (!String.IsNullOrEmpty(formDesignIdsSettings))
            {
                var formDesignIds = formDesignIdsSettings.Split(',').Select(x => int.Parse(x));
                jsonDesign = (from x in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                              join y in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on x.FormDesignID equals y.FormID
                              join z in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on y.DocumentDesignTypeID equals z.DocumentDesignTypeID
                              where (formDesignIds.Contains(y.FormID)
                              && x.EffectiveDate == eff2018)
                              select new JsonDesign
                              {
                                  JsonDesignId = y.FormID,
                                  JsonDesignVersionId = x.FormDesignVersionID,
                                  JsonDesignData = x.FormDesignVersionData,
                                  TableSchemaName = !sysName.Contains(y.Abbreviation.Substring(0, 3).ToLower()) ? y.Abbreviation.Substring(0, 3) : y.Abbreviation.Substring(0, 4),
                                  TableLabel = y.DisplayText,
                                  TableDescription = y.DisplayText,
                                  TableDesignType = z.DocumentDesignName,
                                  VersionNumber = x.VersionNumber,
                              }).ToList();
            }
            if (jsonDesign.Count <= 0)
                jsonDesign = null;
            return jsonDesign;
        }

        private List<JsonDesign> GetFormDesignInformation(int tenantId, int formDesignId, int FormDesignVersionID)
        {
            List<JsonDesign> jsonDesign = new List<JsonDesign>();

            jsonDesign = (from x in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                          join y in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on x.FormDesignID equals y.FormID
                          join z in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on y.DocumentDesignTypeID equals z.DocumentDesignTypeID
                          where y.FormID == formDesignId && x.FormDesignVersionID == FormDesignVersionID
                          select new JsonDesign
                          {
                              JsonDesignId = y.FormID,
                              JsonDesignVersionId = x.FormDesignVersionID,
                              JsonDesignData = x.FormDesignVersionData,
                              TableSchemaName = y.Abbreviation.Substring(0, 3),
                              TableLabel = y.DisplayText,
                              TableDescription = y.DisplayText,
                              TableDesignType = z.DocumentDesignName,
                              VersionNumber = x.VersionNumber,

                          }).ToList();


            foreach (var design in jsonDesign)
            {
                design.PreviousJsonDesignVersionId = GetPreviousFormDesignVersion(tenantId, formDesignId, FormDesignVersionID);
            }

            if (jsonDesign.Count <= 0)
                jsonDesign = null;

            return jsonDesign;
        }


        public List<JData> GetExistingDataForMigration()
        {
            //int[] abc = new int[] { 14484 }; 
            //&& abc.Contains(n.FormInstanceID)

            List<JData> jData = new List<JData>();
            jData = (from x in _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                     join y in _unitOfWork.RepositoryAsync<FormDesign>().Get() on x.FormDesignID equals y.FormID
                     join z in _unitOfWork.RepositoryAsync<FormInstance>().Get() on y.FormID equals z.FormDesignID
                     join n in _unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get() on z.FormInstanceID equals n.FormInstanceID
                     join f in _unitOfWork.RepositoryAsync<FolderVersion>().Get() on z.FolderVersionID equals f.FolderVersionID
                     where (y.IsMasterList == true || y.FormID == GlobalVariables.PBPDesignID || y.FormID == GlobalVariables.MedicalDesignID) && x.FormDesignVersionID == z.FormDesignVersionID
                     select new JData
                     {
                         FormInstanceId = z.FormInstanceID,
                         FormInstanceName = z.Name,
                         FormDesignId = y.FormID,
                         FormDesignVersionId = x.FormDesignVersionID,
                         FormData = n.FormData,
                         FolderID = f.FolderID,
                         FolderVersionID = z.FolderVersionID,
                         EffectiveDate = f.EffectiveDate,
                         IsMasterList = y.IsMasterList,
                         AnchorDocumentID = z.AnchorDocumentID,
                     }).ToList();


            if (jData.Count <= 0)
                jData = null;


            return jData;
        }
        public List<int> GetExistingFormInstanceIdsForMigration()
        {
            List<int> forminstanceIds = new List<int>();
            forminstanceIds = _unitOfWork.RepositoryAsync<FormInstanceIDsQueueForReporting>().Get().Where(y => y.IsActive == true).OrderBy(z => z.ID).Select(x => x.FormInstanceId).ToList();

            if (forminstanceIds.Count <= 0)
                forminstanceIds = null;

            return forminstanceIds;
        }
        public void UpdateFormInstanceIdsOfMigration(int formInstanceId)
        {
            FormInstanceIDsQueueForReporting forminstanceId = _unitOfWork.RepositoryAsync<FormInstanceIDsQueueForReporting>().Get().Where(y => y.FormInstanceId == formInstanceId).FirstOrDefault();

            if (forminstanceId != null)
            {
                forminstanceId.IsActive = false;
                _unitOfWork.RepositoryAsync<FormInstanceIDsQueueForReporting>().Update(forminstanceId);
                _unitOfWork.Save();
            }

        }
        public FormDesignVersionRowModel GetFormDesignVersionById(int formDesignVersionID)
        {
            FormDesignVersionRowModel formDesignVersion = null;
            try
            {
                formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                .Get()
                                .Where(s => s.FormDesignVersionID == formDesignVersionID)
                                .Select(f => new FormDesignVersionRowModel
                                {
                                    FormDesignId = f.FormDesignID,
                                    Version = f.VersionNumber,
                                    EffectiveDate = f.EffectiveDate
                                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return formDesignVersion;
        }
        #endregion Public Methods

        #region Private Methods

        //Delete FormInstances and its Child Table Data
        private void DeleteFormInstance(List<FormInstance> formInstancesList)
        {
            foreach (var forminstance in formInstancesList)
            {
                var forminstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                               .Query()
                                                               .Filter(c => c.FormInstanceID == forminstance.FormInstanceID)
                                                               .Get()
                                                               .FirstOrDefault();
                if (forminstanceDataMap != null)
                {
                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Delete(forminstanceDataMap);
                }
                this._unitOfWork.RepositoryAsync<FormInstance>().Delete(forminstance);
            }
        }

        private string GetNextVersionNumber(string versionNumber, bool isMajorVersion, bool isAddNewVersion)
        {
            string newVersionNumber = "";
            double result;
            Regex re = new Regex(@"^\d{1,3}\.\d{1,2}");
            if (String.IsNullOrEmpty(versionNumber) || re.Match(versionNumber).Length == 0)
            {
                newVersionNumber = "1.0";
            }
            else if (isMajorVersion == false)
            {
                if (isAddNewVersion == true)
                {
                    bool covertedDoubleNumber = double.TryParse(versionNumber, out result);
                    if (covertedDoubleNumber)
                    {
                        newVersionNumber = (result + 0.01).ToString();
                    }
                    else
                    {
                        throw new NotSupportedException("The version Number cannot be converted to double data type");
                    }
                }
                else
                {
                    newVersionNumber = versionNumber;
                }
            }
            else
            {
                bool convertedNumber = double.TryParse(versionNumber, out result);
                if (convertedNumber)
                {
                    newVersionNumber = ((Math.Floor(result) + 1)) + ".0";
                }
                else
                {
                    throw new NotSupportedException("The version Number cannot be converted to double data type");
                }
            }
            return newVersionNumber;
        }

        public List<FormDesignGroupRowMapModel> GetFormDesignsForGroup(int tenantId, int formDesignID)
        {
            List<FormDesignGroupRowMapModel> models = new List<FormDesignGroupRowMapModel>();
            var fdg = from fdgm in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get() where fdgm.FormID == formDesignID select fdgm.FormDesignGroupID;
            if (fdg != null && fdg.Count() > 0)
            {
                int formDesignGroupID = fdg.First();
                var fdgrModels = from fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                 join fdgm in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get() on fd.FormID equals fdgm.FormID
                                 join fg in this._unitOfWork.RepositoryAsync<FormDesignGroup>().Get() on fdgm.FormDesignGroupID equals fg.FormDesignGroupID
                                 join fdm in this._unitOfWork.RepositoryAsync<FormDesignMapping>().Get() on fd.FormID equals fdm.TargetDesignID into gj
                                 from subset in gj.DefaultIfEmpty()
                                 where fdgm.FormDesignGroupID == formDesignGroupID
                                 select new FormDesignGroupRowMapModel
                                 {
                                     Abbreviation = fd.Abbreviation,
                                     AddedBy = fd.AddedBy,
                                     AddedDate = fd.AddedDate,
                                     DisplayText = fd.DisplayText,
                                     DocumentDesignTypeID = fd.DocumentDesignTypeID,
                                     DocumentLocationID = fd.DocumentLocationID,
                                     FormDesignId = fd.FormID,
                                     FormDesignName = fd.FormName,
                                     IsActive = fd.IsActive,
                                     IsIncluded = true,
                                     IsMasterList = fd.IsMasterList,
                                     ParentFormDesignID = subset == null ? 0 : subset.AnchorDesignID,
                                     TenantID = fd.TenantID,
                                     UpdatedBy = fd.UpdatedBy,
                                     UpdatedDate = fd.UpdatedDate,
                                     IsAliasDesignMasterList = fd.IsAliasDesignMasterList,
                                     UsesAliasDesignMasterList = fd.UsesAliasDesignMasterList
                                 };
                if (fdgrModels != null && fdgrModels.Count() > 0)
                {
                    models = fdgrModels.ToList();
                }
            }
            return models;
        }

        #endregion Private Methods


    }
}
