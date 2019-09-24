using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdateViewModels;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.domain.entities.Utility;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
//using tmg.equinox.documentmatch;
using System.Runtime.InteropServices;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.FolderVersionDetail;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using VersionType = tmg.equinox.domain.entities.Enums.VersionType;
using tmg.equinox.iasbuilder;
using System.Data;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.applicationservices
{
    public partial class GlobalUpdateService : IGlobalUpdateService
    {

        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private CommentsData _commentsData { get; set; }
        #endregion Private Members

        #region Public Properties

        public object JsonRequestBehavior { get; private set; }
        public List<UIElement> formElementList;
        List<GlobalUpdateComputedStatus> globalUpdatesExecutionStatus;

        #endregion Public Properties

        #region Constructor
        public GlobalUpdateService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public List<FormDesignVersionRowModel> GetFormVersions(DateTime effectiveDateFrom, DateTime effectiveDateTo)
        {
            List<FormDesignVersionRowModel> formDesignVersions = new List<FormDesignVersionRowModel>();

            try
            {
                formDesignVersions = (from fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Query().Filter(fdv => fdv.EffectiveDate >= effectiveDateFrom && fdv.EffectiveDate <= effectiveDateTo)
                                   .Get()
                                      join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                      on fdv.FormDesignID equals fd.FormID
                                      where fdv.StatusID == 3
                                          && (fd.FormID != (int)GlobalVariables.MASTERLISTFORMDESIGNID)
                                      select new FormDesignVersionRowModel
                                      {
                                          FormDesignId = fdv.FormDesignID,
                                          FormDesignVersionId = fdv.FormDesignVersionID,
                                          EffectiveDate = fdv.EffectiveDate,
                                          Version = fdv.VersionNumber,
                                          FormDesignName = fd.FormName,
                                          StatusId = fdv.StatusID
                                      }).OrderBy(x => x.FormDesignName).ToList();
            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersions;
        }

        public IEnumerable<DocumentVersionUIElementRowModel> GetUIElementListForGuFormDesignVersion(int tenantId, int formDesignVersionId, int globalUpdateId, bool isOnlySelectedElements)
        {
            IList<DocumentVersionUIElementRowModel> uiElementRowModelList = null;
            List<int> keyUIElemets = new List<int>();
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();

                var ExistingUIElements = (from guElements in this._unitOfWork.Repository<FormDesignElementValue>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId && x.FormDesignVersionID == formDesignVersionId)
                                             .Get()
                                          select guElements.UIElementID).ToList();

                if (isOnlySelectedElements)
                {
                    elementList = GetHierarchicalParentUIElement(ExistingUIElements, elementList);
                }
                else
                {
                    keyUIElemets = GetKeyElementsForRepeater(formDesignVersionId);
                }

                if (elementList.Count() > 0)
                {
                    uiElementRowModelList = (from i in elementList
                                             select new DocumentVersionUIElementRowModel
                                             {
                                                 DataType = i.ApplicationDataType.ApplicationDataTypeName,
                                                 ElementType = GetElementType(i),
                                                 Label = i.Label == null ? "[Blank]" : i.Label,
                                                 level = i.ParentUIElementID.HasValue ? GetRowLevel(i.ParentUIElementID, elementList) : 0,
                                                 MaxLength = i is TextBoxUIElement ? ((TextBoxUIElement)i).MaxLength : 0,
                                                 Required = i.Validators.Count > 0 ? i.Validators.FirstOrDefault().IsRequired == true ? "Yes" : "No" : "No",
                                                 Sequence = i.Sequence,
                                                 UIElementID = i.UIElementID,
                                                 UIElementName = i.UIElementName,
                                                 parent = i.ParentUIElementID.HasValue ? i.ParentUIElementID.Value.ToString() : "0",
                                                 isLeaf = i.ParentUIElementID.HasValue ? IsLeafRow(i.UIElementID, elementList) : false,
                                                 isIncluded = (ExistingUIElements.Count() > 0 && ExistingUIElements.Contains(i.UIElementID)) ? true : false,
                                                 IsKey = isOnlySelectedElements ? false : (keyUIElemets.Count() > 0 && keyUIElemets.Contains(i.UIElementID) ? true : false),
                                                 AllowGlobalUpdates = (i.AllowGlobalUpdates == null || i.AllowGlobalUpdates == false ? false : true)
                                             }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }
        /// <summary>
        /// Get List of selected UIElement
        /// </summary>
        /// <param name="globalUpdateId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        public List<FormDesignElementValueVeiwModel> GetSelectedUIElementsList(int globalUpdateId, int formDesignVersionId)
        {

            List<FormDesignElementValueVeiwModel> selectedUIElements = new List<FormDesignElementValueVeiwModel>();

            selectedUIElements = (from guElements in this._unitOfWork.Repository<FormDesignElementValue>()
                                      .Query()
                                      .Filter(x => x.GlobalUpdateID == globalUpdateId && x.FormDesignVersionID == formDesignVersionId)
                                        .Get()
                                  select new FormDesignElementValueVeiwModel
                                  {
                                      FormDesignVersionID = guElements.FormDesignVersionID,
                                      IsUpdated = guElements.IsUpdated,
                                      IsValueUpdated = (guElements.IsUpdated == true ? "Yes" : "No"),
                                      GlobalUpdateID = guElements.GlobalUpdateID,
                                      ElementFullPath = guElements.ElementFullPath,
                                      ElementHeaderName = guElements.ElementHeaderName,
                                      FormDesignElementValueID = guElements.FormDesignElementValueID,
                                      FormDesignID = guElements.FormDesignID,
                                      UIElementID = guElements.UIElementID,
                                      AddedBy = guElements.AddedBy,
                                      AddedDate = guElements.AddedDate,
                                      UpdatedBy = guElements.UpdatedBy,
                                      UpdatedDate = guElements.UpdatedDate,
                                      NewValue = guElements.NewValue
                                  }
                                      ).ToList();

            return selectedUIElements;
        }
        public ServiceResult SaveFormDesignVersionUIElements(int tenantId, int formDesignId, int formDesignVersionId, int globalUpdateId, List<int> selectedUIElementList, string addedBy)
        {

            ServiceResult result = new ServiceResult();
            try
            {
                List<int> existingUIElements = (from guElements in this._unitOfWork.Repository<FormDesignElementValue>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId && x.FormDesignVersionID == formDesignVersionId)
                                              .Get()
                                                select guElements.UIElementID).ToList();

                if (existingUIElements.Count() > 0)
                {
                    List<int> uiElementsToAdd = (from inputs in selectedUIElementList where !existingUIElements.Contains(inputs) select inputs).ToList();
                    List<int> uiElementsToDelete = (from existing in existingUIElements where !selectedUIElementList.Contains(existing) select existing).ToList();

                    //var excludeUIElements = existingUIElements.Intersect(selectedUIElementList).ToList();
                    //var includeUIElements = (from currentElements in selectedUIElementList where !existingUIElements.Contains(currentElements) select currentElements).ToList();
                    //var missingUiElements = (from existingElements in existingUIElements where !selectedUIElementList.Contains(existingElements) select existingElements).ToList();

                    foreach (var item in uiElementsToDelete)
                    {
                        var iteamToDelete = this._unitOfWork.Repository<FormDesignElementValue>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId && x.UIElementID == item && x.FormDesignVersionID == formDesignVersionId).Get().FirstOrDefault();
                        this._unitOfWork.Repository<FormDesignElementValue>().Delete(iteamToDelete);
                        this._unitOfWork.Save();

                        //Delete RuleExpression value if any
                        var iteamToDeleteExpression = (from exp in this._unitOfWork.Repository<ExpressionGu>().Query().Get()
                                                       join rule in this._unitOfWork.Repository<RuleGu>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get()
                                                           on exp.RuleID equals rule.RuleID
                                                       where rule.UIElementID == item
                                                       select new { exp.ExpressionID }).OrderByDescending(exp=>exp.ExpressionID).ToList();
                        foreach (var itemRuleExp in iteamToDeleteExpression)
                        {
                            this._unitOfWork.Repository<ExpressionGu>().Delete(itemRuleExp.ExpressionID);
                            this._unitOfWork.Save();
                        }
                        //Delete Rule value if any
                        var iteamToDeleteRule = this._unitOfWork.Repository<RuleGu>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId && x.UIElementID == item).Get().ToList();
                        foreach (var itemRule in iteamToDeleteRule)
                        {
                            this._unitOfWork.Repository<RuleGu>().Delete(itemRule);
                            this._unitOfWork.Save();
                        }
                    }
                    selectedUIElementList = uiElementsToAdd;
                }


                formElementList = GetUIElemenstFromFormDesignVersion(formDesignVersionId);
                FormDesignElementValue formDesignElementValue;
                foreach (var uiElementId in selectedUIElementList)
                {
                    formDesignElementValue = new FormDesignElementValue();
                    formDesignElementValue.FormDesignID = formDesignId;
                    formDesignElementValue.FormDesignVersionID = formDesignVersionId;
                    formDesignElementValue.GlobalUpdateID = globalUpdateId;
                    formDesignElementValue.UIElementID = uiElementId;
                    formDesignElementValue.ElementFullPath = GetUIElementFullPath(uiElementId);
                    formDesignElementValue.ElementHeaderName = GetUIElementDisplayText(uiElementId);
                    formDesignElementValue.AddedBy = addedBy;
                    formDesignElementValue.AddedDate = DateTime.Now;
                    formDesignElementValue.IsUpdated = false;

                    this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Insert(formDesignElementValue);
                    this._unitOfWork.Save();
                }

                UpdateGlobalUpdateWizardStep(globalUpdateId, (int)tmg.equinox.domain.entities.Enums.IASWizardSteps.ElementSelection, addedBy);
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

        public GridPagingResponse<GlobalUpdateViewModel> GetExistingGlobalUpdatesList(int tenantId, GridPagingRequest gridPagingRequest)
        {
            List<GlobalUpdateViewModel> existingGUList = null;
            int count = 0;
            try
            {
                globalUpdatesExecutionStatus = ComputeGlobalUpdateExecutionStatusText(null, false);
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                existingGUList = (from gu in this._unitOfWork.Repository<GlobalUpdate>().Query().Filter(s => s.IsActive == true).Get()
                                  join guStatus in this._unitOfWork.Repository<GlobalUpdateStatus>().Query().Get()
                                    on gu.GlobalUpdateStatusID equals guStatus.GlobalUpdateStatusID
                                  join guws in this._unitOfWork.RepositoryAsync<IASWizardStep>().Query().Get() on gu.IASWizardStepID equals guws.IASWizardStepID
                                  select new GlobalUpdateViewModel
                                  {
                                      GlobalUpdateID = gu.GlobalUpdateID,
                                      GlobalUpdateStatusID = gu.GlobalUpdateStatusID,
                                      GlobalUpdateName = gu.GlobalUpdateName,
                                      WizardStepsID = gu.IASWizardStepID,
                                      WizardStepName = guws.Name,
                                      EffectiveDateFrom = gu.EffectiveDateFrom,
                                      EffectiveDateTo = gu.EffectiveDateTo,
                                      AddedBy = gu.AddedBy,
                                      GuAddedDate = gu.AddedDate,
                                      IsActive = gu.IsActive,
                                      TenantID = gu.TenantID,
                                      UpdatedBy = gu.UpdatedBy,
                                      UpdatedDate = gu.UpdatedDate,
                                      status = guStatus.GlobalUpdatestatus,
                                      IsIASDownloaded = gu.IsIASDownloaded,
                                      RowID = gu.GlobalUpdateID + "_" + gu.IASWizardStepID,
                                      IsErrorLogDownloaded = gu.IsErrorLogDownloaded,
                                  }).ToList();

                existingGUList.ForEach(x => { x.ExecutionStatusText = GetExecutionStatusText(x.GlobalUpdateID); x.ExecutionStatusSymbol = GetExecutionStatusSymbol(x.GlobalUpdateID); });

                existingGUList = existingGUList.ToList()
                                       .ApplySearchCriteria(criteria)
                                       .ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                       .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                       .ToList();
            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return new GridPagingResponse<GlobalUpdateViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, existingGUList);
        }

        /// <summary>
        /// Get Existing Batches
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public List<BatchViewModel> GetExistingBatchesList(int tenantId)
        {
            List<BatchViewModel> existingBatchList = null;
            try
            {

                existingBatchList = (from gu in this._unitOfWork.Repository<Batch>().Query().Get()
                                     join batchExe in this._unitOfWork.Repository<BatchExecution>().Query().Get()
                                     on gu.BatchID equals batchExe.BatchID
                                     into batchElement
                                     from batchlist in batchElement.DefaultIfEmpty()
                                     select new BatchViewModel
                                     {
                                         ApprovedBy = gu.ApprovedBy,
                                         AddedBy = gu.AddedBy,
                                         AddedDate = gu.AddedDate,
                                         BatchID = gu.BatchID,
                                         BatchName = gu.BatchName,
                                         UpdatedBy = gu.UpdatedBy,
                                         UpdatedDate = gu.UpdatedDate,
                                         ExecutionType = gu.ExecutionType,
                                         IsApproved = gu.IsApproved,
                                         IsApprovedString = (gu.IsApproved == true ? "Yes" : "No"),
                                         ScheduleDate = gu.ScheduleDate,
                                         ScheduledTime = gu.ScheduledTime,
                                         ApprovedDate = gu.ApprovedDate,
                                         BatchExecutionStatus = batchlist.BatchExecutionStatus == null ? (gu.IsApproved == true ? "Not Executed" : "Not Approved") : batchlist.BatchExecutionStatus.StatusName
                                     }).OrderBy(o => o.BatchID).ToList();

                return existingBatchList;
            }
            catch (Exception)
            {

                throw;
            }


        }
        /// <summary>
        /// Get List of IAS added in batch 
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>

        public List<BatchIASMapViewModel> getGUIdsFromBatchMap(Guid batchId)
        {
            List<BatchIASMapViewModel> listIASAddedInBatch = null;
            try
            {

                listIASAddedInBatch = (from batchIASMap in this._unitOfWork.Repository<BatchIASMap>()
                                       .Query()
                                       .Filter(x => x.BatchID == batchId)
                                       .Get()
                                       select new BatchIASMapViewModel
                                       {
                                           GlobalUpdateID = batchIASMap.GlobalUpdateID
                                       }).ToList();


                return listIASAddedInBatch;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public List<BatchExecutionViewModel> GetExecutedBatchesList(int tenantId, int rollBackHrs)
        {
            List<BatchExecutionViewModel> executedBatchesList = null;
            try
            {

                executedBatchesList = (from batchExe in this._unitOfWork.Repository<BatchExecution>().Query().Get()
                                       join batch in this._unitOfWork.Repository<Batch>().Query().Get()
                                       on batchExe.BatchID equals batch.BatchID
                                       join batchExeStatus in this._unitOfWork.Repository<BatchExecutionStatus>().Query().Get()
                                       on batchExe.BatchExecutionStatusID equals batchExeStatus.BatchExecutionStatusID
                                       select new BatchExecutionViewModel
                                       {
                                           BatchExecutionID = batchExe.BatchExecutionID,
                                           BatchExecutionStatusID = batchExe.BatchExecutionStatusID,
                                           BatchID = batchExe.BatchID,
                                           EndDateTime = batchExe.EndDateTime,
                                           StartDateTime = batchExe.StartDateTime,
                                           BatchExecutionStatus = batchExeStatus.StatusName,
                                           BatchName = batch.BatchName,
                                           RollBackComments = batchExe.RollBackComments,
                                           RollBackThrehold = rollBackHrs
                                       }).OrderBy(o => o.BatchID).ToList();

                //executedBatchesList.ForEach(x => x.RollBakThreholdFlag = getDifference(x.EndDateTime, rollBackHrs));
                return executedBatchesList;
            }
            catch (Exception)
            {

                throw;
            }


        }
        private bool getDifference(DateTime endDateTime, int rollBackHrs)
        {
            var diffInHours = (DateTime.Now - endDateTime).TotalHours;
            if (diffInHours < rollBackHrs)
                return true;
            else
                return false;
        }
        public List<IASWizardStepViewModel> GetIASWizardList(int tenantId)
        {
            List<IASWizardStepViewModel> iasWizardStepsList = null;

            try
            {
                iasWizardStepsList = (from c in this._unitOfWork.RepositoryAsync<IASWizardStep>()
                                              .Query()
                                              .Get()

                                      select new IASWizardStepViewModel
                                      {
                                          IASWizardStepsID = c.IASWizardStepID,
                                          Name = c.Name,
                                          NameID = c.NameID
                                      }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasWizardStepsList;
        }

        public List<IASElementImportViewModel> editBatchIASListGrid(Guid batchID)
        {
            List<IASElementImportViewModel> IASListToAdd = new List<IASElementImportViewModel>();
            List<IASElementImportViewModel> importedIASList = null;
            List<BatchIASMapViewModel> IASListForOtherBatches = null;
            List<BatchIASMapViewModel> addedIASList = null;

            try
            {
                //Get all from IASElementImport
                importedIASList = (from impIAS in this._unitOfWork.RepositoryAsync<IASElementImport>()
                                   .Query()
                                   .Get()
                                   join gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Get()
                                   on impIAS.GlobalUpdateID equals gu.GlobalUpdateID
                                   where gu.GlobalUpdateStatusID == (int)GlobalUpdateIASStatus.Complete
                                   select new IASElementImportViewModel()
                                   {
                                       GlobalUpdateID = impIAS.GlobalUpdateID
                                   }
                                   ).Distinct().ToList();

                //Get entries from BatchIASMap for corresponding batchId
                IASListForOtherBatches = (from batchIASMap in this._unitOfWork.RepositoryAsync<BatchIASMap>()
                                   .Query()
                                   .Filter(x => x.BatchID != batchID)
                                   .Get()
                                          select new BatchIASMapViewModel
                                          {
                                              GlobalUpdateID = batchIASMap.GlobalUpdateID
                                          }
                                   ).Distinct().ToList();

                //Get entries from BatchIASMap for corresponding batchId
                addedIASList = (from batchIASMap in this._unitOfWork.RepositoryAsync<BatchIASMap>()
                                   .Query()
                                   .Filter(x => x.BatchID == batchID)
                                   .Get()
                                select new BatchIASMapViewModel
                                {
                                    BatchIASMapID = batchIASMap.BatchIASMapID,
                                    BatchID = batchIASMap.BatchID,
                                    GlobalUpdateID = batchIASMap.GlobalUpdateID
                                }
                                   ).ToList();
                //Get only Global Update Ids for importedIASList
                var importedIASListAllGUIds = importedIASList.Select(x => x.GlobalUpdateID).Distinct();
                //Get only Global Update Ids for addedIASList
                var addedIASListDupliAllGUIds = addedIASList.Select(x => x.GlobalUpdateID).Distinct();

                var GUIDInBatchMapForOtherBatches = IASListForOtherBatches.Select(x => x.GlobalUpdateID).Distinct();

                var excludedGUIdsForOtherBatches = importedIASListAllGUIds.Except(GUIDInBatchMapForOtherBatches).Distinct();
                //Get Common entries from both the lists
                var combinedGUIds = excludedGUIdsForOtherBatches.Concat(addedIASListDupliAllGUIds).Distinct();
                //Remove GlobalUpdate Ids which are present in other batches
                //var finalGUIds = commonGUIds.Except(GUIDInBatchMapForOtherBatches);

                //var finalIASListToAdd = importedIASListAllGUIds.Except(commomnGUIds);
                foreach (var globalUpId in combinedGUIds)
                {
                    List<IASElementImportViewModel> list = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                                            where gu.GlobalUpdateID == globalUpId
                                                            select new IASElementImportViewModel()
                                                            {
                                                                GlobalUpdateID = gu.GlobalUpdateID,
                                                                GlobalUpdateName = gu.GlobalUpdateName,
                                                                EffectiveDateFrom = gu.EffectiveDateFrom,
                                                                EffectiveDateTo = gu.EffectiveDateTo,
                                                                AddedBy = gu.AddedBy,
                                                                AddedDate = gu.AddedDate,
                                                                Include = addedIASListDupliAllGUIds.Contains(globalUpId) ? true : false
                                                            }).ToList();
                    IASListToAdd.AddRange(list);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return IASListToAdd;
        }

        public List<IASElementImportViewModel> viewBatchIASListGrid(Guid batchID)
        {
            List<IASElementImportViewModel> IASListToAdd = new List<IASElementImportViewModel>();
            List<BatchIASMapViewModel> addedIASList = null;

            try
            {

                //Get entries from BatchIASMap for corresponding batchId
                addedIASList = (from batchIASMap in this._unitOfWork.RepositoryAsync<BatchIASMap>()
                                   .Query()
                                   .Filter(x => x.BatchID == batchID)
                                   .Get()
                                select new BatchIASMapViewModel
                                {
                                    BatchIASMapID = batchIASMap.BatchIASMapID,
                                    BatchID = batchIASMap.BatchID,
                                    GlobalUpdateID = batchIASMap.GlobalUpdateID
                                }
                                   ).ToList();

                //Get only Global Update Ids for addedIASList
                var addedIASListDupliAllGUIds = addedIASList.Select(x => x.GlobalUpdateID).Distinct();

                foreach (var globalUpId in addedIASListDupliAllGUIds)
                {
                    List<IASElementImportViewModel> list = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                                            where gu.GlobalUpdateID == globalUpId
                                                            select new IASElementImportViewModel()
                                                            {
                                                                GlobalUpdateID = gu.GlobalUpdateID,
                                                                GlobalUpdateName = gu.GlobalUpdateName,
                                                                EffectiveDateFrom = gu.EffectiveDateFrom,
                                                                EffectiveDateTo = gu.EffectiveDateTo,
                                                                AddedBy = gu.AddedBy,
                                                                AddedDate = gu.AddedDate,
                                                                Include = addedIASListDupliAllGUIds.Contains(globalUpId) ? true : false
                                                            }).ToList();
                    IASListToAdd.AddRange(list);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return IASListToAdd;
        }
        public List<IASElementImportViewModel> listImportedNotAddedIAS()
        {
            List<IASElementImportViewModel> IASListToAdd = new List<IASElementImportViewModel>(); //null;
            List<IASElementImportViewModel> importedIASList = null;
            List<BatchIASMapViewModel> addedIASList = null;

            try
            {
                //Get all from IASElementImport
                importedIASList = (from impIAS in this._unitOfWork.RepositoryAsync<IASElementImport>().Query().Get().Distinct()
                                   join gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Get()
                                   on impIAS.GlobalUpdateID equals gu.GlobalUpdateID
                                   where gu.GlobalUpdateStatusID == (int)GlobalUpdateIASStatus.Complete
                                   select new IASElementImportViewModel()
                                   {
                                       GlobalUpdateID = impIAS.GlobalUpdateID,
                                   }
                                   ).Distinct().ToList();

                //Get all from BatchIASMap
                addedIASList = (from batchIASMap in this._unitOfWork.RepositoryAsync<BatchIASMap>()
                                   .Query()
                                   .Get()
                                select new BatchIASMapViewModel
                                {
                                    BatchIASMapID = batchIASMap.BatchIASMapID,
                                    BatchID = batchIASMap.BatchID,
                                    GlobalUpdateID = batchIASMap.GlobalUpdateID
                                }
                                   ).ToList();
                //Get only Global Update Ids for importedIASList
                var importedIASListAllGUIds = importedIASList.Select(x => x.GlobalUpdateID).Distinct();
                //Get only Global Update Ids for addedIASList
                var addedIASListDupliAllGUIds = addedIASList.Select(x => x.GlobalUpdateID);
                //Get Common entries from both the lists
                var commomnGUIds = importedIASListAllGUIds.Intersect(addedIASListDupliAllGUIds);
                //Remove GlobalUpdate Ids which are present in BatchIASMap
                var finalIASListToAdd = importedIASListAllGUIds.Except(commomnGUIds);
                foreach (var globalUpId in finalIASListToAdd)
                {
                    List<IASElementImportViewModel> list = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                                            where gu.GlobalUpdateID == globalUpId
                                                            select new IASElementImportViewModel()
                                                            {
                                                                GlobalUpdateID = gu.GlobalUpdateID,
                                                                GlobalUpdateName = gu.GlobalUpdateName,
                                                                EffectiveDateFrom = gu.EffectiveDateFrom,
                                                                EffectiveDateTo = gu.EffectiveDateTo,
                                                                AddedBy = gu.AddedBy,
                                                                AddedDate = gu.AddedDate
                                                            }).ToList();
                    IASListToAdd.AddRange(list);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return IASListToAdd;
        }
        public ServiceResult SaveGlobalUpdate(int tenantId, int globalUpdateId, string globalUpdateName, DateTime effectiveDateFrom, DateTime effectiveDateTo, string addedBy)
        {
            GlobalUpdate itemToAdd = new GlobalUpdate();
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Success;
            try
            {
                //Validate Global Update Inputs
                ValidateGlobalUpdatePreCondition(globalUpdateId, globalUpdateName, effectiveDateFrom, effectiveDateTo, ref result);

                //Checking if there are Document versions for specified Date range
                if (result.Result == ServiceResultStatus.Success)
                {
                    if (globalUpdateId != 0)
                    {
                        var globalUpdate = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get() select gu).FirstOrDefault();
                        bool isDateChange = (globalUpdate.EffectiveDateFrom == effectiveDateFrom && globalUpdate.EffectiveDateTo == effectiveDateTo) ? false : true;
                        globalUpdate.GlobalUpdateName = globalUpdateName;
                        globalUpdate.TenantID = tenantId;
                        globalUpdate.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_INPROGRESSID;
                        globalUpdate.IASWizardStepID = GlobalVariables.GB_WIZARDSTEP_ELEMENTSELECTIONID;
                        globalUpdate.EffectiveDateFrom = effectiveDateFrom;
                        globalUpdate.EffectiveDateTo = effectiveDateTo;
                        globalUpdate.IsActive = true;
                        globalUpdate.IsIASDownloaded = false;
                        globalUpdate.IsErrorLogDownloaded = false;
                        globalUpdate.UpdatedBy = addedBy;
                        globalUpdate.UpdatedDate = DateTime.Now;
                        this._unitOfWork.Repository<GlobalUpdate>().Update(globalUpdate);
                        this._unitOfWork.Save();
                        if (isDateChange)
                            DeleteGlobalUpdateData(globalUpdateId);
                    }
                    else
                    {
                        itemToAdd.GlobalUpdateName = globalUpdateName;
                        itemToAdd.TenantID = tenantId;
                        itemToAdd.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_INPROGRESSID;
                        itemToAdd.IASWizardStepID = GlobalVariables.GB_WIZARDSTEP_SETUPID;
                        itemToAdd.EffectiveDateFrom = effectiveDateFrom;
                        itemToAdd.EffectiveDateTo = effectiveDateTo;
                        itemToAdd.IsActive = true;
                        itemToAdd.IsIASDownloaded = false;
                        itemToAdd.IsErrorLogDownloaded = false;
                        itemToAdd.AddedBy = addedBy;
                        itemToAdd.AddedDate = DateTime.Now;
                        itemToAdd.UpdatedBy = null;
                        itemToAdd.UpdatedDate = null;

                        this._unitOfWork.Repository<GlobalUpdate>().Insert(itemToAdd);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                        {
                            Messages = new string[] { itemToAdd.GlobalUpdateID.ToString(), itemToAdd.GlobalUpdateName.ToString(), itemToAdd.EffectiveDateFrom.ToShortDateString(),
                    itemToAdd.EffectiveDateTo.ToShortDateString()}
                        });
                    }
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
        public List<GlobalUpdateViewModel> GetSelectedRowGlobalUpdateData(int? globalUpdateId)
        {
            List<GlobalUpdateViewModel> globalUpdateData = null;
            try
            {
                globalUpdateData = (from gb in this._unitOfWork.Repository<GlobalUpdate>().Query().Filter(g => g.GlobalUpdateID == globalUpdateId).Get()
                                    select new GlobalUpdateViewModel
                                    {
                                        GlobalUpdateID = gb.GlobalUpdateID,
                                        GlobalUpdateStatusID = gb.GlobalUpdateStatusID,
                                        GlobalUpdateName = gb.GlobalUpdateName,
                                        EffectiveDateFrom = gb.EffectiveDateFrom,
                                        EffectiveDateTo = gb.EffectiveDateTo,
                                        WizardStepsID = gb.IASWizardStepID
                                    }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return globalUpdateData;
        }
        public List<FormDesignVersionRowModel> GetUpdatedDocumentDesignVersion(int globalUpdateId)
        {
            List<FormDesignVersionRowModel> documentVersionList = new List<FormDesignVersionRowModel>();
            try
            {
                List<int> formDesignVersions = (from guElements in this._unitOfWork.Repository<FormDesignElementValue>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get()
                                                join formDesign in this._unitOfWork.Repository<FormDesign>().Query().Get()
                                                on guElements.FormDesignID equals formDesign.FormID
                                                join formDesigVersion in this._unitOfWork.Repository<FormDesignVersion>().Query().Filter(fv => fv.StatusID == 3).Get()
                                                on formDesign.FormID equals formDesigVersion.FormDesignID
                                                where formDesigVersion.FormDesignVersionID == guElements.FormDesignVersionID
                                                select formDesigVersion.FormDesignVersionID).Distinct().ToList();


                documentVersionList = (from formDesignVersion in this._unitOfWork.Repository<FormDesignVersion>().Query().Filter(x => formDesignVersions.Contains(x.FormDesignVersionID) && x.StatusID == 3).Get()
                                       join formDesign in this._unitOfWork.Repository<FormDesign>().Query().Get()
                                       on formDesignVersion.FormDesignID equals formDesign.FormID
                                       select new FormDesignVersionRowModel
                                       {
                                           FormDesignId = formDesignVersion.FormDesignID,
                                           FormDesignVersionId = formDesignVersion.FormDesignVersionID,
                                           StatusText = formDesignVersion.Status.Status1,
                                           Version = formDesignVersion.VersionNumber,
                                           FormDesignName = formDesign.FormName
                                       }).ToList();


            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return documentVersionList;
        }

        private string GetUIElementFullPath(int elementID)
        {
            UIElement element = (from elem in formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            string fullName = "";
            if (element != null)
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
            return fullName;
        }

        private string GetUIElementDisplayText(int elementID)
        {
            UIElement element = (from elem in formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            string displayText = "";
            if (element != null)
            {
                displayText = element.Label;
            }
            return displayText;
        }

        private List<UIElement> GetUIElemenstFromFormDesignVersion(int formDesignVersionId)
        {

            List<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                               join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                       .Query()
                                                                       .Get()
                                               on u.UIElementID equals fd.UIElementID
                                               where fd.FormDesignVersionID == formDesignVersionId
                                               select u).ToList();

            return formElementList;
        }

        private bool IsLeafRow(int? elementID, List<UIElement> models)
        {
            try
            {
                foreach (UIElement element in models)
                {
                    if (element.ParentUIElementID == elementID)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        private string GetElementType(UIElement uielement)
        {
            string uIElementType = string.Empty;
            try
            {
                if (uielement is RadioButtonUIElement)
                {
                    uIElementType = ElementTypes.list[0];
                }
                else if (uielement is CheckBoxUIElement)
                {
                    uIElementType = ElementTypes.list[1];
                }
                else if (uielement is TextBoxUIElement)
                {
                    switch (((TextBoxUIElement)uielement).UIElementTypeID)
                    {
                        case 3:
                            uIElementType = ElementTypes.list[2];
                            break;
                        case 4:
                            uIElementType = ElementTypes.list[3];
                            break;
                        case 10:
                            uIElementType = ElementTypes.list[9];
                            break;
                        case 11:
                            uIElementType = ElementTypes.list[10];
                            break;
                        case 13:
                            uIElementType = ElementTypes.list[12];
                            break;
                    }
                }
                else if (uielement is DropDownUIElement)
                {
                    switch (((DropDownUIElement)uielement).UIElementTypeID)
                    {
                        case 5:
                            uIElementType = ElementTypes.list[4];
                            break;
                        case 12:
                            uIElementType = ElementTypes.list[11];
                            break;
                    }
                }
                else if (uielement is CalendarUIElement)
                {
                    uIElementType = ElementTypes.list[5];
                }
                else if (uielement is SectionUIElement)
                {
                    uIElementType = ElementTypes.list[8];
                }
                else if (uielement is RepeaterUIElement)
                {
                    uIElementType = ElementTypes.list[6];
                }
                else if (uielement is TabUIElement)
                {
                    uIElementType = ElementTypes.list[7];
                }
                else
                {
                    uIElementType = "-";
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return uIElementType;
        }

        private int GetRowLevel(int? parentID, List<UIElement> elementList)
        {
            int level = 0;
            try
            {
                while (parentID != null)
                {
                    level++;
                    var result = from element in elementList
                                 where element.UIElementID == parentID
                                 select element;

                    parentID = result.Single().ParentUIElementID;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return level;
        }

        /// <summary>
        /// In this funtion effective date range applied for design versions
        /// </summary>
        /// <param name="effectiveDateFrom"></param>
        /// <param name="effectiveDateTo"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<IASFolderDataModel> GetGlobalUpdateImpactedFolderVersionList(int GlobalUpdateID, DateTime effectiveDateFrom, DateTime effectiveDateTo, int tenantId)
        {
            List<IASFolderDataModel> FolderVersionList = null;

            try
            {
                FolderVersionList = new List<IASFolderDataModel>();
                var formDesignVersionList = (from formDesignVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                             .Query()
                                             .Filter(fil => fil.TenantID == tenantId
                                                         && fil.FormDesignID != GlobalVariables.MASTERLISTFORMDESIGNID
                                                         && fil.EffectiveDate >= effectiveDateFrom
                                                         && fil.EffectiveDate <= effectiveDateTo)
                                             .Get()
                                             .OrderBy(ord => ord.FormDesignID)
                                             join fdev in this._unitOfWork.RepositoryAsync<FormDesignElementValue>()
                                             .Query()
                                             .Filter(j => j.GlobalUpdateID == GlobalUpdateID)
                                             .Get()
                                             on formDesignVersion.FormDesignVersionID equals fdev.FormDesignVersionID
                                             //on formDesignVersion.FormDesignID equals fdev.FormDesignID
                                             select new
                                             {
                                                 FormDesignVersionID = formDesignVersion.FormDesignVersionID,
                                                 FormDesignID = formDesignVersion.FormDesignID
                                             }).OrderBy(ord => ord.FormDesignVersionID).ToList();
                //var formDesignVersionIdList = formDesignVersionList.GroupBy(xy => xy.FormDesignID).Select(xy => xy.FirstOrDefault()).ToList();
                var formDesignVersionIdList = formDesignVersionList.GroupBy(xy => xy.FormDesignVersionID).ToList();

                foreach (var formDesignVersionId in formDesignVersionIdList)
                {
                    int fDVId = Convert.ToInt32(formDesignVersionId.FirstOrDefault().FormDesignVersionID);
                    int formDesignId = Convert.ToInt32(formDesignVersionId.FirstOrDefault().FormDesignID);
                    //int fDVId = Convert.ToInt32(formDesignVersionId.FormDesignVersionID);
                    //int formDesignId = Convert.ToInt32(formDesignVersionId.FormDesignID);

                    var folderVersionIds = (from formInstance in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Filter(fil => fil.TenantID == tenantId
                                                              && fil.IsActive == true
                                                              && fil.FormDesignVersionID >= fDVId
                                                              && fil.FormDesignID == formDesignId)
                                                  .Get()
                                            join formInstanceDataMap in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                            on formInstance.FormInstanceID equals formInstanceDataMap.FormInstanceID
                                            select new FormInstanceViewModel
                                            {
                                                FormInstanceID = formInstance.FormInstanceID,
                                                FolderVersionID = formInstance.FolderVersionID,
                                                Name = formInstance.Name,
                                                FolderID = formInstance.FolderVersion.FolderID,
                                                EffectiveDate = formInstance.FolderVersion.EffectiveDate,
                                                AddedBy = formInstance.AddedBy,
                                                FolderVersionStateID = formInstance.FolderVersion.FolderVersionStateID,
                                                FolderVersionBatchID = formInstance.FolderVersion.FolderVersionBatchID
                                            }).OrderByDescending(ord => ord.FolderVersionID)
                                            .ThenByDescending(ord => ord.FormInstanceID).ToList();
                    
                    //From folderVersionIds if get same EffectiveDate for multiple folderVersions pick the latest 
                    //one from list
                    var effectiveFolderVersionsList = new List<FormInstanceViewModel>();
                    foreach (var version in folderVersionIds)
                    {
                        if (version.FolderVersionStateID != Convert.ToInt32(FolderVersionState.BASELINED) && version.FolderVersionStateID != Convert.ToInt32(FolderVersionState.INPROGRESS_BLOCKED))
                        {
                            if (effectiveFolderVersionsList.Any())
                            {
                                if (!effectiveFolderVersionsList.Any(ver => ver.EffectiveDate == version.EffectiveDate && ver.FolderID == version.FolderID && ver.FolderVersionStateID == version.FolderVersionStateID && ver.FormInstanceID == version.FormInstanceID))
                                {
                                    if (effectiveFolderVersionsList.Any(ver => ver.FolderID == version.FolderID && ver.FolderVersionBatchID != null))
                                    {
                                        if (version.FolderVersionBatchID != null && effectiveFolderVersionsList.Any(ver => ver.FolderID == version.FolderID && ver.FolderVersionBatchID == version.FolderVersionBatchID))
                                        {
                                            effectiveFolderVersionsList.Add(version);
                                        }
                                    }
                                    else
                                    {
                                        effectiveFolderVersionsList.Add(version);
                                    }
                                }
                            }
                            else
                            {
                                effectiveFolderVersionsList.Add(version);
                            }
                        }
                    }
                    var effectiveFolderVersionIds = effectiveFolderVersionsList.OrderBy(ord => ord.FolderVersionID).ToList();

                    int oldFolderId = 0;
                    foreach (var folderVersionId in effectiveFolderVersionIds)
                    {
                        int fVId = Convert.ToInt32(folderVersionId.FolderVersionID);
                        int frmId = Convert.ToInt32(folderVersionId.FormInstanceID);
                        string documentName = Convert.ToString(folderVersionId.Name);
                        string owner = Convert.ToString(folderVersionId.AddedBy);
                        int folderId = Convert.ToInt32(folderVersionId.FolderID);
                        if (oldFolderId != folderId)
                        {
                            oldFolderId = folderId;
                        }

                        var account = (from act in this._unitOfWork.RepositoryAsync<AccountFolderMap>()
                                                    .Query()
                                                    .Include(inc => inc.Account)
                                                    .Filter(fil => fil.FolderID == folderId)
                                                    .Get()
                                       select new
                                       {
                                           AccountName = act.Account.AccountName
                                       }).FirstOrDefault();
                        string accountName = String.Empty;
                        if (account != null)
                        {
                            accountName = Convert.ToString(account.AccountName);
                        }
                        else
                        {
                            accountName = "Portfolio";
                        }

                        var folderVersionList = (from folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                                    .Query()
                                                    .Include(inc => inc.Folder)
                                                    .Filter(fil => fil.TenantID == tenantId
                                                                 && fil.FolderVersionID == fVId)
                                                    .Get()
                                                 select new IASFolderDataModel
                                                 {
                                                     FolderVersionID = folderVersion.FolderVersionID,
                                                     AccountName = accountName,
                                                     FolderID = folderVersion.Folder.FolderID,
                                                     FolderName = folderVersion.Folder.Name,
                                                     FolderVersionNumber = folderVersion.FolderVersionNumber,
                                                     EffectiveDate = folderVersion.EffectiveDate,
                                                     FormInstanceID = frmId,
                                                     FormName = documentName,
                                                     //Owner = folderVersion.AddedBy
                                                     Owner = owner
                                                 }).FirstOrDefault();

                        if (folderVersionList != null)
                        {
                            //FolderVersionList.Add(folderVersionList);

                            if (FolderVersionList.Any())
                            {
                                if (!FolderVersionList.Any(ver => ver.EffectiveDate == folderVersionList.EffectiveDate && ver.FolderID == folderVersionList.FolderID && ver.FolderVersionID == folderVersionList.FolderVersionID && ver.FormInstanceID == folderVersionList.FormInstanceID))
                                {
                                    FolderVersionList.Add(folderVersionList);
                                }
                            }
                            else
                            {
                                FolderVersionList.Add(folderVersionList);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            if (FolderVersionList != null)
                return FolderVersionList.OrderBy(o => o.FolderID);
            else
                return null;
        }

        public List<IASFolderDataModel> GetGlobalUpdatesFolderDataList(int GlobalUpdateID)
        {
            List<IASFolderDataModel> globalUpdatesFolderDataList = null;

            try
            {
                var globalUpdatesFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get()

                                               select new IASFolderDataModel
                                               {
                                                   FormDesignID = j.FormInstance.FormDesignID,
                                                   IASFolderMapID = j.IASFolderMapID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   AccountName = j.AccountName,
                                                   FolderID = j.FolderID,
                                                   FolderName = j.FolderName,
                                                   FolderVersionID = j.FolderVersionID,
                                                   FolderVersionNumber = j.FolderVersionNumber,
                                                   EffectiveDate = j.EffectiveDate,
                                                   FormInstanceID = j.FormInstanceID,
                                                   FormName = j.FormName,
                                                   Owner = j.Owner
                                               }).OrderBy(el => el.FolderID);
                if (globalUpdatesFolderData != null)
                {
                    globalUpdatesFolderDataList = globalUpdatesFolderData.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesFolderDataList;
        }

        public ServiceResult ScheduleIASUpload(int GlobalUpdateID, bool flag, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                GlobalUpdate itemToUpdate = this._unitOfWork.RepositoryAsync<GlobalUpdate>()
                                                               .FindById(GlobalUpdateID);

                if (itemToUpdate != null)
                {
                    if (flag == true)
                    {
                        itemToUpdate.IsErrorLogDownloaded = true;
                        //Error Log Download is in Progress
                        itemToUpdate.GlobalUpdateStatusID = (int)GlobalUpdateIASStatus.ValidationInProgress;
                    }
                    else if (flag == false)
                    {
                        itemToUpdate.IsErrorLogDownloaded = false;
                    }

                    itemToUpdate.IASWizardStepID = (int)tmg.equinox.domain.entities.Enums.IASWizardSteps.GenerateIAS;
                    itemToUpdate.UpdatedBy = addedBy;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<GlobalUpdate>().Update(itemToUpdate);
                    this._unitOfWork.Save();

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

        public ServiceResult ScheduleGlobalUpdate(int GlobalUpdateID, bool flag, string addedBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                GlobalUpdate itemToUpdate = this._unitOfWork.RepositoryAsync<GlobalUpdate>()
                                                               .FindById(GlobalUpdateID);

                if (itemToUpdate != null)
                {
                    if (flag == true)
                    {
                        itemToUpdate.IsIASDownloaded = true;
                        //IAS Download is in Progress
                        itemToUpdate.GlobalUpdateStatusID = (int)GlobalUpdateIASStatus.IASDownloadInProgress;
                    }
                    else if (flag == false)
                    {
                        itemToUpdate.IsIASDownloaded = false;
                    }

                    itemToUpdate.IASWizardStepID = (int)tmg.equinox.domain.entities.Enums.IASWizardSteps.GenerateIAS;
                    itemToUpdate.UpdatedBy = addedBy;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<GlobalUpdate>().Update(itemToUpdate);
                    this._unitOfWork.Save();

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

        public ServiceResult SaveIASFolderDataValues(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy)
        {
            ServiceResult serviceResult = null;
            try
            {
                serviceResult = new ServiceResult();
                if (IASFolderDataList != null && IASFolderDataList.Count() > 0)
                {
                    //Delete earlier saved entries in DB tables GU.IASFolderMap & GU.IASElementExport
                    DeleteIASElementExportLog(GlobalUpdateID);
                    DeleteIASFolderMapLog(GlobalUpdateID);
                    serviceResult = SaveIASFolderData(GlobalUpdateID, IASFolderDataList.OrderBy(o => o.FolderID), addedBy);
                }
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

        private ServiceResult SaveIASFolderData(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy)
        {
            var result = new ServiceResult();

            foreach (var IASFolderData in IASFolderDataList)
            {
                IASFolderMap itemToAdd = new IASFolderMap();
                try
                {
                    itemToAdd.GlobalUpdateID = GlobalUpdateID;
                    itemToAdd.AccountName = IASFolderData.AccountName;
                    itemToAdd.FolderID = IASFolderData.FolderID;
                    itemToAdd.FolderName = IASFolderData.FolderName;
                    itemToAdd.FolderVersionID = IASFolderData.FolderVersionID;
                    itemToAdd.FolderVersionNumber = IASFolderData.FolderVersionNumber;
                    itemToAdd.EffectiveDate = IASFolderData.EffectiveDate;
                    itemToAdd.FormInstanceID = IASFolderData.FormInstanceID;
                    itemToAdd.FormName = IASFolderData.FormName;
                    itemToAdd.Owner = IASFolderData.Owner;
                    itemToAdd.AddedBy = addedBy;
                    itemToAdd.AddedDate = DateTime.Now;
                    itemToAdd.UpdatedBy = null;
                    itemToAdd.UpdatedDate = null;

                    this._unitOfWork.Repository<IASFolderMap>().Insert(itemToAdd);
                    this._unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                    result = ex.ExceptionMessages();
                }
            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        public List<FormDesignElementValueVeiwModel> GetFormDesignVersionUIElements(int GlobalUpdateID)
        {
            List<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = null;

            try
            {
                var globalUpdatesUIElements = (from j in this._unitOfWork.Repository<FormDesignElementValue>()
                                                            .Query()
                                                            .Include(f => f.UIElement)
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID
                                                                && ((f.UIElement is TextBoxUIElement) || (f.UIElement is RadioButtonUIElement)
                                                                || (f.UIElement is DropDownUIElement) || (f.UIElement is CheckBoxUIElement)
                                                                || (f.UIElement is CalendarUIElement)))
                                                            .Get()
                                               join tb in this._unitOfWork.Repository<TextBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals tb.UIElementID
                                                into textboxElement
                                               from textboxlist in textboxElement.DefaultIfEmpty()
                                               join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                                on j.UIElement.UIElementID equals rd.UIElementID
                                                into radioElement
                                               from radiolist in radioElement.DefaultIfEmpty()
                                               join dd in this._unitOfWork.Repository<DropDownUIElement>().Get()
                                                on j.UIElement.UIElementID equals dd.UIElementID
                                                into dropdownElement
                                               from dropdownlist in dropdownElement.DefaultIfEmpty()
                                               join cb in this._unitOfWork.Repository<CheckBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals cb.UIElementID
                                                into checkboxElement
                                               from checkboxlist in checkboxElement.DefaultIfEmpty()
                                               join cal in this._unitOfWork.Repository<CalendarUIElement>().Get()
                                                on j.UIElement.UIElementID equals cal.UIElementID
                                                into calendarElement
                                               from calendarlist in calendarElement.DefaultIfEmpty()
                                               select new FormDesignElementValueVeiwModel
                                               {
                                                   FormDesignElementValueID = j.FormDesignElementValueID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   FormDesignID = j.FormDesignID,
                                                   FormDesignVersionID = j.FormDesignVersionID,
                                                   UIElementID = j.UIElementID,
                                                   ElementFullPath = j.ElementFullPath,
                                                   IsUpdated = j.IsUpdated,
                                                   NewValue = j.NewValue,
                                                   ElementHeaderName = j.ElementHeaderName,
                                                   FormDesignName = j.FormDesign.DisplayText,
                                                   Name = j.UIElement.GeneratedName,
                                                   UIElementTypeID = (textboxlist.UIElementTypeID != null) ? textboxlist.UIElementTypeID : (radiolist.UIElementTypeID != null) ? radiolist.UIElementTypeID : (dropdownlist.UIElementTypeID != null) ? dropdownlist.UIElementTypeID : (checkboxlist.UIElementTypeID != null) ? checkboxlist.UIElementTypeID : (calendarlist.UIElementTypeID != null) ? calendarlist.UIElementTypeID : ElementTypes.BLANKID,
                                                   UIElementName = j.UIElement.UIElementName,
                                                   Label = j.ElementHeaderName,
                                                   OptionLabel = (radiolist.OptionLabel != null) ? radiolist.OptionLabel.ToString() : null,
                                                   OptionLabelNo = (radiolist.OptionLabelNo != null) ? radiolist.OptionLabelNo.ToString() : null,
                                                   ItemData = null
                                               });
                if (globalUpdatesUIElements != null)
                {
                    globalUpdatesUIElementsList = globalUpdatesUIElements.GroupBy(xy => xy.UIElementID).Select(xy => xy.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesUIElementsList;
        }

        public ServiceResult SaveIASElementExportDataValues(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy)
        {
            ServiceResult serviceResult = null;
            try
            {
                serviceResult = new ServiceResult();
                IEnumerable<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = GetFormDesignVersionUIElements(GlobalUpdateID);
                if (globalUpdatesUIElementsList != null && globalUpdatesUIElementsList.Count() > 0)
                {
                    serviceResult = this.IASElementExportData(GlobalUpdateID, IASFolderDataList, addedBy);
                }
            }
            catch (Exception ex)
            {
                //if error occured then update status to 'IAS Generation Failed'
                GlobalUpdate updatestatus = this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(gu => gu.GlobalUpdateID == GlobalUpdateID).Get().FirstOrDefault();
                if (updatestatus.GlobalUpdateStatusID == 3)
                {
                    updatestatus.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_IASGENERATIONFAILED;
                    this._unitOfWork.Save();
                }
                serviceResult = ex.ExceptionMessages();
                serviceResult.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return serviceResult;
        }

        private ServiceResult IASElementExportData(int GlobalUpdateID, IEnumerable<IASFolderDataModel> IASFolderDataList, string addedBy)
        {
            var result = new ServiceResult();
            List<FormDesignElementValueVeiwModel> filter = null;
            int f_FormDesignID = 0;
            int FormDesignVersionID = 0;
            List<GuRuleRowModel> rulesFilter = null;
            IEnumerable<IASFolderDataModel> IASFolderDataLists = IASFolderDataList.OrderBy(f=>f.FormDesignID).ToList();
            //DocumentMatch doc = new DocumentMatch(this._unitOfWork);
            foreach (var IASFolderData in IASFolderDataLists)
            {
                int formInstanceId = Convert.ToInt32(IASFolderData.FormInstanceID);
                int IASFolderMapID = Convert.ToInt32(IASFolderData.IASFolderMapID);
                FormInstanceDataMap formInstanceDataMap = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormInstance)
                                                                        .Filter(c => c.FormInstanceID == formInstanceId)
                                                                        .Get().FirstOrDefault();
                if (formInstanceDataMap != null)
                {
                    //var filter = GetFormDesignVersionUIElements(GlobalUpdateID);
                    //var filter = GetFormDesignVersionElementSelection(GlobalUpdateID, Convert.ToInt32(formInstanceDataMap.FormInstance.FormDesignID));
                    if (formInstanceDataMap.FormInstance.FormDesignID != f_FormDesignID)//1
                    {
                        filter = new List<FormDesignElementValueVeiwModel>();
                        filter = GetFormDesignVersionElementSelection(GlobalUpdateID, Convert.ToInt32(formInstanceDataMap.FormInstance.FormDesignID));
                        f_FormDesignID = formInstanceDataMap.FormInstance.FormDesignID;
                        FormDesignVersionID = 0;
                    }

                    if (FormDesignVersionID == 0)//2
                    {
                        rulesFilter = new List<GuRuleRowModel>();
                        foreach (var uiElement in filter)
                        {
                            FormDesignVersionID = Convert.ToInt32(uiElement.FormDesignVersionID);
                            formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                            IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, uiElement.UIElementID, GlobalUpdateID);
                            if (uiElementRules != null)
                            {
                                rulesFilter.AddRange(uiElementRules.ToList());
                            }
                        }

                        if (rulesFilter != null && rulesFilter.Count() > 0)
                        {
                            foreach (var rule in rulesFilter)
                            {
                                //clean up the null expression for now
                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                {
                                    rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                                }
                                rule.UIElementName = GetGeneratedNameFromID(rule.UIElementID);
                                rule.UIElementFullName = GetFullNameFromID(rule.UIElementID);
                                rule.UIElementFormName = GetFormNameFromID(rule.UIElementID);
                                rule.UIElementTypeID = GetUIElementType(rule.UIElementID);
                                if (rule.IsResultSuccessElement == true)
                                {
                                    rule.SuccessValueFullName = GetFullNameFromName(rule.ResultSuccess);
                                }
                                if (rule.IsResultFailureElement == true)
                                {
                                    rule.FailureValueFullName = GetFullNameFromName(rule.ResultFailure);
                                }

                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                {
                                    foreach (var exp in rule.Expressions)
                                    {
                                        if (exp != null)
                                        {
                                            exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                            if (exp.IsRightOperandElement == true)
                                            {
                                                exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                            }
                                        }
                                    }
                                }
                                rule.IsParentRepeater = IsParentRepeater(rule.UIElementID);
                            }
                        }
                    }
                    //doc.IsMatch(GlobalUpdateID, IASFolderMapID, formInstanceId, addedBy, formInstanceDataMap.FormData, filter, rulesFilter);
                }
            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        private List<FormDesignElementValueVeiwModel> GetFormDesignVersionElementSelection(int GlobalUpdateID, int FormDesignID)
        {
            List<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = null;

            try
            {
                var globalUpdatesUIElements = (from j in this._unitOfWork.Repository<FormDesignElementValue>()
                                                            .Query()
                                                            .Include(f => f.UIElement)
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.FormDesignID == FormDesignID
                                                                && ((f.UIElement is TextBoxUIElement) || (f.UIElement is RadioButtonUIElement)
                                                                || (f.UIElement is DropDownUIElement) || (f.UIElement is CheckBoxUIElement)
                                                                || (f.UIElement is CalendarUIElement)))
                                                            .Get()
                                               join tb in this._unitOfWork.Repository<TextBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals tb.UIElementID
                                                into textboxElement
                                               from textboxlist in textboxElement.DefaultIfEmpty()
                                               join rd in this._unitOfWork.Repository<RadioButtonUIElement>().Get()
                                                on j.UIElement.UIElementID equals rd.UIElementID
                                                into radioElement
                                               from radiolist in radioElement.DefaultIfEmpty()
                                               join dd in this._unitOfWork.Repository<DropDownUIElement>().Get()
                                                on j.UIElement.UIElementID equals dd.UIElementID
                                                into dropdownElement
                                               from dropdownlist in dropdownElement.DefaultIfEmpty()
                                               join cb in this._unitOfWork.Repository<CheckBoxUIElement>().Get()
                                                on j.UIElement.UIElementID equals cb.UIElementID
                                                into checkboxElement
                                               from checkboxlist in checkboxElement.DefaultIfEmpty()
                                               join cal in this._unitOfWork.Repository<CalendarUIElement>().Get()
                                                on j.UIElement.UIElementID equals cal.UIElementID
                                                into calendarElement
                                               from calendarlist in calendarElement.DefaultIfEmpty()
                                               select new FormDesignElementValueVeiwModel
                                               {
                                                   FormDesignElementValueID = j.FormDesignElementValueID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   FormDesignID = j.FormDesignID,
                                                   FormDesignVersionID = j.FormDesignVersionID,
                                                   UIElementID = j.UIElementID,
                                                   ElementFullPath = j.ElementFullPath,
                                                   IsUpdated = j.IsUpdated,
                                                   NewValue = j.NewValue,
                                                   ElementHeaderName = j.ElementHeaderName,
                                                   FormDesignName = j.FormDesign.DisplayText,
                                                   Name = j.UIElement.GeneratedName,
                                                   UIElementTypeID = (textboxlist.UIElementTypeID != null) ? textboxlist.UIElementTypeID : (radiolist.UIElementTypeID != null) ? radiolist.UIElementTypeID : (dropdownlist.UIElementTypeID != null) ? dropdownlist.UIElementTypeID : (checkboxlist.UIElementTypeID != null) ? checkboxlist.UIElementTypeID : (calendarlist.UIElementTypeID != null) ? calendarlist.UIElementTypeID : ElementTypes.BLANKID,
                                                   UIElementName = j.UIElement.UIElementName,
                                                   Label = j.ElementHeaderName,
                                                   OptionLabel = (radiolist.OptionLabel != null) ? radiolist.OptionLabel.ToString() : null,
                                                   OptionLabelNo = (radiolist.OptionLabelNo != null) ? radiolist.OptionLabelNo.ToString() : null,
                                                   IsParentRepeater = (j.UIElement.UIElement2 is RepeaterUIElement) ? true : false
                                               });
                if (globalUpdatesUIElements != null)
                {
                    globalUpdatesUIElementsList = globalUpdatesUIElements.GroupBy(xy => xy.UIElementID).Select(xy => xy.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesUIElementsList;
        }

        public ServiceResult AddIASTemplate(IASFileUploadViewModel viewModel)
        {
            ServiceResult result = null;
            try
            {
                if (viewModel != null)
                {
                    IASFileUpload template = new IASFileUpload();
                    template.GlobalUpdateID = viewModel.GlobalUpdateID;
                    template.FileName = viewModel.FileName;
                    template.FileExtension = viewModel.FileExtension;
                    template.TemplateGuid = viewModel.TemplateGuid;
                    template.AddedBy = viewModel.AddedBy;
                    template.AddedDate = DateTime.Now;
                    template.UpdatedBy = null;
                    template.UpdatedDate = null;

                    this._unitOfWork.Repository<IASFileUpload>().Insert(template);
                    this._unitOfWork.Save();

                    result = new ServiceResult();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result = new ServiceResult();
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceResult UpdateValue(int tenantId, string userName, string elementHeader, int globalUpdateId, int formDesignVersionId, int uiElementId, int uiElementDataTypeId, bool modifyRules, IEnumerable<GuRuleRowModel> rules, bool IsPropertyGridModified)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (IsPropertyGridModified == true && modifyRules == false && rules == null)
                {
                    UpdateElementNewValue(elementHeader, userName, null, globalUpdateId, uiElementId, formDesignVersionId);
                }
                else
                {
                    if (modifyRules == true && rules != null)
                    {
                        UpdateElementNewValue(elementHeader, userName, rules.FirstOrDefault().ResultSuccess, globalUpdateId, uiElementId, formDesignVersionId);
                        ChangeRules(userName, tenantId, formDesignVersionId, uiElementId, globalUpdateId, rules, elementHeader);

                    }

                    else if (modifyRules == true && rules == null)
                    {
                        UpdateElementNewValue(elementHeader, userName, null, globalUpdateId, uiElementId, formDesignVersionId);
                        IEnumerable<GuRuleRowModel> currentRules = GetRulesForUIElement(tenantId, formDesignVersionId, uiElementId, globalUpdateId);
                        if (currentRules != null || currentRules.Count() > 0)
                        {
                            var delRules = from del in currentRules select del;
                            if (delRules != null)
                            {
                                foreach (var delRule in delRules)
                                {
                                    DeleteRule(delRule);
                                }
                            }
                        }
                    }
                }
                UpdateGlobalUpdateWizardStep(globalUpdateId, (int)tmg.equinox.domain.entities.Enums.IASWizardSteps.UpdateSelection, userName);
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

        public IEnumerable<GuRuleRowModel> GetRulesForUIElement(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId)
        {
            IList<GuRuleRowModel> rowModelList = null;
            //call new function
            rowModelList = GetRulesForUIElementHierarchical(tenantId, formDesignVersionId, uiElementId, globalUpdateId);
            if (rowModelList == null)
            {
                try
                {
                    //Get all the rules along with expression for a uielement
                    rowModelList = (from r in this._unitOfWork.RepositoryAsync<RuleGu>()
                                                      .Query()
                                                      .Include(c => c.ExpressionsGu)
                                                      .Include(c => c.TargetProperty)
                                                      .Get()

                                    where r.UIElementID == uiElementId && r.GlobalUpdateID == globalUpdateId

                                    select new GuRuleRowModel
                                    {
                                        Expressions = (from exp in r.ExpressionsGu
                                                       select new ExpressionRowModel
                                                       {
                                                           ExpressionId = exp.ExpressionID,
                                                           LeftOperand = exp.LeftOperand,
                                                           LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                           OperatorTypeId = exp.OperatorTypeID,
                                                           RightOperand = exp.RightOperand,
                                                           RuleId = exp.RuleID,
                                                           ExpressionTypeId = exp.ExpressionTypeID,
                                                           IsRightOperandElement = exp.IsRightOperandElement,
                                                           TenantId = tenantId
                                                       }).AsEnumerable(),
                                        ResultFailure = r.ResultFailure,
                                        ResultSuccess = r.ResultSuccess,
                                        IsResultFailureElement = r.IsResultFailureElement,
                                        IsResultSuccessElement = r.IsResultSuccessElement,
                                        Message = r.Message,
                                        RuleId = r.RuleID,
                                        TargetPropertyId = r.TargetPropertyID,
                                        TargetProperty = r.TargetProperty.TargetPropertyName,
                                        UIElementID = r.UIElementID
                                    }).ToList();
                    if (rowModelList.Count() == 0)
                        rowModelList = null;
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                }
            }
            return rowModelList;
        }

        private string GetGeneratedNameFromID(int elementID)
        {
            string generatedName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            generatedName = element.GeneratedName;
            return generatedName;
        }

        private string GetFullNameFromID(int elementID)
        {
            string fullName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            fullName = GetFullName(element);
            return fullName;
        }

        private string GetFormNameFromID(int elementID)
        {
            string formName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            formName = element.UIElementName;
            return formName;
        }

        private int GetUIElementType(int elementID)
        {
            int elementTypeId = 0;
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            if (element != null)
            {
                var props = element.GetType().GetProperties();
                var prop = from pro in props where pro.Name == "UIElementTypeID" select pro;
                if (prop != null && prop.Count() > 0)
                {
                    elementTypeId = (int)prop.First().GetValue(element);
                }
            }

            return elementTypeId;
        }

        private string GetFullNameFromName(string elementName)
        {
            string fullName = "";
            if (!String.IsNullOrEmpty(elementName))
            {
                UIElement element = (from elem in formElementList
                                     where elem.UIElementName == elementName
                                     select elem).FirstOrDefault();
                fullName = GetFullName(element);

            }
            return fullName;
        }

        private string GetFullName(UIElement element)
        {
            string fullName = "";
            if (element != null)
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
            return fullName;
        }

        private bool IsParentRepeater(int elementID)
        {
            bool isParentRepeater = false;
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            if (element != null)
            {
                if (element.UIElement2 is RepeaterUIElement)
                {
                    isParentRepeater = true;
                }
            }

            return isParentRepeater;
        }

        public ElementHeaderViewModel ConfirmedUpdateValueNotification(int uiElementId, int globalUpdateId, int formDesignVersionId)
        {
            ElementHeaderViewModel confirmationHeader = new ElementHeaderViewModel();
            try
            {
                List<string> validationElements = new List<string>();
                UIElement uiElement = (from element in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.UIElementID == uiElementId).Get() select element).FirstOrDefault();

                var uiElementRules = (from rule in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get() where rule.UIElementID == uiElementId select rule).Count();
                var uiElementDataSources = (from dataSource in this._unitOfWork.RepositoryAsync<DataSourceMapping>().Get() where dataSource.UIElementID == uiElementId select dataSource).Count();
                var uiElementValidatiors = (from validators in this._unitOfWork.RepositoryAsync<Validator>().Get() where validators.UIElementID == uiElementId select validators).Count();
                //var cascadingRules = (from uiElements in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.UIElementID == uiElementId).Get()
                //                      from expressions in this._unitOfWork.RepositoryAsync<Expression>().Query().Filter(x => (uiElements.UIElementName == x.LeftOperand) || (x.IsRightOperandElement == true && uiElements.UIElementName == x.RightOperand)).Get()
                //                      select uiElements.UIElementID).Count();

                var cascadingRules = (from expressions in this._unitOfWork.RepositoryAsync<Expression>().Query().Filter(x => (x.LeftOperand== uiElement.UIElementName) || (x.IsRightOperandElement == true && uiElement.UIElementName == x.RightOperand)).Get()
                                      select expressions.ExpressionID).Count();


                confirmationHeader.ElementHeaderText = (from guUpdate in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Get()
                                                        where guUpdate.GlobalUpdateID == globalUpdateId && guUpdate.FormDesignVersionID == formDesignVersionId && guUpdate.UIElementID == uiElementId
                                                        select guUpdate.ElementHeaderName).FirstOrDefault();

                if (uiElementRules > 0)
                {
                    confirmationHeader.HasRule = true;
                    validationElements.Add("Rules");
                }

                if (uiElementDataSources > 0)
                {
                    confirmationHeader.HasDataSource = true;
                    validationElements.Add("DataSource");
                }

                if (uiElementValidatiors > 0)
                {
                    confirmationHeader.HasValidation = true;
                    validationElements.Add("Validation");
                }
                if (cascadingRules > 0)
                {
                    confirmationHeader.HasCascadingRules = true;
                    validationElements.Add("Dependent Rules");
                }

                confirmationHeader.ValidationMessage = "Updating Value for " + confirmationHeader.ElementHeaderText + " might affect the associated " + string.Join(" , ", validationElements) + " Do you want to continue?";
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return confirmationHeader;
        }

        public IEnumerable<DocumentVersionUIElementRowModel> GetUpdateSectionUIElements(int tenantId, int formDesignVersionId)
        {
            IList<DocumentVersionUIElementRowModel> uiElementRowModelList = null;
            List<int> keyUIElemets = new List<int>();
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();


                if (elementList.Count() > 0)
                {
                    uiElementRowModelList = (from i in elementList
                                             select new DocumentVersionUIElementRowModel
                                             {
                                                 DataType = i.ApplicationDataType.ApplicationDataTypeName,
                                                 ElementType = GetElementType(i),
                                                 Label = i.Label == null ? "[Blank]" : i.Label,
                                                 level = i.ParentUIElementID.HasValue ? GetRowLevel(i.ParentUIElementID, elementList) : 0,
                                                 UIElementID = i.UIElementID,
                                                 UIElementName = i.UIElementName,
                                                 parent = i.ParentUIElementID.HasValue ? i.ParentUIElementID.Value.ToString() : "0",
                                                 isLeaf = i.ParentUIElementID.HasValue ? IsLeafRow(i.UIElementID, elementList) : false,
                                             }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }

        private void ChangeRules(string userName, int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId, IEnumerable<GuRuleRowModel> newRules, string elementHeader)
        {
            IEnumerable<GuRuleRowModel> currentRules = GetRulesForUIElementHierarchical(tenantId, formDesignVersionId, uiElementId, globalUpdateId);
            if ((newRules == null || newRules.Count() == 0) && (currentRules == null || currentRules.Count() == 0))
            {
                return;
            }
            if (currentRules == null)
            {
                currentRules = new List<GuRuleRowModel>();
            }

            //delete rules that are not returned
            List<int> newRuleIDs = (from newRule in newRules select newRule.RuleId).ToList();
            var delRules = from del in currentRules where !newRuleIDs.Contains(del.RuleId) select del;
            if (delRules != null)
            {
                foreach (var delRule in delRules)
                {
                    DeleteRule(delRule);

                }
            }

            foreach (GuRuleRowModel newRule in newRules)
            {

                var currentRule = (from c in currentRules where c.RuleId == newRule.RuleId select c).FirstOrDefault();
                if (currentRule != null && currentRule.RuleId == newRule.RuleId)
                {
                    //update rule
                    UpdateRule(userName, newRule);
                }
                else
                {
                    //add new rule
                    AddRule(userName, uiElementId, newRule, globalUpdateId);
                }
            }
        }

        private void AddRule(string userName, int uiElementID, GuRuleRowModel model, int globalUpdateId)
        {
            RuleGu rule = new RuleGu();
            rule.AddedBy = userName;
            rule.AddedDate = DateTime.Now;
            rule.RuleName = "RULE";
            rule.RuleTargetTypeID = 1;
            rule.ResultFailure = model.ResultFailure;
            rule.ResultSuccess = model.ResultSuccess;
            rule.IsResultFailureElement = model.IsResultFailureElement;
            rule.IsResultSuccessElement = model.IsResultSuccessElement;
            rule.Message = model.Message;
            rule.UIElementID = uiElementID;
            rule.GlobalUpdateID = globalUpdateId;
            rule.TargetPropertyID = model.TargetPropertyId;

            this._unitOfWork.RepositoryAsync<RuleGu>().Insert(rule);
            this._unitOfWork.Save();

            if (model.RootExpression != null)
            {
                List<ExpressionRowModel> expressions = new List<ExpressionRowModel>();
                GetExpressions(model.RootExpression, null, ref expressions);
                expressions.Reverse();
                Dictionary<int, int> expressionIds = new Dictionary<int, int>();
                foreach (ExpressionRowModel expModel in expressions)
                {
                    ExpressionGu exp = new ExpressionGu();
                    exp.AddedBy = userName;
                    exp.AddedDate = DateTime.Now;
                    exp.LeftOperand = expModel.LeftOperand;
                    exp.LogicalOperatorTypeID = expModel.LogicalOperatorTypeId > 0 ? expModel.LogicalOperatorTypeId : 1; //default to 1
                    exp.OperatorTypeID = expModel.OperatorTypeId > 0 ? expModel.OperatorTypeId : 1; //default to 1
                    exp.RightOperand = expModel.RightOperand;
                    exp.ExpressionTypeID = expModel.ExpressionTypeId;
                    exp.IsRightOperandElement = expModel.IsRightOperandElement;
                    exp.RuleID = rule.RuleID;
                    int previousExpressionId = expModel.ExpressionId;
                    if (expModel.ParentExpressionId.HasValue == true)
                    {
                        exp.ParentExpressionID = expressionIds[expModel.ParentExpressionId.Value];
                    }
                    this._unitOfWork.RepositoryAsync<ExpressionGu>().Insert(exp);
                    this._unitOfWork.Save();
                    expressionIds.Add(previousExpressionId, exp.ExpressionID);
                }
            }
        }

        private void DeleteRule(GuRuleRowModel rule)
        {
            List<ExpressionRowModel> expressions = new List<ExpressionRowModel>();
            if (rule.RootExpression != null)
            {
                GetExpressions(rule.RootExpression, null, ref expressions);
                foreach (ExpressionRowModel expression in expressions)
                {
                    this._unitOfWork.RepositoryAsync<ExpressionGu>().Delete(expression.ExpressionId);
                    this._unitOfWork.Save();
                }
            }
            this._unitOfWork.RepositoryAsync<RuleGu>().Delete(rule.RuleId);
            this._unitOfWork.Save();
        }

        private void GetExpressions(ExpressionRowModel expression, Nullable<int> parentExpressionId, ref List<ExpressionRowModel> expressions)
        {
            if (expression != null)
            {
                if (expression.Expressions != null && expression.Expressions.Count() > 0)
                {
                    foreach (ExpressionRowModel model in expression.Expressions)
                    {
                        GetExpressions(model, expression.ExpressionId, ref expressions);
                    }
                }
                expression.ParentExpressionId = parentExpressionId;
                expressions.Add(expression);
            }
        }

        private void UpdateRule(string userName, GuRuleRowModel rule)
        {
            RuleGu ruleToUpdate = this._unitOfWork.RepositoryAsync<RuleGu>().FindById(rule.RuleId);
            ruleToUpdate.UpdatedBy = userName;
            ruleToUpdate.UpdatedDate = DateTime.Now;
            ruleToUpdate.ResultFailure = rule.ResultFailure;
            ruleToUpdate.ResultSuccess = rule.ResultSuccess;
            ruleToUpdate.IsResultFailureElement = rule.IsResultFailureElement;
            ruleToUpdate.IsResultSuccessElement = rule.IsResultSuccessElement;
            ruleToUpdate.Message = rule.Message;
            ruleToUpdate.UIElementID = rule.UIElementID;
            ruleToUpdate.TargetPropertyID = rule.TargetPropertyId;
            this._unitOfWork.RepositoryAsync<RuleGu>().Update(ruleToUpdate);
            this._unitOfWork.Save();


            var expressions = this._unitOfWork.RepositoryAsync<ExpressionGu>().Query()
                                                                          .Filter(c => c.RuleID == rule.RuleId)
                                                                          .Get();

            if (rule.RootExpression != null)
            {
                List<ExpressionRowModel> newExpressions = new List<ExpressionRowModel>();
                GetExpressions(rule.RootExpression, null, ref newExpressions);
                if (expressions != null)
                {
                    List<int> newExpIDs = (from exp in newExpressions select exp.ExpressionId).ToList();
                    var delExpressions = from del in expressions where !newExpIDs.Contains(del.ExpressionID) orderby del.ExpressionID descending select del;
                    if (delExpressions != null)
                    {
                        foreach (var delExp in delExpressions)
                        {
                            this._unitOfWork.RepositoryAsync<ExpressionGu>().Delete(delExp.ExpressionID);
                        }
                        this._unitOfWork.Save();
                    }
                }
                newExpressions.Reverse();
                Dictionary<int, int> expressionIds = new Dictionary<int, int>();
                foreach (var exp in newExpressions)
                {
                    ExpressionGu updateExp = (from e in expressions where e.ExpressionID == exp.ExpressionId select e).FirstOrDefault();
                    if (updateExp != null && updateExp.ExpressionID == exp.ExpressionId)
                    {
                        updateExp.UpdatedBy = userName;
                        updateExp.UpdatedDate = DateTime.Now;
                        updateExp.LeftOperand = exp.LeftOperand;
                        updateExp.RightOperand = exp.RightOperand;
                        updateExp.LogicalOperatorTypeID = exp.LogicalOperatorTypeId > 0 ? exp.LogicalOperatorTypeId : 1; //default to 1
                        updateExp.OperatorTypeID = exp.OperatorTypeId > 0 ? exp.OperatorTypeId : 1; //default to 1
                        updateExp.ExpressionTypeID = exp.ExpressionTypeId;
                        updateExp.IsRightOperandElement = exp.IsRightOperandElement;
                        this._unitOfWork.RepositoryAsync<ExpressionGu>().Update(updateExp);
                        this._unitOfWork.Save();
                    }
                    else
                    {
                        ExpressionGu newExp = new ExpressionGu();
                        newExp.AddedBy = userName;
                        newExp.AddedDate = DateTime.Now;
                        newExp.LeftOperand = exp.LeftOperand;
                        newExp.LogicalOperatorTypeID = exp.LogicalOperatorTypeId > 0 ? exp.LogicalOperatorTypeId : 1; //default to 1
                        newExp.OperatorTypeID = exp.OperatorTypeId > 0 ? exp.OperatorTypeId : 1; //default to 1
                        newExp.RightOperand = exp.RightOperand;
                        newExp.ExpressionTypeID = exp.ExpressionTypeId;
                        newExp.IsRightOperandElement = exp.IsRightOperandElement;
                        newExp.RuleID = rule.RuleId;
                        int previousExpressionId = exp.ExpressionId;
                        if (exp.ParentExpressionId.HasValue == true)
                        {
                            if (exp.ParentExpressionId.Value > -1)
                            {
                                newExp.ParentExpressionID = exp.ParentExpressionId;
                            }
                            else
                            {
                                newExp.ParentExpressionID = expressionIds[exp.ParentExpressionId.Value];
                            }
                        }

                        this._unitOfWork.RepositoryAsync<ExpressionGu>().Insert(newExp);
                        this._unitOfWork.Save();
                        expressionIds.Add(previousExpressionId, newExp.ExpressionID);
                    }
                }
            }
            //if expression is null the delete the existing expressions for Rule if any.
            else
            {
                if (expressions != null)
                {
                    foreach (var delExp in expressions)
                    {
                        this._unitOfWork.RepositoryAsync<ExpressionGu>().Delete(delExp.ExpressionID);
                    }
                    this._unitOfWork.Save();
                }

            }
        }

        private IList<GuRuleRowModel> GetRulesForUIElementHierarchical(int tenantId, int formDesignVersionId, int uiElementId, int globalUpdateId)
        {

            //  int globalUpdateId = (from s in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Query().Filter(x => x.FormDesignVersionID == formDesignVersionId && x.UIElementID == uiElementId).Get()
            //                       select s.GlobalUpdateID).FirstOrDefault();

            IList<GuRuleRowModel> rowModelList = null;
            try
            {
                //Get all the rules along with expression for a uielement
                rowModelList = (from r in this._unitOfWork.RepositoryAsync<RuleGu>()
                                                           .Query()
                                                           .Include(c => c.ExpressionsGu)
                                                           .Include(c => c.TargetProperty)
                                                           .Get()
                                where r.UIElementID == uiElementId && r.GlobalUpdateID == globalUpdateId

                                select new GuRuleRowModel
                                {
                                    Expressions = (from exp in r.ExpressionsGu
                                                   select new ExpressionRowModel
                                                   {
                                                       ExpressionId = exp.ExpressionID,
                                                       LeftOperand = exp.LeftOperand,
                                                       LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                       OperatorTypeId = exp.OperatorTypeID,
                                                       RightOperand = exp.RightOperand,
                                                       RuleId = exp.RuleID,
                                                       TenantId = tenantId,
                                                       ParentExpressionId = exp.ParentExpressionID,
                                                       ExpressionTypeId = exp.ExpressionTypeID,
                                                       IsRightOperandElement = exp.IsRightOperandElement
                                                   }).AsEnumerable(),
                                    ResultFailure = r.ResultFailure,
                                    ResultSuccess = r.ResultSuccess,
                                    IsResultFailureElement = r.IsResultFailureElement,
                                    IsResultSuccessElement = r.IsResultSuccessElement,
                                    Message = r.Message,
                                    RuleId = r.RuleID,
                                    TargetPropertyId = r.TargetPropertyID,
                                    TargetProperty = r.TargetProperty.TargetPropertyName,
                                    UIElementID = r.UIElementID
                                }).ToList();
                if (rowModelList.Count() == 0)
                {
                    rowModelList = null;
                }
                else
                {
                    foreach (var rowModel in rowModelList)
                    {
                        if (rowModel.Expressions != null && rowModel.Expressions.Count() > 0)
                        {
                            rowModel.RootExpression = GenerateHierarchicalExpression(rowModel.Expressions.ToList());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;

            //-----------------------Initial --------------------------- 
        }

        private ExpressionRowModel GenerateHierarchicalExpression(List<ExpressionRowModel> expressionList)
        {
            var expression = expressionList.Where(n => n.ParentExpressionId == null).FirstOrDefault();
            if (expression != null)
            {
                GenerateParentExpression(expressionList, ref expression);
            }
            return expression;
        }

        private void GenerateParentExpression(List<ExpressionRowModel> expressionList, ref ExpressionRowModel parentExpression)
        {
            var parent = parentExpression;
            var childExpressions = from exp in expressionList where exp.ParentExpressionId == parent.ExpressionId select exp;
            if (childExpressions != null && childExpressions.Count() > 0)
            {
                parentExpression.Expressions = new List<ExpressionRowModel>();
                foreach (var childExpression in childExpressions)
                {
                    ExpressionRowModel child = childExpression;
                    GenerateParentExpression(expressionList, ref child);
                    parentExpression.Expressions.Add(childExpression);
                }
            }
        }

        private void UpdateElementNewValue(string elementHeader, string userName, string newValue, int globalUpdateId, int uiElementId, int formDesignVersionId)
        {
            FormDesignElementValue elementSelection = (from element in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Get()
                                                       where element.GlobalUpdateID == globalUpdateId && element.FormDesignVersionID == formDesignVersionId
                                                       && element.UIElementID == uiElementId
                                                       select element
                                              ).FirstOrDefault();
            elementSelection.NewValue = newValue;
            elementSelection.ElementHeaderName = elementHeader;
            elementSelection.IsUpdated = true;
            elementSelection.UpdatedDate = DateTime.Now;
            elementSelection.UpdatedBy = userName;
            this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Update(elementSelection);
            this._unitOfWork.Save();
        }
        private List<UIElement> GetHierarchicalParentUIElement(List<int> selectedUIElement, List<UIElement> formDesignUIelements)
        {
            // List<KeyValuePair<int?, int?>> childParentUIElements = new List<KeyValuePair<int?, int?>>();
            List<int?> uiElementstoDisplay = new List<int?>();
            int? parentUIElementID;
            int? childUIElementID;
            foreach (var uiElementId in selectedUIElement)
            {
                childUIElementID = uiElementId;
                //childParentUIElements.Add(new KeyValuePair<int?, int?>(uiElementId,childUIElementID));

                // uiElementstoDisplay.Add(childUIElementID);

                while (childUIElementID != null)
                {
                    parentUIElementID = (from elem in formDesignUIelements
                                         where elem.UIElementID == childUIElementID
                                         select elem.ParentUIElementID).FirstOrDefault();

                    if (parentUIElementID > 0)
                    {

                        if (uiElementstoDisplay.Any(x => x == parentUIElementID))
                        {
                            childUIElementID = null;
                        }

                        else
                        {
                            uiElementstoDisplay.Add(parentUIElementID);
                            childUIElementID = parentUIElementID;
                        }
                    }
                    else
                    {
                        childUIElementID = null;
                    }
                }
            }
            uiElementstoDisplay.AddRange(selectedUIElement.Select(x => (int?)x));
            formDesignUIelements = (from uiElements in formDesignUIelements where uiElementstoDisplay.Any(x => x == uiElements.UIElementID) select uiElements).ToList();

            return formDesignUIelements;
        }
        public ServiceResult SaveBatch(string batchName, string executionType, DateTime? scheduleDate, TimeSpan? scheduledTime, DateTime addedDate, string addedBy, List<int> globalUpdateIDArray, int thresholdLimit)
        {
            var result = new ServiceResult();
            result.Result = ServiceResultStatus.Success;
            if (executionType == GlobalVariables.GB_RealtimeExecutionType)
            {
                scheduleDate = null;
                scheduledTime = null;
            }
            Batch batch = new Batch();
            BatchIASMap batchIASMap = new BatchIASMap();
            var listGUIds = globalUpdateIDArray;
            try
            {
                if (executionType == GlobalUpdateExecutionType.Realtime.ToString())
                {
                    //int FormDesignVerionsCount = (from fdv in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Query().Filter(x => globalUpdateIDArray.Contains(x.GlobalUpdateID))
                    //                              .Get()
                    //                              select fdv.FormDesignVersionID
                    //                             ).Distinct().Count();

                    int formInstanceCount = (from fldrMap in this._unitOfWork.RepositoryAsync<IASFolderMap>().Query().Get()
                                             join formIns in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get()
                                             on fldrMap.FormInstanceID equals formIns.FormInstanceID
                                             //join iasEleImp in this._unitOfWork.RepositoryAsync<IASElementImport>().Query().Filter(x => x.AcceptChange == true).Get()
                                             join iasEleImp in this._unitOfWork.RepositoryAsync<IASElementImport>().Query().Get()
                                             on fldrMap.IASFolderMapID equals iasEleImp.IASFolderMapID
                                             where globalUpdateIDArray.Contains(fldrMap.GlobalUpdateID)
                                             select fldrMap.FormInstanceID)
                                             .ToList()
                                             .Distinct()
                                             .Count();

                    if (formInstanceCount > thresholdLimit)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                        {
                            Messages = new string[] { "Realtime execution threshold limit exceeded. Please change Execution Type to Scheduled!!" }
                        });
                    }
                }

                if (result.Result == ServiceResultStatus.Success)
                {

                    List<BatchViewModel> listBatches = new List<BatchViewModel>();
                    listBatches = (from btch in this._unitOfWork.Repository<Batch>()
                                              .Query()
                                              .Get()
                                   select new BatchViewModel
                                   {
                                       BatchID = btch.BatchID,
                                       BatchName = btch.BatchName
                                   }
                                              ).ToList();

                    if (!(listBatches.Select(x => x.BatchName).Contains(batchName)))
                    {
                        Guid batchId = Guid.NewGuid();
                        batch.BatchID = batchId;
                        batch.AddedBy = addedBy;
                        batch.AddedDate = addedDate;
                        batch.BatchName = batchName;
                        batch.ExecutionType = executionType;
                        batch.ScheduleDate = scheduleDate;
                        batch.ScheduledTime = scheduledTime;
                        batch.IsApproved = false;
                        //batch.ApprovedBy = "GU";
                        this._unitOfWork.Repository<Batch>().Insert(batch);
                        this._unitOfWork.Save();
                        foreach (var globalUpdateID in listGUIds)
                        {
                            batchIASMap.GlobalUpdateID = globalUpdateID;
                            batchIASMap.BatchID = batchId;
                            this._unitOfWork.Repository<BatchIASMap>().Insert(batchIASMap);
                            this._unitOfWork.Save();
                        }

                        //Update Global Update Status on Batch Generation
                        var globalUpdates = this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => listGUIds.Contains(x.GlobalUpdateID)).Get().ToList();
                        globalUpdates.ForEach(x => { x.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_IASSCHEDULEDEXECUTION; x.UpdatedDate = DateTime.Now; x.UpdatedBy = addedBy; });
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                        {
                            Messages = new string[] { "There is already an entry of Batch with the provided Batch Name!!" }
                        });
                    }

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
        public ServiceResult CheckDuplicateFolderVersionExistsInSelectedBatchIAS(List<int> globalUpdateIDArray)
        {
            var result = new ServiceResult();
            result.Result = ServiceResultStatus.Success;
            try 
            {
                List<int> folderIds = new List<int>();
                foreach (var guId in globalUpdateIDArray)
                {
                    var folderId = this._unitOfWork.RepositoryAsync<IASFolderMap>().Query().Filter(g => g.GlobalUpdateID == guId).Get()
                        .Select(y => new { FolderID = y.FolderID, GlobalUpdateID = y.GlobalUpdateID })
                        .ToList().Distinct();
                    foreach (var fid in folderId)
                    {
                        folderIds.Add(fid.FolderID);
                    }
                }

                var checkFolderVersionCount = folderIds.GroupBy(k => k)
                                .Where(g => g.Count() > 1)
                                .Count();
                if (checkFolderVersionCount > 1)
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
        public ServiceResult UpdateBatch(Guid batchId, string batchName, string executionType, DateTime? scheduleDate, TimeSpan? scheduledTime, DateTime updatedDate, string updatedBy, List<int> globalUpdateIDArray, int thresholdLimit)
        {
            var result = new ServiceResult();
            result.Result = ServiceResultStatus.Success;
            //Batch batch = new Batch();
            BatchIASMap batchIASMap = new BatchIASMap();
            var listGUIds = globalUpdateIDArray;
            if (executionType == GlobalVariables.GB_RealtimeExecutionType)
            {
                scheduleDate = null;
                scheduledTime = null;
            }
            try
            {
                if (executionType == GlobalUpdateExecutionType.Realtime.ToString())
                {
                    //int FormDesignVerionsCount = (from fdv in this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Query().Filter(x => globalUpdateIDArray.Contains(x.GlobalUpdateID))
                    //                              .Get()
                    //                              select fdv.FormDesignVersionID
                    //                             ).Distinct().Count();

                    int formInstanceCount = (from fldrMap in this._unitOfWork.RepositoryAsync<IASFolderMap>().Query().Get()
                                             join formIns in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get()
                                             on fldrMap.FormInstanceID equals formIns.FormInstanceID
                                             //join iasEleImp in this._unitOfWork.RepositoryAsync<IASElementImport>().Query().Filter(x => x.AcceptChange == true).Get()
                                             join iasEleImp in this._unitOfWork.RepositoryAsync<IASElementImport>().Query().Get()
                                             on fldrMap.IASFolderMapID equals iasEleImp.IASFolderMapID
                                             where globalUpdateIDArray.Contains(fldrMap.GlobalUpdateID)
                                             select fldrMap.FormInstanceID)
                                             .ToList()
                                             .Distinct()
                                             .Count();

                    if (formInstanceCount > thresholdLimit)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                        {
                            Messages = new string[] { "Realtime execution threshold limit exceeded. Please change Execution Type to Scheduled!!" }
                        });
                    }
                }
                if (result.Result == ServiceResultStatus.Success)
                {
                    //Get entry for batch with given batchId
                    Batch batchTemp = this._unitOfWork.Repository<Batch>()
                         .Query()
                         .Filter(x => x.BatchID == batchId)
                         .Get()
                         .FirstOrDefault();
                    var batchNameFromObj = batchTemp.BatchName;

                    //Get all entries from BatchIASMap for corresponding BatchId in Batch table
                    var listBatchIASMaps = (from btch in this._unitOfWork.Repository<BatchIASMap>()
                                               .Query()
                                               .Filter(x => x.BatchID == batchId)
                                               .Get()
                                            select btch).ToList();

                    //Get global updates to reset globalUpdateStatusz
                    var unSelectedGlobalUpdateIds = (from iasBatchMap in listBatchIASMaps.Where(x => !globalUpdateIDArray.Contains(x.GlobalUpdateID))
                                                     join gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                                     on iasBatchMap.GlobalUpdateID equals gu.GlobalUpdateID
                                                     select gu.GlobalUpdateID).ToList();

                    var unSelectedGlobalUpdates = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => unSelectedGlobalUpdateIds.Contains(x.GlobalUpdateID))
                                                             .Get()
                                                   select gu).ToList();
                    unSelectedGlobalUpdates.ForEach(x => { x.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_COMPLETEID; x.UpdatedDate = DateTime.Now; x.UpdatedBy = updatedBy; });
                    this._unitOfWork.Save();

                    //Delete all entries for above batchId in BatchIASMap table
                    foreach (var IASMap in listBatchIASMaps)
                    {
                        // batchIASMapObj.BatchID = IASMap;
                        this._unitOfWork.Repository<BatchIASMap>().Delete(IASMap);
                        this._unitOfWork.Save();
                    }
                    //Update Batch with all params except BatchName as it is same
                    if (batchNameFromObj == batchName)
                    {
                        batchTemp.UpdatedBy = updatedBy;
                        batchTemp.UpdatedDate = updatedDate;
                        batchTemp.ExecutionType = executionType;
                        batchTemp.ScheduleDate = scheduleDate;
                        batchTemp.ScheduledTime = scheduledTime;
                        batchTemp.IsApproved = false;
                        this._unitOfWork.Repository<Batch>().Update(batchTemp);
                        this._unitOfWork.Save();
                        foreach (var globalUpdateID in listGUIds)
                        {
                            batchIASMap.GlobalUpdateID = globalUpdateID;
                            batchIASMap.BatchID = batchTemp.BatchID;
                            this._unitOfWork.Repository<BatchIASMap>().Insert(batchIASMap);
                            this._unitOfWork.Save();
                        }

                        //Update Global Update Status on Batch Edit
                        var globalUpdates = this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => listGUIds.Contains(x.GlobalUpdateID)).Get().ToList();
                        globalUpdates.ForEach(x => { x.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_IASSCHEDULEDEXECUTION; x.UpdatedDate = DateTime.Now; x.UpdatedBy = updatedBy; });
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        //Get list of all batches in Batch
                        List<BatchViewModel> listBatches = new List<BatchViewModel>();
                        listBatches = (from btch in this._unitOfWork.Repository<Batch>()
                                                  .Query()
                                                  .Get()
                                       select new BatchViewModel
                                       {
                                           BatchID = btch.BatchID,
                                           BatchName = btch.BatchName
                                       }
                                       ).ToList();

                        if (!(listBatches.Select(x => x.BatchName).Contains(batchName)))
                        {
                            //Update entry in table Batch with new batch name
                            batchTemp.BatchName = batchName;
                            batchTemp.UpdatedBy = updatedBy;
                            batchTemp.UpdatedDate = updatedDate;
                            batchTemp.ExecutionType = executionType;
                            batchTemp.ScheduleDate = scheduleDate;
                            batchTemp.ScheduledTime = scheduledTime;
                            batchTemp.IsApproved = false;
                            this._unitOfWork.Repository<Batch>().Update(batchTemp);
                            this._unitOfWork.Save();
                            foreach (var globalUpdateID in listGUIds)
                            {
                                batchIASMap.GlobalUpdateID = globalUpdateID;
                                batchIASMap.BatchID = batchTemp.BatchID;
                                this._unitOfWork.Repository<BatchIASMap>().Insert(batchIASMap);
                                this._unitOfWork.Save();
                            }

                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                            {
                                Messages = new string[] { "There is already an entry of Batch with the provided Batch Name!!" }
                            });
                        }
                    }
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

        public ServiceResult ApproveBatch(string batchName, string currentUser)
        {
            var result = new ServiceResult();

            Batch batchTemp = this._unitOfWork.Repository<Batch>()
                     .Query()
                     .Filter(x => x.BatchName == batchName)
                     .Get()
                     .FirstOrDefault();
            if (batchTemp.IsApproved != true)
            {
                batchTemp.IsApproved = true;
                batchTemp.ApprovedBy = currentUser;
                batchTemp.ApprovedDate = DateTime.Now;
                this._unitOfWork.Repository<Batch>().Update(batchTemp);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            else
            {
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }
        public void DeleteGlobalUpdateData(int globalUpdateId)
        {
            this._unitOfWork.Repository<IASFolderMap>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASFolderMap>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<IASFileUpload>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASFileUpload>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<IASElementImport>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASElementImport>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<ErrorLog>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<ErrorLog>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<IASElementExport>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASElementExport>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<FormDesignElementValue>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FormDesignElementValue>().Delete(x));
            this._unitOfWork.Save();

            this._unitOfWork.Repository<BatchIASMap>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<Batch>().Delete(x));
            this._unitOfWork.Save();

            var rules = (from rule in this._unitOfWork.Repository<RuleGu>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get() select rule).Distinct().ToList();
            foreach (var rule in rules)
            {
                var expressions = (from exp in this._unitOfWork.Repository<ExpressionGu>().Query().Filter(x => x.RuleID == rule.RuleID).Get() orderby exp.ExpressionID descending select exp).ToList();

                foreach (var expression in expressions)
                {
                    this._unitOfWork.RepositoryAsync<ExpressionGu>().Delete(expression);
                    this._unitOfWork.Save();
                }
                this._unitOfWork.RepositoryAsync<RuleGu>().Delete(rule);
                this._unitOfWork.Save();
            }

            //this._unitOfWork.Repository<RuleGu>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<RuleGu>().Delete(x));
            //this._unitOfWork.Save();
        }

        private List<int> GetKeyElementsForRepeater(int formDesignVersionId)
        {
            List<int> keyUIElements = (from keyRepeaterUIElements in this._unitOfWork.RepositoryAsync<DataSourceMapping>().Query().Filter(x => x.FormDesignVersionID == formDesignVersionId && x.IsKey == true)
                                      .Get()
                                       select keyRepeaterUIElements.UIElementID).ToList();
            return keyUIElements;
        }
        private void UpdateGlobalUpdateWizardStep(int globalUpdateId, int wizardStepId, string userName)
        {
            GlobalUpdate globalUpdate = (from guElement in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get() where guElement.GlobalUpdateID == globalUpdateId select guElement
                                        ).FirstOrDefault();
            globalUpdate.IASWizardStepID = wizardStepId;
            globalUpdate.UpdatedBy = userName;
            globalUpdate.UpdatedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<GlobalUpdate>().Update(globalUpdate);
            this._unitOfWork.Save();
        }
        private void DeleteIASElementExportLog(int GlobalUpdateID)
        {
            var iasElementExportLogToDelete = this._unitOfWork.RepositoryAsync<IASElementExport>()
                                            .Query()
                                            .Filter(c => c.GlobalUpdateID == GlobalUpdateID)
                                            .Get().ToList();

            if (iasElementExportLogToDelete.Any())
            {
                foreach (var item in iasElementExportLogToDelete)
                {
                    this._unitOfWork.RepositoryAsync<IASElementExport>().Delete(item);
                }
                this._unitOfWork.Save();
            }
        }
        private void DeleteIASFolderMapLog(int GlobalUpdateID)
        {
            var iasFolderMapLogToDelete = this._unitOfWork.RepositoryAsync<IASFolderMap>()
                                            .Query()
                                            .Filter(c => c.GlobalUpdateID == GlobalUpdateID)
                                            .Get().ToList();

            if (iasFolderMapLogToDelete.Any())
            {
                foreach (var item in iasFolderMapLogToDelete)
                {
                    this._unitOfWork.RepositoryAsync<IASFolderMap>().Delete(item);
                }
                this._unitOfWork.Save();
            }
        }

        public bool IsValidIASUpload(int GlobalUpdateID)
        {
            bool isExist = true;
            var elements = (from elem in this._unitOfWork.Repository<BatchIASMap>().Query()
                                .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                               .Get()
                            select elem).Distinct().ToList();
            if (elements.Count > 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public void ValidateGlobalUpdatePreCondition(int globalUpdateId, string GlobalUpdateName, DateTime EffectiveDateFrom, DateTime EffectiveDateTo, ref ServiceResult result)
        {
            var formDesignVersions = (from fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Query()
                                     .Filter(fdv => fdv.EffectiveDate >= EffectiveDateFrom && fdv.EffectiveDate <= EffectiveDateTo && fdv.StatusID == 3 && fdv.FormDesignID != (int)GlobalVariables.MASTERLISTFORMDESIGNID)
                                     .Get()
                                      select fdv.FormDesignVersionID)
                                      .Count();
            if (formDesignVersions == 0)
            {
                result.Result = ServiceResultStatus.Failure;
                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                {
                    Messages = new string[] { "There are no Document Versions for specified Effective date range!!" }
                });
            }

            if ((result.Result != ServiceResultStatus.Failure))
            {
                var globalUpdateList = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => x.GlobalUpdateName == GlobalUpdateName && x.GlobalUpdateID != globalUpdateId).Get()
                                        select gu).Count();
                if (globalUpdateList > 0)
                {
                    result.Result = ServiceResultStatus.Failure;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem()
                    {
                        Messages = new string[] { "There is already an entry of Global Update with the provided Global Update Name!!" }
                    });
                }
            }
        }

        #region Retro Methods for Global Update

        public ServiceResult ExecuteBatch(Guid BatchID, string BatchName, int tenantId, string userName, int userId)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                System.DateTime startDateTime = DateTime.Now;

                List<int> globalUpdateIDsList = (from batchIASMap in this._unitOfWork.Repository<BatchIASMap>()
                                                    .Query()
                                                    .Filter(x => x.BatchID == BatchID)
                                                    .Get()
                                                 select batchIASMap.GlobalUpdateID).ToList();

                foreach (int GlobalUpdateID in globalUpdateIDsList)
                {
                    List<int> folderIDsList = GetGlobalUpdateFolderIDsList(GlobalUpdateID);
                    GlobalUpdateViewModel selectedGlobalUpdate = GetSelectedGlobalUpdateData(GlobalUpdateID);
                    UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.IASExecutionInProgress);
                    LockFoldersInBatchExecution(folderIDsList, tenantId, userId, true);
                    foreach (int FolderID in folderIDsList)
                    {
                        List<IASFolderDataModel> impactedRows = GetIASFolderIDsDataList(GlobalUpdateID, FolderID);

                        if (impactedRows.Count > 0)
                        {
                            List<RetroChangeViewModel> retroChangesList = new List<RetroChangeViewModel>();
                            bool isVisited = false;
                            foreach (var item in impactedRows)
                            {
                                RetroChangeViewModel retroChange = new RetroChangeViewModel();
                                retroChange.FolderVersionId = item.FolderVersionID;
                                retroChange.EffectiveDate = (DateTime)selectedGlobalUpdate.EffectiveDateFrom;
                                retroChange.IsCopyRetro = true;
                                if ((retroChange.EffectiveDate <= selectedGlobalUpdate.EffectiveDateFrom) && !isVisited)
                                {
                                    retroChange.IsCopyRetro = false;
                                    isVisited = true;
                                }
                                retroChangesList.Add(retroChange);
                            }

                            var folderVersionList = GetVersionNumberForAccountRetroChanges(1, FolderID);

                            var folderVersionList1 = folderVersionList.OrderBy(el => el.FolderVersionId);

                            var retroChangesList1 = retroChangesList.OrderBy(el => el.FolderVersionId);
                            List<RetroChangeViewModel> retroChangesList2 = retroChangesList1.ToList();

                            serviceResult = GlobalUpdateFolderVersionRetroChanges(retroChangesList2, folderVersionList1, (DateTime)selectedGlobalUpdate.EffectiveDateFrom, GlobalUpdateID, BatchName, userName, userId, BatchID);
                        }

                        IASFolderDataModel baselineDt = GetIASBaselineFolderIDsDataList(GlobalUpdateID, FolderID);

                        if (baselineDt != null)
                        {
                            int newFldrVrsionId = 0;
                            VersionNumberBuilder builder = null;
                            builder = new VersionNumberBuilder();
                            var nextVersionNumber = builder.GetNextMinorVersionNumber(baselineDt.FolderVersionNumber, baselineDt.EffectiveDate);
                            string comments = String.Format(CommentsData.GlobalUpdateBaselineComments, userName, nextVersionNumber, BatchName);

                            serviceResult = GlobalUpdateBaseLineFolder(BatchID, GlobalUpdateID, tenantId, 0, baselineDt.FolderID, baselineDt.FolderVersionID, userId, userName, nextVersionNumber, comments,//GlobalVariables.GB_BASELINE_COMMENT,
                            baselineDt.EffectiveDate, false, out newFldrVrsionId, isNotApproved: false, isNewVersion: false);

                        }
                    }
                    LockFoldersInBatchExecution(folderIDsList, tenantId, userId, false);
                    if (serviceResult.Result == ServiceResultStatus.Success)
                    {
                        UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.IASExecutionComplete);
                    }
                    else if (serviceResult.Result == ServiceResultStatus.Failure)
                    {
                        UpdateGlobalUpdateIASStatus(GlobalUpdateID, (int)GlobalUpdateIASStatus.IASExecutionFailed);
                    }
                }
                SaveAuditReportLog(BatchID, userName);
                System.DateTime endDateTime = DateTime.Now;
                saveBatchExecution(BatchID, startDateTime, endDateTime, userName);

                GenerateAuditReport(BatchID);
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
        private void saveBatchExecution(Guid batchId, DateTime startDateTime, DateTime endDateTime, string userName)
        {
            BatchExecution batchExe = new BatchExecution();
            batchExe.BatchExecutionStatusID = GlobalVariables.GB_BatchExecutionStatus;
            batchExe.BatchID = batchId;
            batchExe.StartDateTime = startDateTime;
            batchExe.EndDateTime = endDateTime;
            batchExe.AddedDate = DateTime.Now;
            batchExe.AddedBy = userName;
            batchExe.RollBackComments = GlobalVariables.GB_BatchExecutionComment;
            this._unitOfWork.Repository<BatchExecution>().Insert(batchExe);
            this._unitOfWork.Save();
        }
        private void LockFoldersInBatchExecution(List<int> FolderIDsList, int TenantID, int UserID, bool IsLock)
        {
            try
            {
                foreach (int FolderID in FolderIDsList)
                {
                    if (IsLock)
                    {
                        FolderLock folderLock = new FolderLock();
                        folderLock.IsLocked = true;
                        folderLock.LockedBy = UserID;
                        folderLock.TenantID = TenantID;
                        folderLock.FolderID = FolderID;
                        folderLock.LockedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FolderLock>().Insert(folderLock);
                        this._unitOfWork.Save();
                    }
                    else
                    {
                        var iteamToDelete = this._unitOfWork.Repository<FolderLock>().Query().Filter(x => x.FolderID == FolderID).Get().FirstOrDefault();
                        this._unitOfWork.RepositoryAsync<FolderLock>().Delete(iteamToDelete);
                        this._unitOfWork.Save();
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
        private List<int> GetGlobalUpdateFolderIDsList(int GlobalUpdateID)
        {
            List<int> globalUpdatesFolderIDsList = null;

            try
            {
                var globalUpdatesFolderID = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID)
                                                            .Get().Distinct()

                                             select new
                                             {
                                                 FolderID = j.FolderID
                                             }).Select(e => e.FolderID).Distinct();
                if (globalUpdatesFolderID != null)
                {
                    globalUpdatesFolderIDsList = globalUpdatesFolderID.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return globalUpdatesFolderIDsList;
        }

        private List<IASFolderDataModel> GetConsolidatedIASFolderIDsDataList(List<int> GlobalUpdateIDList, int FolderID)
        {
            List<IASFolderDataModel> iasFolderDataList = null;
            var folderversion = _unitOfWork.Repository<FolderVersion>().Query().Filter(c => c.FolderID == FolderID).Get().FirstOrDefault();
            var releasedWorkflowState = _unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderversion.FolderVersionID);
            try
            {
                var iasFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.FolderID == FolderID
                                                                    && GlobalUpdateIDList.Contains(f.GlobalUpdateID)
                                                                    && f.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID
                                                                    && f.FolderVersion.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED)
                                                            .Get()

                                     select new IASFolderDataModel
                                     {
                                         GlobalUpdateID = j.GlobalUpdateID,
                                         AccountName = j.AccountName,
                                         FolderID = j.FolderID,
                                         FolderName = j.FolderName,
                                         FolderVersionID = j.FolderVersionID,
                                         FolderVersionNumber = j.FolderVersionNumber,
                                         EffectiveDate = j.EffectiveDate,
                                         FormName = j.FormName
                                     }).OrderBy(ord => ord.FolderVersionID);
                if (iasFolderData != null)
                {
                    iasFolderDataList = iasFolderData.ToList();
                    //Get impactedRows everytime latest from folderversion table in case of uploading multiple IAS on 'Global Update List' screen. 
                    foreach (var iasFolder in iasFolderDataList)
                    {
                        var iasnewFolderVersion = (from j in this._unitOfWork.Repository<FolderVersion>()
                                                            .Query()
                                                            .Filter(f => f.FolderID == iasFolder.FolderID && f.EffectiveDate == iasFolder.EffectiveDate
                                                                    && f.WFStateID == releasedWorkflowState.WorkFlowVersionStateID
                                                                    && f.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED)
                                                            .Get()

                                                   select new
                                                   {
                                                       FolderVersionID = j.FolderVersionID,
                                                       FolderVersionNumber = j.FolderVersionNumber
                                                   }).OrderByDescending(o => o.FolderVersionID).FirstOrDefault();
                        iasFolder.FolderVersionID = iasnewFolderVersion.FolderVersionID;
                        iasFolder.FolderVersionNumber = iasnewFolderVersion.FolderVersionNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasFolderDataList;
        }

        private List<IASFolderDataModel> GetIASFolderIDsDataList(int GlobalUpdateID, int FolderID)
        {
            List<IASFolderDataModel> iasFolderDataList = null;
            var folderversion = _unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c=>c.FolderID == FolderID).Get().FirstOrDefault();
            var releasedWorkflowState = _unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderversion.FolderVersionID);
            try
            {
                var iasFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.FolderID == FolderID
                                                                    && f.FolderVersion.WFStateID == releasedWorkflowState.WorkFlowVersionStateID
                                                                    && f.FolderVersion.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED)
                                                            .Get()

                                     select new IASFolderDataModel
                                     {
                                         GlobalUpdateID = j.GlobalUpdateID,
                                         AccountName = j.AccountName,
                                         FolderID = j.FolderID,
                                         FolderName = j.FolderName,
                                         FolderVersionID = j.FolderVersionID,
                                         FolderVersionNumber = j.FolderVersionNumber,
                                         EffectiveDate = j.EffectiveDate,
                                         FormName = j.FormName
                                     }).OrderBy(ord => ord.FolderVersionID);
                if (iasFolderData != null)
                {
                    iasFolderDataList = iasFolderData.ToList();
                    //Get impactedRows everytime latest from folderversion table in case of uploading multiple IAS on 'Global Update List' screen. 
                    foreach (var iasFolder in iasFolderDataList)
                    {
                        var iasnewFolderVersion = (from j in this._unitOfWork.Repository<FolderVersion>()
                                                            .Query()
                                                            .Filter(f => f.FolderID == iasFolder.FolderID && f.EffectiveDate == iasFolder.EffectiveDate
                                                                    && f.WFStateID == releasedWorkflowState.WorkFlowVersionStateID
                                                                    && f.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED)
                                                            .Get()

                                                   select new
                                                   {
                                                       FolderVersionID = j.FolderVersionID,
                                                       FolderVersionNumber = j.FolderVersionNumber
                                                   }).OrderByDescending(o => o.FolderVersionID).FirstOrDefault();
                        iasFolder.FolderVersionID = iasnewFolderVersion.FolderVersionID;
                        iasFolder.FolderVersionNumber = iasnewFolderVersion.FolderVersionNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasFolderDataList;
        }

        private IASFolderDataModel GetIASBaselineFolderIDsDataList(int GlobalUpdateID, int FolderID)
        {
            IASFolderDataModel iasBaselineFolderData = null;
            try
            {
                iasBaselineFolderData = (from j in this._unitOfWork.Repository<IASFolderMap>()
                                                            .Query()
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID && f.FolderID == FolderID
                                                                    && f.FolderVersion.FolderVersionStateID != (int)domain.entities.Enums.FolderVersionState.RELEASED
                                                                    && f.FolderVersion.FolderVersionStateID != (int)domain.entities.Enums.FolderVersionState.INPROGRESS_BLOCKED)
                                                            .Get()

                                         select new IASFolderDataModel
                                         {
                                             GlobalUpdateID = j.GlobalUpdateID,
                                             AccountName = j.AccountName,
                                             FolderID = j.FolderID,
                                             FolderName = j.FolderName,
                                             FolderVersionID = j.FolderVersionID,
                                             FolderVersionNumber = j.FolderVersionNumber,
                                             EffectiveDate = j.EffectiveDate,
                                             FormName = j.FormName
                                         }).FirstOrDefault();
                if (iasBaselineFolderData != null)
                {
                    //Get impactedRows everytime latest from folderversion table in case of uploading multiple IAS on 'Global Update List' screen. 
                    var iasnewFolderVersionB = (from j in this._unitOfWork.Repository<FolderVersion>()
                                                        .Query()
                                                        .Filter(f => f.FolderID == iasBaselineFolderData.FolderID && f.EffectiveDate == iasBaselineFolderData.EffectiveDate
                                                                && f.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.INPROGRESS)
                                                        .Get()

                                                select new
                                                {
                                                    FolderVersionID = j.FolderVersionID,
                                                    FolderVersionNumber = j.FolderVersionNumber
                                                }).OrderByDescending(o => o.FolderVersionID).FirstOrDefault();
                    if (iasnewFolderVersionB != null)
                    {
                        iasBaselineFolderData.FolderVersionID = iasnewFolderVersionB.FolderVersionID;
                        var iasnewFolderVersion = (from j in this._unitOfWork.Repository<FolderVersion>()
                                                            .Query()
                                                            .Filter(f => f.FolderID == iasBaselineFolderData.FolderID
                                                                    && f.FolderVersionStateID == (int)domain.entities.Enums.FolderVersionState.RELEASED)
                                                            .Get()

                                                   select new
                                                   {
                                                       FolderVersionID = j.FolderVersionID,
                                                       FolderVersionNumber = j.FolderVersionNumber
                                                   }).OrderByDescending(o => o.FolderVersionID).FirstOrDefault();
                        if (iasnewFolderVersion != null)
                        {
                            double versionNumeric = Convert.ToDouble(iasnewFolderVersion.FolderVersionNumber.Split('_')[1]) + 0.01;
                            string nextVersionNumber = Convert.ToString(iasBaselineFolderData.FolderVersionNumber.Split('_')[0]) + "_" + versionNumeric;
                            iasBaselineFolderData.FolderVersionNumber = nextVersionNumber;
                        }
                        else
                        {
                            iasBaselineFolderData.FolderVersionNumber = iasnewFolderVersionB.FolderVersionNumber;
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

            return iasBaselineFolderData;
        }
        private GlobalUpdateViewModel GetSelectedGlobalUpdateData(int globalUpdateId)
        {
            GlobalUpdateViewModel globalUpdateData = null;
            try
            {
                globalUpdateData = (from gb in this._unitOfWork.Repository<GlobalUpdate>().Query().Filter(g => g.GlobalUpdateID == globalUpdateId).Get()
                                    select new GlobalUpdateViewModel
                                    {
                                        GlobalUpdateID = gb.GlobalUpdateID,
                                        GlobalUpdateStatusID = gb.GlobalUpdateStatusID,
                                        GlobalUpdateName = gb.GlobalUpdateName,
                                        EffectiveDateFrom = gb.EffectiveDateFrom,
                                        EffectiveDateTo = gb.EffectiveDateTo,
                                        WizardStepsID = gb.IASWizardStepID
                                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return globalUpdateData;
        }

        private IEnumerable<FolderVersionViewModel> GetVersionNumberForAccountRetroChanges(int tenantId, int folderId)
        {
            List<FolderVersionViewModel> FolderVersionList = null;

            try
            {
                var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Filter(c => c.FolderVersionID ==  folderId).Get().FirstOrDefault();
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderversion.FolderVersionID);
                FolderVersionList = (from c in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId && c.IsActive
                                                         && c.FolderID == folderId && c.WFStateID == workflowState.WorkFlowVersionStateID)
                                              .Get()
                                     select new FolderVersionViewModel
                                     {
                                         FolderVersionId = c.FolderVersionID,
                                         EffectiveDate = c.EffectiveDate,
                                         FolderVersionNumber = c.FolderVersionNumber,
                                         FolderId = c.FolderID,
                                         TenantID = c.TenantID,
                                         WFStateID = (int)c.WFStateID,
                                         IsActive = c.IsActive,
                                         VersionTypeID = c.VersionTypeID,
                                         Comments = c.Comments
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

        //Retro functionality for Global Update code changes
        private ServiceResult GlobalUpdateFolderVersionRetroChanges(List<RetroChangeViewModel> retroChangesList,
            IEnumerable<FolderVersionViewModel> folderVersionList, DateTime retroEffectiveDate, int GlobalUpdateID, string GlobalUpdateName, string userName, int userId, Guid BatchID)
        {
            ServiceResult serviceResult = null;
            VersionNumberBuilder builder = null;
            try
            {
                serviceResult = new ServiceResult();
                folderVersionList = folderVersionList.Where(e =>
                                                      retroChangesList.Any(e1 => e1.FolderVersionId == e.FolderVersionId));

                if (folderVersionList != null && folderVersionList.Any())
                {//order of folderVersionList & retroChangesList has problem to insert in new major version for global update
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
                                var nextVersionNumber = builder.GetNextMajorVersionNumberForRetroChanges(versionNumber,
                                                                    folderVersion.EffectiveDate);
                                string comments = String.Format(CommentsData.GlobalUpdateRetroComments, userName, nextVersionNumber, GlobalUpdateName);

                                folderVersion.FolderVersionNumber = nextVersionNumber;
                                folderVersion.IsCopyRetro = retroChange.IsCopyRetro;
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
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                                            TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        FolderVersionBatch batch = new FolderVersionBatch();

                        batch.AddedBy = userName;
                        batch.AddedDate = DateTime.Now;
                        batch.EffectiveDate = retroEffectiveDate;

                        _unitOfWork.RepositoryAsync<FolderVersionBatch>().Insert(batch);
                        _unitOfWork.Save();

                        var effectiveFolderVersionList = folderVersionList.OrderBy(ord =>
                                                                ord.FolderVersionNumber.Split('_')[0])
                                                            .ThenBy(ord =>
                                                                Convert.ToDouble(ord.FolderVersionNumber.Split('_')[1]));

                        serviceResult = this.GlobalUpdateFolderVersionRetro(effectiveFolderVersionList,
                                                batch.FolderVersionBatchID, GlobalUpdateID, userName, userId, BatchID);

                        this._unitOfWork.Save();
                        scope.Complete();
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

        private ServiceResult GlobalUpdateFolderVersionRetro(IEnumerable<FolderVersionViewModel> folderVersions, int folderVersionBatchId,
            int GlobalUpdateID, string userName, int userId, Guid BatchID)
        {
            var result = new ServiceResult();

            foreach (var folderVersion in folderVersions)
            {
                //Get List FormInstance id related to specific folderVersion
                var formInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                        .Query()
                                                        .Filter(c => c.TenantID == folderVersion.TenantID &&
                                                            c.FolderVersionID == folderVersion.FolderVersionId &&
                                                            c.IsActive == true)
                                                        .Get().ToList().Select(c => c.FormInstanceID);


                int newFolderVersionId = GlobalUpdateCopyFolderVersionRetroChanges(folderVersion, folderVersionBatchId, userName);

                GlobalUpateExecutionLogViewModel log = new GlobalUpateExecutionLogViewModel();
                log.Result = GlobalVariables.GB_RETRO_STATUS;
                log.Comments = GlobalVariables.GB_RETRO_COMMENT;
                log.OldFolderVersionID = folderVersion.FolderVersionId;
                log.NewFolderVersionID = newFolderVersionId;
                log.NewFolderVersionNumber = folderVersion.FolderVersionNumber;
                saveBaselineLog(BatchID, log, userName);

                //if (!folderVersion.IsCopyRetro)
                //{
                foreach (var forminstanceId in formInstanceList)
                {
                    //Copy all form instances related to olderfolderVersion  to newly created FolderVersion
                    GlobalUpdateCopyFormInstanceRetroChanges(folderVersion.TenantID, newFolderVersionId, forminstanceId,
                        string.Empty, GlobalUpdateID, userName);
                }
                //}
                //Copy all workflowStates related to olderfolderVersion  to newly created FolderVersion with Published status
                GlobalUpdateCreateFolderVersionWorkFlowStateRetroChanges(newFolderVersionId, userName, userId, folderVersion.TenantID);

                //Copy all applicable teams related to olderfolderVersion  to newly created FolderVersion
                GlobalUpdateCopyFolderVersionApplicableTeam(folderVersion.TenantID, folderVersion.FolderVersionId, newFolderVersionId, userName, userId);
            }
            result.Result = ServiceResultStatus.Success;
            return result;
        }

        private int GlobalUpdateCopyFolderVersionRetroChanges(FolderVersionViewModel folderVersion, int folderVersionBatchId, string userName)
        {
            WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetReleasedWorkflowState(this._unitOfWork, folderVersion.FolderVersionId);

            var newVersion = new FolderVersion();
            newVersion.FolderVersionNumber = folderVersion.FolderVersionNumber;
            newVersion.FolderID = folderVersion.FolderId;
            newVersion.EffectiveDate = folderVersion.EffectiveDate;
            newVersion.AddedBy = userName;
            newVersion.AddedDate = DateTime.Now;
            newVersion.Comments = folderVersion.Comments;
            newVersion.IsActive = folderVersion.IsActive;
            newVersion.WFStateID = workflowState.WorkFlowVersionStateID;
            newVersion.VersionTypeID = (int)tmg.equinox.domain.entities.Enums.VersionType.Retro;
            newVersion.TenantID = folderVersion.TenantID;
            newVersion.FolderVersionStateID = (int)tmg.equinox.domain.entities.Enums.FolderVersionState.RELEASED;
            newVersion.FolderVersionBatchID = folderVersionBatchId;
            this._unitOfWork.RepositoryAsync<FolderVersion>().Insert(newVersion);
            this._unitOfWork.Save();
            return newVersion.FolderVersionID;
        }

        private void GlobalUpdateCreateFolderVersionWorkFlowStateRetroChanges(int newFolderVersionID, string addedBy, int userId, int tenantId)
        {
            var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c=>c.Folder).Filter(c => c.FolderVersionID == newFolderVersionID).Get().FirstOrDefault();
            //Retrive sequence of Setup State           
            WorkFlowVersionState firstWorkflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);

            var newfirstWorkflowState = new FolderVersionWorkFlowState();
            newfirstWorkflowState.TenantID = tenantId;
            newfirstWorkflowState.IsActive = true;
            newfirstWorkflowState.AddedDate = DateTime.Now;
            newfirstWorkflowState.AddedBy = addedBy;
            newfirstWorkflowState.FolderVersionID = newFolderVersionID;
            newfirstWorkflowState.WFStateID = firstWorkflowState.WorkFlowVersionStateID;
            newfirstWorkflowState.UserID = userId;
            newfirstWorkflowState.ApprovalStatusID = Convert.ToInt32(ApprovalStatus.APPROVED);
            this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(newfirstWorkflowState);

            this._unitOfWork.Save();


            int workflowStateId = firstWorkflowState.WorkFlowVersionStateID;
            bool isReleased = true;
            while (isReleased)
            {
                //Retrive sequence of Setup State           
                int approvalStatusId = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED);
                WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetNextWorkflowState(this._unitOfWork, newFolderVersionID, workflowStateId, approvalStatusId, true);

                var newworkflowState = new FolderVersionWorkFlowState();
                newworkflowState.TenantID = tenantId;
                newworkflowState.IsActive = true;
                newworkflowState.AddedDate = DateTime.Now;
                newworkflowState.AddedBy = addedBy;
                newworkflowState.FolderVersionID = newFolderVersionID;
                newworkflowState.WFStateID = workflowState.WorkFlowVersionStateID;
                newworkflowState.UserID = userId;
                newworkflowState.ApprovalStatusID = Convert.ToInt32(ApprovalStatus.APPROVED);
                this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Insert(newworkflowState);

                this._unitOfWork.Save();

                workflowStateId = workflowState.WorkFlowVersionStateID;
                if (workflowStateId == 7)
                {
                    isReleased = false;
                }
            }

        }

        private int GlobalUpdateCopyFormInstanceRetroChanges(int tenantId, int folderVersionId, int formInstanceId, string formName, int GlobalUpdateID, string addedBy)
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
                newformInstanceId = AddFormInstance(tenantId, folderVersionId, formDesign.FormDesignVersionID, formDesign.FormDesignID, formName, addedBy);

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
                            if (formDesign.FormDesignID != GlobalVariables.MILESTONECHECKLISTFORMDESIGNID)
                            {
                                var formInstanceDataMap = new FormInstanceDataMap();
                                formInstanceDataMap.FormInstanceID = newformInstanceId;
                                formInstanceDataMap.ObjectInstanceID = map.ObjectInstanceID;

                                //Get Updated json with changed new values in FormData json
                                var filter = GetIASElementImportDataList(formInstanceId, GlobalUpdateID, Convert.ToInt32(map.FormInstance.FormDesignID));
                                if (filter.Count > 0)
                                {
                                    int FormDesignVersionID = Convert.ToInt32(map.FormInstance.FormDesignVersionID);
                                    formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                                    List<GuRuleRowModel> rulesFilter = new List<GuRuleRowModel>();
                                    foreach (var uiElement in filter)
                                    {
                                        IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, uiElement.UIElementID, GlobalUpdateID);
                                        if (uiElementRules != null)
                                        {
                                            rulesFilter.AddRange(uiElementRules.ToList());
                                        }
                                    }

                                    if (rulesFilter != null && rulesFilter.Count() > 0)
                                    {
                                        foreach (var rule in rulesFilter)
                                        {
                                            //clean up the null expression for now
                                            if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                            {
                                                rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                                            }
                                            rule.UIElementName = GetGeneratedNameFromID(rule.UIElementID);
                                            rule.UIElementFullName = GetFullNameFromID(rule.UIElementID);
                                            rule.UIElementFormName = GetFormNameFromID(rule.UIElementID);
                                            rule.UIElementTypeID = GetUIElementType(rule.UIElementID);
                                            if (rule.IsResultSuccessElement == true)
                                            {
                                                rule.SuccessValueFullName = GetFullNameFromName(rule.ResultSuccess);
                                            }
                                            if (rule.IsResultFailureElement == true)
                                            {
                                                rule.FailureValueFullName = GetFullNameFromName(rule.ResultFailure);
                                            }

                                            if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                            {
                                                foreach (var exp in rule.Expressions)
                                                {
                                                    if (exp != null)
                                                    {
                                                        exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                                        if (exp.IsRightOperandElement == true)
                                                        {
                                                            exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                                        }
                                                    }
                                                }
                                            }
                                            rule.IsParentRepeater = IsParentRepeater(rule.UIElementID);
                                        }
                                    }

                                   // DocumentMatch doc = new DocumentMatch(this._unitOfWork);
                                   // string targetJsonString = doc.GetUpdatedFormDataJSON(map.FormData, filter, rulesFilter);
                                   // formInstanceDataMap.FormData = targetJsonString;
                                }
                                else
                                {
                                    formInstanceDataMap.FormData = map.FormData;
                                }

                                this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(formInstanceDataMap);
                                this._unitOfWork.Save();
                            }
                            else
                            {
                                var formInstanceDataMap = new FormInstanceDataMap();
                                formInstanceDataMap.FormInstanceID = newformInstanceId;
                                formInstanceDataMap.ObjectInstanceID = map.ObjectInstanceID;
                                formInstanceDataMap.FormData = map.FormData;
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

                                //Get Updated json with changed new values in FormData json
                                var filter = GetIASElementImportDataList(formInstanceId, GlobalUpdateID, Convert.ToInt32(map.FormInstance.FormDesignID));
                                if (filter.Count > 0)
                                {
                                    int FormDesignVersionID = Convert.ToInt32(map.FormInstance.FormDesignVersionID);
                                    formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                                    List<GuRuleRowModel> rulesFilter = new List<GuRuleRowModel>();
                                    foreach (var uiElement in filter)
                                    {
                                        IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, uiElement.UIElementID, GlobalUpdateID);
                                        if (uiElementRules != null)
                                        {
                                            rulesFilter.AddRange(uiElementRules.ToList());
                                        }
                                    }

                                    if (rulesFilter != null && rulesFilter.Count() > 0)
                                    {
                                        foreach (var rule in rulesFilter)
                                        {
                                            //clean up the null expression for now
                                            if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                            {
                                                rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                                            }
                                            rule.UIElementName = GetGeneratedNameFromID(rule.UIElementID);
                                            rule.UIElementFullName = GetFullNameFromID(rule.UIElementID);
                                            rule.UIElementFormName = GetFormNameFromID(rule.UIElementID);
                                            rule.UIElementTypeID = GetUIElementType(rule.UIElementID);
                                            if (rule.IsResultSuccessElement == true)
                                            {
                                                rule.SuccessValueFullName = GetFullNameFromName(rule.ResultSuccess);
                                            }
                                            if (rule.IsResultFailureElement == true)
                                            {
                                                rule.FailureValueFullName = GetFullNameFromName(rule.ResultFailure);
                                            }

                                            if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                            {
                                                foreach (var exp in rule.Expressions)
                                                {
                                                    if (exp != null)
                                                    {
                                                        exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                                        if (exp.IsRightOperandElement == true)
                                                        {
                                                            exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                                        }
                                                    }
                                                }
                                            }
                                            rule.IsParentRepeater = IsParentRepeater(rule.UIElementID);
                                        }
                                    }

                                    //DocumentMatch doc = new DocumentMatch(this._unitOfWork);
                                    //string targetJsonString = doc.GetUpdatedFormDataJSON(map.RepeaterData, filter, rulesFilter);
                                    //formInstanceRepeaterDataMap.RepeaterData = targetJsonString;
                                }
                                else
                                {
                                    formInstanceRepeaterDataMap.RepeaterData = map.RepeaterData;
                                }
                                formInstanceRepeaterDataMap.AddedBy = addedBy;
                                formInstanceRepeaterDataMap.AddedDate = DateTime.Now;
                                formInstanceRepeaterDataMap.IsActive = true;
                                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Insert(formInstanceRepeaterDataMap);
                                this._unitOfWork.Save();
                            }
                            else
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
                    CopyAccountProductMap(folderVersionId, addedBy, formInstanceId, newformInstanceId);
                }
            }

            return newformInstanceId;
        }

        private void CopyAccountProductMap(int newfolderVersionID, string addedBy, int oldFormInstanceId, int newFormInstanceId)
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
                accountProductMapToAdd.ProductID = accountProductMap.ProductID;
                accountProductMapToAdd.ProductType = accountProductMap.ProductType;
                accountProductMapToAdd.PlanCode = accountProductMap.PlanCode;
                accountProductMapToAdd.TenantID = accountProductMap.TenantID;
                accountProductMapToAdd.FormInstanceID = newFormInstanceId;
                accountProductMapToAdd.IsActive = true;
                this._unitOfWork.RepositoryAsync<AccountProductMap>().Insert(accountProductMapToAdd);
                this._unitOfWork.Save();
            }
        }

        private int AddFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formDesignId, string formName, string addedBy)
        {

            if (!this._unitOfWork.RepositoryAsync<FormInstance>().IsFormInstanceExists(tenantId, folderVersionId, formDesignId, formDesignVersionId))
            {
                FormInstance frmInstance = new FormInstance();
                frmInstance.FolderVersionID = folderVersionId;
                frmInstance.FormDesignVersionID = formDesignVersionId;
                frmInstance.FormDesignID = formDesignId;
                frmInstance.TenantID = tenantId;
                frmInstance.AddedBy = addedBy;
                frmInstance.AddedDate = DateTime.Now;
                frmInstance.Name = formName;
                frmInstance.IsActive = true;

                this._unitOfWork.RepositoryAsync<FormInstance>().Insert(frmInstance);
                this._unitOfWork.Save();

                return frmInstance.FormInstanceID;
            }
            else
            {
                return 0;
            }
        }

        private List<IASElementImportViewModel> GetIASElementImportDataList(int formInstanceId, int GlobalUpdateID, int FormDesignID)
        {
            List<IASElementImportViewModel> iasElementImportDataList = null;

            try
            {
                var iasElementImportData = (from j in this._unitOfWork.Repository<IASElementImport>()
                                                            .Query()
                                                            .Include(f => f.UIElement)
                                                            .Filter(f => f.GlobalUpdateID == GlobalUpdateID
                                                                    && f.IASFolderMap.FormInstanceID == formInstanceId
                                                                    && f.IASFolderMap.FormInstance.FormDesignID == FormDesignID
                                                                    && f.AcceptChange == true)
                                                            .Get()
                                            join fdEV in this._unitOfWork.Repository<FormDesignElementValue>().Get()
                                            on j.UIElement.UIElementID equals fdEV.UIElementID
                                            select new IASElementImportViewModel
                                               {
                                                   FormDesignID = j.IASFolderMap.FormInstance.FormDesignID,
                                                   IASFolderMapID = j.IASFolderMapID,
                                                   GlobalUpdateID = j.GlobalUpdateID,
                                                   UIElementID = j.UIElementID,
                                                   UIElementTypeID = j.UIElementTypeID,
                                                   UIElementName = j.UIElementName,
                                                   ElementHeaderName = j.ElementHeaderName,
                                                   OldValue = j.OldValue,
                                                   NewValue = j.NewValue,
                                                   AcceptChange = j.AcceptChange,
                                                   Label = j.ElementHeaderName,
                                                   Name = j.UIElement.GeneratedName,
                                                   OptionLabel = null,
                                                   OptionLabelNo = null,
                                                   ItemData = null,
                                                   ElementFullPath = fdEV.ElementFullPath,
                                                   IsParentRepeater = (j.UIElement.UIElement2 is RepeaterUIElement) ? true : false
                                               });
                if (iasElementImportData != null)
                {
                    iasElementImportDataList = iasElementImportData.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return iasElementImportDataList;
        }

        private class CommentsData
        {
            public const string InitialVersionComments = "{0} created Initial minor version '{1}'";
            public const string RollbackComments = "{0} rolling back the Folder Version '{1}' ";
            public const string GlobalUpdateRetroComments = "Global Update '{2}' was applied on FolderVersion '{1}' by {0} ";
            public const string NewVersionComments = "{0} created new FolderVersion '{1}' ";
            public const string GlobalUpdateBaselineComments = "Global Update Baseline '{2}' was applied on FolderVersion '{1}' by {0} ";
        }
        //Ended Retro functionality for Global Update code changes

        private void UpdateGlobalUpdateIASStatus(int GlobalUpdateID, int GlobalUpdateStatusID)
        {
            GlobalUpdate globalUpdate = (from element in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                         where element.GlobalUpdateID == GlobalUpdateID
                                         select element).FirstOrDefault();
            globalUpdate.GlobalUpdateStatusID = GlobalUpdateStatusID;
            this._unitOfWork.RepositoryAsync<GlobalUpdate>().Update(globalUpdate, true);
            this._unitOfWork.Save();
        }

        #endregion Retro Methods for Global Update

        #region Baseline Methods for Global Update
        public List<GlobalUpateExecutionLogViewModel> getFolderVersionsBaselined(Guid batchId, int userId, string userName)
        {
            List<BaselineDataViewModel> listFolderVersionData = new List<BaselineDataViewModel>();
            List<GlobalUpateExecutionLogViewModel> baselinedFoldersLog = new List<GlobalUpateExecutionLogViewModel>();

            try
            {
                var listGUIds = (from batchIasMap in this._unitOfWork.Repository<BatchIASMap>()
                             .Query()
                             .Filter(x => x.BatchID == batchId)
                             .Get()
                                 select batchIasMap.GlobalUpdateID).ToList();

                var folderVersionIdList =
                                    (from iasFldrMap in this._unitOfWork.Repository<IASFolderMap>()
                                         .Query()
                                         .Get()
                                     where listGUIds.Contains(iasFldrMap.GlobalUpdateID)
                                     select iasFldrMap.FolderVersionID).ToList();

                listFolderVersionData =
                                    (from fidrVersion in this._unitOfWork.Repository<FolderVersion>()
                                         .Query()
                                         .Get()
                                     where folderVersionIdList.Contains(fidrVersion.FolderVersionID) & fidrVersion.FolderVersionStateID == 1
                                     select new BaselineDataViewModel
                                     {
                                         batchId = batchId,
                                         tenantId = 1,
                                         folderId = fidrVersion.FolderID,
                                         folderVersionId = fidrVersion.FolderVersionID,
                                         versionNumber = fidrVersion.FolderVersionNumber,
                                         comments = GlobalVariables.GB_BASELINE_COMMENT,
                                         effectiveDate = fidrVersion.EffectiveDate,
                                         isRelease = fidrVersion.FolderVersionStateID == 1 ? false : true
                                     }).ToList();

                baselinedFoldersLog = GlobalUpdateBaseLineFolderList(batchId, listFolderVersionData, userId, userName);
                return baselinedFoldersLog;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public List<GlobalUpateExecutionLogViewModel> GlobalUpdateBaseLineFolderList(Guid batchId, List<BaselineDataViewModel> baselineData, int userID, string userName)
        {
            List<GlobalUpateExecutionLogViewModel> resultList = null;
            try
            {
                resultList = new List<GlobalUpateExecutionLogViewModel>();
                if (baselineData != null)
                {
                    foreach (var baselineDt in baselineData)
                    {
                        int newFldrVrsionId = 0;
                        VersionNumberBuilder builder = null;
                        builder = new VersionNumberBuilder();
                        var nextVersionNumber = builder.GetNextMinorVersionNumber(baselineDt.versionNumber, baselineDt.effectiveDate);
                        int GlobalUpdateID = 0;
                        //GlobalUpateExecutionLogViewModel resultSingleFolder = GlobalUpdateBaseLineFolder(GlobalUpdateID, baselineDt.tenantId, 0, baselineDt.folderId, baselineDt.folderVersionId, userID, userName, nextVersionNumber, baselineDt.comments,
                        //baselineDt.effectiveDate, baselineDt.isRelease, out newFldrVrsionId, isNotApproved: false, isNewVersion: false);
                        ////Add result of each folder to list
                        //resultList.Add(resultSingleFolder);
                        //saveBaselineLog(batchId, resultSingleFolder, userName);
                    }
                    return resultList;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return resultList;
        }
        public ServiceResult GlobalUpdateBaseLineFolder(Guid BatchID, int GlobalUpdateID, int tenantId, int? notApprovedWorkflowStateId, int folderId, int folderVersionId, int userId,
          string addedBy, string versionNumber, string comments, DateTime? effectiveDate,
          bool isRelease, out int newFldrVrsionId, bool isNotApproved, bool isNewVersion)
        {
            ServiceResult result = null;

            newFldrVrsionId = 0;
            try
            {
                result = new ServiceResult();

                //Get List FormInstance id related to specific folderVersion
                IEnumerable<int> FormInstanceList = this._unitOfWork.RepositoryAsync<FormInstance>()
                                                      .Query()
                                                      .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.IsActive == true)
                                                      .Get().ToList().Select(c => c.FormInstanceID);

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                {
                    //Copy all the record related olderFolderversion to New FolderVersion 
                    //Returns newly generated FolderVersion id
                    int NewFolderVersionId = GlobalUpdateCopyFolderVersion(tenantId, notApprovedWorkflowStateId, folderVersionId, addedBy, versionNumber,
                                                comments, effectiveDate, isRelease, isNotApproved, isNewVersion);
                    newFldrVrsionId = NewFolderVersionId;

                    foreach (var forminstanceId in FormInstanceList)
                    {
                        //Copy all form instances related to olderfolderVersion  to newly created FolderVersion
                        GlobalUpdateCopyFormInstance(tenantId, NewFolderVersionId, forminstanceId, string.Empty, isNewVersion, addedBy, GlobalUpdateID);
                    }

                    //Copy all workflowstates related to olderfolderVersion  to newly created FolderVersion
                    GlobalUpdateCopyFolderVersionWorkFlowState(tenantId, folderVersionId, NewFolderVersionId, addedBy, userId, comments, isRelease, isNotApproved, notApprovedWorkflowStateId);

                    if (!isNewVersion)
                    {
                        //Copy all applicable teams related to olderfolderVersion  to newly created FolderVersion
                        GlobalUpdateCopyFolderVersionApplicableTeam(tenantId, folderVersionId, NewFolderVersionId, addedBy, userId);
                    }

                    if (!isNewVersion)
                    {
                        //Copy all users related to workflowstates of olderfolderVersion to newly created FolderVersion's workflowstates
                        GlobalUpdateCopyWorkFlowStateUserMap(tenantId, folderVersionId, NewFolderVersionId, addedBy, userId);
                    }

                    GlobalUpateExecutionLogViewModel log = new GlobalUpateExecutionLogViewModel();
                    log.Result = GlobalVariables.GB_BASELINE_STATUS;
                    log.Comments = GlobalVariables.GB_BASELINE_COMMENT;
                    log.OldFolderVersionID = folderVersionId;
                    log.NewFolderVersionID = NewFolderVersionId;
                    log.NewFolderVersionNumber = versionNumber;

                    saveBaselineLog(BatchID, log, addedBy);

                    result.Result = ServiceResultStatus.Success;
                    this.GlobalUpdateUpdateFolderChange(tenantId, addedBy, folderId, folderVersionId);
                    scope.Complete();

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


        private int GlobalUpdateCopyFolderVersion(int tenantId, int? notApprovedWorkflowStateId, int folderVersionId, string addedBy, string versionNumber,
          string comments, DateTime? effectiveDate, bool isRelease, bool isNotApproved, bool isNewVersion)
        {

            //Get All FolderVersion Data related to FolderVersionID           
            FolderVersion folderVersionData = this._unitOfWork.RepositoryAsync<FolderVersion>().FindById(folderVersionId);

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
                    folderVersionData.FolderVersionStateID = Convert.ToInt32(tmg.equinox.domain.entities.Enums.FolderVersionState.BASELINED);
                    this._unitOfWork.RepositoryAsync<FolderVersion>().Update(folderVersionData);

                    version.WFStateID = folderVersionData.WFStateID;
                }
                version.FolderVersionNumber = versionNumber;
                version.FolderID = folderVersionData.FolderID;
                version.EffectiveDate = effectiveDate == null ? folderVersionData.EffectiveDate : effectiveDate.Value;
                version.AddedBy = addedBy;
                version.AddedDate = DateTime.Now;
                version.Comments = comments;
                version.IsActive = folderVersionData.IsActive;
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
        public void GlobalUpdateUpdateFolderChange(int tenantId, string userName, int? folderId, int? folderVersionId)
        {
            try
            {
                Folder folderToUpdate = null;

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
        public int GlobalUpdateCopyFormInstance(int tenantId, int folderVersionId, int formInstanceId, string formName, bool isNewVersion, string addedBy, int GlobalUpdateID)
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
                newformInstanceId = GlobalUpdateAddFormInstance(tenantId, folderVersionId, formDesign.FormDesignVersionID, formDesign.FormDesignID, formName, addedBy);

                if (newformInstanceId > 0)
                {
                    using (var scope = new TransactionScope())
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
                                if (formDesign.FormDesignID != GlobalVariables.MILESTONECHECKLISTFORMDESIGNID)
                                {
                                    FormInstanceDataMap formInstanceDataMap = new FormInstanceDataMap();
                                    formInstanceDataMap.FormInstanceID = newformInstanceId;
                                    formInstanceDataMap.ObjectInstanceID = map.ObjectInstanceID;

                                    //Get Updated json with changed new values in FormData json
                                    var filter = GetIASElementImportDataList(formInstanceId, GlobalUpdateID, Convert.ToInt32(map.FormInstance.FormDesignID));
                                    if (filter.Count > 0)
                                    {
                                        int FormDesignVersionID = Convert.ToInt32(map.FormInstance.FormDesignVersionID);
                                        formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                                        List<GuRuleRowModel> rulesFilter = new List<GuRuleRowModel>();
                                        foreach (var uiElement in filter)
                                        {
                                            IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, uiElement.UIElementID, GlobalUpdateID);
                                            if (uiElementRules != null)
                                            {
                                                rulesFilter.AddRange(uiElementRules.ToList());
                                            }
                                        }

                                        if (rulesFilter != null && rulesFilter.Count() > 0)
                                        {
                                            foreach (var rule in rulesFilter)
                                            {
                                                //clean up the null expression for now
                                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                                {
                                                    rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                                                }
                                                rule.UIElementName = GetGeneratedNameFromID(rule.UIElementID);
                                                rule.UIElementFullName = GetFullNameFromID(rule.UIElementID);
                                                rule.UIElementFormName = GetFormNameFromID(rule.UIElementID);
                                                rule.UIElementTypeID = GetUIElementType(rule.UIElementID);
                                                if (rule.IsResultSuccessElement == true)
                                                {
                                                    rule.SuccessValueFullName = GetFullNameFromName(rule.ResultSuccess);
                                                }
                                                if (rule.IsResultFailureElement == true)
                                                {
                                                    rule.FailureValueFullName = GetFullNameFromName(rule.ResultFailure);
                                                }

                                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                                {
                                                    foreach (var exp in rule.Expressions)
                                                    {
                                                        if (exp != null)
                                                        {
                                                            exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                                            if (exp.IsRightOperandElement == true)
                                                            {
                                                                exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                                            }
                                                        }
                                                    }
                                                }
                                                rule.IsParentRepeater = IsParentRepeater(rule.UIElementID);
                                            }
                                        }
                                        //Removed document Match

                                        //DocumentMatch doc = new DocumentMatch(this._unitOfWork);
                                        //string targetJsonString = doc.GetUpdatedFormDataJSON(map.FormData, filter, rulesFilter);
                                        //formInstanceDataMap.FormData = targetJsonString;
                                    }
                                    else
                                    {
                                        formInstanceDataMap.FormData = map.FormData;
                                    }
                                    this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Insert(formInstanceDataMap);
                                    this._unitOfWork.Save();
                                }
                                else if (!isNewVersion)
                                {
                                    FormInstanceDataMap formInstanceDataMap = new FormInstanceDataMap();
                                    formInstanceDataMap.FormInstanceID = newformInstanceId;
                                    formInstanceDataMap.ObjectInstanceID = map.ObjectInstanceID;
                                    formInstanceDataMap.FormData = map.FormData;
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

                                    //Get Updated json with changed new values in FormData json
                                    var filter = GetIASElementImportDataList(formInstanceId, GlobalUpdateID, Convert.ToInt32(map.FormInstance.FormDesignID));
                                    if (filter.Count > 0)
                                    {
                                        int FormDesignVersionID = Convert.ToInt32(map.FormInstance.FormDesignVersionID);
                                        formElementList = GetUIElemenstFromFormDesignVersion(FormDesignVersionID);
                                        List<GuRuleRowModel> rulesFilter = new List<GuRuleRowModel>();
                                        foreach (var uiElement in filter)
                                        {
                                            IEnumerable<GuRuleRowModel> uiElementRules = GetRulesForUIElement(1, FormDesignVersionID, uiElement.UIElementID, GlobalUpdateID);
                                            if (uiElementRules != null)
                                            {
                                                rulesFilter.AddRange(uiElementRules.ToList());
                                            }
                                        }

                                        if (rulesFilter != null && rulesFilter.Count() > 0)
                                        {
                                            foreach (var rule in rulesFilter)
                                            {
                                                //clean up the null expression for now
                                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                                {
                                                    rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();
                                                }
                                                rule.UIElementName = GetGeneratedNameFromID(rule.UIElementID);
                                                rule.UIElementFullName = GetFullNameFromID(rule.UIElementID);
                                                rule.UIElementFormName = GetFormNameFromID(rule.UIElementID);
                                                rule.UIElementTypeID = GetUIElementType(rule.UIElementID);
                                                if (rule.IsResultSuccessElement == true)
                                                {
                                                    rule.SuccessValueFullName = GetFullNameFromName(rule.ResultSuccess);
                                                }
                                                if (rule.IsResultFailureElement == true)
                                                {
                                                    rule.FailureValueFullName = GetFullNameFromName(rule.ResultFailure);
                                                }

                                                if (rule.Expressions != null && rule.Expressions.Count() > 0)
                                                {
                                                    foreach (var exp in rule.Expressions)
                                                    {
                                                        if (exp != null)
                                                        {
                                                            exp.LeftOperandName = GetFullNameFromName(exp.LeftOperand);
                                                            if (exp.IsRightOperandElement == true)
                                                            {
                                                                exp.RightOperandName = GetFullNameFromName(exp.RightOperand);
                                                            }
                                                        }
                                                    }
                                                }
                                                rule.IsParentRepeater = IsParentRepeater(rule.UIElementID);
                                            }
                                        }

                                        //DocumentMatch doc = new DocumentMatch(this._unitOfWork);
                                        //string targetJsonString = doc.GetUpdatedFormDataJSON(map.RepeaterData, filter, rulesFilter);
                                        //formInstanceRepeaterDataMap.RepeaterData = targetJsonString;
                                    }
                                    else
                                    {
                                        formInstanceRepeaterDataMap.RepeaterData = map.RepeaterData;
                                    }
                                    formInstanceRepeaterDataMap.AddedBy = addedBy;
                                    formInstanceRepeaterDataMap.AddedDate = DateTime.Now;
                                    formInstanceRepeaterDataMap.IsActive = true;
                                    this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Insert(formInstanceRepeaterDataMap);
                                    this._unitOfWork.Save();
                                }
                                else if (!isNewVersion)
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
                        GlobalUpdateCopyAccountProductMap(folderVersionId, addedBy, formInstanceId, newformInstanceId);
                        scope.Complete();
                    }
                }
            }

            return newformInstanceId;
        }
        public int GlobalUpdateAddFormInstance(int tenantId, int folderVersionId, int formDesignVersionId, int formDesignId, string formName, string addedBy)
        {

            if (!this._unitOfWork.RepositoryAsync<FormInstance>().IsFormInstanceExists(tenantId, folderVersionId, formDesignId, formDesignVersionId))
            {
                FormInstance frmInstance = new FormInstance();
                frmInstance.FolderVersionID = folderVersionId;
                frmInstance.FormDesignVersionID = formDesignVersionId;
                frmInstance.FormDesignID = formDesignId;
                frmInstance.TenantID = tenantId;
                frmInstance.AddedBy = addedBy;
                frmInstance.AddedDate = DateTime.Now;
                frmInstance.Name = formName;
                frmInstance.IsActive = true;

                this._unitOfWork.RepositoryAsync<FormInstance>().Insert(frmInstance);
                this._unitOfWork.Save();

                return frmInstance.FormInstanceID;
            }
            else
            {
                return 0;
            }
        }

        public List<GlobalUpdateComputedStatus> GetLatestGlobalUpdateStatus(List<int> globalUpdateIds)
        {
            ServiceResult result = new ServiceResult();
            List<GlobalUpdateComputedStatus> globalUpdateRefreshStatus = ComputeGlobalUpdateExecutionStatusText(globalUpdateIds, true);
            return globalUpdateRefreshStatus;
        }

        private void GlobalUpdateCopyFolderVersionWorkFlowState(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId, string comment, bool isRelease, bool isNotApproved, int? notApprovedWorkflowStateId)
        {
            if (isNotApproved)
            {
                //WorkFlowVersionState workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(tenantId);
                var folderversion = this._unitOfWork.RepositoryAsync<FolderVersion>().Query().Include(c => c.Folder).Filter(c => c.FolderVersionID == newFolderVersionID).Get().FirstOrDefault();
                var workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)folderversion.CategoryID, folderversion.Folder.IsPortfolio);

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
                    WorkFlowVersionState nextWorkflowState = null; int workflowStateID = 0; int approvalStatusID = 0;
                    while (nextWorkflowState == null || nextWorkflowState.WorkFlowVersionStateID != notApprovedWorkflowStateId)
                    {
                        workflowStateID = (nextWorkflowState == null) ? workflowState.WorkFlowVersionStateID : nextWorkflowState.WorkFlowVersionStateID;

                        //Approval status of old folder version to copy.
                        approvalStatusID = this._unitOfWork.Repository<FolderVersionWorkFlowState>()
                                                               .Query()
                                                               .Filter(c => c.FolderVersionID == oldFolderVersionID && c.WFStateID == workflowStateID)
                                                               .Get()
                                                               .Select(c => c.ApprovalStatusID).FirstOrDefault();

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

        private void GlobalUpdateCopyFolderVersionApplicableTeam(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId)
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

        private void GlobalUpdateCopyWorkFlowStateUserMap(int tenantId, int oldFolderVersionID, int newFolderVersionID, string addedBy, int userId)
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
                this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Insert(workFlowStateUser);
            }

            this._unitOfWork.Save();
        }

        public void GlobalUpdateCopyAccountProductMap(int newfolderVersionID, string addedBy, int oldFormInstanceId, int newFormInstanceId)
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
                accountProductMapToAdd.ProductID = accountProductMap.ProductID;
                accountProductMapToAdd.ProductType = accountProductMap.ProductType;
                accountProductMapToAdd.PlanCode = accountProductMap.PlanCode;

                accountProductMapToAdd.TenantID = accountProductMap.TenantID;
                accountProductMapToAdd.FormInstanceID = newFormInstanceId;
                accountProductMapToAdd.IsActive = true;
                this._unitOfWork.RepositoryAsync<AccountProductMap>().Insert(accountProductMapToAdd);
                this._unitOfWork.Save();
            }
        }
        private void saveBaselineLog(Guid batchID, GlobalUpateExecutionLogViewModel resultSingleFolder, string userName)
        {
            GlobalUpateExecutionLog globalUpateExecutionLog = new GlobalUpateExecutionLog();
            globalUpateExecutionLog.NewFolderVersionNumber = resultSingleFolder.NewFolderVersionNumber;
            globalUpateExecutionLog.NewFolderVersionID = resultSingleFolder.NewFolderVersionID;
            globalUpateExecutionLog.OldFolderVersionID = resultSingleFolder.OldFolderVersionID;
            globalUpateExecutionLog.Result = resultSingleFolder.Result;
            globalUpateExecutionLog.Comments = resultSingleFolder.Comments;
            globalUpateExecutionLog.BatchID = batchID;
            this._unitOfWork.RepositoryAsync<GlobalUpateExecutionLog>().Insert(globalUpateExecutionLog);

            this._unitOfWork.Save();
        }

        #endregion Baseline Methods for Global Update


        #region Generate AuditReport

        public ServiceResult GenerateAuditReport(Guid batchId)
        {
            ServiceResult result = new ServiceResult();
            string auditReportFilePath = System.Configuration.ConfigurationManager.AppSettings["AuditReportFilePath"];
            try
            {
                if (batchId != null)
                {
                    Batch batchInfo = this._unitOfWork.RepositoryAsync<Batch>().Query().Filter(x => x.BatchID == batchId).Get().FirstOrDefault();

                    AuditReportBuilder auditReport = new AuditReportBuilder();
                    DataSet tableDataset = new DataSet();
                    DataTable auditReportTable = new DataTable(batchInfo.BatchName);
                    string path = auditReportFilePath + batchId + GlobalUpdateConstants.ExcelFileExtension;
                    var properties = typeof(AuditReportViewModel).GetProperties();

                    tableDataset.Tables.Add(auditReportTable);

                    auditReportTable.Columns.Add("Account Name", typeof(string));
                    auditReportTable.Columns.Add("Folder Name", typeof(string));
                    auditReportTable.Columns.Add("Folder Version No.", typeof(string));
                    auditReportTable.Columns.Add("Folder Effective Date", typeof(string));
                    auditReportTable.Columns.Add("Document Name", typeof(string));
                    auditReportTable.Columns.Add("Status", typeof(string));

                    var auditReportCollection = (from audit in this._unitOfWork.RepositoryAsync<AuditReport>().Query().Filter(x => x.BatchID == batchId).Get()
                                                 join batchIASMap in this._unitOfWork.RepositoryAsync<BatchIASMap>().Query().Filter(x => x.BatchID == batchId).Get()
                                                 on audit.BatchID equals batchIASMap.BatchID
                                                 join folderMap in this._unitOfWork.RepositoryAsync<IASFolderMap>().Get()
                                                     //on audit.IASFolderMapID equals folderMap.IASFolderMapID
                                                 on batchIASMap.GlobalUpdateID equals folderMap.GlobalUpdateID
                                                 join guExLog in this._unitOfWork.RepositoryAsync<GlobalUpateExecutionLog>().Query().Filter(x => x.BatchID == batchId).Get()
                                                 on folderMap.FolderVersionID equals guExLog.OldFolderVersionID
                                                 select new AuditReportViewModel
                                                 {
                                                     AccountName = folderMap.AccountName,
                                                     FolderName = folderMap.FolderName,
                                                     FolderVersionNumber = guExLog.NewFolderVersionNumber,
                                                     EffectiveDate = folderMap.EffectiveDate.ToString(),
                                                     DocumentName = folderMap.FormName,
                                                     UpdateStatus = audit.UpdateStatus
                                                 }).ToList();

                    auditReportCollection.ToList().ForEach(i => auditReportTable.Rows.Add(properties.Select(p => p.GetValue(i, null)).ToArray()));
                    auditReport.GenerateExcel(path, tableDataset);
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

        private void SaveAuditReportLog(Guid batchID, string userName)
        {
            AuditReport auditReportLog = new AuditReport();
            auditReportLog.BatchID = batchID;
            auditReportLog.UpdateStatus = "Success";
            auditReportLog.AddedBy = userName;
            auditReportLog.AddedDate = DateTime.Now;
            this._unitOfWork.RepositoryAsync<AuditReport>().Insert(auditReportLog);

            this._unitOfWork.Save();
        }

        #endregion Generate AuditReport
        #region RollBackBatch
        public ServiceResult rollBackBatch(Guid batchID, string rollbackComment)
        {
            ServiceResult result = new ServiceResult();
            BatchExecution batchExecution = new BatchExecution();
            batchExecution = this._unitOfWork.RepositoryAsync<BatchExecution>()
                                                  .Query()
                                                  .Filter(c => c.BatchID == batchID)
                                                  .Get()
                                                  .FirstOrDefault();


            List<GlobalUpateExecutionLog> globalUpateExecutionLog = new List<GlobalUpateExecutionLog>();
            try
            {
                globalUpateExecutionLog = (from exeLogs in this._unitOfWork.RepositoryAsync<GlobalUpateExecutionLog>()
                                   .Query()
                                   .Filter(exeLogs => exeLogs.BatchID == batchID)
                                   .Get()
                                           select exeLogs).ToList();

                foreach (var exeLog in globalUpateExecutionLog)
                {

                    //Start WorkFlowStateFolderVersionMap
                    this._unitOfWork.Repository<WorkFlowStateFolderVersionMap>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<WorkFlowStateFolderVersionMap>().Delete(x));
                    this._unitOfWork.Save();
                    //End WorkFlowStateFolderVersionMap

                    //Start EmailLog
                    this._unitOfWork.Repository<EmailLog>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<EmailLog>().Delete(x));
                    this._unitOfWork.Save();
                    //End EmailLog

                    //Start FormInstance
                    var formInstanceList = this._unitOfWork.Repository<FormInstance>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList();
                    foreach (var formInstance in formInstanceList)
                    {
                        this._unitOfWork.Repository<AccountProductMap>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<AccountProductMap>().Delete(x));
                        this._unitOfWork.Save();

                        //--Start journal
                        var journalList = this._unitOfWork.Repository<Journal>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList();
                        foreach (var journal in journalList)
                        {
                            this._unitOfWork.Repository<JournalResponse>().Query().Filter(x => x.JournalID == journal.JournalID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<JournalResponse>().Delete(x));
                            this._unitOfWork.Save();

                            this._unitOfWork.Repository<Journal>().Delete(journal);
                            this._unitOfWork.Save();
                        }
                        //--End journal

                        //--Start FormInstanceDataMap
                        var formInstanceDataMapList = this._unitOfWork.Repository<FormInstanceDataMap>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList();

                        foreach (var formInstanceDataMap in formInstanceDataMapList)
                        {
                            this._unitOfWork.Repository<FormInstanceRepeaterDataMap>().Query().Filter(x => x.FormInstanceDataMapID == formInstanceDataMap.FormInstanceDataMapID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Delete(x));
                            this._unitOfWork.Save();

                            this._unitOfWork.Repository<FormInstanceDataMap>().Delete(formInstanceDataMap);
                            this._unitOfWork.Save();
                        }
                        //--End FormInstanceDataMap

                        //--Start IASFolderMap
                        var iasFolderMapList = this._unitOfWork.Repository<IASFolderMap>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList();

                        foreach (var iasFolderMap in iasFolderMapList)
                        {
                            this._unitOfWork.Repository<IASElementImport>().Query().Filter(x => x.IASFolderMapID == iasFolderMap.IASFolderMapID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASElementImport>().Delete(x));
                            this._unitOfWork.Save();

                            this._unitOfWork.Repository<IASFolderMap>().Delete(iasFolderMap);
                            this._unitOfWork.Save();
                        }
                        //--End IASFolderMap

                        //--Start FormInstanceActivityLog
                        this._unitOfWork.Repository<FormInstanceActivityLog>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Delete(x));
                        this._unitOfWork.Save();
                        //--End FormInstanceActivityLog

                        //--Start FormInstanceRepeaterDataMap
                        this._unitOfWork.Repository<FormInstanceRepeaterDataMap>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Delete(x));
                        this._unitOfWork.Save();
                        //--End FormInstanceRepeaterDataMap

                        //--Start CustomerCareDocument
                        //this._unitOfWork.Repository<CustomerCareDocument>().Query().Filter(x => x.FormInstanceID == formInstance.FormInstanceID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<CustomerCareDocument>().Delete(x));
                        //this._unitOfWork.Save();
                        //--End CustomerCareDocument

                        this._unitOfWork.Repository<FormInstance>().Delete(formInstance);
                        this._unitOfWork.Save();
                    }

                    //End FormInstance

                    //Start FolderVersionWorkFlowState
                    this._unitOfWork.Repository<FolderVersionWorkFlowState>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Delete(x));
                    this._unitOfWork.Save();
                    //End FolderVersionWorkFlowState

                    //Start AccountProductMap
                    this._unitOfWork.Repository<AccountProductMap>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<AccountProductMap>().Delete(x));
                    this._unitOfWork.Save();
                    //End AccountProductMap

                    //Start Journal
                    var journal1List = this._unitOfWork.Repository<Journal>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList();

                    foreach (var journal1 in journal1List)
                    {
                        this._unitOfWork.Repository<JournalResponse>().Query().Filter(x => x.JournalID == journal1.JournalID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<JournalResponse>().Delete(x));
                        this._unitOfWork.Save();

                        this._unitOfWork.Repository<Journal>().Delete(journal1);
                        this._unitOfWork.Save();

                    }
                    //End Journal

                    //Start IASFolderMap
                    var iasFolderMap1List = this._unitOfWork.Repository<IASFolderMap>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList();

                    foreach (var iasFolderMap1 in iasFolderMap1List)
                    {
                        this._unitOfWork.Repository<IASElementImport>().Query().Filter(x => x.IASFolderMapID == iasFolderMap1.IASFolderMapID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<IASElementImport>().Delete(x));
                        this._unitOfWork.Save();

                        this._unitOfWork.Repository<IASFolderMap>().Delete(iasFolderMap1);
                        this._unitOfWork.Save();

                    }
                    //End IASFolderMap

                    //Start FormInstanceActivityLog
                    this._unitOfWork.Repository<FormInstanceActivityLog>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FormInstanceActivityLog>().Delete(x));
                    this._unitOfWork.Save();
                    //End FormInstanceActivityLog

                    //Start CustomerCareDocument
                    //this._unitOfWork.Repository<CustomerCareDocument>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<CustomerCareDocument>().Delete(x));
                    //this._unitOfWork.Save();
                    //End CustomerCareDocument

                    //Start WorkFlowStateUserMap
                    this._unitOfWork.Repository<WorkFlowStateUserMap>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<WorkFlowStateUserMap>().Delete(x));
                    this._unitOfWork.Save();
                    //End WorkFlowStateUserMap

                    //Start FolderVersion
                    this._unitOfWork.Repository<FolderVersion>().Query().Filter(x => x.FolderVersionID == exeLog.NewFolderVersionID).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<FolderVersion>().Delete(x));
                    this._unitOfWork.Save();
                    //End FolderVersion 

                    if (exeLog.Comments == GlobalVariables.GB_BASELINE_COMMENT)
                    {
                        var iteamToUpdate = this._unitOfWork.Repository<FolderVersion>().Query().Filter(x => x.FolderVersionID == exeLog.OldFolderVersionID).Get().FirstOrDefault();
                        if (iteamToUpdate != null)
                        {
                            iteamToUpdate.FolderVersionStateID = Convert.ToInt32(FolderVersionState.INPROGRESS);
                            this._unitOfWork.Repository<FolderVersion>().Update(iteamToUpdate);
                            this._unitOfWork.Save();
                        }
                    }

                }
                batchExecution.RollBackComments = rollbackComment;
                batchExecution.BatchExecutionStatusID = GlobalVariables.GB_BatchExecutionStatusRollbacked;
                this._unitOfWork.Repository<BatchExecution>().Update(batchExecution);
                this._unitOfWork.Save();
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
        #endregion RollBackBatch


        #region GlobalUpdate Execution Status

        private List<GlobalUpdateComputedStatus> ComputeGlobalUpdateExecutionStatusText(List<int> globalUpdateIds, bool isRefreshRequest)
        {
            // List<GlobalUpdate> globalUpdates = new List<GlobalUpdate>();
            List<GlobalUpdate> globalUpdates = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Include(x => x.GlobalUpdateStatus).Get() select gu).ToList();

            if (isRefreshRequest)
                globalUpdates = globalUpdates.Where(x => globalUpdateIds.Contains(x.GlobalUpdateID)).ToList();

            List<GlobalUpdateComputedStatus> executionStatus = new List<GlobalUpdateComputedStatus>();

            foreach (var globalUpdate in globalUpdates)
            {
                switch (globalUpdate.GlobalUpdateStatusID)
                {
                    case 1:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.PendingFinzalization, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.InProgressSymbol });
                        break;
                    case 2:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.ValidationSuccess, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.CompletedSymbol });
                        break;
                    case 3:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.IASReport, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.InProgressSymbol });
                        break;
                    case 4:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.IASReport, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.CompletedSymbol });
                        break;
                    case 5:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.Validation, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.InProgressSymbol });
                        break;
                    case 6:
                        bool validationError = ComputeValidationSymbol(globalUpdate.GlobalUpdateID);
                        executionStatus.Add(new GlobalUpdateComputedStatus
                        {
                            GlobalUpdateId = globalUpdate.GlobalUpdateID,
                            ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus,
                            ExecutionStatusText = validationError ? GlobalUpdateExecutionConstant.Validation : GlobalUpdateExecutionConstant.ValidationSuccess
                            ,
                            ExecutionStatusSymbol = validationError ? GlobalUpdateExecutionConstant.ErrorSymbol : GlobalUpdateExecutionConstant.CompletedSymbol
                        });
                        break;
                    case 7:
                        executionStatus.Add(new GlobalUpdateComputedStatus
                          {
                              GlobalUpdateId = globalUpdate.GlobalUpdateID,
                              ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus,
                              ExecutionStatusText = GlobalUpdateExecutionConstant.IASReport + GlobalUpdateExecutionConstant.ContactSupportTeam
                            ,
                              ExecutionStatusSymbol = GlobalUpdateExecutionConstant.ErrorSymbol
                          });
                        break;
                    case 8:
                        executionStatus.Add(new GlobalUpdateComputedStatus
                        {
                            GlobalUpdateId = globalUpdate.GlobalUpdateID,
                            ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus,
                            ExecutionStatusText = GlobalUpdateExecutionConstant.Validation + GlobalUpdateExecutionConstant.Processing + GlobalUpdateExecutionConstant.ContactSupportTeam
                           ,
                            ExecutionStatusSymbol = GlobalUpdateExecutionConstant.ErrorSymbol
                        });
                        break;
                    case 9:
                        executionStatus.Add(new GlobalUpdateComputedStatus
                        {
                            GlobalUpdateId = globalUpdate.GlobalUpdateID,
                            ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus,
                            ExecutionStatusText = GlobalUpdateExecutionConstant.Execution + GlobalUpdateExecutionConstant.ContactSupportTeam,
                            ExecutionStatusSymbol = GlobalUpdateExecutionConstant.ErrorSymbol
                        });
                        break;
                    case 10:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.Execution, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.InProgressSymbol });
                        break;
                    case 11:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.Execution, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.CompletedSymbol });
                        break;
                    case 12:
                        Batch iasBatch = GlobalUpdateBatch(globalUpdate.GlobalUpdateID);
                        bool isRealtime = (iasBatch.ExecutionType.Trim() == GlobalVariables.GB_RealtimeExecutionType) ? true : false;
                        executionStatus.Add(new GlobalUpdateComputedStatus
                        {
                            GlobalUpdateId = globalUpdate.GlobalUpdateID
                          ,
                            ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus
                          ,
                            ExecutionStatusText = isRealtime ? GlobalUpdateExecutionConstant.Execution : GlobalUpdateExecutionConstant.Execution + " ( " + iasBatch.ScheduleDate.Value.ToShortDateString() + " ) "
                          ,
                            ExecutionStatusSymbol = isRealtime ? GlobalUpdateExecutionConstant.RealtimeSymbol : GlobalUpdateExecutionConstant.ScheduledSymbol

                        });
                        break;
                    default:
                        executionStatus.Add(new GlobalUpdateComputedStatus { GlobalUpdateId = globalUpdate.GlobalUpdateID, ExecutionStatus = globalUpdate.GlobalUpdateStatus.GlobalUpdatestatus, ExecutionStatusText = GlobalUpdateExecutionConstant.UnknownAction + GlobalUpdateExecutionConstant.ContactSupportTeam, ExecutionStatusSymbol = GlobalUpdateExecutionConstant.NASymbol });
                        break;
                }
            }
            return executionStatus;
        }

        private bool ComputeValidationSymbol(int globalUpdateId)
        {
            var validationErrors = (from error in this._unitOfWork.RepositoryAsync<ErrorLog>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get() select error.ErrorLogID).ToList();
            return validationErrors.Count() > 0 ? true : false;// GlobalUpdateExecutionConstant.ErrorSymbol : GlobalUpdateExecutionConstant.CompletedSymbol;
        }

        private string GetExecutionStatusText(int globalUpdateId)
        {
            return globalUpdatesExecutionStatus.Where(x => x.GlobalUpdateId == globalUpdateId).Select(sel => sel.ExecutionStatusText).FirstOrDefault();
        }

        private string GetExecutionStatusSymbol(int globalUpdateId)
        {
            return globalUpdatesExecutionStatus.Where(x => x.GlobalUpdateId == globalUpdateId).Select(sel => sel.ExecutionStatusSymbol).FirstOrDefault();
        }

        private Batch GlobalUpdateBatch(int globalUpdateId)
        {
            var globalUpdateExecutionStatus = (from iasMap in this._unitOfWork.RepositoryAsync<BatchIASMap>().Query().Filter(x => x.GlobalUpdateID == globalUpdateId).Get()
                                               join batch in this._unitOfWork.RepositoryAsync<Batch>().Get()
                                               on iasMap.BatchID equals batch.BatchID
                                               select batch).FirstOrDefault();
            return globalUpdateExecutionStatus;
        }

        #endregion GlobalUpdate Execution Status

        public ServiceResult DeleteBatch(Guid batchId, string deletedBy)
        {
            ServiceResult result = new ServiceResult();
            try
            {

                var globalUpdateIdsForBatch = (from iasBatchMap in this._unitOfWork.RepositoryAsync<BatchIASMap>().Query().Filter(x => x.BatchID == batchId).Get()
                                               join gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Get()
                                               on iasBatchMap.GlobalUpdateID equals gu.GlobalUpdateID
                                               select gu.GlobalUpdateID).ToList();

                var globalUpdatesForBatch = (from gu in this._unitOfWork.RepositoryAsync<GlobalUpdate>().Query().Filter(x => globalUpdateIdsForBatch.Contains(x.GlobalUpdateID))
                                             .Get()
                                             select gu).ToList();

                globalUpdatesForBatch.ForEach(x => { x.GlobalUpdateStatusID = GlobalVariables.GB_STATUS_COMPLETEID; x.UpdatedDate = DateTime.Now; x.UpdatedBy = deletedBy; });
                this._unitOfWork.Save();

                this._unitOfWork.Repository<BatchIASMap>().Query().Filter(x => x.BatchID == batchId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<BatchIASMap>().Delete(x));
                this._unitOfWork.Save();

                this._unitOfWork.Repository<Batch>().Query().Filter(x => x.BatchID == batchId).Get().ToList().ForEach(x => this._unitOfWork.RepositoryAsync<Batch>().Delete(x));
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }
    }


    internal class GlobalUpdateExecutionConstant
    {
        public const string Execution = "Execution";
        public const string PendingFinzalization = "Finalization";
        public const string IASReport = "IAS Generation";
        public const string Validation = "Validation";
        public const string ValidationSuccess = "Validation / IAS Upload";
        public const string UnknownAction = "Unknown Error";
        public const string InProgressSymbol = "1";
        public const string CompletedSymbol = "2";
        public const string ErrorSymbol = "3";
        public const string NASymbol = "4";
        public const string RealtimeSymbol = "5";
        public const string ScheduledSymbol = "6";
        public const string ContactSupportTeam = ".Contact Support Team.";
        public const string Processing = " Processing";
    }
}