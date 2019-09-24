using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class FormInstanceRuleExecutionLogService : IFormInstanceRuleExecutionLogService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        public FormInstanceRuleExecutionLogService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public GridPagingResponse<FormInstanceRuleExecutionLogViewModel> GetRuleExecutionLogData(int formInstanceID, int parentRowID, bool isParentData, string sessionId, GridPagingRequest gridPagingRequest)
        {

            List<FormInstanceRuleExecutionLogViewModel> ruleExecutionLogList = null;
            int count = 0;
            SearchCriteria criteria = new SearchCriteria();

            try
            {
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                ruleExecutionLogList = (from log in this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionLog>().Get().Where(x => x.FormInstanceID == formInstanceID && x.IsParentRow == isParentData && x.ParentRowID == (parentRowID != -1 ? parentRowID : x.ParentRowID) && x.SessionID == (sessionId != "" ? sessionId : x.SessionID)).OrderByDescending(x => x.RuleExecutionLoggerID) //.ThenByDescending(c => c.RuleExecutionLoggerID)
                                        select new FormInstanceRuleExecutionLogViewModel
                                        {
                                            ID = log.RuleExecutionLoggerID,
                                            FormInstanceID = log.FormInstanceID,
                                            SessionID = log.SessionID,
                                            IsParentRow = log.IsParentRow,
                                            ParentRowID = log.ParentRowID,
                                            OnEvent = log.OnEvent,
                                            ElementID = log.ElementID,
                                            ElementPath = log.ElementPath,
                                            ElementLabel = log.ElementLabel,
                                            OldValue = log.OldValue,
                                            NewValue = log.NewValue,
                                            ImpactedElementID = log.ImpactedElementID,
                                            ImpactedElementPath = log.ImpactedElementPath,
                                            ImpactedElementLabel = log.ImpactedElementLabel,
                                            ImpactDescription = log.ImpactDescription,
                                            PropertyType = log.PropertyType,
                                            RuleID = log.RuleID,
                                            FolderVersion = log.FolderVersion,
                                            UpdatedBy = log.UpdatedBy,
                                            UpdatedDate = log.UpdatedDate,
                                            IsNewRecord = false
                                        }).ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord).ToList()
                                      .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<FormInstanceRuleExecutionLogViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, ruleExecutionLogList);
        }

        public ServiceResult SaveFormInstanceRuleExecutionlogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<FormInstanceRuleExecutionLogViewModel> loggerDataJsonObject)
        {
            ServiceResult result = null;

            result = new ServiceResult();
            try
            {
                if (loggerDataJsonObject != null)
                {

                    var loggerNewRecords = loggerDataJsonObject.Where(c => c.IsNewRecord == true).ToList();
                    for (int i = 0; i < loggerNewRecords.Count; i++)
                    {
                        FormInstanceRuleExecutionLog ruleExecutionLog = new FormInstanceRuleExecutionLog();
                        ruleExecutionLog.SessionID = loggerNewRecords[i].SessionID;
                        ruleExecutionLog.FormInstanceID = formInstanceId;
                        ruleExecutionLog.FolderID = folderId;
                        ruleExecutionLog.FolderVersionID = folderVersionId;
                        ruleExecutionLog.FolderVersion = loggerNewRecords[i].FolderVersion;
                        ruleExecutionLog.FormDesignID = formDesignId;
                        ruleExecutionLog.FormDesignVersionID = formDesignVersionId;
                        ruleExecutionLog.IsParentRow = loggerNewRecords[i].IsParentRow;
                        ruleExecutionLog.ParentRowID = loggerNewRecords[i].ParentRowID;
                        ruleExecutionLog.OnEvent = loggerNewRecords[i].OnEvent;
                        ruleExecutionLog.ElementID = loggerNewRecords[i].ElementID;
                        ruleExecutionLog.ElementLabel = loggerNewRecords[i].ElementLabel;
                        ruleExecutionLog.ElementPath = loggerNewRecords[i].ElementPath;
                        ruleExecutionLog.OldValue = loggerNewRecords[i].OldValue;
                        ruleExecutionLog.NewValue = loggerNewRecords[i].NewValue;
                        ruleExecutionLog.ImpactedElementID = loggerNewRecords[i].ImpactedElementID;
                        ruleExecutionLog.ImpactedElementLabel = loggerNewRecords[i].ImpactedElementLabel;
                        ruleExecutionLog.ImpactedElementPath = loggerNewRecords[i].ImpactedElementPath;
                        ruleExecutionLog.ImpactDescription = loggerNewRecords[i].ImpactDescription;
                        ruleExecutionLog.PropertyType = loggerNewRecords[i].PropertyType;
                        ruleExecutionLog.RuleID = loggerNewRecords[i].RuleID;
                        ruleExecutionLog.AddedBy = loggerNewRecords[i].UpdatedBy;
                        ruleExecutionLog.AddedDate = DateTime.Now;
                        ruleExecutionLog.UpdatedBy = loggerNewRecords[i].UpdatedBy;
                        ruleExecutionLog.UpdatedDate = DateTime.Now;
                        ruleExecutionLog.IsNewRecord = true;

                        this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionLog>().Insert(ruleExecutionLog);
                    }
                    this._unitOfWork.Save();
                    HttpContext.Current.Session.Add("CurrrentSessionId", Guid.NewGuid().ToString());
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

        public RuleRowModel GetRuleDescription(int ruleID)
        {
            RuleRowModel objRule = null;
            try
            {
                objRule = (from rule in this._unitOfWork.RepositoryAsync<Rule>().Get()
                            where rule.RuleID == ruleID
                           select new RuleRowModel
                            {
                                RuleId = rule.RuleID,
                                RuleDescription = rule.RuleDescription
                            }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return objRule;
        }

        public List<FormInstanceRuleExecutionServerLogViewModel> GetRuleExecutionServerLogData(int formInstnaceId, string sessionId, int parentRowID, bool isParentData)
        {
            List<FormInstanceRuleExecutionServerLogViewModel> ruleExecutionServerLogList = null;
            try
            {
                ruleExecutionServerLogList = (from serverLog in this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionServerLog>().Get().Where(x => x.FormInstanceID == formInstnaceId && x.SessionID == sessionId && x.IsParentRow == isParentData && x.ParentRowID == (parentRowID != -1 ? parentRowID : x.ParentRowID)).OrderBy(x => x.RowID)
                                              select new FormInstanceRuleExecutionServerLogViewModel
                                              {
                                                  ID = serverLog.RowID,
                                                  SessionID = serverLog.SessionID,
                                                  FormInstanceID = serverLog.FormInstanceID,
                                                  ElementID = serverLog.ElementID,
                                                  LoadedElement = serverLog.LoadedElement,
                                                  IsParentRow = serverLog.IsParentRow,
                                                  ParentRowID = serverLog.ParentRowID,
                                                  OnEvent = serverLog.OnEvent,
                                                  OldValue = serverLog.OldValue,
                                                  NewValue = serverLog.NewValue,
                                                  ImpactedElementID = serverLog.ImpactedElementID,
                                                  RuleID = serverLog.RuleID,
                                                  Result = serverLog.Result,
                                                  LogFor = serverLog.LogFor
                                              }).ToList();

                if(ruleExecutionServerLogList != null)
                {
                    foreach(FormInstanceRuleExecutionServerLogViewModel logentry in ruleExecutionServerLogList)
                    {
                        RemoveFromServerLogData(logentry.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ruleExecutionServerLogList;
        }

        private void RemoveFromServerLogData(int RowId)
        {
            this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionServerLog>().Delete(RowId);
            this._unitOfWork.Save();
        }
        public void SaveFormInstanceRuleExecutionServerlogData(int formInstanceId, int parentRowID, RuleDesign rule, bool result)
        {
            FormInstanceRuleExecutionServerLog ruleExecutionServerLog = new FormInstanceRuleExecutionServerLog();
            ruleExecutionServerLog.SessionID = System.Web.HttpContext.Current.Session["CurrrentSessionId"].ToString();
            ruleExecutionServerLog.FormInstanceID = formInstanceId;
            ruleExecutionServerLog.LoadedElement = "";
            ruleExecutionServerLog.IsParentRow = false;
            ruleExecutionServerLog.ParentRowID = parentRowID;
            ruleExecutionServerLog.OnEvent = "";
            ruleExecutionServerLog.ElementID = -1;
            ruleExecutionServerLog.OldValue = "";
            ruleExecutionServerLog.NewValue = (rule.TargetPropertyTypeId == 4?(result?(rule.SuccessValue != null ? rule.SuccessValue : "") :(rule.FailureValue != null ? rule.FailureValue : "")) :"");
            ruleExecutionServerLog.ImpactedElementID = rule.UIELementID;
            ruleExecutionServerLog.RuleID = rule.RuleID;
            ruleExecutionServerLog.Result = result;
            ruleExecutionServerLog.LogFor = "OnElement";

            this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionServerLog>().Insert(ruleExecutionServerLog);
            this._unitOfWork.Save();
        }

        public int SaveFormInstanceRuleExecutionServerlogDataOnLoad(int formInstanceId,int elementID, string loadedElement)
        {
            FormInstanceRuleExecutionServerLog ruleExecutionServerLog = new FormInstanceRuleExecutionServerLog();
            ruleExecutionServerLog.SessionID = System.Web.HttpContext.Current.Session["CurrrentSessionId"].ToString();
            ruleExecutionServerLog.FormInstanceID = formInstanceId;
            ruleExecutionServerLog.LoadedElement = loadedElement;
            ruleExecutionServerLog.IsParentRow = true;
            ruleExecutionServerLog.ParentRowID = -1;
            ruleExecutionServerLog.OnEvent = "On Load Server";
            ruleExecutionServerLog.ElementID = elementID;
            ruleExecutionServerLog.OldValue = "";
            ruleExecutionServerLog.NewValue = "";
            ruleExecutionServerLog.ImpactedElementID =-1;
            ruleExecutionServerLog.RuleID =-1;
            ruleExecutionServerLog.Result = false;
            ruleExecutionServerLog.LogFor = "OnLoad";

            this._unitOfWork.RepositoryAsync<FormInstanceRuleExecutionServerLog>().Insert(ruleExecutionServerLog);
            this._unitOfWork.Save();
            return ruleExecutionServerLog.RowID;
        }
    }
}
