using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdate;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public partial class GlobalUpdateBatchService : IGlobalUpdateBatchService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        private IGlobalUpdateService _globalUpdateService { get; set; }
        private IIASDocumentService _iasDocumentService { get; set; }
        private bool winServiceresult { get; set; }

        public GlobalUpdateBatchService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _globalUpdateService = new GlobalUpdateService(_unitOfWork);
            _iasDocumentService = new IASDocumentService(_unitOfWork);
        }

        public bool ExecuteGlobalUpdateBatch()
        {
            try
            {
                ExecuteAvailableBatchesData();
                winServiceresult = true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                winServiceresult = false;
                if (reThrow) throw ex;

            }
            return winServiceresult;
        }

        //This method will decide the initialization of Windows Service
        public bool CheckIfBatchExitsForProcess()
        {
            try
            {
                var noOfBatches = GetAvailableBatches();
                winServiceresult = (noOfBatches != null && noOfBatches.Count() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return winServiceresult;
        }

        private List<Guid> GetAvailableBatches()
        {
            string todaysDate = DateTime.Now.ToShortDateString();
            var executedBatches = (from batchExecution in this._unitOfWork.Repository<BatchExecution>().Query().Get() select batchExecution.BatchID);

            //TODO- Need to store execution type in DB instead of string and hence change the following hardcoded execution type through enum
            var noOfBatches = (from batch in this._unitOfWork.Repository<Batch>().Query().Filter(x => x.IsApproved == true && x.ExecutionType == "Scheduled"
                                   && !executedBatches.Contains(x.BatchID)).Get()
                               select new
                               {
                                   BatchId = batch.BatchID,
                                   ScheduleDate = (DateTime)batch.ScheduleDate
                               }).ToList();

            var batchIds = (from batches in noOfBatches where batches.ScheduleDate.ToShortDateString() == todaysDate select batches.BatchId).ToList();

            return batchIds;
        }

        private ServiceResult ExecuteAvailableBatchesData()
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                string todaysDate = DateTime.Now.ToShortDateString();
                var executedBatches = (from batchExecution in this._unitOfWork.Repository<BatchExecution>().Query().Get() select batchExecution.BatchID);

                var noOfBatches = (from batch in this._unitOfWork.Repository<Batch>().Query()
                                       .Filter(x => x.IsApproved == true && x.ExecutionType == "Scheduled"
                                           && !executedBatches.Contains(x.BatchID)).Get()
                                   select new
                                   {
                                       BatchID = batch.BatchID,
                                       BatchName = batch.BatchName,
                                       ScheduleDate = (DateTime)batch.ScheduleDate
                                   }).ToList();

                var batchesData = (from batches in noOfBatches where batches.ScheduleDate.ToShortDateString() == todaysDate select batches).ToList();

                if (batchesData.Count > 0)
                {
                    foreach (var batch in batchesData)
                    {
                        serviceResult = this._globalUpdateService.ExecuteBatch(batch.BatchID, batch.BatchName, 1, "SuperUser", 19);
                    }
                }
            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return serviceResult;
        }

        //This method will decide the initialization of Windows Service
        public bool CheckIfIASExistsForProcess()
        {
            try
            {
                var noOfIAS = GetAvailableIAS();
                winServiceresult = (noOfIAS != null && noOfIAS.Count() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return winServiceresult;
        }

        private List<int> GetAvailableIAS()
        {
            var noOfIAS = (from ias in this._unitOfWork.Repository<GlobalUpdate>().Query()
                               .Filter(f => f.IsIASDownloaded == true)
                               .Get()
                           select ias.GlobalUpdateID).ToList(); 

            return noOfIAS;
        }

        public bool ExecuteIASGeneration(string iasfolderPath)
        {
            try
            {

                GlobalUpdate iasData = (from s in this._unitOfWork.Repository<GlobalUpdate>().Query()
                                               .Filter(x => x.IsIASDownloaded == true)
                                               .Get()
                                        select s).FirstOrDefault();
                if (iasData != null)
                {
                    ExecuteAvailableIASData(iasfolderPath, iasData);
                }

                winServiceresult = true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }

            return winServiceresult;
        }

        private ServiceResult ExecuteAvailableIASData(string iasfolderPath, GlobalUpdate ias)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                string folderPath = iasfolderPath;
                string filePath = string.Empty;

                IEnumerable<FormDesignElementValueVeiwModel> globalUpdatesUIElementsList = _globalUpdateService.GetFormDesignVersionUIElements(ias.GlobalUpdateID);
                if (globalUpdatesUIElementsList.Count() > 0)
                {
                    ServiceResult resultScheduleGlobalUpdate = _globalUpdateService.ScheduleGlobalUpdate(ias.GlobalUpdateID, false, "SuperUser");
                    if (resultScheduleGlobalUpdate.Result == ServiceResultStatus.Success)
                    {
                        IEnumerable<IASFolderDataModel> IASFolderDataList = _globalUpdateService.GetGlobalUpdateImpactedFolderVersionList(ias.GlobalUpdateID, ias.EffectiveDateFrom, ias.EffectiveDateTo, 1);

                        ServiceResult resultIASFolderData = _globalUpdateService.SaveIASFolderDataValues(ias.GlobalUpdateID, IASFolderDataList, "SuperUser");

                        if (resultIASFolderData.Result == ServiceResultStatus.Success)
                        {
                            ServiceResult resultIASElementExportData = null;

                            IEnumerable<IASFolderDataModel> globalUpdatesIASFolderDataList = _globalUpdateService.GetGlobalUpdatesFolderDataList(ias.GlobalUpdateID);

                            resultIASElementExportData = _globalUpdateService.SaveIASElementExportDataValues(ias.GlobalUpdateID, globalUpdatesIASFolderDataList, "SuperUser");

                            if (resultIASElementExportData.Result == ServiceResultStatus.Success)
                            {
                                serviceResult = this._iasDocumentService.ExportIASExcelTemplate(ias.GlobalUpdateID, ias.GlobalUpdateName, ias.EffectiveDateFrom, ias.EffectiveDateTo, folderPath, out filePath);
                            }
                        }
                    }
                }

                if (serviceResult == null || serviceResult.Result == ServiceResultStatus.Failure)
                {
                    this._iasDocumentService.UpdateGlobalUpdateIASStatus(ias.GlobalUpdateID, (int)GlobalUpdateIASStatus.IASDownloadFailed);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return serviceResult;
        }
        //This method will decide the initialization of Windows Service
        public bool CheckIfErrorLogExistsForProcess()
        {
            try
            {
                var noOfErrorLog = GetAvailableErrorLog();
                winServiceresult= (noOfErrorLog != null && noOfErrorLog.Count() > 0) ? true : false;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return winServiceresult;
        }

        private List<int> GetAvailableErrorLog()
        {
            var noOfErrorLog = (from ias in this._unitOfWork.Repository<GlobalUpdate>().Query()
                                    .Filter(f => f.IsErrorLogDownloaded == true)
                                    .Get()
                                select ias.GlobalUpdateID).ToList();

            return noOfErrorLog;
        }

        public bool ExecuteErrorLogGeneration(string errorLogfolderPath, string importIASPath)
        {
            //string text = "This is the demo text to check windows service, " +
            //          "begin the batch execution from this method. ";

            //System.IO.File.WriteAllText(@"D:\DemoFile" + DateTime.Now.ToString("HHmmss") + ".txt", text);

            try
            {
                GlobalUpdate iasData = (from s in this._unitOfWork.Repository<GlobalUpdate>().Query()
                                               .Filter(x => x.IsErrorLogDownloaded == true)
                                               .Get()
                                        select s).FirstOrDefault();
                if (iasData != null)
                {
                    ExecuteAvailableErrorLogData(errorLogfolderPath, importIASPath, iasData);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return true;
        }

        private ServiceResult ExecuteAvailableErrorLogData(string errorLogfolderPath, string importIASPath, GlobalUpdate ias)
        {
            ServiceResult serviceResult = new ServiceResult();
            try
            {
                string folderPath = errorLogfolderPath;
                string filePath = string.Empty;

                IASFileUpload fileUpload = (from iasFileUpload in this._unitOfWork.RepositoryAsync<IASFileUpload>().Query()
                                            .Filter(x => x.GlobalUpdateID == ias.GlobalUpdateID)
                                            .Get()
                                            select iasFileUpload).OrderByDescending(ord => ord.IASFileUploadID).FirstOrDefault();
                string globalTemplateGuid = fileUpload.TemplateGuid;
                string importPath = importIASPath + globalTemplateGuid;
                ServiceResult resultScheduleIASUpload = _globalUpdateService.ScheduleIASUpload(ias.GlobalUpdateID, false, "SuperUser");
                if (resultScheduleIASUpload.Result == ServiceResultStatus.Success)
                {
                    serviceResult = this._iasDocumentService.ValidateIASExcelTemplate(ias.GlobalUpdateID, ias.GlobalUpdateName, ias.EffectiveDateFrom, ias.EffectiveDateTo, importPath, "SuperUser", folderPath, out filePath);
                }

                if (serviceResult == null || serviceResult.Result == ServiceResultStatus.Failure)
                {
                    this._iasDocumentService.UpdateGlobalUpdateIASStatus(ias.GlobalUpdateID, (int)GlobalUpdateIASStatus.ErrorLogDownloadFailed);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                throw ex;
            }
            return serviceResult;
        }
    }
}
