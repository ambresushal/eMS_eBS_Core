using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Script.Serialization;
using tmg.equinox.applicationservices.ExitValidate;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.caching;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.notification;
using tmg.equinox.queueprocess.exitvalidate;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class ExitValidateService : IExitValidateService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }

        private string UserName { get; set; }
        IBackgroundJobManager _hangFireJobManager;
        IExitValidateEnqueueService _exitValidateEnqueueService;
        private static readonly ILog _logger = LogProvider.For<ExitValidateService>();
        private IFormDesignService _formDesignService;
        INotificationService _notificationService;
        #endregion Private Memebers

        #region Constructor
        public ExitValidateService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ExitValidateService(IUnitOfWorkAsync unitOfWork, IBackgroundJobManager hangFireJobManager, IExitValidateEnqueueService exitValidateEnqueueService, IFormDesignService formDesignService, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            this._hangFireJobManager = hangFireJobManager;
            this._exitValidateEnqueueService = exitValidateEnqueueService;
            this._formDesignService = formDesignService;
            _notificationService = notificationService;
        }

        public void InitializeVariables(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        public ExitValidateViewModel Validate(ExitValidateViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string productId = GetMedicareDocumentName(model.FormInstanceID);               
                int formInstanceID = model.FormName.Equals("PBPView") ? model.FormInstanceID : GetPBPViewFormInstanceID(model.FolderVersionID, model.FormInstanceID);
                ExitValidateQueue AddToInsert = new ExitValidateQueue();
                AddToInsert.Status = "Enqueued";
                AddToInsert.FormInstanceID = formInstanceID;
                AddToInsert.FolderVersionID = model.FolderVersionID;
                AddToInsert.FolderID = model.FolderID;
                AddToInsert.ProductID = productId ?? model.Name;
                AddToInsert.UserID = model.UserID;
                AddToInsert.Sections = JsonConvert.SerializeObject(model.Sections);
                AddToInsert.SetAsDefault = model.SetAsDefault;
                AddToInsert.AddedDate = DateTime.Now;
                AddToInsert.CompletedDate = DateTime.Now;
                AddToInsert.IsLatest = model.IsLatest;
                AddToInsert.IsQueuedForWFStateUpdate = model.IsQueuedForWFStateUpdate;
                AddToInsert.UsersInterestedInStatus = model.UsersInterestedInStatus;
                this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Insert(AddToInsert);
                this._unitOfWork.Save();

                model.FormDesignVersionID = this._unitOfWork.RepositoryAsync<FormInstance>()
                                             .Query()
                                             .Filter(c => c.FormInstanceID == AddToInsert.FormInstanceID)
                                             .Get().Select(c => c.FormDesignVersionID).FirstOrDefault();

                model.FormInstanceID = formInstanceID;

                model.QueueID = AddToInsert.ExitValidateQueueID;
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return model;
        }

        public ExitValidateViewModel GetDefaultSectionData(int? CurrentUserId)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            ExitValidateViewModel evModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                             where e.UserID == CurrentUserId
                                             orderby e.ExitValidateQueueID descending
                                             select (new ExitValidateViewModel
                                             {
                                                 Section = e.Sections,
                                                 SetAsDefault = e.SetAsDefault
                                             })).ToList<ExitValidateViewModel>().FirstOrDefault();
            if (evModel == null)
            {
                evModel = new ExitValidateViewModel();
            }
            evModel.Sections = evModel.Sections != null ? js.Deserialize<string[]>(evModel.Section) : evModel.Sections;
            evModel.IsExitValidate = ConfigurationManager.AppSettings["ExitValidate"] ?? string.Empty;
            return evModel;
        }

        public ServiceResult UpdateQueue(int ExitValidateQueueID, ExitValidateViewModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ExitValidateQueue itemToUpdate = this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                                .Where(s => s.ExitValidateQueueID.Equals(ExitValidateQueueID)
                                                ).FirstOrDefault();

                if (itemToUpdate != null)
                {
                    itemToUpdate.Status = model.Status;
                    if (model.Status == "Succeeded")
                    {
                        itemToUpdate.CompletedDate = DateTime.Now;
                    }
                    itemToUpdate.JobId = model.JobId;
                    itemToUpdate.ErrorMessage = model.ErrorMessage;
                    itemToUpdate.IsLatest = true;
                    this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }

                if (itemToUpdate.Status == "Succeeded" && result.Result == ServiceResultStatus.Success)
                {
                    List<ExitValidateQueue> getUpdateTOLatest = (from q in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                                                 where q.FormInstanceID.Equals(itemToUpdate.FormInstanceID)
                                                                 select q).ToList();

                    foreach (ExitValidateQueue queued in getUpdateTOLatest)
                    {
                        if (queued.ExitValidateQueueID != ExitValidateQueueID)
                        {
                            queued.IsLatest = false;
                            this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Update(queued);
                        }
                    }
                    this._unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateImportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ServiceResult UpdateExitValidateFilePath(int ExitValidateQueueID, ExitValidateViewModel model, string ZipFilePath)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                using (var scope = new TransactionScope())
                {
                    ExitValidateQueue queueDetails = this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Find(ExitValidateQueueID);

                    queueDetails.ExitValidateFilePath = model.ExitValidateFilePath;
                    queueDetails.PlanAreaFilePath = model.PlanAreaFilePath;
                    queueDetails.VBIDFilePath = model.VBIDFilePath;
                    _unitOfWork.RepositoryAsync<ExitValidateQueue>().Update(queueDetails);
                    _unitOfWork.Save();
                    scope.Complete();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                _logger.ErrorException("Error in UpdateExportQueue : ", ex);
                throw ex;
            }
            return result;
        }

        public ExitValidateViewModel GetExitValidateMappings(int ExitValidateQueueID)
        {
            ExitValidateViewModel _evModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                              join f in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                              on e.FolderVersionID equals f.FolderVersionID
                                              join i in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                              on e.FormInstanceID equals i.FormInstanceID
                                              join acc in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                                              on i.AnchorDocumentID equals acc.FormInstanceID
                                              where e.ExitValidateQueueID == ExitValidateQueueID
                                              select (new ExitValidateViewModel
                                              {
                                                  FormInstanceID = e.FormInstanceID,
                                                  FolderVersionID = e.FolderVersionID,
                                                  FolderID = e.FolderID,
                                                  ProductID = e.ProductID,
                                                  UserID = e.UserID,
                                                  QueueID = e.ExitValidateQueueID,
                                                  JobId = e.JobId,
                                                  Year = f.EffectiveDate,
                                                  Name = i.Name,
                                                  AnchorDocumentID = i.AnchorDocumentID,
                                                  AddedDate = e.AddedDate,
                                                  Status = e.Status,
                                                  UsersInterestedInStatus = e.UsersInterestedInStatus,
                                                  QID = acc.ProductID
                                              })).ToList<ExitValidateViewModel>().FirstOrDefault();
            return _evModel;
        }

        public string GetPBPPlanAreaFileName(int formInstanceId)
        {
            string pbpPlanAreaFileName = string.Empty;
            int pbpImportQueueID = _unitOfWork.Repository<PBPImportDetails>().Get()
                               .Where(t => t.FormInstanceID == formInstanceId && t.IsActive == true)
                               .OrderByDescending(t => t.PBPImportQueueID)
                               .Select(t => t.PBPImportQueueID).FirstOrDefault();

            pbpPlanAreaFileName = _unitOfWork.Repository<PBPImportQueue>().Get()
                           .Where(t => t.PBPImportQueueID == pbpImportQueueID)
                           .Select(t => t.PBPPlanAreaFileName).FirstOrDefault();
            return pbpPlanAreaFileName;
        }

        public int GetPBPViewFormInstanceID(int folderVersionId, int? formInstanceID)
        {
            int? anchorDocumentId = (from a in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     where a.FormInstanceID == formInstanceID
                                     select a.AnchorDocumentID).FirstOrDefault();

            int PBPViewFormInstanceID = this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                         .Where(s => s.FolderVersionID.Equals(folderVersionId)
                                         && s.IsActive == true
                                         && s.FormDesignID.Equals(2367)
                                         && s.AnchorDocumentID == anchorDocumentId
                                         )
                                         .Select(s => s.FormInstanceID).FirstOrDefault();

            return PBPViewFormInstanceID;
        }

        public string GetMedicareDocumentName(int formInstanceID)
        {
            int? anchorDocumentId = (from a in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     where a.FormInstanceID == formInstanceID
                                     select a.AnchorDocumentID).FirstOrDefault();

            var medicareFormInstance = (from frmInst in this._unitOfWork.Repository<FormInstance>().Query().Get()
                                        where frmInst.FormInstanceID == anchorDocumentId && frmInst.IsActive == true
                                        select frmInst).FirstOrDefault();

            string formInstanceName = medicareFormInstance != null ? medicareFormInstance.Name : string.Empty;
            return formInstanceName;
        }

        public int GetPBPDatabase1Up(int? anchorDocumentID)
        {
            int pbpDatabase1Up = _unitOfWork.Repository<PBPImportDetails>().Get()
                                .Where(t => t.FormInstanceID == anchorDocumentID)
                                .Select(t => t.PBPDatabase1Up).FirstOrDefault();
            return pbpDatabase1Up;
        }

        public ServiceResult AddExitValidateResults(ExitValidateViewModel model, string reportFileName)
        {
            ServiceResult result = new ServiceResult();
            List<ExitValidateResultViewModel> evExcelErrorsList = null;

            try
            {
                evExcelErrorsList = GetExitValidateResult(reportFileName);
                List<ExitValidateResult> evErrorList = new List<ExitValidateResult>();
                foreach (ExitValidateResultViewModel evError in evExcelErrorsList)
                {
                    int errorCount = evError.Errors.Count();
                    if (errorCount > 0)
                    {
                        foreach (ErrorsList el in evError.Errors)
                        {
                            ExitValidateResult AddToInsert = new ExitValidateResult();
                            AddToInsert.ExitValidateQueueID = model.QueueID;
                            AddToInsert.FormInstanceID = model.FormInstanceID;
                            AddToInsert.ContractNumber = "";
                            AddToInsert.PlanName = "";
                            AddToInsert.Section = evError.Section;
                            AddToInsert.Status = evError.Status;
                            AddToInsert.Result = evError.Result;

                            AddToInsert.Error = el.ERROR;
                            AddToInsert.Question = el.QUESTION;
                            AddToInsert.Screen = el.SCREEN;
                            AddToInsert.PBPFIELD = el.FIELD;
                            AddToInsert.PBPCOLUMN = el.COLUMN;
                            evErrorList.Add(AddToInsert);
                        }
                    }
                }
                //evErrorList = evErrorList.Distinct(new ExitValidateResultComparer()).ToList();
                foreach (var error in evErrorList)
                {
                    this._unitOfWork.RepositoryAsync<ExitValidateResult>().Insert(error);
                }                
                this._unitOfWork.Save();
                SendEVCompletionotification(model.QueueID, false);
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                SendEVCompletionotification(model.QueueID,  true);
                result.Result = ServiceResultStatus.Failure;
                throw ex;
            }
            return result;
        }

        public void SendEVCompletionotification(int exitValidateQueueID, bool hasError, int userId = 0)
        {
            try
            {
                
                ExitValidateViewModel evModel = GetExitValidateMappings(exitValidateQueueID);
                
                bool isEVCompletedWithoutError = true;
                
                var exitValidationresult = (from evresult in this._unitOfWork.Repository<ExitValidateResult>().Get()
                                            where evresult.ExitValidateQueueID == evModel.QueueID
                                            && (!evresult.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package"))
                                            select evresult.ExitValidateResultID);
                if (exitValidationresult != null && exitValidationresult.Count() > 0)
                {
                    isEVCompletedWithoutError = false;
                }
                var currentUserId = userId == 0 ? evModel.UserID : userId;
                var folder = (from x in _unitOfWork.RepositoryAsync<Folder>().Get()
                              where x.FolderID == evModel.FolderID
                              select x).FirstOrDefault();
                var folderName = "";
                if (folder != null)
                {
                    folderName = folder.Name;
                }
                var usersInterestedInStatus = evModel.UsersInterestedInStatus;
                List<string> usersToSenDNotification = new List<string>();
                if(usersInterestedInStatus != null)
                {
                    usersToSenDNotification = usersInterestedInStatus.Split(',').ToList();
                }
                usersToSenDNotification.Add(currentUserId.ToString());
                usersToSenDNotification = usersToSenDNotification.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                foreach (var usertoSendNotification in usersToSenDNotification)
                {
                    var user = (from x in _unitOfWork.RepositoryAsync<User>().Get()
                                where x.UserID.ToString() == usertoSendNotification
                                select x).FirstOrDefault();
                    var userName = "";
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                    if (hasError || evModel.Status == "Failed")
                    {
                        List<Paramters> paramater = new List<Paramters>();
                        var productName = evModel.ProductID == null ? "" : evModel.ProductID;
                        paramater.Add(new Paramters { key = "message", Value = productName + " for Folder " + folderName + " Failed Exit Validation." });
                        _notificationService.SendNotification(
                                    new NotificationInfo
                                    {
                                        SentTo = userName,
                                        MessageKey = MessageKey.TASK_EV_COMPLETED,
                                        ParamterValues = paramater,
                                        loggedInUserName = userName,
                                    });
                    }
                    else
                    {
                        if (evModel.Status == "Enqueued" || (evModel.Status == "Processing" && userId != 0))
                        {
                            List<Paramters> paramater = new List<Paramters>();
                            var productName = evModel.ProductID == null ? "" : evModel.ProductID;
                            paramater.Add(new Paramters { key = "message", Value = productName + " for Folder " + folderName + " is processing Exit Validation." });
                            _notificationService.SendNotification(
                                        new NotificationInfo
                                        {
                                            SentTo = userName,
                                            MessageKey = MessageKey.TASK_EV_COMPLETED,
                                            ParamterValues = paramater,
                                            loggedInUserName = userName,
                                        });
                            //get top queue entry for FormInstanceID
                            var evqueue = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                         where e.ExitValidateQueueID == evModel.QueueID
                                         select e).FirstOrDefault();
                            if (evqueue != null && userId != 0)
                            {
                                evqueue.UsersInterestedInStatus = evqueue.UsersInterestedInStatus + "," + userId;
                                this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Update(evqueue);
                                this._unitOfWork.Save();
                            }
                            
                        }
                        else
                        if (isEVCompletedWithoutError)
                        {
                            List<Paramters> paramater = new List<Paramters>();
                            var productName = evModel.ProductID == null ? "" : evModel.ProductID;
                            paramater.Add(new Paramters { key = "message", Value = productName + " for Folder " + folderName + " succeeded Exit Validation." });
                            _notificationService.SendNotification(
                                        new NotificationInfo
                                        {
                                            SentTo = userName,
                                            MessageKey = MessageKey.TASK_EV_COMPLETED,
                                            ParamterValues = paramater,
                                            loggedInUserName = userName,
                                        });
                        }
                        else
                        {
                            List<Paramters> paramater = new List<Paramters>();
                            var productName = evModel.ProductID == null ? "" : evModel.ProductID;
                            paramater.Add(new Paramters { key = "message", Value = productName + " for Folder " + folderName + " completes Exit Validation with errors. Please refer the Exit Validate tab in the plan to resolve errors." });
                            _notificationService.SendNotification(
                                        new NotificationInfo
                                        {
                                            SentTo = userName,
                                            MessageKey = MessageKey.TASK_EV_COMPLETED,
                                            ParamterValues = paramater,
                                            loggedInUserName = userName,
                                        });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<ExitValidateResultViewModel> GetExitValidateResult(string reportFileName)
        {
            string[] inputLines = System.IO.File.ReadAllLines(reportFileName);
            List<string> headers = GetHeaders();
            string errors = "";
            string section = "";
            bool isSectionErrorLine = false;
            int idx = 1;
            List<ExitValidateResultViewModel> evErrorList = new List<ExitValidateResultViewModel>();
            foreach (var str in inputLines)
            {
                if (headers.Contains(str))
                {
                    if (!String.IsNullOrEmpty(errors))
                    {
                        ExitValidateResultViewModel model = new ExitValidateResultViewModel();
                        model.Section = section;
                        model.Status = "";
                        model.Result = "";
                        model.Errors = SplitErrors(errors);
                        evErrorList.Add(model);
                        errors = "";
                    }
                    section = str;
                    isSectionErrorLine = false;
                }
                else
                {
                    isSectionErrorLine = true;
                }
                if (!String.IsNullOrEmpty(section) && isSectionErrorLine == true)
                {
                    errors = errors + str;
                }
                if (!String.IsNullOrEmpty(errors) && idx == inputLines.Length)
                {
                    ExitValidateResultViewModel model = new ExitValidateResultViewModel();
                    model.Section = section;
                    model.Status = "";
                    model.Result = "";
                    model.Errors = SplitErrors(errors);
                    evErrorList.Add(model);
                }
                idx++;
            }
            return evErrorList;
        }

        public List<ErrorsList> SplitErrors(string errors)
        {
            List<ErrorsList> erList = new List<ErrorsList>();
            if (!string.IsNullOrEmpty(errors))
            {
                var matches = Regex.Split(errors, "(?=ERROR:)|(?=WARNING:)");
                foreach (var match in matches)
                {
                    ErrorsList el = new ErrorsList();
                    if (match.IndexOf("ERROR", 0, StringComparison.InvariantCultureIgnoreCase) >= 0 || match.IndexOf("WARNING", 0, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        var errorParts = Regex.Split(match, "(?=QUESTION:)|(?=SCREEN:)|(?=FIELD:)|(?=COLUMN:)");
                        foreach (var error in errorParts)
                        {
                            if (error.Contains("ERROR:"))
                            {
                                el.ERROR = error.Replace("ERROR:", "").Trim();
                            }
                            else if (error.Contains("WARNING:"))
                            {
                                el.ERROR = error.Replace("WARNING:", "").Trim();
                            }
                            else if (error.Contains("QUESTION:"))
                            {
                                el.QUESTION = error.Replace("QUESTION:", "").Trim();
                            }
                            else if (error.Contains("SCREEN:"))
                            {
                                el.SCREEN = error.Replace("SCREEN:", "").Trim();
                            }
                            else if (error.Contains("FIELD:"))
                            {
                                el.FIELD = error.Replace("FIELD:", "").Trim();
                            }
                            else if (error.Contains("COLUMN:"))
                            {
                                el.COLUMN = error.Replace("COLUMN:", "").Trim();
                            }
                        }
                        if (!String.IsNullOrEmpty(el.ERROR))
                        {
                            erList.Add(el);
                        }
                    }
                }
                if (erList.Count > 0)
                {
                    erList = erList.Distinct(new ExitValidateErrorComparer()).ToList();
                }
            }
            return erList;
        }

        public List<ExitValidateResultViewModel> GetExitValidateErrors(int formInstanceId)
        {
            //get top queue entry for FormInsatnceID
            //get records for queueid
            List<ExitValidateResultViewModel> erModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateResult>().Get()
                                                         where e.FormInstanceID == formInstanceId
                                                         select (new ExitValidateResultViewModel
                                                         {
                                                             ExitValidateResultID = e.ExitValidateResultID,
                                                             ExitValidateQueueID = e.ExitValidateQueueID,
                                                             FormInstanceID = e.FormInstanceID,
                                                             ContractNumber = e.ContractNumber,
                                                             PlanName = e.PlanName,
                                                             Section = e.Section,
                                                             Status = e.Status,
                                                             Result = e.Result,
                                                             Error = e.Error,
                                                             Question = e.Question,
                                                             Screen = e.Screen,
                                                         })).ToList<ExitValidateResultViewModel>();
            return erModel;
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        public string ProcessExitValidateAutomation(ExitValidateViewModel model)
        {
            string scriptName = System.Configuration.ConfigurationManager.AppSettings["ExitValidateAutomationScript"];
            string reportVariablesFile = System.Configuration.ConfigurationManager.AppSettings["ExitValidateReportVariablesFile"];

            //Wait time for kill the Exit Validate process
            string waitForExit = System.Configuration.ConfigurationManager.AppSettings["ExitValidateWaitForExitTime"] ?? string.Empty;
            int waitTime;
            int.TryParse(waitForExit, out waitTime);
            if (waitTime <= 0)
            {
                waitTime = 600000;
            }

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "cscript.exe";
            ps.UseShellExecute = false;
            string reportFileName = "ExitValidate_" + model.JobId + '_' + model.QueueID + '_' + model.FormInstanceID;
            File.WriteAllText(reportVariablesFile, reportFileName);
            ps.Arguments = string.Format("\"{0}\" \"{1}\"", scriptName, reportFileName);
            Process proc = Process.Start(ps);
            //Wait for the process to exit or time out : make configurable
            proc.WaitForExit(waitTime);
            string reportPath = System.Configuration.ConfigurationManager.AppSettings["ExitValidateAutomationReportPath"];
            reportFileName = reportPath + reportFileName;
            return reportFileName;
        }

        public string ProcessPBPExitValidate(ExitValidateViewModel model, out bool isKilledExitValidate)
        {
            //exe path -ExitValidatePBPExePath
            string year = model.Year.ToString("yyyy");
            string evExeFile = string.Format(System.Configuration.ConfigurationManager.AppSettings["ExitValidatePBPExePath"], year);

            //Wait time for kill the Exit Validate process
            string waitForExit = System.Configuration.ConfigurationManager.AppSettings["ExitValidateWaitForExitTime"] ?? string.Empty;
            int waitTime;
            int.TryParse(waitForExit, out waitTime);
            if (waitTime <= 0)
            {
                waitTime = 600000;
            }

            //mdb path
            string mdbPath = string.Format(System.Configuration.ConfigurationManager.AppSettings["ExitValidateMDBPath"], year);
            string pbpPath = mdbPath + string.Format("PBP{0}.mdb", year);
            string pbpPlansPath = mdbPath + string.Format("PBPPLANS{0}.mdb", year);
            string pbpVBIDPath = mdbPath + string.Format("VBID_PBP{0}.mdb", year);
            
            //report file
            string reportPath = string.Format(System.Configuration.ConfigurationManager.AppSettings["ExitValidateReportsPath"], year);
            string reportFileName = "ExitValidate_" + model.JobId + '_' + model.QueueID + '_' + model.FormInstanceID + ".txt";
            reportFileName = reportPath + reportFileName;
            
            //qid
            string qid = model.QID;

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = evExeFile;
            ps.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", qid, pbpPath, pbpPlansPath, pbpVBIDPath, reportFileName);
            ps.UseShellExecute = false;
            isKilledExitValidate = false;
            Process proc = Process.Start(ps);
            if (proc.WaitForExit(waitTime) == false)
            {
                proc.Kill();
                //log message
                isKilledExitValidate = true;
            }
            return reportFileName;
        }

        public bool IsExitValidateInProgress(int formInstanceId, int folderVersionId)
        {
            //enqueued or processing
            bool result = false;
            
            int pbpFormInstanceId = GetPBPViewFormInstanceID(folderVersionId, formInstanceId);
            
            //get top queue entry for FormInstanceID
            var queues = from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                         where e.FormInstanceID == pbpFormInstanceId && (e.Status == "Enqueued" || e.Status == "Processing")
                         select e;
            if (queues != null && queues.Count() > 0)
            {
                result = true;
            }
            return result;
        }

        public List<ExitValidateResultViewModel> GetLatestExitValidateResults(int formInstanceId)
        {
            int queueId = 0;
            List<ExitValidateResultViewModel> erModel = new List<ExitValidateResultViewModel>();
            //get top queue entry for FormInstanceID
            var queues = from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                         where e.FormInstanceID == formInstanceId && e.IsLatest == true
                         orderby e.CompletedDate descending
                         select e;
            if (queues != null && queues.Count() > 0)
            {
                queueId = queues.First().ExitValidateQueueID;
            }

            if (queueId > 0)
            {
                //get records for queueid
                erModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateResult>().Get()
                               //join m in this._unitOfWork.RepositoryAsync<PBPExportToMDBMapping>().Get()
                               //on e.PBPFIELD equals m.FieldName 
                           where e.ExitValidateQueueID == queueId //&& m.Year == 2019
                           orderby e.Section ascending
                           select (new ExitValidateResultViewModel
                           {
                               ExitValidateResultID = e.ExitValidateResultID,
                               ExitValidateQueueID = e.ExitValidateQueueID,
                               FormInstanceID = e.FormInstanceID,
                               ContractNumber = e.ContractNumber,
                               PlanName = e.PlanName,
                               Section = e.Section,
                               Status = e.Status,
                               Result = e.Result,
                               Error = e.Error,
                               Question = e.Question,
                               Screen = e.Screen,
                           })).ToList<ExitValidateResultViewModel>();

                //erModel = GetElementDetails(erModel, formInstanceId);
            }
            return erModel;
        }

        public List<ExitValidateResultViewModel> GetVBIDViewExitValidateResults(int formInstanceId)
        {
            int queueId = 0;
            List<ExitValidateResultViewModel> erModel = new List<ExitValidateResultViewModel>();
            //get top queue entry for FormInstanceID
            var queues = from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                         where e.FormInstanceID == formInstanceId && e.IsLatest == true
                         orderby e.CompletedDate descending
                         select e;
            if (queues != null && queues.Count() > 0)
            {
                queueId = queues.First().ExitValidateQueueID;
            }

            if (queueId > 0)
            {
                //get records for queueid
                erModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateResult>().Get()
                           join vbd in this._unitOfWork.RepositoryAsync<VBIDExportToMDBMapping>().Get() on e.PBPFIELD equals vbd.FieldName
                           where e.ExitValidateQueueID == queueId
                           orderby e.Section ascending
                           select (new ExitValidateResultViewModel
                           {
                               ExitValidateQueueID = e.ExitValidateQueueID,
                               FormInstanceID = e.FormInstanceID,
                               ContractNumber = e.ContractNumber,
                               PlanName = e.PlanName,
                               Section = e.Section,
                               Status = e.Status,
                               Result = e.Result,
                               Error = e.Error,
                               Question = e.Question,
                               Screen = e.Screen,
                               PBPViewSection = vbd.RootSectionName
                           })).Distinct().ToList<ExitValidateResultViewModel>();


            }
            return erModel;
        }

        // Field Mapping for navigate and focus 
        //public List<ExitValidateResultViewModel> GetElementDetails(List<ExitValidateResultViewModel> eleDetails, int formInstanceId)
        //{
        //    int formDesignVersionId = (from e in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
        //                               where e.FormInstanceID == formInstanceId
        //                               select (e.FormDesignVersionID)).FirstOrDefault();
        //    // Section List 
        //    List<SectionDesign> secList = GetSectionsList(1, formDesignVersionId.ToString(), _formDesignService);

        //    foreach (var model in eleDetails)
        //    {
        //        string[] elList = model.JsonPath.Replace("[0]", "").Split('.');
        //        string patentGenerated = elList.FirstOrDefault();
        //        string parentOfChildName = elList[elList.Length - 2];
        //        string childGeneratedName = elList.LastOrDefault();

        //        int parentUiElementID = (from e in this._unitOfWork.RepositoryAsync<UIElement>().Get()
        //                                 join m in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
        //                                 on e.UIElementID equals m.UIElementID
        //                                 where (e.GeneratedName == parentOfChildName && m.EffectiveDate.ToString().IndexOf("2019") != -1)
        //                                 select e.UIElementID).ToList().FirstOrDefault();

        //        var details = (from e in this._unitOfWork.RepositoryAsync<UIElement>().Get()
        //                       join m in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
        //                       on e.UIElementID equals m.UIElementID
        //                       where (e.GeneratedName == patentGenerated ||
        //                       (e.GeneratedName == childGeneratedName && e.ParentUIElementID == parentUiElementID))
        //                       && m.EffectiveDate.ToString().IndexOf("2019") != -1
        //                       select new
        //                       {
        //                           e.UIElementID,
        //                           e.UIElementName,
        //                           e.GeneratedName,
        //                           e.Label,
        //                       }).ToList();

        //        string ParentSectionName = (from e in details where e.GeneratedName == patentGenerated orderby e.UIElementID ascending select e.UIElementName).FirstOrDefault();
        //        string ChildElementName = (from e in details where e.GeneratedName == childGeneratedName select e.UIElementName).FirstOrDefault();
        //        int SecID = (from e in details where e.GeneratedName == patentGenerated select e.UIElementID).FirstOrDefault();
        //        //string ParentSectionLabel = secList.Where(s => s.ID == details.FirstOrDefault().UIElementID).Count() != 0 ? (secList.Where(s => s.ID == details.FirstOrDefault().UIElementID).FirstOrDefault().Label) : string.Empty;
        //        string ParentSectionLabel = secList.Where(s => s.ID == SecID).Count() != 0 ? (secList.Where(s => s.ID == SecID).FirstOrDefault().Label) : string.Empty;

        //        model.SectionID = ParentSectionName;
        //        model.Section = ParentSectionLabel;
        //        model.ElementID = ChildElementName + model.FormInstanceID;
        //        model.GeneratedName = childGeneratedName;
        //        model.FormInstance = "tab" + model.FormInstanceID;
        //        model.RowNum = string.Empty;
        //        model.RowIdProperty = string.Empty;
        //    }
        //    return eleDetails;
        //}

        public int DeleteExtraRowsPlanAreaFile(string QID, OleDbConnection conn)
        {
            int rowAffected = 0;
            try
            {
                string PBPVersionStr = string.Empty;
                if (!string.IsNullOrEmpty(QID))
                {
                    PBPVersionStr = string.Concat("DELETE FROM pbpplans where NOT (PBP_A_SQUISH_ID='", QID, "')");
                    OleDbCommand cmdPBPVersion = new OleDbCommand(PBPVersionStr, conn);
                    rowAffected = cmdPBPVersion.ExecuteNonQuery();

                    PBPVersionStr = string.Concat("DELETE FROM pbpregions where NOT (PBP_A_SQUISH_ID='", QID, "')");
                    cmdPBPVersion = new OleDbCommand(PBPVersionStr, conn);
                    rowAffected += cmdPBPVersion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return rowAffected;
        }

        public int AddPlanAreaFileRow(JObject source, OleDbConnection conn)
        {
            int rowAffected = 0;
            try
            {
                var _evPlanAreaService = new ExitValidatePlanAreaFileMapping();

                _evPlanAreaService._dictPBPRegions = _evPlanAreaService._dictPBPRegions.ToDictionary(x => x.Key, x => !string.IsNullOrEmpty(x.Value) ? source.SelectToken(x.Value).ToString() : string.Empty);
                string pbpRegionQuery = _evPlanAreaService.GetPBPRegions(_evPlanAreaService._dictPBPRegions);
                OleDbCommand cmdPBPRegion = new OleDbCommand(pbpRegionQuery, conn);
                rowAffected = cmdPBPRegion.ExecuteNonQuery();

                _evPlanAreaService._dictPBPPlans = _evPlanAreaService._dictPBPPlans.ToDictionary(x => x.Key, x => !string.IsNullOrEmpty(x.Value) ? source.SelectToken(x.Value).ToString() : string.Empty);
                string pbpPlanQuery = _evPlanAreaService.GetPBPPlans(_evPlanAreaService._dictPBPPlans);
                OleDbCommand cmdPBPPlan = new OleDbCommand(pbpPlanQuery, conn);
                rowAffected += cmdPBPPlan.ExecuteNonQuery();

                _evPlanAreaService._dictPlanAreas = _evPlanAreaService._dictPlanAreas.ToDictionary(x => x.Key, x => !string.IsNullOrEmpty(x.Value) ? source.SelectToken(x.Value).ToString() : string.Empty);
                string pbpPlanAreas = _evPlanAreaService.GetPlanAreas(_evPlanAreaService._dictPlanAreas);
                OleDbCommand cmdPlanAreas = new OleDbCommand(pbpPlanAreas, conn);
                rowAffected += cmdPlanAreas.ExecuteNonQuery();
                
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return rowAffected;
        }

        public int UpdatePBPFileRow(OleDbConnection conn)
        {
            int rowAffected = 0;
            try
            {
                string pbpRegionQuery = "UPDATE PBP SET ISINUSE = ' ', ABSTRAID = ' '";
                OleDbCommand cmdPBPRegion = new OleDbCommand(pbpRegionQuery, conn);
                rowAffected = cmdPBPRegion.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return rowAffected;
        }

        private List<string> GetHeaders()
        {
            return new List<string> {
                "Section A",
                "Section B-1",
                "Section B-2",
                "Section B-3",
                "Section B-4",
                "Section B-5",
                "Section B-6",
                "Section B-7",
                "Section B-8",
                "Section B-9",
                "Section B-10",
                "Section B-11",
                "Section B-12",
                "Section B-13",
                "Section B-14",
                "Section B-15",
                "Section B-16",
                "Section B-17",
                "Section B-18",
                "Section B-19",
                "Section B-20",
                "Section C",
                "Section D",
                "Medicare Rx Drug Section"

            };
        }

        public List<ExitValidateExportedList> GetExitValidateExportedList()
        {
            List<ExitValidateExportedList> evExportModel = new List<ExitValidateExportedList>();

            evExportModel = (from q in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                             join v in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                             on q.FolderVersionID equals v.FolderVersionID
                             join i in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                             on q.FormInstanceID equals i.FormInstanceID
                             join f in this._unitOfWork.RepositoryAsync<Folder>().Get()
                             on q.FolderID equals f.FolderID
                             join u in this._unitOfWork.RepositoryAsync<User>().Get()
                             on q.UserID equals u.UserID
                             where q.IsLatest == true //&& q.Status == "Succeeded"
                             orderby q.AddedDate descending, f.Name ascending, q.ProductID ascending
                             select (new ExitValidateExportedList
                             {
                                 ExitValidateQueueID = q.ExitValidateQueueID,
                                 FormInstanceID = q.FormInstanceID,
                                 FolderVersionID = q.FolderVersionID,
                                 FolderID = q.FolderID,
                                 FormDesignVersionID = i.FormDesignVersionID,
                                 FolderName = f.Name,
                                 ProductID = q.ProductID,
                                 FolderVersion = v.FolderVersionNumber,
                                 QueuedDateTime = q.AddedDate,
                                 CompletedDateTime = q.Status.Equals("Succeeded") ? q.CompletedDate : null,
                                 Status = q.Status,
                                 User = u.UserName,
                                 ErrorMessage = q.ErrorMessage
                             })).ToList<ExitValidateExportedList>();

            return evExportModel;
        }

        private string GetSectionName(string evSection)
        {
            string retVal = "";
            switch (evSection)
            {
                case "":
                    break;

            }
            return retVal;
        }

        public ServiceResult AddExitValidatePBPExportError(ExitValidateViewModel evModel, PBPExportToMDBMappingViewModel model)
        {
            return AddExitValidateExportError(evModel, model.JsonPath, model.Length, model.FieldName);
        }

        public ServiceResult AddExitValidateVBIDExportError(ExitValidateViewModel evModel, VBIDExportToMDBMappingViewModel model)
        {
            return AddExitValidateExportError(evModel, model.JsonPath, model.Length, model.FieldName);
        }
        public List<SectionDesign> GetSectionsList(int tenantId, string formDesignVersionId, IFormDesignService _formDesignService)
        {
            FormDesignVersionDetail detail = null;
            List<SectionDesign> sectionsList = new List<SectionDesign>();

            string formDesign = String.Empty;
            try
            {
                FormDesignDataCacheHandler formDesignCacheHandler = new FormDesignDataCacheHandler();
                formDesign = formDesignCacheHandler.Get(tenantId, Convert.ToInt32(formDesignVersionId), _formDesignService);
                detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);

                sectionsList = (from d in detail.Sections
                                select (new SectionDesign
                                {
                                    ID = d.ID,
                                    GeneratedName = d.GeneratedName,
                                    FullName = d.FullName,
                                    Label = d.Label,
                                })).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return sectionsList;
        }

        public List<ExitValidateMapModel> GetSectionMapModels()
        {
            return new List<ExitValidateMapModel>()
            {
                new ExitValidateMapModel() {PBPSection="Section A", PBPScreenNumber="",PBPView="Section A "},
                new ExitValidateMapModel() {PBPSection="Section B-1", PBPScreenNumber="#1a",PBPView="Section B: #1a Inpatient Hospital - Acute "},
                new ExitValidateMapModel() {PBPSection="Section B-1", PBPScreenNumber="#1b",PBPView="Section B: #1b Inpatient Hospital - Psychiatric "},
                new ExitValidateMapModel() {PBPSection="Section B-2", PBPScreenNumber="#2",PBPView="Section B: #2 Skilled Nursing Facility (SNF) "},
                new ExitValidateMapModel() {PBPSection="Section B-3", PBPScreenNumber="#3",PBPView="Section B: #3 Cardiac and Pulmonary Rehabilitation Services "},
                new ExitValidateMapModel() {PBPSection="Section B-4", PBPScreenNumber="#4a",PBPView="Section B: #4a Emergency Care "},
                new ExitValidateMapModel() {PBPSection="Section B-4", PBPScreenNumber="#4b",PBPView="Section B: #4b Urgently Needed Services "},
                new ExitValidateMapModel() {PBPSection="Section B-4", PBPScreenNumber="#4c",PBPView="Section B: #4c Worldwide Emergency/Urgent Coverage "},
                new ExitValidateMapModel() {PBPSection="Section B-5", PBPScreenNumber="#5",PBPView="Section B: #5 Partial Hospitalization "},
                new ExitValidateMapModel() {PBPSection="Section B-6", PBPScreenNumber="#6",PBPView="Section B: #6 Home Health Services "},
                new ExitValidateMapModel() {PBPSection="Section B-7", PBPScreenNumber="#7",PBPView="Section B: #7 Health Care Professional Services "},
                new ExitValidateMapModel() {PBPSection="Section B-8", PBPScreenNumber="#8",PBPView="Section B: #8 Outpatient Procedures, Tests, Labs &amp; Radiology Services "},
                new ExitValidateMapModel() {PBPSection="Section B-9", PBPScreenNumber="#9",PBPView="Section B: #9 Outpatient Services "},
                new ExitValidateMapModel() {PBPSection="Section B-10", PBPScreenNumber="#10",PBPView="Section B: #10 Ambulance/Transportation Services "},
                new ExitValidateMapModel() {PBPSection="Section B-11", PBPScreenNumber="#11",PBPView="Section B: #11 DME, Prosthetics, and Medical &amp; Diabetic Supplies "},
                new ExitValidateMapModel() {PBPSection="Section B-12", PBPScreenNumber="#12",PBPView="Section B: #12 Dialysis Services "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13a",PBPView="Section B: #13a Acupuncture "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13b",PBPView="Section B: #13b OTC Items "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13c",PBPView="Section B: #13c Meal Benefit "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13d",PBPView="Section B: #13d Other One "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13e",PBPView="Section B: #13e Other Two "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13f",PBPView="Section B: #13f Other Three "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13g",PBPView="Section B: #13g Dual Eligible SNPs with Highly Integrated Services "},
                new ExitValidateMapModel() {PBPSection="Section B-13", PBPScreenNumber="#13h",PBPView="Section B: #13h Additional Services "},
                new ExitValidateMapModel() {PBPSection="Section B-14", PBPScreenNumber="#14a",PBPView="Section B: #14a Medicare-covered Zero Dollar Preventive Services "},
                new ExitValidateMapModel() {PBPSection="Section B-14", PBPScreenNumber="#14b",PBPView="Section B: #14b Annual Physical Exam "},
                new ExitValidateMapModel() {PBPSection="Section B-14", PBPScreenNumber="#14c",PBPView="Section B: #14c Other Defined Supplemental Benefits "},
                new ExitValidateMapModel() {PBPSection="Section B-14", PBPScreenNumber="#14d",PBPView="Section B: #14d Kidney Disease Education Services "},
                new ExitValidateMapModel() {PBPSection="Section B-14", PBPScreenNumber="#14e",PBPView="Section B: #14e Other Medicare-covered Preventive Services "},
                new ExitValidateMapModel() {PBPSection="Section B-15", PBPScreenNumber="#15",PBPView="Section B: #15 Medicare Part B Rx Drugs General "},
                new ExitValidateMapModel() {PBPSection="Section B-16", PBPScreenNumber="#16a",PBPView="Section B: #16a Preventive Dental "},
                new ExitValidateMapModel() {PBPSection="Section B-16", PBPScreenNumber="#16b",PBPView="Section B: #16b Comprehensive Dental "},
                new ExitValidateMapModel() {PBPSection="Section B-17", PBPScreenNumber="#17a",PBPView="Section B: #17a Eye Exams "},
                new ExitValidateMapModel() {PBPSection="Section B-17", PBPScreenNumber="#17b",PBPView="Section B: #17b Eyewear "},
                new ExitValidateMapModel() {PBPSection="Section B-18", PBPScreenNumber="#18a",PBPView="Section B: #18a Hearing Exams "},
                new ExitValidateMapModel() {PBPSection="Section B-18", PBPScreenNumber="#18b",PBPView="Section B: #18b Hearing Aids "},
                new ExitValidateMapModel() {PBPSection="Section B-19", PBPScreenNumber="#19",PBPView="Section B: #19 Value-Based Insurance Design (VBID) Model Test "},
                new ExitValidateMapModel() {PBPSection="Section B-20", PBPScreenNumber="#20",PBPView="Section B: #20 Outpatient Drugs "},
                new ExitValidateMapModel() {PBPSection="Section D", PBPScreenNumber="",PBPView="Section D - Plan Level"}
            };
        }

        public DataTable GetExitValidateResultDataTable(int formInstanceId)
        {
            DataTable dt = new DataTable();
            try
            {
                List<ExitValidateResultViewModel> exitValidateResult = GetLatestExitValidateResults(formInstanceId);

                dt.Columns.Add("Result Id");
                dt.Columns.Add("Section");
                dt.Columns.Add("Screen");
                dt.Columns.Add("Question");
                dt.Columns.Add("Error");

                foreach (var model in exitValidateResult)
                {
                    DataRow row = dt.NewRow();
                    row["Result Id"] = model.ExitValidateResultID;
                    row["Section"] = model.Section;
                    row["Screen"] = model.Screen;
                    row["Question"] = model.Question;
                    row["Error"] = model.Error;
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return dt;
        }

        /// <summary>
        /// This methos checks if EV is completed for all formInstances and it does not have any validation errors
        /// </summary>
        /// <param name="folderversionId"></param>
        /// <returns></returns>
        public bool CheckExitValidationCompletedForFolderVersion(int folderversionId)
        {
            bool isExitValidationCompletedForFolderversion = false;
            try
            {
                var pbpFormDesignId = GlobalVariables.PBPDesignID;
                var pbpFormInstancesForTheExitValidation = (from frmInst in this._unitOfWork.Repository<FormInstance>().Get()
                                                            where frmInst.FolderVersionID == folderversionId && frmInst.FormDesignID == pbpFormDesignId && frmInst.IsActive == true
                                                            select frmInst).ToList();
                //Check formInstance has completed EV
                foreach (var frmInstance in pbpFormInstancesForTheExitValidation)
                {
                    var formInstanceEv = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                          where e.FormInstanceID == frmInstance.FormInstanceID && (e.Status == "Succeeded") && e.CompletedDate > (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate)
                                          select e).ToList();
                    if (formInstanceEv != null && formInstanceEv.Count() > 0)
                    {
                        var latestEVExecution = formInstanceEv.Where(row => row.IsLatest == true).FirstOrDefault();
                        if (latestEVExecution != null)
                        {
                            //Check if EV execution does not have validation errors
                            var exitValidationresult = (from evresult in this._unitOfWork.Repository<ExitValidateResult>().Get()
                                                        where evresult.ExitValidateQueueID == latestEVExecution.ExitValidateQueueID
                                                       && (!evresult.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package"))
                                                        select evresult.ExitValidateResultID);
                            if (exitValidationresult != null && exitValidationresult.Count() > 0)
                            {
                                return isExitValidationCompletedForFolderversion;
                            }
                        }
                    }
                    else
                    {
                        return isExitValidationCompletedForFolderversion;
                    }
                }
                isExitValidationCompletedForFolderversion = true;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isExitValidationCompletedForFolderversion;
        }

        public ServiceResult ExitValidateFolderversion(int folderversionId, int userId, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var pbpFormDesignId = GlobalVariables.PBPDesignID;
                var pbpFormInstancesForTheExitValidation = (from frmInst in this._unitOfWork.Repository<FormInstance>().Query().Include(inc => inc.FolderVersion).Get()
                                                            where frmInst.FolderVersionID == folderversionId && frmInst.FormDesignID == pbpFormDesignId && frmInst.IsActive == true
                                                            select frmInst).ToList();
                //Check formInstance has completed EV
                foreach (var frmInstance in pbpFormInstancesForTheExitValidation)
                {
                    var formInstanceEv = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                          where e.FormInstanceID == frmInstance.FormInstanceID && (e.CompletedDate > (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate))
                                          select e).ToList();
                    
                    if (formInstanceEv == null || formInstanceEv.Count == 0)
                    {
                        //Queue formInstance for EV
                        ExitValidateViewModel model = new ExitValidateViewModel();
                        model.AddedBy = userName;
                        model.AddedDate = DateTime.Now;
                        model.FolderVersionID = folderversionId;
                        model.FormInstanceID = frmInstance.FormInstanceID;
                        model.FormDesignVersionID = frmInstance.FormDesignVersionID;
                        model.UserID = userId;
                        model.ProductID = frmInstance.Name;
                        model.FormName = frmInstance.Name;
                        model.FolderID = frmInstance.FolderVersion.FolderID;
                        model.IsQueuedForWFStateUpdate = true;
                        model.UsersInterestedInStatus = "";
                        Validate(model);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Return True - Notify EV run Successfully without error
        /// Return false - Notify EV run Successfully with error
        /// Return null -  No need to sent Notification
        /// </summary>
        /// <param name="folderversionId"></param>
        /// <returns></returns>
        public bool? CheckEVNotificationSentToUser(int folderversionId)
        {
            bool? isExitValidationNotificationSent = null;
            try
            {
                var formInstanceForNotification = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                      where e.FolderVersionID == folderversionId && e.IsLatest == true && e.IsQueuedForWFStateUpdate == true && e.IsNotificationSent == false
                                      select e).ToList();

                if(formInstanceForNotification == null || formInstanceForNotification.Count == 0)
                {
                    return isExitValidationNotificationSent;
                }

                var pbpFormDesignId = GlobalVariables.PBPDesignID;
                var pbpFormInstancesForTheExitValidation = (from frmInst in this._unitOfWork.Repository<FormInstance>().Get()
                                                            where frmInst.FolderVersionID == folderversionId && frmInst.FormDesignID == pbpFormDesignId && frmInst.IsActive == true
                                                            select frmInst).ToList();
                //Check formInstance has completed EV
                var hasAllPlansCompletedEV = false;
                foreach (var frmInstance in pbpFormInstancesForTheExitValidation)
                {
                    var formInstanceEv = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                          where e.FormInstanceID == frmInstance.FormInstanceID && e.IsLatest == true && e.IsQueuedForWFStateUpdate == true && e.CompletedDate > (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate)
                                          select e).FirstOrDefault();
                    if (formInstanceEv != null && (formInstanceEv.Status == "Processing" || formInstanceEv.Status == "Enqueued" || formInstanceEv.Status == "Failed"))
                    {
                        return null;
                    }
                    else if (formInstanceEv != null && formInstanceEv.Status == "Succeeded")
                    {
                        hasAllPlansCompletedEV = true;
                    }
                }
                if (hasAllPlansCompletedEV)
                {
                    foreach (var frmInstance in pbpFormInstancesForTheExitValidation)
                    {
                        var formInstanceEv = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                              where e.FormInstanceID == frmInstance.FormInstanceID && e.IsLatest == true && e.IsQueuedForWFStateUpdate == true && e.CompletedDate > (frmInstance.UpdatedDate == null ? frmInstance.AddedDate : frmInstance.UpdatedDate)
                                              select e).FirstOrDefault();
                        var isNotificationSent = formInstanceEv.IsNotificationSent;
                        if (!isNotificationSent)
                        {
                            //Check if EV execution does not have validation errors
                            var exitValidationresult = (from evresult in this._unitOfWork.Repository<ExitValidateResult>().Get()
                                                        where evresult.ExitValidateQueueID == formInstanceEv.ExitValidateQueueID
                                                        && (!evresult.Error.Contains("Are you sure you want to delete Optional Supplemental Benefit Package"))
                                                        select evresult.ExitValidateResultID);
                            if (exitValidationresult != null && exitValidationresult.Count() > 0)
                            {
                                isExitValidationNotificationSent = false;
                            }
                            else
                            {
                                if (isExitValidationNotificationSent != false)
                                    isExitValidationNotificationSent = true;
                            }
                            formInstanceEv.IsNotificationSent = true;
                            this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Update(formInstanceEv);
                            this._unitOfWork.Save();
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
            return isExitValidationNotificationSent;
        }

        public List<FormInstance> GetFormInstancesForEV(int folderversionId)
        {
            List<FormInstance> formInstances = new List<FormInstance>();
            try
            {
                var pbpFormDesignId = GlobalVariables.PBPDesignID;
                formInstances = (from frmInst in this._unitOfWork.Repository<FormInstance>().Query().Include(inc => inc.FolderVersion).Get()
                                 where frmInst.FolderVersionID == folderversionId && frmInst.FormDesignID == pbpFormDesignId && frmInst.IsActive == true
                                 select frmInst).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstances;
        }

        public ExitValidateViewModel GetFormInstancesEVStatus(int formInstanceId)
        {
            ExitValidateViewModel _evModel = new ExitValidateViewModel();
            try
            {
                _evModel = (from e in this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                            where e.FormInstanceID == formInstanceId && e.IsLatest == true
                            orderby e.ExitValidateQueueID descending
                            select (new ExitValidateViewModel
                            {
                                FormInstanceID = e.FormInstanceID,
                                FolderVersionID = e.FolderVersionID,
                                FolderID = e.FolderID,
                                ProductID = e.ProductID,
                                UserID = e.UserID,
                                QueueID = e.ExitValidateQueueID,
                                JobId = e.JobId,
                                Status = e.Status,
                                CompletedDate = e.CompletedDate,
                                IsQueuedForWFStateUpdate = e.IsQueuedForWFStateUpdate
                            })).ToList<ExitValidateViewModel>().FirstOrDefault();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return _evModel;
        }

        private ServiceResult AddExitValidateExportError(ExitValidateViewModel evModel, string jsonPath, int fieldLength, string fieldName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                int formDesignVersionId = (from e in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                           where e.FormInstanceID == evModel.FormInstanceID
                                           select (e.FormDesignVersionID)).FirstOrDefault();

                string[] elList = jsonPath.Replace("[0]", "").Split('.');
                string sectionName = elList.FirstOrDefault();
                string elementName = elList.LastOrDefault();
                string screenName = elList[elList.Length - 2];

                int screenID = (from e in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                join m in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                on e.UIElementID equals m.UIElementID
                                where (e.GeneratedName == screenName && m.EffectiveDate.ToString().IndexOf("2019") != -1)
                                select e.UIElementID).ToList().FirstOrDefault();

                var elementScreenDetails = (from e in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                            join m in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                            on e.UIElementID equals m.UIElementID
                                            where (e.GeneratedName == screenName ||
                                            (e.GeneratedName == elementName && e.ParentUIElementID == screenID))
                                            && m.EffectiveDate.ToString().IndexOf("2019") != -1
                                            select new
                                            {
                                                e.UIElementID,
                                                e.UIElementName,
                                                e.GeneratedName,
                                                e.Label,
                                            }).ToList();

                List<SectionDesign> sectionList = GetSectionsList(1, formDesignVersionId.ToString(), _formDesignService);
                string ParentSectionLabel = sectionList.Where(s => s.GeneratedName.Contains(sectionName)).Select(s => s.Label).FirstOrDefault().ToString();
                ExitValidateMapModel SectionDtails = GetSectionMapModels().Where(s => s.PBPView.Trim() == ParentSectionLabel.Trim()).Select(s => s).FirstOrDefault();

                if (SectionDtails != null)
                {
                    ExitValidateResult AddToInsert = new ExitValidateResult();
                    AddToInsert.ExitValidateQueueID = evModel.QueueID;
                    AddToInsert.FormInstanceID = evModel.FormInstanceID;
                    AddToInsert.ContractNumber = "";
                    AddToInsert.PlanName = "";
                    AddToInsert.Section = SectionDtails == null ? string.Empty : SectionDtails.PBPSection;
                    AddToInsert.Status = string.Empty;
                    AddToInsert.Result = string.Empty;
                    AddToInsert.Error = "Character limit exceeded. (max-" + fieldLength + ")";
                    AddToInsert.Question = elementScreenDetails.Where(x => x.GeneratedName == elementName).Select(x => x.Label).FirstOrDefault().ToString();
                    AddToInsert.Screen = (SectionDtails == null ? ParentSectionLabel.Split(' ')[2] : SectionDtails.PBPScreenNumber) + " " +
                                            elementScreenDetails.Where(x => x.GeneratedName == screenName).Select(x => x.Label).FirstOrDefault().ToString();
                    AddToInsert.PBPFIELD = fieldName;
                    //AddToInsert.PBPCOLUMN = model.COLUMN;

                    this._unitOfWork.RepositoryAsync<ExitValidateResult>().Insert(AddToInsert);
                    this._unitOfWork.Save();
                }

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                string customMsg = "Character limit failed for Field: " + fieldName + " json Path: " + jsonPath + " FormInstanceID: " + evModel.FormInstanceID;
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);

                result.Result = ServiceResultStatus.Failure;
            }
            return result;

        }

        public ExitValidateQueue GetExitValidateQueue(int ExitValidateQueueID)
        {
            var evQueue = this._unitOfWork.RepositoryAsync<ExitValidateQueue>().Get()
                                                    .Where(s => s.ExitValidateQueueID.Equals(ExitValidateQueueID)
                                                    ).FirstOrDefault();
            return evQueue;
        }

    }
}
