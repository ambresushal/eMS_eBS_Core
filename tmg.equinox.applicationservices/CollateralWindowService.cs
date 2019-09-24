using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.CollateralWindowService;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class CollateralWindowService : ICollateralWindowServices
    {
        #region Private Memebers

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private string UserName { get; set; }

        #endregion Private Members

        #region Constructor
        public CollateralWindowService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public IEnumerable<viewmodels.CollateralWindowService.CollateralProcessGovernanceViewModel> GetCollateralProcessGovernance()
        {
            IList<CollateralProcessGovernanceViewModel> collateralProcessGovernList = null;
            try
            {
                collateralProcessGovernList = (from c in this._unitOfWork.RepositoryAsync<CollateralProcessGovernance>()
                                                                                 .Query()
                                                                                 .Filter(c => c.ProcessStatus1Up == 1)
                                                                                 .OrderBy(c => c.OrderByDescending(d => d.AddedDate))
                                                                                 .Get()
                                               select new CollateralProcessGovernanceViewModel
                                         {
                                             ProcessGovernance1up = c.ProcessGovernance1up,
                                             Processor1Up = c.Processor1Up,
                                             ProcessStatus1Up = c.ProcessStatus1Up,
                                             ProcessType = c.ProcessType,
                                             RunDate = c.RunDate,
                                             StartDate = c.StartDate,
                                             EndDate = c.EndDate,
                                             AddedBy = c.AddedBy,
                                             AddedDate = c.AddedDate,
                                             IsActive = c.IsActive,
                                             HasError = c.HasError,
                                             ErrorDescription = c.ErrorDescription

                                         }).ToList();
                if (collateralProcessGovernList.Count() == 0)
                    collateralProcessGovernList = new List<CollateralProcessGovernanceViewModel>();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return collateralProcessGovernList;
        }

        public IEnumerable<viewmodels.CollateralWindowService.CollateralProcessQueueViewModel> GetCollateralProcessQueue(int ProcessGovernance1up)
        {
            IList<CollateralProcessQueueViewModel> collateralProcessQueueList = null;
            try
            {
                collateralProcessQueueList = (from c in this._unitOfWork.RepositoryAsync<CollateralProcessQueue>()
                                                                                 .Query()
                                                                                 .Filter(c => c.ProcessGovernance1Up == ProcessGovernance1up)
                                                                                 .Get()
                                              select new CollateralProcessQueueViewModel
                                               {

                                                   CollateralProcessQueue1Up = c.CollateralProcessQueue1Up,
                                                   ProcessGovernance1Up = c.ProcessGovernance1Up,
                                                   ProductID = c.ProductID,
                                                   ProductName = c.ProductName,
                                                   AccountID = c.AccountID,
                                                   AccountName = c.AccountName,
                                                   FolderID = c.FolderID,
                                                   FolderName = c.FolderName,
                                                   FolderVersionID = c.FolderVersionID,
                                                   FolderVersionNumber = c.FolderVersionNumber,
                                                   FormInstanceID = c.FormInstanceID,
                                                   FormInstanceName = c.FormInstanceName,
                                                   FormDesignID = c.FormDesignID,
                                                   FormDesignVersionID = c.FormDesignVersionID,
                                                   EffectiveDate = c.EffectiveDate,
                                                   StartTime = c.StartTime,
                                                   EndTime = c.EndTime,
                                                   ProcessStatus1Up = c.ProcessStatus1Up,
                                                   TemplateReportID = c.TemplateReportID,
                                                   TemplateReportVersionID = c.TemplateReportVersionID,
                                                   TemplateReportVersionEffectiveDate = c.TemplateReportVersionEffectiveDate,
                                                   CollateralStorageLocation = c.CollateralStorageLocation,
                                                   HasError = c.HasError,
                                                   IsActive = c.IsActive,
                                                   CreatedBy = c.CreatedBy,
                                                   CreatedDate = c.CreatedDate,
                                                   UpdatedBy = c.UpdatedBy,
                                                   UpdatedDate = c.UpdatedDate,
                                                   ErrorDescription = c.ErrorDescription,
                                                   FilePath = c.FilePath

                                               }).ToList();
                if (collateralProcessQueueList.Count() == 0)
                    collateralProcessQueueList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return collateralProcessQueueList;
        }

        public IEnumerable<CollateralProcessQueueViewModel> GetStatuswiseCollateralProcessQueue(int ProcessGovernance1up, int status1up)
        {
            List<CollateralProcessQueueViewModel> collateralProcessQueueList = null;
            try
            {
                collateralProcessQueueList = (from c in this._unitOfWork.RepositoryAsync<CollateralProcessQueue>()
                                                                                 .Query()
                                                                                 .Filter(c => c.ProcessGovernance1Up == ProcessGovernance1up & c.ProcessStatus1Up == status1up)
                                                                                 .Get()
                                              select new CollateralProcessQueueViewModel
                                              {

                                                  CollateralProcessQueue1Up = c.CollateralProcessQueue1Up,
                                                  ProcessGovernance1Up = c.ProcessGovernance1Up,
                                                  ProductID = c.ProductID,
                                                  ProductName = c.ProductName,
                                                  AccountID = c.AccountID,
                                                  AccountName = c.AccountName,
                                                  FolderID = c.FolderID,
                                                  FolderName = c.FolderName,
                                                  FolderVersionID = c.FolderVersionID,
                                                  FolderVersionNumber = c.FolderVersionNumber,
                                                  FormInstanceID = c.FormInstanceID,
                                                  FormInstanceName = c.FormInstanceName,
                                                  FormDesignID = c.FormDesignID,
                                                  FormDesignVersionID = c.FormDesignVersionID,
                                                  EffectiveDate = c.EffectiveDate,
                                                  StartTime = c.StartTime,
                                                  EndTime = c.EndTime,
                                                  ProcessStatus1Up = c.ProcessStatus1Up,
                                                  TemplateReportID = c.TemplateReportID,
                                                  TemplateReportVersionID = c.TemplateReportVersionID,
                                                  TemplateReportVersionEffectiveDate = c.TemplateReportVersionEffectiveDate,
                                                  CollateralStorageLocation = c.CollateralStorageLocation,
                                                  HasError = c.HasError,
                                                  IsActive = c.IsActive,
                                                  CreatedBy = c.CreatedBy,
                                                  CreatedDate = c.CreatedDate,
                                                  UpdatedBy = c.UpdatedBy,
                                                  UpdatedDate = c.UpdatedDate,
                                                  ErrorDescription = c.ErrorDescription,
                                                  FilePath = c.FilePath

                                              }).ToList();

                if (collateralProcessQueueList == null)
                    collateralProcessQueueList = new List<CollateralProcessQueueViewModel>();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return collateralProcessQueueList;
        }

        public void UpdateCollateralProcessGovData(int processGovernance1up, int processStatus1Up)
        {
            CollateralProcessGovernance pGovernance = _unitOfWork.RepositoryAsync<CollateralProcessGovernance>().FindById(processGovernance1up);

            if (pGovernance != null)
            {
                pGovernance.ProcessStatus1Up = processStatus1Up;
                this._unitOfWork.Clear(pGovernance);
                this._unitOfWork.RepositoryAsync<CollateralProcessGovernance>().UpdateCollateralEntity(pGovernance);
                this._unitOfWork.Save();
            }
        }

        public void UpdateCollateralStatus(int collateralProcessQueue1Up, int processStatus1Up, string message)
        {
            CollateralProcessQueue pQueue = _unitOfWork.RepositoryAsync<CollateralProcessQueue>().FindById(collateralProcessQueue1Up);
            if (pQueue != null)
            {
                ServiceResult result = new ServiceResult();
                pQueue.ProcessStatus1Up = processStatus1Up;
                pQueue.UpdatedDate = DateTime.Now;
                this._unitOfWork.Clear<CollateralProcessQueue>(pQueue);
                this._unitOfWork.Repository<CollateralProcessQueue>().UpdateCollateralEntity(pQueue);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }

            if (!string.IsNullOrEmpty(message))
            {
                CollateralProcessQueueStatus queue = new CollateralProcessQueueStatus();
                queue.CollateralProcessQueue1Up = collateralProcessQueue1Up;
                queue.Message = message;
                this._unitOfWork.RepositoryAsync<CollateralProcessQueueStatus>().Insert(queue);
                this._unitOfWork.Save();
            }
        }

        public void UpdateCollateralFilePath(string strWordPath, ColleteralFilePath strPDFPath, int collateralProcessQueue1Up, int processStatus1Up)
        {
            CollateralProcessQueue pQueue = _unitOfWork.RepositoryAsync<CollateralProcessQueue>().FindById(collateralProcessQueue1Up);
            if (pQueue != null)
            {
                pQueue.ProcessStatus1Up = processStatus1Up;
                pQueue.FilePath = strPDFPath.PDF;
                pQueue.PrintxFilePath = strPDFPath.PrintX;
                pQueue.WordFilePath = strWordPath;
                pQueue.UpdatedDate = DateTime.Now;
                this._unitOfWork.Clear(pQueue);
                this._unitOfWork.RepositoryAsync<CollateralProcessQueue>().UpdateCollateralEntity(pQueue);
                this._unitOfWork.Save();
            }
        }

        public bool HasSBDesignTemplate(int templateReportID, int templateReportVersionID, out int formDesignVersionID)
        {
            bool result = false;
            formDesignVersionID = 0;

            var templateFormDesignMap = (from map in this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                         join design in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on map.FormDesignID equals design.FormID
                                         where map.TemplateReportID == templateReportID
                                         && map.TemplateReportVersionID == templateReportVersionID
                                         && design.FormName == "SBDesign"
                                         select map).FirstOrDefault();

            if (templateFormDesignMap != null)
            {
                result = true;
                formDesignVersionID = templateFormDesignMap.FormDesignVersionID;
            }

            return result;
        }

        public bool HasEOCCombinedDesignTemplate(int templateReportID, int templateReportVersionID, out int formDesignVersionID)
        {
            bool result = false;
            formDesignVersionID = 0;

            string[] templateList = { };

            if (ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != null && ConfigurationManager.AppSettings["CombinedEOCTemplateList"] != "")
            {
                templateList = ConfigurationManager.AppSettings["CombinedEOCTemplateList"].Split(',');
            }

            var templateFormDesignMap = (from tmplt in _unitOfWork.Repository<TemplateReport>().Get()
                                         join map in this._unitOfWork.RepositoryAsync<TemplateReportFormDesignVersionMap>().Get()
                                         on tmplt.TemplateReportID equals map.TemplateReportID
                                         join design in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on map.FormDesignID equals design.FormID
                                         where map.TemplateReportID == templateReportID
                                         && map.TemplateReportVersionID == templateReportVersionID
                                         && design.FormName == "MedicareANOC/EOCDesign"
                                         && templateList.Contains(tmplt.TemplateReportName)
                                         select map).FirstOrDefault();

            if (templateFormDesignMap != null)
            {
                result = true;
                formDesignVersionID = templateFormDesignMap.FormDesignVersionID;
            }

            return result;
        }

        public List<SBCollateralProcessQueueViewModel> HasMultilpeAnchorDocument(int queue1Up)
        {
            var anchorDocuments = (from anchor in this._unitOfWork.RepositoryAsync<SBCollateralProcessQueue>().Get()
                                   where anchor.CollateralProcessQueue1Up == queue1Up
                                   select new SBCollateralProcessQueueViewModel
                                   {
                                       FolderID = anchor.FolderID,
                                       FolderName = anchor.FolderName,
                                       FolderVersionEffectiveDate = anchor.FolderVersionEffectiveDate,
                                       FolderVersionID = anchor.FolderVersionID,
                                       FolderVersionNumber = anchor.FolderVersionNumber,
                                       FormDesignID = anchor.FormDesignID,
                                       FormDesignVersionID = anchor.FormDesignVersionID,
                                       FormInstanceID = anchor.FormInstanceID,
                                       FormInstanceName = anchor.FormInstanceName
                                   }).ToList();
            return anchorDocuments;
        }

        public List<string> GetDataAndDesignJSON(int? formDesignId, int? formDesignVersionId, int? formInstanceId)
        {
            List<string> jsonList = new List<string>();
            string formDesignVersionData = _unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                                                                 .Where(c => c.FormDesignID == formDesignId && c.FormDesignVersionID == formDesignVersionId)
                                                                                 .OrderByDescending(c => c.AddedDate)
                                                                                 .Select(c => c.FormDesignVersionData).FirstOrDefault();

            string formdata = _unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FormInstanceDataMap>().Get()
                                                                                 .Where(c => c.FormInstanceID == formInstanceId)
                                                                                 .Select(c => c.FormData).FirstOrDefault();

            jsonList.Add(formdata);
            jsonList.Add(formDesignVersionData);

            return jsonList;
        }

        public void AddLogEntry(int tenantID, int processGovernance1up, int severity, string message)
        {
            SqlParameter paramTenantId = new SqlParameter("@tenantId", tenantID);
            SqlParameter paramProcGovernance = new SqlParameter("@ProcessGovernance1up", processGovernance1up);
            SqlParameter paramSeverity = new SqlParameter("@Severity", severity);
            SqlParameter paramMessage = new SqlParameter("@Message", message);
            var log = this._unitOfWork.Repository<LogActivityDetail>().ExecuteSql("exec [Log].[LogActivity] @tenantId, @ProcessGovernance1up,@Severity,@Message", paramTenantId, paramProcGovernance, paramSeverity, paramMessage).ToList();
        }
        public void LoadActivity(int tenantID, int processGovernance1up, string message)
        {
            SqlParameter paramTenantId = new SqlParameter("@TenantID", tenantID);
            SqlParameter paramProcGovernance = new SqlParameter("@ProcessGovernance1up", processGovernance1up);
            SqlParameter paramMessage = new SqlParameter("@ActivityContextName", message);

            var log = this._unitOfWork.Repository<LogActivityDetail>().ExecuteSql("exec [Log].[LoadActivityContext] @tenantId, @ProcessGovernance1up,@ActivityContextName", paramTenantId, paramProcGovernance, paramMessage).ToList();
        }

        public void UnLoadActivity(int tenantID, int processGovernance1up, string message)
        {
            SqlParameter paramTenantId = new SqlParameter("@TenantID", tenantID);
            SqlParameter paramProcGovernance = new SqlParameter("@ProcessGovernance1up", processGovernance1up);
            SqlParameter paramMessage = new SqlParameter("@ActivityContextName", message);
            var log = this._unitOfWork.Repository<LogActivityDetail>().ExecuteSql("exec [Log].[UnloadActivityContext] @tenantId, @ProcessGovernance1up,@ActivityContextName", paramTenantId, paramProcGovernance, paramMessage).ToList();
        }

        public FormInstanceViewModel GetAnchorFromView(int viewFormInstanceId)
        {
            FormInstance form =  _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                                           .Where(i => i.FormInstanceID == viewFormInstanceId).FirstOrDefault();

            IEnumerable<FormInstanceViewModel> formInstanceIDList = (from instance in _unitOfWork.RepositoryAsync<FormInstance>().Get()
                                                                            .Where(i => i.FormInstanceID == form.AnchorDocumentID)
                                                                     select new FormInstanceViewModel
                                                                     {
                                                                         FormInstanceID = instance.FormInstanceID,
                                                                         FormDesignVersionID = instance.FormDesignVersionID
                                                                     }).ToList();

            return formInstanceIDList.FirstOrDefault();
        }
        public void DeleteSevenDaysOldCollateral(int numberOfDays)
        {
            string strLogMessage = string.Empty;
            string strLogValidPathMessage = string.Empty;
            try
            {
                SqlParameter numberOfDaysParam = new SqlParameter("@numberOfDays", numberOfDays);
                var log = this._unitOfWork.Repository<CollateralProcessQueue>().ExecuteSql("exec [dbo].[DELETE_COLLATERAL_BEFORE_NO_DAYS_OLD] @numberOfDays", numberOfDaysParam).ToList();
                strLogMessage = "Entry in Database for deleting Collateral is successfully removed. ";
                if (log != null)
                {
                    strLogMessage = strLogMessage + "|Deleting Collateral files Count:" + log.Count;
                    foreach (var collateralProcessQ in log)
                    {
                        if (collateralProcessQ is CollateralProcessQueue)
                        {
                            CollateralProcessQueue objCollateralProcessQueue = collateralProcessQ as CollateralProcessQueue;
                            if (objCollateralProcessQueue != null)
                            {
                                try
                                {
                                    // Deleting pdf files on physical location
                                    if (objCollateralProcessQueue.FilePath != null)
                                    {
                                        if (System.IO.File.Exists(objCollateralProcessQueue.FilePath))
                                        {
                                            System.IO.File.Delete(objCollateralProcessQueue.FilePath);
                                            strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Collateral PDF file successfully deleted, path is:" + objCollateralProcessQueue.FilePath;
                                        }
                                        else
                                        {
                                            strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Deleting Collateral files Error: CollateralProcessQueue.FilePath is not exist, path is:" + objCollateralProcessQueue.FilePath;
                                        }
                                    }
                                    else
                                    {
                                        strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Deleting Collateral files Error: CollateralProcessQueue.FilePath object is null";
                                    }
                                    // Deleting Word files on physical location
                                    if (objCollateralProcessQueue.WordFilePath != null)
                                    {
                                        if (System.IO.File.Exists(objCollateralProcessQueue.WordFilePath))
                                        {
                                            System.IO.File.Delete(objCollateralProcessQueue.WordFilePath);
                                            strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Collateral MSWord file successfully deleted, path is:" + objCollateralProcessQueue.WordFilePath;
                                        }
                                        else
                                        {
                                            strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Deleting Collateral files Error: CollateralProcessQueue.WordFilePath is not exist, path is:" + objCollateralProcessQueue.WordFilePath;
                                        }
                                    }
                                    else
                                    {
                                        strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Deleting Collateral files Error: CollateralProcessQueue.WordFilePath object is null";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    strLogMessage = strLogMessage + "|" + objCollateralProcessQueue.ProcessGovernance1Up + "Error in deleting Seven Days old Collateral: " + ex.Message;
                                }
                            }
                            else
                            {
                                strLogMessage = strLogMessage + "|" + "Deleting Collateral files Error: CollateralProcessQueue object is null";
                            }
                        }
                    }
                    strLogMessage = strLogMessage + "|" + "Deleting Collateral files is successfully removed from given path.";
                }
                else
                {
                    strLogMessage = strLogMessage + "|" + "Deleting Collateral files Error: Log object is null";
                }
                this.AddLogEntry(1, 0, 1, strLogMessage);
                //Deleting Other Files which present on the location and not present on the database
                IList<CollateralProcessQueue> collateralProcessQueueList = null;
                collateralProcessQueueList = (from c in this._unitOfWork.RepositoryAsync<CollateralProcessQueue>()
                                                                                .Query()
                                                                                .Get() select c).ToList();
                if (collateralProcessQueueList != null)
                {
                    strLogValidPathMessage = "Valid Collateral files Count:" + collateralProcessQueueList.Count;
                    string strFolderPath = string.Empty;
                    if (collateralProcessQueueList.Count > 0)
                    {
                        strFolderPath = collateralProcessQueueList[0].FilePath.Substring(0, collateralProcessQueueList[0].FilePath.LastIndexOf(@"\"));
                    }
                    if (!string.IsNullOrEmpty(strFolderPath))
                    {
                        strLogValidPathMessage = strLogValidPathMessage + "|" + "Valid Collateral Folder path is :" + strFolderPath;
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strFolderPath);
                        List<string> filesLists = new List<string>();

                        if (di != null)
                        {
                            foreach (CollateralProcessQueue objValidCollateralFiles in collateralProcessQueueList)
                            {
                                if(!string.IsNullOrEmpty(objValidCollateralFiles.FilePath))
                                {
                                    if (!filesLists.Contains(objValidCollateralFiles.FilePath.Trim()))
                                    {
                                        filesLists.Add(objValidCollateralFiles.FilePath.Trim());
                                    }
                                }
                                if (!string.IsNullOrEmpty(objValidCollateralFiles.WordFilePath))
                                {
                                    if (!filesLists.Contains(objValidCollateralFiles.WordFilePath.Trim()))
                                    {
                                        filesLists.Add(objValidCollateralFiles.WordFilePath.Trim());
                                    }
                                }
                            }

                            foreach (System.IO.FileInfo file in di.GetFiles())
                            {
                                string currentFilePath = file.FullName.Trim();
                                if (!filesLists.Contains(currentFilePath))
                                {
                                    if (System.IO.File.Exists(currentFilePath))
                                    {
                                        try
                                        {
                                            System.IO.File.Delete(currentFilePath);
                                            strLogValidPathMessage = strLogValidPathMessage + "|" + "Collateral Invalid  file successfully deleted, path is:" + currentFilePath;
                                        }
                                        catch(Exception expInv)
                                        {
                                            strLogValidPathMessage = strLogValidPathMessage + "|" + "Deleting InValid Collateral files Error: , path is:" + currentFilePath + " Exception is: " + expInv.Message;
                                        }
                                        
                                    }
                                    else
                                    {
                                        strLogValidPathMessage = strLogValidPathMessage + "|" + "Deleting InValid Collateral files Error: CollateralProcessQueue.FilePath is not exist, path is:" + currentFilePath;
                                    }
                                }
                                
                            }
                        }
                        else
                        {
                            strLogValidPathMessage = strLogValidPathMessage + "|" + "Valid Collateral Folder Directory is null:";
                        }
  
                    }
                    else
                    {
                        strLogValidPathMessage = strLogValidPathMessage + "|" + "Valid Collateral Folder path is null:";
                    }
                }
                else
                {
                    strLogValidPathMessage = strLogValidPathMessage + "|" + "Valid Collateral files Error: valid Log object is null";
                }
                this.AddLogEntry(1, 0, 1, strLogValidPathMessage);
            }
            catch (Exception ex)
            {
                this.AddLogEntry(1, 0, 1, "Error in deleting Seven Days old Collateral: " + ex.Message + "|"+ strLogMessage + "|" + strLogValidPathMessage);
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
        }
    }

}
