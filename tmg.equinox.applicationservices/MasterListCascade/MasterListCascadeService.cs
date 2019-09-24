using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using System.Data;
using System.Data.SqlClient;

namespace tmg.equinox.applicationservices.masterlistcascade
{
    public class MasterListCascadeService : IMasterListCascadeService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IMasterListService _mlService;
        private IFormInstanceDataServices _fidService;
        private IFormDesignService _formDesignService;
        public MasterListCascadeService(IUnitOfWorkAsync unitOfWork, IMasterListService mlService, IFormInstanceDataServices fidService, IFormDesignService formDesignService)
        {
            _unitOfWork = unitOfWork;
            _mlService = mlService;
            _fidService = fidService;
            _formDesignService = formDesignService;
        }
        public List<MasterListCascadeViewModel> GetMasterListCascade(int formDesignID, int formDesignVersionID)
        {
            List<MasterListCascadeViewModel> mlCascade = new List<MasterListCascadeViewModel>();
            var mlCas = from c in this._unitOfWork.RepositoryAsync<MasterListCascade>()
                                                                       .Query()
                                                                       .Filter(c => c.MasterListDesignID == formDesignID && c.MasterListDesignVersionID == formDesignVersionID && c.IsActive == true)
                                                                       .Get()
                        select new MasterListCascadeViewModel
                        {
                            FilterExpressionRule = c.FilterExpressionRule,
                            IsActive = c.IsActive,
                            MasterListCascadeID = c.MasterListCascadeID,
                            MasterListDesignID = c.MasterListDesignID,
                            MasterListDesignVersionID = c.MasterListDesignVersionID,
                            MasterListJSONPath = c.MasterListJSONPath,
                            TargetDesignID = c.TargetDesignID,
                            TargetDesignVersionID = c.TargetDesignVersionID,
                            TargetDocumentType = c.TargetDocumentType,
                            UpdateExpressionRule = c.UpdateExpressionRule,
                            CompareMacroJSON = c.CompareMacroJSON,
                            MasterListCompareJSON = c.MasterListCompareJSON
                        };
            if (mlCas != null && mlCas.Count() > 0)
            {
                mlCascade = mlCas.ToList();
            }
            return mlCascade;
        }

        public int AddMasterListCascadeBatch(MasterListCascadeViewModel mlCascade, string userName)
        {
            MasterListCascadeBatch batch = new MasterListCascadeBatch();
            batch.FormDesignVersionID = mlCascade.MasterListDesignVersionID;
            batch.QueuedDate = DateTime.Now;
            batch.StartDate = DateTime.Now;
            batch.QueuedBy = userName;

            //default to Queued when added
            batch.Status = 1;
            _unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Insert(batch);
            _unitOfWork.Save();
            return batch.MasterListCascadeBatchID;
        }

        public ServiceResult UpdateMasterListCascadeBatch(int masterListCascadeBatchID, int status, string message)
        {
            ServiceResult result = new ServiceResult();
            var masterlistBatch = _unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Get(a => a.MasterListCascadeBatchID == masterListCascadeBatchID);
            if (masterlistBatch != null && masterlistBatch.Count() > 0)
            {
                MasterListCascadeBatch batch = masterlistBatch.First();
                batch.Status = status;
                batch.Message = message;
                if (status > 2)
                {
                    batch.EndDate = DateTime.Now;
                }
                try
                {
                    _unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Update(batch, true);
                    _unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
                catch (Exception ex)
                {
                    result.Result = ServiceResultStatus.Failure;
                    ServiceResultItem item = new ServiceResultItem();
                    string resultMessage = " Update Status Failed : Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace;
                    item.Messages = new string[] { resultMessage };
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(item);
                    result.Items = items;
                }
            }
            return result;
        }

        public JObject GetMasterListSectionData(MasterListCascadeViewModel mlCascade, int folderversionID, int formInstanceID)
        {
            JObject sectionData = new JObject();
            string sectionName = mlCascade.MasterListJSONPath.Split('.')[0];
            if (!String.IsNullOrEmpty(sectionName))
            {
                string formDesignVersion = _formDesignService.GetCompiledFormDesignVersion(1, mlCascade.MasterListDesignVersionID);
                FormDesignVersionDetail detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesignVersion);
                string sectionStr = "";
                if (folderversionID > 0 && formInstanceID > 0)
                {
                    sectionStr = _fidService.GetSectionData(1, formInstanceID, sectionName, detail, "admin");
                }
                sectionData = JObject.Parse(sectionStr);
            }
            return sectionData;
        }

        public List<DocumentFilterResult> FilterDocuments(DateTime effectiveDate, string filterPath, int formDesignID, int formDesignVersionID, List<MLPlanCode> planCodes, int folderVersionID, int masterListCascadeID)
        {
            SqlParameter planCodeList = new SqlParameter("@PlanCodeList", SqlDbType.Structured)
            {
                TypeName = "[ML].[PlanCode]"
            };
            if (planCodes != null)
            {
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(planCodes));
                planCodeList.Value = dt;
            }
            SqlParameter cascadeYearParam = new SqlParameter("@CascadeYear", effectiveDate.Year.ToString());
            SqlParameter formDesignIDParam = new SqlParameter("@FormDesignID", formDesignID);
            SqlParameter formDesignVersionIDParam = new SqlParameter("@FormDesignVersionID", formDesignVersionID);
            SqlParameter filterPathParam = new SqlParameter("@FilterPath", filterPath);
            SqlParameter folderVersionIDParam = new SqlParameter("@FolderVersionID", folderVersionID);
            SqlParameter effectiveDateParam = new SqlParameter("@EffectiveDate", effectiveDate);

            var results = this._unitOfWork.Repository<DocumentFilter>().ExecuteSql("exec [ML].[FilterCascadeDocuments] @PlanCodeList,@FilterPath,@CascadeYear,@EffectiveDate,@FormDesignID,@FormDesignVersionID,@FolderVersionID", planCodeList, filterPathParam, cascadeYearParam, effectiveDateParam, formDesignIDParam, formDesignVersionIDParam, folderVersionIDParam);
            List<DocumentFilter> filterResults = results.ToList();
            var finalResults = from docFilter in filterResults
                               select new DocumentFilterResult
                               {
                                   DocumentID = docFilter.DocumentID,
                                   FolderVersionID = docFilter.FolderVersionID,
                                   FormDesignID = docFilter.FormDesignID,
                                   FormDesignVersionID = docFilter.FormDesignVersionID,
                                   FormInstanceID = docFilter.FormInstanceID,
                                   FolderID = docFilter.FolderID,
                                   EffectiveDate = docFilter.EffectiveDate,
                                   FolderVersionNumber = docFilter.FolderVersionNumber,
                                   MasterListCascadeID = masterListCascadeID
                               };
            return finalResults.ToList();
        }

        public List<MasterListCascadeDocumentRuleViewModel> GetRules(int formDesignID, int formDesignVersionID, int masterListFormDesignID, int masterListFormDesignVersionID)
        {
            List<MasterListCascadeDocumentRuleViewModel> rules = new List<MasterListCascadeDocumentRuleViewModel>();
            var mlCas = from c in this._unitOfWork.RepositoryAsync<MasterListCascadeDocumentRule>()
                                                                       .Query()
                                                                       .Filter(c => c.FormDesignID == formDesignID && c.FormDesignVersionID == formDesignVersionID && c.IsActive == true && c.MasterListFormDesignID == masterListFormDesignID && c.MasterListFormDesignVersionID == masterListFormDesignVersionID)
                                                                       .Get().OrderBy(a => a.SequenceNo)
                        select new MasterListCascadeDocumentRuleViewModel
                        {
                            CompiledRuleJSON = c.CompiledRuleJSON,
                            FormDesignID = c.FormDesignID,
                            FormDesignVersionID = c.FormDesignVersionID,
                            TargetElementPath = c.TargetElementPath,
                            DocumentRuleTypeID = c.DocumentRuleTypeID,
                            RuleJSON = c.RuleJSON,
                            SequenceNo = c.SequenceNo
                        };
            if (mlCas != null && mlCas.Count() > 0)
            {
                rules = mlCas.ToList();
            }
            return rules;
        }

        public int AddMasterListCascadeBatchDetail(MasterListCascadeBatchDetailViewModel batchDetailModel)
        {
            MasterListCascadeBatchDetail batchDetail = new MasterListCascadeBatchDetail();
            batchDetail.IsTargetMasterList = batchDetailModel.IsTargetMasterList;
            batchDetail.MasterListCascadeBatchID = batchDetailModel.MasterListCascadeBatchID;
            batchDetail.MasterListCascadeID = batchDetailModel.MasterListCascadeID;
            batchDetail.Message = batchDetailModel.Message;
            batchDetail.NewFolderVersionID = batchDetailModel.NewFolderVersionID;
            batchDetail.NewFormInstanceID = batchDetailModel.NewFormInstanceID;
            batchDetail.PreviousFolderVersionID = batchDetailModel.PreviousFolderVersionID;
            batchDetail.PreviousFormInstanceID = batchDetailModel.PreviousFormInstanceID;
            batchDetail.ProcessedDate = DateTime.Now;
            batchDetail.Status = batchDetailModel.Status;
            batchDetail.TargetFolderID = batchDetailModel.TargetFolderID;
            _unitOfWork.RepositoryAsync<MasterListCascadeBatchDetail>().Insert(batchDetail);
            _unitOfWork.Save();
            return batchDetail.MasterListCascadeBatchID;
        }

        public ServiceResult UpdateMasterListCascadeBatchDetail(int masterListCascadeBatchDetailID, int status, string message)
        {
            ServiceResult result = new ServiceResult();
            var masterlistBatchDetail = _unitOfWork.RepositoryAsync<MasterListCascadeBatchDetail>().Get(a => a.MasterListCascadeDetailID == masterListCascadeBatchDetailID);
            if (masterlistBatchDetail != null && masterlistBatchDetail.Count() > 0)
            {
                MasterListCascadeBatchDetail batchDetail = masterlistBatchDetail.First();
                batchDetail.Status = status;
                batchDetail.Message = message;
                try
                {
                    _unitOfWork.RepositoryAsync<MasterListCascadeBatchDetail>().Update(batchDetail, true);
                    _unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
                catch (Exception ex)
                {
                    result.Result = ServiceResultStatus.Failure;
                    ServiceResultItem item = new ServiceResultItem();
                    string resultMessage = " Update Status Failed : Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace;
                    item.Messages = new string[] { resultMessage };
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(item);
                    result.Items = items;
                }
            }
            return result;
        }

        public List<MasterListCascadeBatchViewModel> GetMasterListCascadeBatch()
        {
            List<MasterListCascadeBatchViewModel> mlCascadeBatch = new List<MasterListCascadeBatchViewModel>();

            var mlCasBatch = (from masterListCascadeBatch in this._unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Get()
                              join formDesignVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                              on masterListCascadeBatch.FormDesignVersionID equals formDesignVersion.FormDesignVersionID
                              join masterListCascadeStatus in this._unitOfWork.RepositoryAsync<MasterListCascadeStatus>().Get()
                              on masterListCascadeBatch.Status equals masterListCascadeStatus.StatusID
                              join formDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                              on formDesignVersion.FormDesignID equals formDesign.FormID
                              select new MasterListCascadeBatchViewModel
                              {
                                  MasterListCascadeBatchID = masterListCascadeBatch.MasterListCascadeBatchID,
                                  MasterListName = formDesign.FormName,
                                  StatusName = masterListCascadeStatus.Status,
                                  StartDate = masterListCascadeBatch.StartDate,
                                  EndDate = masterListCascadeBatch.EndDate,
                                  ErrorMessage = masterListCascadeBatch.Message,
                                  QueuedBy = masterListCascadeBatch.QueuedBy
                              });

            if (mlCasBatch != null && mlCasBatch.Count() > 0)
            {
                mlCascadeBatch = mlCasBatch.ToList();
                mlCascadeBatch = mlCascadeBatch.OrderByDescending(x => x.StartDate).ToList();
            }

            return mlCascadeBatch;
        }

        public List<MasterListCascadeBatchDetailViewModel> GetMasterListCascadeBatchDetail(int masterListCascadeBatchID)
        {

            List<MasterListCascadeBatchDetailViewModel> mlCascadeBatchDetail = new List<MasterListCascadeBatchDetailViewModel>();

            var mlCasBatchDetail = (from masterListCascadeBatchDetail in this._unitOfWork.RepositoryAsync<MasterListCascadeBatchDetail>().Get()
                                    join masterListCascadeStatus in this._unitOfWork.RepositoryAsync<MasterListCascadeStatus>().Get()
                                    on masterListCascadeBatchDetail.Status equals masterListCascadeStatus.StatusID
                                    join folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                    on masterListCascadeBatchDetail.NewFolderVersionID equals folderVersion.FolderVersionID
                                    join folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                    on folderVersion.FolderID equals folder.FolderID
                                    join formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                    on masterListCascadeBatchDetail.NewFormInstanceID equals formInstance.FormInstanceID
                                    join accountProductMap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                                    on formInstance.FormInstanceID equals accountProductMap.FormInstanceID
                                    where masterListCascadeBatchDetail.MasterListCascadeBatchID == masterListCascadeBatchID

                                    select new MasterListCascadeBatchDetailViewModel
                                    {
                                        FolderName = folder.Name,
                                        FolderVersionNumber = folderVersion.FolderVersionNumber,
                                        FormInstanceName = formInstance.Name,
                                        FormDesignName = formInstance.FormDesign.FormName,
                                        PlanCode = accountProductMap.PlanCode,
                                        StatusName = masterListCascadeStatus.Status,
                                        ProcessedDate = masterListCascadeBatchDetail.ProcessedDate,
                                        Message = masterListCascadeBatchDetail.Message
                                    });

            if (mlCasBatchDetail != null && mlCasBatchDetail.Count() > 0)
            {
                mlCascadeBatchDetail = mlCasBatchDetail.ToList();
                mlCascadeBatchDetail = mlCascadeBatchDetail.OrderByDescending(x => x.ProcessedDate).ToList();
            }

            return mlCascadeBatchDetail;
        }

        public ElementDocumentRuleViewModel GetElementDocumentRule(int formDesignVersionID, string sourceElementPath)
        {
            ElementDocumentRuleViewModel model = new ElementDocumentRuleViewModel();

            var edrModel = (from edr in this._unitOfWork.RepositoryAsync<ElementDocumentRule>().Get()
                            where edr.FormDesignVersionID == formDesignVersionID && edr.SourcePath == sourceElementPath
                            select new ElementDocumentRuleViewModel
                            {
                                FormDesignID = edr.FormDesignID,
                                FormDesignVersionID = edr.FormDesignVersionID,
                                RuleJSON = edr.RuleJSON,
                                TargetFieldPaths = edr.TargetPaths
                            });

            if (edrModel != null && edrModel.Count() > 0)
            {
                model = edrModel.First();
            }
            return model;
        }

        public List<ElementDocumentRuleViewModel> GetElementDocumentRules(int formDesignVersionID, string sourceElementPath)
        {
            List<ElementDocumentRuleViewModel> models = new List<ElementDocumentRuleViewModel>();
            //|| edr.ParentFormDesignVersionID == formDesignVersionID
            var edrModel = (from edr in this._unitOfWork.RepositoryAsync<ElementDocumentRule>().Get()
                            where (edr.FormDesignVersionID == formDesignVersionID || edr.ParentFormDesignVersionID == formDesignVersionID) && edr.SourcePath == sourceElementPath
                            select new ElementDocumentRuleViewModel
                            {
                                FormDesignID = edr.FormDesignID,
                                FormDesignVersionID = edr.FormDesignVersionID,
                                RuleJSON = edr.RuleJSON,
                                TargetFieldPaths = edr.TargetPaths,
                                ParentFormDesignVersionID = edr.ParentFormDesignVersionID ?? 0
                            });

            if (edrModel != null && edrModel.Count() > 0)
                edrModel.ToList();

            return edrModel.ToList();
        }

        public int getFormInstancePBPView(int forminstanceid, int formDesignVersionID)
        {
            //ElementDocumentRuleViewModel model = new ElementDocumentRuleViewModel();

           var model = (from i in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                     where i.AnchorDocumentID == forminstanceid && i.FormDesignVersionID == formDesignVersionID
                     select new 
                     {
                         i.FormInstanceID
                     }).FirstOrDefault();

          //  int formid = Convert.ToInt32(model.FormDesignVersionID);
            return model.FormInstanceID;
        }

        public List<ElementDocumentRuleViewModel> GetODMElementDocumentRules(int formDesignVersionId)
        {
            List<ElementDocumentRuleViewModel> rules = new List<ElementDocumentRuleViewModel>();
            if (formDesignVersionId != 0)
            {
                rules = (from edr in this._unitOfWork.RepositoryAsync<ElementDocumentRule>().Get()
                         where edr.FormDesignVersionID == formDesignVersionId && edr.RunOnMigration == true
                         select new ElementDocumentRuleViewModel
                         {
                             FormDesignID = edr.FormDesignID,
                             FormDesignVersionID = edr.FormDesignVersionID,
                             RuleJSON = edr.RuleJSON,
                             TargetFieldPaths = edr.TargetPaths
                         }).ToList();
            }
            return rules;
        }

        public MasterListCascadeBatchViewModel GetQueuedBatch()
        {
            MasterListCascadeBatchViewModel mlCascadeBatch = null;

            var mlCasBatch = (from masterListCascadeBatch in this._unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Get()
                              join masterListCascade in this._unitOfWork.RepositoryAsync<MasterListCascade>().Get()
                              on masterListCascadeBatch.FormDesignVersionID equals masterListCascade.MasterListDesignVersionID
                              join masterListCascadeStatus in this._unitOfWork.RepositoryAsync<MasterListCascadeStatus>().Get()
                              on masterListCascadeBatch.Status equals masterListCascadeStatus.StatusID
                              join formDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                              on masterListCascade.MasterListDesignID equals formDesign.FormID
                              where masterListCascade.IsActive == true && masterListCascadeBatch.Status < 3
                              select new MasterListCascadeBatchViewModel
                              {
                                  MasterListCascadeBatchID = masterListCascadeBatch.MasterListCascadeBatchID,
                                  MasterListName = formDesign.FormName,
                                  StatusName = masterListCascadeStatus.Status,
                                  StartDate = masterListCascadeBatch.StartDate,
                                  EndDate = masterListCascadeBatch.EndDate,
                                  ErrorMessage = masterListCascadeBatch.Message,
                                  QueuedBy = masterListCascadeBatch.QueuedBy
                              });

            if (mlCasBatch != null && mlCasBatch.Count() > 0)
            {
                mlCascadeBatch = mlCasBatch.First();
            }

            return mlCascadeBatch;
        }

        public MasterListCascadeBatchDetailViewModel GetInProgressMatchCascade(int formInstanceId)
        {
            MasterListCascadeBatchDetailViewModel model = null;
            var mlCasBatch = from masterListCascadeBatchDetail in this._unitOfWork.RepositoryAsync<MasterListCascadeBatchDetail>().Get()
                              join masterListCascadeBatch in this._unitOfWork.RepositoryAsync<MasterListCascadeBatch>().Get()
                              on masterListCascadeBatchDetail.MasterListCascadeBatchID equals masterListCascadeBatch.MasterListCascadeBatchID
                              join masterListCascade in this._unitOfWork.RepositoryAsync<MasterListCascade>().Get()
                              on masterListCascadeBatch.FormDesignVersionID equals masterListCascade.MasterListDesignVersionID
                              join masterListCascadeStatus in this._unitOfWork.RepositoryAsync<MasterListCascadeStatus>().Get()
                              on masterListCascadeBatch.Status equals masterListCascadeStatus.StatusID
                              join formDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                              on masterListCascade.MasterListDesignID equals formDesign.FormID
                              where masterListCascade.IsActive == true && masterListCascadeBatch.Status < 3
                              && masterListCascadeBatchDetail.PreviousFormInstanceID == formInstanceId
                             select new MasterListCascadeBatchDetailViewModel
                             {
                                 NewFolderVersionID = masterListCascadeBatchDetail.NewFolderVersionID,
                                 PreviousFolderVersionID = masterListCascadeBatchDetail.PreviousFolderVersionID,
                                 NewFormInstanceID = masterListCascadeBatchDetail.NewFormInstanceID,
                                 PreviousFormInstanceID = masterListCascadeBatchDetail.PreviousFormInstanceID
                             };
            if(mlCasBatch != null && mlCasBatch.Count() > 0)
            {
                model = mlCasBatch.First();
            }
            return model;
        }
    }
}
