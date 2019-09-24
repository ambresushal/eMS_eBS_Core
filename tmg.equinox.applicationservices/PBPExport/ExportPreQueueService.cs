using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Data;
using tmg.equinox.domain.entities.Enums;
using System;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Data.SqlClient;

namespace tmg.equinox.applicationservices.PBPExport
{
    public class ExportPreQueueService : IExportPreQueueService
    {

        #region Private Memebers
        public IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion

        public ExportPreQueueService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public ExportPreQueueViewModel GetExportPreQueueList()
        {
            ExportPreQueueViewModel PreQueueList = (from q in this._unitOfWork.RepositoryAsync<ExportPreQueue>().Get()
                                                    where q.PreQueueStatus.Equals(ExportPreStatus.InQueued)
                                                    && q.IsActive == true
                                                    select new ExportPreQueueViewModel
                                                    {
                                                        ExportPreQueue1Up = q.ExportPreQueue1Up,
                                                        PBPDatabase1Up = q.PBPDatabase1Up,
                                                        PreQueueStatus = q.PreQueueStatus,
                                                        ExportId = q.PBPExportId,
                                                        AddedBy = q.AddedBy,
                                                        UserId = q.CurrentUserId
                                                    }).OrderBy(s => s.ExportPreQueue1Up)
                                                    .FirstOrDefault();
            return PreQueueList;
        }

        public ServiceResult UpdateStatus(int exportPreQueueId, string status)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ExportPreQueue itemToUpdateList = this._unitOfWork.RepositoryAsync<ExportPreQueue>().Get()
                                                .Where(s => s.ExportPreQueue1Up.Equals(exportPreQueueId)
                                                ).FirstOrDefault();

                if (itemToUpdateList != null)
                {
                    itemToUpdateList.PreQueueStatus = status;
                    itemToUpdateList.UpdatedBy = "TMG Super User";
                    itemToUpdateList.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<ExportPreQueue>().Update(itemToUpdateList, true);
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

        public ServiceResult PreQueueExport(int pBPExportId, int pbpDatabaseId, string addedBy, int? userId)
        {
            ServiceResult result = new ServiceResult();

            ExportPreQueue AddPreQueue = new ExportPreQueue();
            AddPreQueue.PBPDatabase1Up = pbpDatabaseId;
            AddPreQueue.PBPExportId = pBPExportId;
            AddPreQueue.PreQueueStatus = ExportPreStatus.InQueued;
            AddPreQueue.AddedBy = addedBy;
            AddPreQueue.AddedDate = DateTime.Now;
            AddPreQueue.IsActive = true;
            AddPreQueue.CurrentUserId = Convert.ToInt32(userId);
            this._unitOfWork.RepositoryAsync<ExportPreQueue>().Insert(AddPreQueue);
            this._unitOfWork.Save();
            return result;
        }

        public ServiceResult UpdatePreQueueLog(int forminstanceId, int exportPreQueueId, string status, Exception errorLog)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string ErrorMsg = string.Empty;
                if (errorLog != null)
                {
                    result = ExceptionExtensions.ExceptionMessages(errorLog);
                }
                foreach (var item in result.Items)
                {
                    foreach (var ele in item.Messages)
                    {
                        ErrorMsg += ele;
                    }
                }

                    SqlParameter paramTenantId = new SqlParameter("@status", status);
                    SqlParameter paramProcGovernance = new SqlParameter("@ErrorLog", ErrorMsg);
                    SqlParameter sqlforminstanceId = new SqlParameter("@FromInstanceId", forminstanceId);
                    SqlParameter sqlexportPreQueueId = new SqlParameter("@ExportPreQueueId", exportPreQueueId);
               
                var log = this._unitOfWork.Repository<ExportPreQueueLog>()
                         .ExecuteSql("exec [dbo].[UpdateExportPreQueueLog] @status,@ErrorLog,@FromInstanceId,@ExportPreQueueId",
                         paramTenantId, paramProcGovernance, sqlforminstanceId, sqlexportPreQueueId).ToList().FirstOrDefault();
                result.Result = ServiceResultStatus.Success;
               
            }
            catch (Exception ex)
            {
                //result.Result = ServiceResultStatus.Failure;
                //bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                //if (reThrow)
                //    throw;
            }
            return result;
        }

        public ServiceResult SaveFormInstanceForPreQueue(List<ExportPreQueueLog> forinstanceIdList)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                this._unitOfWork.RepositoryAsync<ExportPreQueueLog>().InsertRange(forinstanceIdList);
                this._unitOfWork.Save();
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
    }
}
