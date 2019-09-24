using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class FormInstanceService : IFormInstanceService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        private static readonly object LockObj = new object();
        public FormInstanceService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public int GetAnchorDocumentID(int formInstanceID)
        {
            int result = 0;
            try
            {
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Get()
                                 .Where(s => s.FormInstanceID == formInstanceID)
                                 .FirstOrDefault();
                if (formInstance != null)
                {
                    result = formInstance.AnchorDocumentID ?? 0;
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

        public int GetDocID(int formInstanceID)
        {
            int result = 0;
            try
            {
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Get()
                                 .Where(s => s.FormInstanceID == formInstanceID)
                                 .FirstOrDefault();
                if (formInstance != null)
                {
                    result = formInstance.DocID;
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

        public int GetViewDocumentID(int anchorDocumentID, int formDesignID)
        {
            int result = 0;
            try
            {
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Get()
                                 .Where(s => s.AnchorDocumentID == anchorDocumentID && s.FormDesignID == formDesignID && s.IsActive == true)
                                 .FirstOrDefault();
                if (formInstance != null)
                {
                    result = formInstance.FormInstanceID;
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

        public List<int> GetViewDocumentIDs(int anchorDocumentID, int formDesignID)
        {
            List<int> formInstanceIds = new List<int>();
            try
            {

                var formInstances = this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Get()
                                 .Where(s => s.AnchorDocumentID == anchorDocumentID && s.FormDesignID == formDesignID && s.IsActive == true)
                                 .Select(t => t.FormInstanceID);
                if (formInstances != null && formInstances.Count() > 0)
                {
                    formInstanceIds = formInstances.ToList();
                }
                else
                {
                    formInstanceIds.Add(0);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstanceIds;
        }

        public int GetDocumentID(int folderVersionID, int formDesignID)
        {
            int result = 0;
            try
            {
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>()
                                 .Get()
                                 .Where(s => s.FolderVersionID == folderVersionID && s.FormDesignID == formDesignID)
                                 .FirstOrDefault();
                if (formInstance != null)
                {
                    result = formInstance.FormInstanceID;
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

        public SourceDesignDetails GetViewByAnchor(int folderVersionID, int veiwDesignID, int anchorDocumentID)
        {
            var viewDetails = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fd.FormID
                               join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on fd.FormID equals fdv.FormDesignID
                               where fi.FolderVersionID == folderVersionID && fi.AnchorDocumentID == anchorDocumentID && fi.FormDesignID == veiwDesignID
                               select new SourceDesignDetails
                               {
                                   FormName = fd.FormName,
                                   FormDesignVersionId = fi.FormDesignVersionID,
                                   FormInstanceId = fi.FormInstanceID,
                                   RuleEventTree = fdv.RuleEventMapJSON
                               }).FirstOrDefault();

            return viewDetails;
        }

        public string GetProxyNumber(int formInstanceId)
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


        public List<OONGroupEntryModel> GetOONGroupEntries(int formDesignVersionId)
        {
            List<OONGroupEntryModel> groupEntryModels = new List<OONGroupEntryModel>();
            var models = from entry in this._unitOfWork.RepositoryAsync<OONGroupEntry>().Get()
                         where entry.IsActive == true &&
                         entry.FormDesignVersionId == formDesignVersionId
                         select entry;
            if (models != null && models.Count() > 0)
            {
                groupEntryModels = (from model in models
                                    select new OONGroupEntryModel
                                    {
                                        BenefitCode = model.BenefitCode,
                                        BenefitName = model.BenefitName,
                                        BenefitGroup = model.BenefitGroup,
                                        BenefitType = model.BenefitType,
                                        FieldSubType = model.FieldSubType,
                                        FieldType = model.FieldType,
                                        IsActive = model.IsActive,
                                        Package = model.Package,
                                        SOTFieldPath = model.SOTFieldPath
                                    }).ToList();
            }
            return groupEntryModels;
        }

        public void UpdateFormInstanceComplianceValidationlog(int formInstanceId, List<FormInstanceComplianceValidationlog> validationErrors, int collateralProcessQueue1Up)
        {
            lock (LockObj)
            {
                if (collateralProcessQueue1Up == 0)
                {
                    // _unitOfWork.Repository<FormInstanceComplianceValidationlog>().ExecuteSql("delete from Fldr.FormInstanceComplianceValidationlog where FormInstanceID ={0}", formInstanceId);
                    IEnumerable<FormInstanceComplianceValidationlog> result = (_unitOfWork.RepositoryAsync<FormInstanceComplianceValidationlog>()
                                                                             .Get()
                                                                             .Where(m => m.FormInstanceID == formInstanceId && (m.CollateralProcessQueue1Up == null && m.CollateralProcessQueue1Up == collateralProcessQueue1Up)).ToList());


                    _unitOfWork.Repository<FormInstanceComplianceValidationlog>().DeleteRange(result);
                }
                _unitOfWork.Repository<FormInstanceComplianceValidationlog>().InsertRange(validationErrors);
                _unitOfWork.Save();
            }
        }
        public IEnumerable<ComplianceValidationlogModel> GetComplianceValidationlog(int formInstanceId, string userName, int collateralQueueId)
        {
            Expression<Func<FormInstanceComplianceValidationlog, bool>> filter;
            if (userName == "superuser")
            {
                if (collateralQueueId == 0)
                {
                    filter = m => m.FormInstanceID == formInstanceId && (m.CollateralProcessQueue1Up == 0 || m.CollateralProcessQueue1Up == null);
                }
                else
                {
                    filter = m => m.CollateralProcessQueue1Up == collateralQueueId;
                }
            }
            else
            {
                if (collateralQueueId == 0)
                {
                    filter = (m => m.FormInstanceID == formInstanceId && (m.CollateralProcessQueue1Up == 0 || m.CollateralProcessQueue1Up == null) && (m.ValidationType == "NotResolved" || m.ValidationType == "Resolved" || m.ValidationType == "Error"));
                }
                else
                {
                    filter = (m => m.CollateralProcessQueue1Up == collateralQueueId && (m.ValidationType == "NotResolved" || m.ValidationType == "Resolved" || m.ValidationType == "Error"));
                }
            }

            var result = (from c in _unitOfWork.Repository<FormInstanceComplianceValidationlog>()
                                                                     .Query()
                                                                     .Filter(filter)
                                                                     .Get()
                          select new ComplianceValidationlogModel
                          {
                              ComplianceType = c.ComplianceType,
                              No = c.No,
                              Error = c.Error,
                              ValidationType = c.ValidationType
                          }).ToList();

            if (result.Count == 0)
            {
                List<ComplianceValidationlogModel> list = new List<ComplianceValidationlogModel>();
                list.Add(new ComplianceValidationlogModel
                {
                    ComplianceType = "",
                    No = 0,
                    Error = "",
                    ValidationType = ""
                });
                return list;
            }
            return result;
        }

        public FormInstanceExportPDF GetFormInstanceDetails(int formInstanceId)
        {
            FormInstanceExportPDF model = new FormInstanceExportPDF();

            model = (from frmInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                             .Get().Where(c => c.FormInstanceID == formInstanceId)
                     join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                                  on frmInstance.FolderVersionID equals fldrVersion.FolderVersionID
                     join folderV in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                                   on fldrVersion.FolderID equals folderV.FolderID
                     select new FormInstanceExportPDF
                     {
                         FormInstanceID = frmInstance.FormInstanceID,
                         FolderVersionID = fldrVersion.FolderVersionID,
                         FolderId = folderV.FolderID,
                         TenantID = frmInstance.TenantID,
                         FormDesignVersionID = frmInstance.FormDesignVersionID,
                         FormName = frmInstance.Name,

                         FolderName = folderV.Name,
                         FolderVersionNumber = fldrVersion.FolderVersionNumber,
                         EffectiveDate = fldrVersion.EffectiveDate

                     }).FirstOrDefault();


            return model;
        }

        public List<SourceDesignDetails> GetQHPViewByAnchor(List<int> formInstanceIds, int formDesignID, bool offExchangeOnly)
        {
            List<SourceDesignDetails> viewDetails = null;

            if (offExchangeOnly)
            {
                viewDetails = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               join ins in formInstanceIds on fi.AnchorDocumentID equals ins
                               join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fd.FormID
                               join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on fd.FormID equals fdv.FormDesignID
                               join map in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get() on fi.AnchorDocumentID equals map.FormInstanceID
                               where fi.FormDesignID == formDesignID && (map.PlanCode == "Both (Display as On/Off Exchange)" || map.PlanCode == "Off Exchange")
                               select new SourceDesignDetails
                               {
                                   FormName = fd.FormName,
                                   FormDesignVersionId = fi.FormDesignVersionID,
                                   FormInstanceId = fi.FormInstanceID,
                                   RuleEventTree = fdv.RuleEventMapJSON,
                                   FolderVersionId = fi.FolderVersionID
                               }).ToList();
            }
            else
            {
                viewDetails = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                               join ins in formInstanceIds on fi.AnchorDocumentID equals ins
                               join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fd.FormID
                               join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on fd.FormID equals fdv.FormDesignID
                               where fi.FormDesignID == formDesignID
                               select new SourceDesignDetails
                               {
                                   FormName = fd.FormName,
                                   FormDesignVersionId = fi.FormDesignVersionID,
                                   FormInstanceId = fi.FormInstanceID,
                                   RuleEventTree = fdv.RuleEventMapJSON,
                                   FolderVersionId = fi.FolderVersionID
                               }).ToList();
            }
            return viewDetails;
        }

        //method to get all forminstances based on document name and effective year, returns forminstances for max folder version (released or in-progress)
        public FormInstanceViewModel GetFormInstancesByName(string documentName, string effYear, int formDesignID)
        {
            List<FormInstanceViewModel> formInstances = null;
            FormInstanceViewModel formInstance = null;
            formInstances = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                             join fidm in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get() on fi.FormInstanceID equals fidm.FormInstanceID
                             join fd in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fd.FolderVersionID
                             join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on fi.FormDesignVersionID equals fdv.FormDesignVersionID
                             join fdi in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals fdi.FormID
                             where fi.Name == documentName && fi.IsActive == true && fd.FolderVersionNumber.StartsWith(effYear) && fi.FormDesignID == formDesignID
                             select new FormInstanceViewModel
                             {
                                 FormData = fidm.FormData,
                                 Name = fi.Name,
                                 FolderID = fi.FolderVersion.FolderID,
                                 FolderVersionID = fi.FolderVersionID,
                                 DocID = fi.DocID,
                                 FolderVersionNumber = fd.FolderVersionNumber,
                                 FormDesignName = fdi.FormName,
                                 FolderVersionStateID = fd.FolderVersionStateID
                             }).ToList();
            if (formInstances != null && formInstances.Count() > 0)
            {
                //get the release version, if not found then get the in-progress version
                formInstances = formInstances.Where(c => c.FolderVersionStateID == (int)(int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED).Count() > 0 ? formInstances.Where(c => c.FolderVersionStateID == (int)(int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED).ToList() : formInstances.Where(c => c.FolderVersionStateID == (int)(int)tmg.equinox.domain.entities.Enums.FolderVersionState.INPROGRESS).ToList();
                if (formInstances.Count() > 0)
                {
                    //find the max folder version
                    string maxFolderVersionNUmber = formInstances.Max(c => c.FolderVersionNumber);
                    formInstance = formInstances.Where(c => c.FolderVersionNumber == maxFolderVersionNUmber).FirstOrDefault();
                }
            }

            return formInstance;
        }
        //method to get all records from jsonmapping table
        public List<JsonFieldMappingViewModelExtended> GetJsonFieldsData()
        {
            List<JsonFieldMappingViewModelExtended> recordList = null;
            recordList = (from fi in this._unitOfWork.RepositoryAsync<JsonResultMapping>().Get()
                          where fi.IsActive == true
                          select new JsonFieldMappingViewModelExtended()
                          {
                              JSONPath = fi.JSONPath,
                              Label = fi.Label,
                              FieldName = fi.FieldName,
                              DesignType = fi.DesignType,
                          }).ToList();
            return recordList;
        }

        public void UpdateFormInstanceComplianceValidationlogForPrintX(int formInstanceId, FormInstanceComplianceValidationlog logs, int collateralProcessQueue1Up)
        {
            _unitOfWork.Repository<FormInstanceComplianceValidationlog>().Insert(logs);
            _unitOfWork.Save();

        }

    }
}
