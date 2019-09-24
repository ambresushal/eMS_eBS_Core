using System;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.comparesync;
using System.Collections.Generic;
using tmg.equinox.repository.extensions;


namespace tmg.equinox.applicationservices
{
    public class SyncDocumentService : ISyncDocumentService
    {
        #region Private Members

        private IUnitOfWorkAsync _unitOfWork { get; set; }

        #endregion Private Members

        #region Constructor

        public SyncDocumentService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion Constructor


        public GridPagingResponse<SyncDocumentMacroViewModel> GetMacroList(int tenantId, int formInstanceID, GridPagingRequest request, string userName, int roleID)
        {
            List<SyncDocumentMacroViewModel> macros = null;
            int count = 0;
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(request.filters);

                //Get FormDesin ID from FormInstanceID
                int formDesignID = 0;
                var formInstance = this._unitOfWork.RepositoryAsync<FormInstance>().Get().Where(s => s.FormInstanceID == formInstanceID).FirstOrDefault();
                if (formInstance != null)
                {
                    formDesignID = formInstance.FormDesignID;
                    if (formDesignID > 0)
                    {
                        var results = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                                       where m.FormDesignID == formDesignID
                                       select m);

                        if (roleID != 1 && roleID != 11)
                        {
                            results = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                                       where m.FormDesignID == formDesignID && (m.isPublic == true || m.AddedBy == userName)
                                       select m);
                        }

                        macros = results.Select(m => new SyncDocumentMacroViewModel()
                        {
                            AddedBy = m.AddedBy,
                            AddedDate = m.AddedDate,
                            FormDesignID = m.FormDesignID,
                            FormDesignVersionID = m.FormDesignVersionID,
                            MacroID = m.MacroID,
                            MacroName = m.MacroName,
                            Notes = m.Notes,
                            IsPublic = m.isPublic,
                            MacroJSON = m.MacroJSON,
                            UpdatedBy = m.UpdatedBy,
                            UpdatedDate = m.UpdatedDate
                        }).ApplySearchCriteria(criteria).ApplyOrderBy(request.sidx, request.sord)
                                    .GetPage(request.page, request.rows, out count)
                                    .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return new GridPagingResponse<SyncDocumentMacroViewModel>(request.page, count, request.rows, macros);
        }

        public SyncDocumentMacroViewModel GetMacroById(int macroID)
        {
            SyncDocumentMacroViewModel model = null;
            try
            {
                model = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                         where m.MacroID == macroID
                         select new SyncDocumentMacroViewModel
                         {
                             MacroID = m.MacroID,
                             MacroName = m.MacroName,
                             MacroJSON = m.MacroJSON,
                             Notes = m.Notes,
                             IsPublic = m.isPublic,
                             FormDesignID = m.FormDesignID,
                             FormDesignVersionID = m.FormDesignVersionID
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

        public GridPagingResponse<SyncGroupLogViewModel> GetGroupLogList(int tenantId, int macroId, GridPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public GridPagingResponse<SyncDocumentLogViewModel> GetDocumentListForGroup(int tenantId, int syncGroupLogId, GridPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public string GetMacroJSONString(int tenantId, int macroId)
        {
            string json = "";
            try
            {
                var macro = this._unitOfWork.RepositoryAsync<SyncDocumentMacro>()
                                        .Get()
                                        .Where(s => s.MacroID == macroId)
                                        .FirstOrDefault();

                if (macro != null)
                {
                    json = macro.MacroJSON;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return json;
        }

        public ServiceResult InsertMacro(int tenantId, int formInstanceID, SyncDocumentMacroViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SyncDocumentMacro objMacro = new SyncDocumentMacro()
                {
                    MacroName = model.MacroName,
                    Notes = model.Notes,
                    FormDesignID = model.FormDesignID,
                    MacroJSON = model.MacroJSON,
                    FormDesignVersionID = model.FormDesignVersionID,
                    isLocked = false,
                    isPublic = model.IsPublic,
                    AddedBy = model.AddedBy,
                    AddedDate = model.AddedDate
                };

                this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Insert(objMacro);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem { Messages = new string[] { objMacro.MacroID.ToString() } });
                result.Items = items;
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

        public ServiceResult isMacroExist(string macroName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SyncDocumentMacro macro = this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                    .Where(a => a.MacroName.ToString() == macroName).FirstOrDefault();

                if (macro == null)
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else {
                    result.Result = ServiceResultStatus.Failure;
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem { Messages = new string[] { "Please Enter Unique Macro Name" } });
                    result.Items = items;
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

        public ServiceResult UpdateMacro(int tenantId, int macroId, string macroJSON)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var model = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                             where m.MacroID == macroId
                             select m).FirstOrDefault();
                if (model != null)
                {
                    model.MacroJSON = macroJSON;

                    this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Update(model);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
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

        public ServiceResult DeleteMacro(int tenantId, int macroId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var model = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                             where m.MacroID == macroId
                             select m).FirstOrDefault();
                if (model != null)
                {
                    this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Delete(model.MacroID);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
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

        public ServiceResult CopyMacro(int tenantId, int macroId, string macroName, string notes, bool isPublic)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var model = (from m in this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Get()
                             where m.MacroID == macroId
                             select m).FirstOrDefault();
                if (model != null)
                {
                    model.MacroName = macroName;
                    model.Notes = notes;
                    model.isPublic = isPublic;
                    this._unitOfWork.RepositoryAsync<SyncDocumentMacro>().Insert(model);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem { Messages = new string[] { model.MacroID.ToString() } });
                    result.Items = items;
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

        public string GetSourceDocumentData(int formInstanceID)
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

        public ServiceResult InsertSyncGroupLog(SyncGroupLogViewModel model, string currentUser)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SyncGroupLog groupLog = new SyncGroupLog();
                groupLog.FolderID = model.FolderID;
                groupLog.FolderVersionID = model.FolderVersionID;
                groupLog.FolderVersionNumber = model.FolderVersionNumber;
                groupLog.MacroID = model.MacroID;
                groupLog.SourceDocumentID = model.SourceDocumentID;
                groupLog.SyncBy = currentUser;
                groupLog.SyncDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<SyncGroupLog>().Insert(groupLog);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem { Messages = new string[] { groupLog.SyncGroupLogID.ToString() } });
                result.Items = items;
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
        public ServiceResult InsertSyncDocumentLogs(IList<SyncDocumentLogViewModel> models)
        {
            ServiceResult result = null;
            result = new ServiceResult();
            try
            {
                if (models != null)
                {
                    var syncDocs = from model in models
                                   select new SyncDocumentLog
                                   {
                                       FolderID = model.FolderID,
                                       FolderVersionID = model.FolderVersionID,
                                       IsSyncAllowed = model.IsSyncAllowed,
                                       LastUpdatedDate = DateTime.Now,
                                       Notes = model.Notes,
                                       SourceDocumentID = model.SourceDocumentID,
                                       SyncCompleted = model.SyncCompleted,
                                       SyncGroupLogID = model.SyncGroupLogID,
                                       TargetDesignID = model.TargetDesignID,
                                       TargetDesignVersionID = model.TargetDesignVersionID,
                                       TargetDocumentID = model.TargetDocumentID,
                                   };
                    this._unitOfWork.RepositoryAsync<SyncDocumentLog>().InsertRange(syncDocs);
                    this._unitOfWork.Save();
                }
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

        
    }
}
