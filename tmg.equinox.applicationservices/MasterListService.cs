using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Diagnostics.Contracts;
using tmg.equinox.repository.extensions;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.applicationservices.viewmodels;
using Newtonsoft.Json.Linq;

namespace tmg.equinox.applicationservices
{
    public class MasterListService : IMasterListService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public MasterListService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
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
        public DateTime GetEffectiveDate(int folderVersionId)
        {
            DateTime effectiveDate = (from fv in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                .Query()
                                                .Filter(c => c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                .Get()
                                      select fv.EffectiveDate).FirstOrDefault();
            return effectiveDate;
        }

        public List<int> GetFormInstanceIds(DateTime EffectiveDate, string ruleAlias,string documentFilter)
        {
            List<int> lstIds = new List<int>();
            var formInstanceId = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                             .Query()
                                             .Include(c => c.FormDesign)
                                             .Include(c => c.FolderVersion)
                                             .Filter(h => h.FormDesign.IsMasterList == true && h.FormDesign.IsActive == true && h.IsActive == true && h.FolderVersion.EffectiveDate <= EffectiveDate)
                                             .Get()
                                  where ruleAlias == frm.FormDesign.FormName
                                  && (frm.Name == documentFilter || String.IsNullOrEmpty(documentFilter))
                                  orderby frm.FolderVersionID descending
                                  select frm.FormInstanceID
                      ).FirstOrDefault();
            if (formInstanceId != null && formInstanceId != 0)
            {
                lstIds.Add(formInstanceId);
            }
            //List<int> lstIds = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
            //                                 .Query()
            //                                 .Include(c => c.FormDesign)
            //                                 .Include(c => c.FolderVersion)
            //                                 .Filter(h => h.FormDesign.IsMasterList == true && h.FormDesign.IsActive == true && h.IsActive == true && h.FolderVersion.EffectiveDate <= EffectiveDate)
            //                                 .Get()
            //                    where ruleAlias == frm.FormDesign.FormName
            //                    group frm by frm.FormDesign.FormName into g
            //                    select g.OrderByDescending(s => s.FolderVersion.EffectiveDate)
            //                    .ThenByDescending(f => f.FolderVersionID)
            //                            .FirstOrDefault(p => g.Any() || p.FolderVersion.FolderVersionStateID == (int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED).FormInstanceID
            //                            ).ToList();
            //select g.OrderByDescending(s => s.FolderVersion.EffectiveDate).ThenByDescending(f => f.FolderVersionID).FirstOrDefault().FormInstanceID).ToList();
            return lstIds;
        }

        public List<int> GetFormInstanceIds(DateTime EffectiveDate)
        {
            var lstformInstance = (from frm in this._unitOfWork.RepositoryAsync<FormInstance>()
                                             .Query()
                                             .Include(c => c.FormDesign)
                                             .Include(c => c.FolderVersion)
                                             .Filter(h => h.FormDesign.IsMasterList == true && h.FormDesign.IsActive == true && h.IsActive == true && h.FolderVersion.EffectiveDate <= EffectiveDate)
                                             .Get()
                                   select frm).ToList();

            List<int> lstIds = (from frm1 in lstformInstance
                                group frm1 by frm1.FormDesign.FormName into g
                                select g.OrderByDescending(s => s.FolderVersion.EffectiveDate)
                                .ThenByDescending(f => f.FolderVersionID)
                                        .FirstOrDefault(p => g.Any() || p.FolderVersion.FolderVersionStateID == (int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED).FormInstanceID
                                        ).ToList();

            return lstIds;
        }

        public string GetSectionNameFromFormInstanceID(int tenantId, int formInstanceID)
        {
            string formName = "";
            try
            {
                formName = (from ins in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                            join des in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on ins.FormDesignID equals des.FormID
                            where ins.FormInstanceID == formInstanceID
                            select des.FormName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formName;
        }

        public IEnumerable<KeyValue> GetUIElementTypes(int tenantId)
        {
            IList<KeyValue> uiElementTypesList = null;
            try
            {
                uiElementTypesList = (from c in this._unitOfWork.RepositoryAsync<UIElementType>()
                                                                .Query()
                                                                .Filter(c => c.IsActive == true)
                                                                .Get()
                                      select new KeyValue
                                      {
                                          Key = c.UIElementTypeID,
                                          Value = c.DisplayText
                                      }).ToList();
                if (uiElementTypesList.Count() == 0)
                    uiElementTypesList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementTypesList;
        }

        public IEnumerable<KeyValue> GetApplicationDataTypes(int tenantId)
        {
            IList<KeyValue> applicationDataTypesList = null;
            try
            {
                applicationDataTypesList = (from c in this._unitOfWork.RepositoryAsync<ApplicationDataType>()
                                                                .Query()
                                                                .Get()
                                            select new KeyValue
                                            {
                                                Key = c.ApplicationDataTypeID,
                                                Value = c.DisplayText
                                            }).ToList();
                if (applicationDataTypesList.Count() == 0)
                    applicationDataTypesList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return applicationDataTypesList;
        }

        public IEnumerable<KeyValue> GetLayoutTypes(int tenantId)
        {
            IList<KeyValue> layoutTypesList = null;
            try
            {
                layoutTypesList = (from c in this._unitOfWork.RepositoryAsync<LayoutType>()
                                                                .Query()
                                                                .Get()
                                   select new KeyValue
                                   {
                                       Key = c.LayoutTypeID,
                                       Value = c.DisplayText
                                   }).ToList();
                if (layoutTypesList.Count() == 0)
                    layoutTypesList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return layoutTypesList;
        }

        public IEnumerable<KeyValue> GetLogicalOperatorTypes(int tenantId)
        {
            IList<KeyValue> logicalOperatorTypeList = null;
            try
            {
                logicalOperatorTypeList = (from c in this._unitOfWork.RepositoryAsync<LogicalOperatorType>()
                                                                .Query()
                                                                .Get()
                                           select new KeyValue
                                           {
                                               Key = c.LogicalOperatorTypeID,
                                               Value = c.DisplayText
                                           }).ToList();
                if (logicalOperatorTypeList.Count() == 0)
                    logicalOperatorTypeList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return logicalOperatorTypeList;
        }

        public IEnumerable<KeyValue> GetOperatorTypes(int tenantId)
        {
            IList<KeyValue> operatorTypeList = null;
            try
            {
                operatorTypeList = (from c in this._unitOfWork.RepositoryAsync<OperatorType>()
                                                                .Query()
                                                                .Get()
                                    select new KeyValue
                                    {
                                        Key = c.OperatorTypeID,
                                        Value = c.DisplayText
                                    }).ToList();
                if (operatorTypeList.Count() == 0)
                    operatorTypeList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return operatorTypeList;
        }

        public IEnumerable<KeyValue> GetStatusTypes(int tenantId)
        {
            IList<KeyValue> statusList = null;
            try
            {
                statusList = (from c in this._unitOfWork.RepositoryAsync<domain.entities.Models.Status>()
                                                                .Query()
                                                                .Get()
                              select new KeyValue
                              {
                                  Key = c.StatusID,
                                  Value = c.Status1
                              }).ToList();
                if (statusList.Count() == 0)
                    statusList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return statusList;
        }

        public IEnumerable<KeyValue> GetTargetPropertyTypes(int tenantId)
        {
            IList<KeyValue> targetPropertyList = null;
            try
            {
                targetPropertyList = (from c in this._unitOfWork.RepositoryAsync<TargetProperty>()
                                                                .Query()
                                                                .Get()
                                      select new KeyValue
                                      {
                                          Key = c.TargetPropertyID,
                                          Value = c.TargetPropertyName
                                      }).ToList();
                if (targetPropertyList.Count() == 0)
                    targetPropertyList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return targetPropertyList;
        }

        public IEnumerable<KeyValue> GetLibraryRegexes(int tenantId)
        {
            IList<KeyValue> libraryRegexList = null;
            try
            {
                libraryRegexList = (from c in this._unitOfWork.RepositoryAsync<RegexLibrary>()
                                                                .Query()
                                                                .Get()
                                    select new KeyValue
                                    {
                                        Key = c.LibraryRegexID,
                                        Value = c.LibraryRegexName
                                    }).ToList();
                if (libraryRegexList.Count() == 0)
                    libraryRegexList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return libraryRegexList;

        }

        public IEnumerable<KeyValue> GetApprovalStatusTypeList(int tenantId)
        {
            IList<KeyValue> approvalStatusTypeList = null;
            try
            {
                approvalStatusTypeList = (from c in this._unitOfWork.RepositoryAsync<WorkFlowStateApprovalTypeMaster>()
                                                                 .Query()
                                                                 .Get()
                                          select new KeyValue
                                          {
                                              Key = c.WorkFlowStateApprovalTypeID,
                                              Value = c.WorkFlowStateApprovalTypeName
                                          }).ToList();

                if (approvalStatusTypeList.Count() == 0)
                    approvalStatusTypeList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return approvalStatusTypeList;
        }

        public IEnumerable<KeyValue> GetOwnerList(int tenantId)
        {
            IList<KeyValue> ownerList = new List<KeyValue>();
            try
            {
                ownerList = (from c in this._unitOfWork.RepositoryAsync<User>()
                                                                .Query()
                                                                .Get().Where(c => c.IsActive == true)
                             select new KeyValue
                             {
                                 Key = c.UserID,
                                 Value = c.UserName
                             }).ToList();

                if (ownerList.Count() == 0)
                    ownerList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return ownerList;
        }

        public ServiceResult SaveMasterListImportData(string FileName, string FilePath, int FormInstanceID, string Comment,  string AddedBy,  DateTime AddedDate, string Status)
        {
            ServiceResult result = new ServiceResult();
            List<ServiceResultItem> items = new List<ServiceResultItem>();
            try
            {
                MasterListImport masterListImport = new MasterListImport();

                masterListImport.FileName = FileName;
                masterListImport.FilePath = FilePath;
                masterListImport.FormInstanceID = FormInstanceID;
                masterListImport.Comment = Comment;
                masterListImport.AddedBy = AddedBy;
                masterListImport.AddedDate = AddedDate;
                masterListImport.Status = Status;


                this._unitOfWork.RepositoryAsync<MasterListImport>().Insert(masterListImport);
                this._unitOfWork.Save();

                
                result.Result = ServiceResultStatus.Success;


            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                throw ex;
            }
            return result;
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

        public MasterListVersions GetMasterListVersions(int formDesignVersionID,int folderVersionID)
        {
            MasterListVersions mlVersions = new MasterListVersions();
            List<int> folderVersionStates = new List<int>();
            folderVersionStates.AddRange(new int[] { 3 });
            var versionList = from fi in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                              join fdv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                              on fi.FolderVersionID equals fdv.FolderVersionID
                              where fi.FormDesignVersionID == formDesignVersionID && folderVersionStates.Contains(fdv.FolderVersionStateID)
                              orderby fdv.EffectiveDate, fdv.FolderVersionNumber, fi.FormInstanceID, fi.FolderVersionID descending
                              select new {
                                  FormInstanceID = fi.FormInstanceID,
                                  FolderVersionID = fi.FolderVersionID,
                                  EffectiveDate = fdv.EffectiveDate,
                                  VersionNumber = fdv.FolderVersionNumber,
                                  StateID = fdv.FolderVersionStateID
                              };
            if (versionList != null && versionList.Count() > 0)
            {
                var latestVersion = from c in versionList where c.FolderVersionID == folderVersionID select c;
                if (latestVersion != null && latestVersion.Count() > 0)
                {
                    var latestVer = latestVersion.First();
                    mlVersions.CurrentFolderVersionID = latestVer.FolderVersionID;
                    mlVersions.CurrentFormInstanceID = latestVer.FormInstanceID;
                    mlVersions.CurrentEffectiveDate = latestVer.EffectiveDate;
                    var prevVersion = from c in versionList where
                                      c.EffectiveDate <= latestVer.EffectiveDate
                                      && c.FolderVersionID !=  latestVer.FolderVersionID orderby c.EffectiveDate descending
                                      select c;
                    if(prevVersion != null && prevVersion.Count() > 0)
                    {
                        var prVer = prevVersion.First();
                        mlVersions.PreviousFolderVersionID = prVer.FolderVersionID;
                        mlVersions.PreviousFormInstanceID = prVer.FormInstanceID;
                    }
                }
            }
            return mlVersions;
        }

        public MasterListFormDesignViewModel GetFolderVersionFormDesign(int folderVersionID)
        {
            MasterListFormDesignViewModel designViewModel = new MasterListFormDesignViewModel();
            var formInstance = (from fi in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fdv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                on fi.FolderVersionID equals fdv.FolderVersionID
                                where fi.FolderVersionID == folderVersionID select fi).FirstOrDefault();
            if(formInstance != null)
            {
                designViewModel.FormDesignID = formInstance.FormDesignID;
                designViewModel.FormDesignVersionID = formInstance.FormDesignVersionID;
                designViewModel.FormInstanceID = formInstance.FormInstanceID;
            }
            return designViewModel;
        }

        public int GetAliasMasterListForEffectiveDate(int tenantId, DateTime effectiveDate)
        {
            int formDesignVersionId = 0;
            var res = from des in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                      join desVer in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on des.FormID equals desVer.FormDesignID
                      where des.IsAliasDesignMasterList == true && desVer.EffectiveDate <= effectiveDate select desVer;
            if(res != null && res.Count() > 0)
            {
                var orderedRes = res.OrderByDescending(a => a.EffectiveDate);
                foreach(var ordRes in orderedRes)
                {
                    if(ordRes.StatusID == 1 || ordRes.StatusID == 3)
                    {
                        formDesignVersionId = ordRes.FormDesignVersionID;
                    }
                    if (ordRes.StatusID == 3)
                    {
                        break;
                    }
                }
            }
            return formDesignVersionId;
        }


        public JObject GetAliasMasterListDataForDesignVersion(int tenantId, int formDesignVersionId)
        {
            JObject resObj = new JObject();
            int formInstanceId = 0;
            var res = from ins in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                      join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on ins.FolderVersionID equals fv.FolderVersionID
                      where ins.FormDesignVersionID == formDesignVersionId && (fv.FolderVersionStateID == 1 || fv.FolderVersionStateID == 3)
                      select new
                      {
                          FormInstanceID = ins.FormInstanceID,
                          EffectiveDate = fv.EffectiveDate,
                          StateID = fv.FolderVersionStateID,
                          FolderVersionID=fv.FolderVersionID
                      };
            if (res != null && res.Count() > 0)
            {
                var orderedRes = res.OrderByDescending(a => a.EffectiveDate).ThenByDescending(d=>d.FolderVersionID);
                foreach (var ordRes in orderedRes)
                {
                    if (ordRes.StateID == 1 || ordRes.StateID == 3)
                    {
                        formInstanceId = ordRes.FormInstanceID;
                    }
                    if (ordRes.StateID == 3)
                    {
                        break;
                    }
                }
            }
            FormInstanceDataMap formInstance = null;
            formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().GetFormInstanceDataDecompressed(formInstanceId);
            string data = "";
            if (formInstance != null)
            {
                data = formInstance.FormData;
                resObj = JObject.Parse(data);
            }
            return resObj;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
