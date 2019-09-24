using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.emailnotification;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.emailnotification.Model;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.caching;
using Newtonsoft.Json;
using tmg.equinox.notification;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using tmg.equinox.setting.Interface;
using tmg.equinox.setting.Common;

namespace tmg.equinox.applicationservices
{
    public class PlanTaskUserMappingService : IPlanTaskUserMappingService
    {
        #region Private Memebers
        private Func<string, IUnitOfWorkAsync> _emailUnitOfWork { get; set; }
        private ISettingManager _settingManager;

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        INotificationService _notificationService;
        #endregion Private Members

        #region Constructor
        public PlanTaskUserMappingService(Func<string, IUnitOfWorkAsync> emailUnitOfWork, IUnitOfWorkAsync unitOfWork, INotificationService notificationService, ISettingManager settingManager)
        {
            this._unitOfWork = unitOfWork;
            _emailUnitOfWork = emailUnitOfWork;
            _notificationService = notificationService;
            _settingManager = settingManager;
        }
        #endregion Constructor

        public DPFPlanTaskUserMapping GetDPFPlanTaskUserMapping(int PlanTaskUserMappingId)
        {
            DPFPlanTaskUserMapping planTask = null;

            planTask = (from c in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(row => row.ID == PlanTaskUserMappingId) select c).FirstOrDefault();
            return planTask;
        }

        public IEnumerable<KeyValue> GetTeamMemberList(string strUserName)
        {
            int UserId = this._unitOfWork.RepositoryAsync<User>().Query()
                    .Filter(c => c.UserName == strUserName).Get().Select(c => c.UserID).FirstOrDefault();

            int TeamID = this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Query()
                    .Filter(c => c.UserID == UserId).Get().Select(c => c.ApplicableTeamID).FirstOrDefault();

            IList<KeyValue> ownerList = new List<KeyValue>();
            List<KeyValue> listObjects = new List<KeyValue>();
            try
            {
                ownerList = (from user in this._unitOfWork.RepositoryAsync<User>()
                                                                .Query()
                                                                .Get().Where(c => c.IsActive == true)
                             join teamAsoc in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(a => a.ApplicableTeamID == TeamID && a.IsDeleted == false)
           on user.UserID equals teamAsoc.UserID
                             select new KeyValue
                             {
                                 Key = user.UserID,
                                 Value = user.UserName
                             }).ToList();

                if (ownerList.Count() == 0)
                    ownerList = null;

                listObjects = (from obj in ownerList
                               select obj).GroupBy(n => new { n.Key })
                                                         .Select(g => g.FirstOrDefault())
                                                         .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return listObjects;
        }

        public ServiceResult SavePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            ServiceResult result = new ServiceResult();
            DPFPlanTaskUserMapping objPlanTaskUserMapping;
            try
            {
                if (objPlanTaskModel != null)
                {
                    for (int i = 0; i < objPlanTaskModel.FormInstanceIdList.Count; i++)
                    {
                        objPlanTaskUserMapping = new DPFPlanTaskUserMapping();
                        objPlanTaskUserMapping.WFStateID = objPlanTaskModel.WFStateID;
                        objPlanTaskUserMapping.TaskID = objPlanTaskModel.TaskID;
                        objPlanTaskUserMapping.AssignedDate = DateTime.Now;
                        objPlanTaskUserMapping.AssignedUserName = objPlanTaskModel.AssignedUserName;
                        objPlanTaskUserMapping.ManagerUserName = objPlanTaskModel.ManagerUserName;
                        objPlanTaskUserMapping.StartDate = objPlanTaskModel.StartDate;
                        objPlanTaskUserMapping.DueDate = objPlanTaskModel.DueDate;
                        objPlanTaskUserMapping.Status = !String.IsNullOrEmpty(objPlanTaskModel.Status.Trim())? objPlanTaskModel.Status : WatchTaskStatus.Assigned.ToString();
                        //Check the due date if due date is less than current saving date then mark status as Late
                        var currentDateOnly = DateTime.Now.Date;
                        if (objPlanTaskUserMapping.DueDate < currentDateOnly)
                        {
                            objPlanTaskUserMapping.Status = WatchTaskStatus.Late.ToString();
                            SendLatePushNotification(objPlanTaskModel, objPlanTaskModel.UpdatedBy);
                        }
                        //TODO: Added code temporary to get the functionality working                    
                        objPlanTaskUserMapping.Order = objPlanTaskModel.Order;
                        objPlanTaskUserMapping.Duration = objPlanTaskModel.Duration;
                        //objPlanTaskUserMapping.Comments = Convert.ToInt32(objPlanTaskModel.Comments);
                        objPlanTaskUserMapping.CompletedDate = objPlanTaskModel.CompletedDate;
                        objPlanTaskUserMapping.AddedDate = objPlanTaskModel.AddedDate = DateTime.Now;
                        objPlanTaskUserMapping.AddedBy = objPlanTaskModel.AddedBy;
                        objPlanTaskUserMapping.UpdatedDate = objPlanTaskModel.UpdatedDate = DateTime.Now;
                        objPlanTaskUserMapping.UpdatedBy = objPlanTaskModel.UpdatedBy;
                        //objPlanTaskUserMapping.Attachment = objPlanTaskModel.Attachment;
                        objPlanTaskUserMapping.ActualTime = objPlanTaskModel.ActualTime;
                        objPlanTaskUserMapping.EstimatedTime = objPlanTaskModel.EstimatedTime;
                        if (objPlanTaskUserMapping.Status == WatchTaskStatus.Late.ToString())
                            objPlanTaskUserMapping.LateStatusDone = true;
                        objPlanTaskUserMapping.FolderVersionID = objPlanTaskModel.FolderVersionID;

                        objPlanTaskModel.Plan = objPlanTaskModel.FormInstanceNameList[i];
                        objPlanTaskUserMapping.PlanTaskUserMappingDetails = CreateTaskDesignJSON(objPlanTaskModel, Convert.ToString(objPlanTaskModel.FormInstanceIdList[i]));

                        this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Insert(objPlanTaskUserMapping);
                        this._unitOfWork.Save();
                        int planTaskMappingId = objPlanTaskUserMapping.ID;

                        //Save task Comments from grid
                        var taskCommentsData = JsonConvert.DeserializeObject(objPlanTaskModel.TaskComments);
                        List<JToken> taskCommentsList = ((JArray)taskCommentsData).ToList();
                        CommentViewModel taskCommentsModel = null;
                        for (int j = taskCommentsList.Count - 1; j >= 0; j--)
                        {
                            taskCommentsModel = new CommentViewModel();
                            taskCommentsModel.AddedBy = objPlanTaskUserMapping.AddedBy;
                            taskCommentsModel.AddedDate = Convert.ToDateTime(taskCommentsList[j]["Datetimestamp"]);
                            taskCommentsModel.Comment = Convert.ToString(taskCommentsList[j]["Comment"]);
                            taskCommentsModel.TaskID = planTaskMappingId;
                            taskCommentsModel.FolderVersionID = Convert.ToInt32(taskCommentsList[j]["FolderVersionID"]);
                            taskCommentsModel.Attachment = Convert.ToString(taskCommentsList[j]["Attachment"]);
                            taskCommentsModel.filename = Convert.ToString(taskCommentsList[j]["filename"]);
                            taskCommentsModel.Status = objPlanTaskModel.Status;
                            bool isSave = SaveTaskComments(taskCommentsModel);
                        }

                        List<Paramters> paramater = new List<Paramters>();
                        paramater.Add(new Paramters { key = "user", Value = objPlanTaskUserMapping.AddedBy });
                        paramater.Add(new Paramters { key = "taskName", Value = objPlanTaskModel.TaskDescription });
                        paramater.Add(new Paramters { key = "Folder name", Value = objPlanTaskModel.FolderName });
                        paramater.Add(new Paramters { key = "Accountname", Value = objPlanTaskModel.AccountName });



                        string[] assignedUsers = new string[] { };
                        if (objPlanTaskModel.AssignedUserName.IndexOf(',') >= 0)
                            assignedUsers = objPlanTaskModel.AssignedUserName.Split(',');
                        else
                            assignedUsers = new string[] { objPlanTaskModel.AssignedUserName };

                        foreach (string user in assignedUsers)
                        {

                            _notificationService.SendNotification(
                                new NotificationInfo
                                {
                                    SentTo = user,
                                    MessageKey = MessageKey.TASK_ASSIGNED,
                                    ParamterValues = paramater,
                                    loggedInUserName = objPlanTaskUserMapping.AddedBy,

                                });
                        }
                    }
                }
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
            return result;
        }

        public ServiceResult DeletePlanTaskUserMappingByFolderversionId(int folderversionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var planTaskMappings = (from c in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(row => row.FolderVersionID == folderversionId) select c).ToList();
                foreach (DPFPlanTaskUserMapping planTask in planTaskMappings)
                {
                    this._unitOfWork.RepositoryAsync<TaskComments>().DeleteRange(c => c.ID == planTask.ID);
                    this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Delete(planTask);
                    this._unitOfWork.Save();
                }
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
            return result;
        }

        public ServiceResult DeletePlanTaskUserMappingByFormInstanceId(int folderversionId, int formInstanceId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var planTaskMappings = (from c in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(row => row.FolderVersionID == folderversionId) select c).ToList();
                foreach (DPFPlanTaskUserMapping planTask in planTaskMappings)
                {
                    if (!String.IsNullOrEmpty(planTask.PlanTaskUserMappingDetails))
                    {
                        var designDetails = JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(planTask.PlanTaskUserMappingDetails);
                        if (designDetails.FormInstanceId.ToString().Trim() == formInstanceId.ToString())
                        {
                            this._unitOfWork.RepositoryAsync<TaskComments>().DeleteRange(c => c.ID == planTask.ID);
                            this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Delete(planTask);
                            this._unitOfWork.Save();
                        }
                    }
                }
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
            return result;
        }
        public Boolean SavetaskPlanNewFolderVersion(int FolderversionId, string currentUser)
        {

            DPFPlanTaskUserMapping objPlanTaskUserMapping = new DPFPlanTaskUserMapping();

            List<TaskListViewModel> stdTaskList = null;
            List<User> managerList = (from user in this._unitOfWork.RepositoryAsync<User>().Get()
                                      join team in this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Get().Where(c => c.IsDeleted == false && c.IsTeamManager == true)
                                      on user.UserID equals team.UserID
                                      select user
                                         ).ToList();



            stdTaskList = (from task in _unitOfWork.RepositoryAsync<TaskList>().Get().Where(t => t.IsStandardTask == true)
                           join wFTaskMap in _unitOfWork.RepositoryAsync<WorkflowTaskMap>().Get()
                           on task.TaskID equals wFTaskMap.TaskID
                           where wFTaskMap.IsActive == true
                           select new TaskListViewModel
                           {
                               AddedBy = task.AddedBy,
                               AddedDate = DateTime.Now,
                               WFStateID = wFTaskMap.WFStateID,
                               TaskID = task.TaskID,
                               TaskDescription = task.TaskDescription,
                           }).ToList();

            foreach (var item in stdTaskList)
            {
                objPlanTaskUserMapping.AddedBy = currentUser;
                objPlanTaskUserMapping.UpdatedBy = currentUser;
                objPlanTaskUserMapping.FolderVersionID = FolderversionId;
                objPlanTaskUserMapping.WFStateID = item.WFStateID;
                objPlanTaskUserMapping.TaskID = item.TaskID;
                objPlanTaskUserMapping.AddedDate = DateTime.Now;
                objPlanTaskUserMapping.AssignedDate = DateTime.Now;
                objPlanTaskUserMapping.StartDate = DateTime.Now;
                objPlanTaskUserMapping.DueDate = DateTime.Now;
                objPlanTaskUserMapping.UpdatedDate = DateTime.Now;

                //objPlanTaskUserMapping.AssignedUserName = item.AddedBy;
                //objPlanTaskUserMapping.ManagerUserName = item.AddedBy;
                objPlanTaskUserMapping.Status = "Assigned";
                objPlanTaskUserMapping.Duration = 1;
                _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Insert(objPlanTaskUserMapping);
                _unitOfWork.Save();

                //send notification
                string folderName = (from folder in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                     join version in this._unitOfWork.RepositoryAsync<FolderVersion>().Get().Where(c => c.FolderVersionID == FolderversionId)
                                     on folder.FolderID equals version.FolderID
                                     select folder.Name).FirstOrDefault();

                List<Paramters> paramater = new List<Paramters>();
                paramater.Add(new Paramters { key = "Folder name", Value = folderName });
                paramater.Add(new Paramters { key = "taskName", Value = item.TaskDescription });

                foreach (User user in managerList)
                {
                    _notificationService.SendNotification(
                        new NotificationInfo
                        {
                            SentTo = user.UserName,
                            MessageKey = MessageKey.TASK_STANDARD_CREATED,
                            ParamterValues = paramater,
                            loggedInUserName = objPlanTaskUserMapping.AddedBy,
                        });
                }
            }
            return true;

        }


        private string CreateTaskDesignJSON(DPFPlanTaskUserMappingViewModel objPlanTaskModel, string formInstanceID)
        {
            string planTaskUserMappingDetailsJSON = string.Empty;
            string traverseString = "FormDesignVersionId:{FormDesignVersionId},FormInstanceId:{FormInstanceId},SectionId:{SectionId}";
            string viewID = string.Empty, formInstanceId = string.Empty, sectionID = string.Empty; ;
            try
            {
                PlanTaskUserMappingDetails planTaskUserMappingDetails = new PlanTaskUserMappingDetails();

                planTaskUserMappingDetails.SectionId = objPlanTaskModel.SectionID;
                planTaskUserMappingDetails.FormDesignVersionId = objPlanTaskModel.ViewID;
                planTaskUserMappingDetails.FormInstanceId = formInstanceID;//string.Join(",", objPlanTaskModel.FormInstanceIdList);
                planTaskUserMappingDetails.FormInstanceLabel = objPlanTaskModel.Plan;//string.Join(",", objPlanTaskModel.FormInstanceNameList);
                planTaskUserMappingDetails.SectionLabel = objPlanTaskModel.SectionLabels;
                planTaskUserMappingDetails.FormDesignVersionLabel = objPlanTaskModel.ViewLabels;

                viewID = objPlanTaskModel.ViewID != null ? objPlanTaskModel.ViewID : string.Empty;
                formInstanceId = planTaskUserMappingDetails.FormInstanceId != null ? planTaskUserMappingDetails.FormInstanceId : string.Empty;
                sectionID = objPlanTaskModel.SectionID != null ? objPlanTaskModel.SectionID : string.Empty;

                var first = viewID.IndexOf(',') >= 0 ? viewID.Substring(0, viewID.IndexOf(',')) : viewID;
                traverseString = traverseString.Replace("{FormDesignVersionId}", first);

                first = formInstanceId.IndexOf(',') >= 0 ? formInstanceId.Substring(0, formInstanceId.IndexOf(',')) : formInstanceId;
                traverseString = traverseString.Replace("{FormInstanceId}", first);

                first = sectionID.IndexOf(',') >= 0 ? sectionID.Substring(0, sectionID.IndexOf(',')) : sectionID;
                traverseString = traverseString.Replace("{SectionId}", first);

                planTaskUserMappingDetails.TaskTraverseDetails = traverseString;
                planTaskUserMappingDetailsJSON = JsonConvert.SerializeObject(planTaskUserMappingDetails);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                return null;
            }
            return planTaskUserMappingDetailsJSON;
        }

        public ServiceResult UpdatePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            ServiceResult result = new ServiceResult();
            string exsistingUser = string.Empty;
            try
            {
                if (objPlanTaskModel != null)
                {
                    DPFPlanTaskUserMapping objDPFPlanTaskUserMapping = _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
                                                            .Where(i => i.ID == objPlanTaskModel.ID)
                                                            .Select(i => i).FirstOrDefault();
                    if (objDPFPlanTaskUserMapping != null)
                    {
                        exsistingUser = objDPFPlanTaskUserMapping.AssignedUserName;
                        objDPFPlanTaskUserMapping.ID = objPlanTaskModel.ID;
                        objDPFPlanTaskUserMapping.WFStateID = objPlanTaskModel.WFStateID;
                        objDPFPlanTaskUserMapping.TaskID = objPlanTaskModel.TaskID;
                        objDPFPlanTaskUserMapping.AssignedDate = DateTime.Now;
                        objDPFPlanTaskUserMapping.AssignedUserName = objPlanTaskModel.AssignedUserName;
                        objDPFPlanTaskUserMapping.ManagerUserName = objPlanTaskModel.ManagerUserName;
                        objDPFPlanTaskUserMapping.StartDate = objPlanTaskModel.StartDate;
                        objDPFPlanTaskUserMapping.DueDate = objPlanTaskModel.DueDate;
                        objDPFPlanTaskUserMapping.Status = objPlanTaskModel.Status;
                        objDPFPlanTaskUserMapping.CompletedDate = objPlanTaskModel.CompletedDate;
                        objDPFPlanTaskUserMapping.UpdatedDate = objPlanTaskModel.UpdatedDate = DateTime.Now;
                        objDPFPlanTaskUserMapping.UpdatedBy = objPlanTaskModel.UpdatedBy;
                        objDPFPlanTaskUserMapping.FolderVersionID = objPlanTaskModel.FolderVersionID;
                        objDPFPlanTaskUserMapping.EstimatedTime = objPlanTaskModel.EstimatedTime;
                        objDPFPlanTaskUserMapping.ActualTime = objPlanTaskModel.ActualTime;
                        objDPFPlanTaskUserMapping.Order = objPlanTaskModel.Order;
                        objDPFPlanTaskUserMapping.Duration = objPlanTaskModel.Duration;
                        if (!objDPFPlanTaskUserMapping.LateStatusDone)
                        {
                            var currentDateOnly = DateTime.Now.Date;
                            if (objDPFPlanTaskUserMapping.DueDate < currentDateOnly)
                            {
                                objDPFPlanTaskUserMapping.Status = WatchTaskStatus.Late.ToString();
                                objDPFPlanTaskUserMapping.LateStatusDone = true;
                                SendLatePushNotification(objPlanTaskModel, objPlanTaskModel.UpdatedBy);
                            }
                        }
                        if (objDPFPlanTaskUserMapping.Status == WatchTaskStatus.Completed.ToString() || objDPFPlanTaskUserMapping.Status == GlobalVariables.CompletedFail || objDPFPlanTaskUserMapping.Status == GlobalVariables.CompletedPass)
                        {
                            objDPFPlanTaskUserMapping.CompletedDate = DateTime.Now;
                            objDPFPlanTaskUserMapping.LateStatusDone = true;
                            objDPFPlanTaskUserMapping.LateStatusDone = true;
                        }
                        if (objDPFPlanTaskUserMapping.PlanTaskUserMappingDetails != null)
                        {
                            var designDetails = JsonConvert.DeserializeObject<PlanTaskUserMappingDetails>(objDPFPlanTaskUserMapping.PlanTaskUserMappingDetails);
                            if (designDetails.SectionId != objPlanTaskModel.SectionID || designDetails.FormDesignVersionId != objPlanTaskModel.ViewID)
                            {
                                objDPFPlanTaskUserMapping.PlanTaskUserMappingDetails = CreateTaskDesignJSON(objPlanTaskModel, Convert.ToString(objPlanTaskModel.FormInstanceIdList[0]));
                            }
                        }
                        else
                            objDPFPlanTaskUserMapping.PlanTaskUserMappingDetails = CreateTaskDesignJSON(objPlanTaskModel, Convert.ToString(objPlanTaskModel.FormInstanceIdList[0]));
                        this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(objDPFPlanTaskUserMapping);
                        this._unitOfWork.Save();

                        var taskCommentsData = JsonConvert.DeserializeObject(objPlanTaskModel.TaskComments);
                        List<JToken> taskCommentsList = ((JArray)taskCommentsData).ToList();
                        CommentViewModel taskCommentsModel = null;
                        for (int i = taskCommentsList.Count - 1; i >= 0; i--)
                        {
                            taskCommentsModel = new CommentViewModel();
                            taskCommentsModel.AddedBy = objPlanTaskModel.UpdatedBy;
                            taskCommentsModel.AddedDate = DateTime.Now;
                            taskCommentsModel.Comment = Convert.ToString(taskCommentsList[i]["Comment"]);
                            taskCommentsModel.FolderVersionID = objDPFPlanTaskUserMapping.FolderVersionID;
                            taskCommentsModel.Attachment = Convert.ToString(taskCommentsList[i]["Attachment"]);
                            taskCommentsModel.filename = Convert.ToString(taskCommentsList[i]["filename"]);
                            taskCommentsModel.TaskID = objDPFPlanTaskUserMapping.ID;
                            taskCommentsModel.Status = objDPFPlanTaskUserMapping.Status;
                            bool isSave = SaveTaskComments(taskCommentsModel);
                        }

                    }

                    List<Paramters> paramater = new List<Paramters>();
                    paramater.Add(new Paramters { key = "user", Value = objPlanTaskModel.UpdatedBy });
                    paramater.Add(new Paramters { key = "taskName", Value = objPlanTaskModel.TaskDescription });
                    paramater.Add(new Paramters { key = "Folder name", Value = objPlanTaskModel.FolderName });
                    paramater.Add(new Paramters { key = "Accountname", Value = objPlanTaskModel.AccountName });

                    string[] assignedUsers = new string[] { };
                    string[] preAssignedUsers = new string[] { };
                    string[] newUsers = new string[] { };
                    if (objPlanTaskModel.AssignedUserName.IndexOf(',') >= 0)
                        assignedUsers = objPlanTaskModel.AssignedUserName.Split(',');
                    else
                        assignedUsers = new string[] { objPlanTaskModel.AssignedUserName };

                    exsistingUser = exsistingUser != null ? exsistingUser : objPlanTaskModel.AssignedUserName;
                    if (exsistingUser.IndexOf(',') >= 0)
                        preAssignedUsers = exsistingUser.Split(',');
                    else
                        preAssignedUsers = new string[] { exsistingUser };

                    foreach (string user in assignedUsers)
                    {

                        _notificationService.SendNotification(
                            new NotificationInfo
                            {
                                SentTo = user,
                                MessageKey = preAssignedUsers.Contains(user) ? MessageKey.TASK_UPDATE : MessageKey.TASK_ASSIGNED,
                                ParamterValues = paramater,
                                loggedInUserName = objPlanTaskModel.UpdatedBy,
                            });

                    }
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public ServiceResult UpdateQueuePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DPFPlanTaskUserMapping objPlanTaskUserMapping = new DPFPlanTaskUserMapping();
                string previousStatus = string.Empty;
                if (objPlanTaskModel != null)
                {
                    DPFPlanTaskUserMapping objDPFPlanTaskUserMapping = _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
                                                            .Where(i => i.ID == objPlanTaskModel.ID)
                                                            .Select(i => i).FirstOrDefault();

                    if (objDPFPlanTaskUserMapping != null)
                    {
                        previousStatus = objDPFPlanTaskUserMapping.Status;
                        objPlanTaskUserMapping.ID = objPlanTaskModel.ID;
                        objPlanTaskUserMapping.Status = objPlanTaskModel.Status;
                        if (objPlanTaskModel.Status == WatchTaskStatus.Completed.ToString() || objPlanTaskModel.Status == GlobalVariables.CompletedFail || objPlanTaskModel.Status == GlobalVariables.CompletedPass)
                        {
                            objPlanTaskUserMapping.CompletedDate = DateTime.Now;
                            objPlanTaskUserMapping.LateStatusDone = true;
                        }
                        objPlanTaskUserMapping.UpdatedDate = objPlanTaskModel.UpdatedDate;
                        objDPFPlanTaskUserMapping.UpdatedBy = objPlanTaskModel.UpdatedBy;
                        objDPFPlanTaskUserMapping.Status = objPlanTaskUserMapping.Status;
                        objDPFPlanTaskUserMapping.CompletedDate = objPlanTaskUserMapping.CompletedDate;
                        objDPFPlanTaskUserMapping.LateStatusDone = objPlanTaskUserMapping.LateStatusDone;
                        objDPFPlanTaskUserMapping.ActualTime = objPlanTaskModel.ActualTime;
                        this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(objDPFPlanTaskUserMapping);
                        this._unitOfWork.Save();

                        var taskCommentsData = JsonConvert.DeserializeObject(objPlanTaskModel.TaskComments);
                        List<JToken> taskCommentsList = ((JArray)taskCommentsData).ToList();
                        CommentViewModel taskCommentsModel = null;
                        for (int i = taskCommentsList.Count - 1; i >= 0; i--)
                        {
                            taskCommentsModel = new CommentViewModel();
                            taskCommentsModel.AddedBy = objDPFPlanTaskUserMapping.UpdatedBy;
                            taskCommentsModel.AddedDate = DateTime.Now;
                            taskCommentsModel.Comment = Convert.ToString(taskCommentsList[i]["Comment"]);
                            taskCommentsModel.FolderVersionID = objDPFPlanTaskUserMapping.FolderVersionID;
                            taskCommentsModel.Attachment = Convert.ToString(taskCommentsList[i]["Attachment"]);
                            taskCommentsModel.filename = Convert.ToString(taskCommentsList[i]["filename"]);
                            taskCommentsModel.TaskID = objDPFPlanTaskUserMapping.ID;
                            taskCommentsModel.Status = objDPFPlanTaskUserMapping.Status;
                            bool isSave = SaveTaskComments(taskCommentsModel);
                        }
                    }

                    //send notification
                    if (previousStatus != objPlanTaskModel.Status)
                    {
                        List<Paramters> paramater = new List<Paramters>();
                        paramater.Add(new Paramters { key = "user", Value = objPlanTaskModel.UpdatedBy });
                        paramater.Add(new Paramters { key = "taskName", Value = objPlanTaskModel.TaskDescription });
                        paramater.Add(new Paramters { key = "Folder name", Value = objPlanTaskModel.FolderName });
                        paramater.Add(new Paramters { key = "Accountname", Value = objPlanTaskModel.AccountName });

                        _notificationService.SendNotification(
                            new NotificationInfo
                            {
                                //SentTo = objPlanTaskModel.ManagerUserName,

                                SentTo = objDPFPlanTaskUserMapping.ManagerUserName != null ? objDPFPlanTaskUserMapping.ManagerUserName : string.Empty,
                                MessageKey = objPlanTaskModel.Status.Contains("Completed") ? MessageKey.TASK_COMPLETED : MessageKey.TASK_STATUS_UPDATE,
                                ParamterValues = paramater,
                                loggedInUserName = objPlanTaskModel.UpdatedBy,
                            });
                    }

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        private List<Paramters> SetEmailNotificationParamters(DPFPlanTaskUserMappingViewModel objPlanTaskModel)
        {
            List<Paramters> paramater = new List<Paramters>();
            paramater.Add(new Paramters { key = "user", Value = "superuser" });
            paramater.Add(new Paramters { key = "taskName", Value = objPlanTaskModel.TaskDescription });
            paramater.Add(new Paramters { key = "Folder name", Value = objPlanTaskModel.FolderName });

            return paramater;
        }
        public bool SendEmailForNewTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM)
        {
            bool isEmailSend = false;

            string userName = objPlanTaskVM.AssignedUserName;
            string UserEmail = string.Empty;
            string ManagerEmail = string.Empty;
            GetEmailIdForUserAndTeamManager(userName, ref UserEmail, ref ManagerEmail);

            if (!string.IsNullOrEmpty(UserEmail))
            {
                var _emailService = new EmailNotificationService(this._emailUnitOfWork);
                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.NewTaskAssignment);
                templateInfo.SetValue("#TaskDescription#", objPlanTaskVM.TaskDescription);
                templateInfo.SetValue("#Plan#", objPlanTaskVM.Plan);
                templateInfo.SetValue("#FolderName#", objPlanTaskVM.FolderName);
                templateInfo.SetValue("#DueDate#", FormatDateField(objPlanTaskVM.DueDate));
                templateInfo.SetValue("#EffectiveDate#", FormatDateField(objPlanTaskVM.EffectiveDate));
                templateInfo.SetValue("#WorkflowState#", objPlanTaskVM.WorkflowState);
                templateInfo.SetValue("#AssignedUserName#", objPlanTaskVM.AssignedUserName);

                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                emailNotificationInfo.TemplateInfo = templateInfo;
                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                emailNotificationInfo.ToAddresses = new List<string> { UserEmail };
                //emailNotificationInfo.CCAddresses = new List<string> { "cc Address1", "cc Address2", "cc Address3" };
                //emailNotificationInfo.Attachments = new List<string> { "attachement path", "attachement path1", "attachement path2" };
                emailNotificationInfo.Source = "New task assignment Email";
                emailNotificationInfo.SourceDescription = "Sending New task assignment Email to assigned user";

                _emailService.SendEmail(emailNotificationInfo);
                isEmailSend = true;
            }
            //_emailService.Execute();
            return isEmailSend;
        }

        public bool SendEmailForNewTaskAssignmentForAllPlans(DPFPlanTaskUserMappingViewModel objPlanTaskVM)
        {
            bool isEmailSend = false;

            string userName = objPlanTaskVM.AssignedUserName;
            string UserEmail = string.Empty;
            string ManagerEmail = string.Empty;
            GetEmailIdForUserAndTeamManager(userName, ref UserEmail, ref ManagerEmail);

            if (!string.IsNullOrEmpty(UserEmail))
            {
                var _emailService = new EmailNotificationService(this._emailUnitOfWork);
                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.NewTaskAssignmentForAllPlans);
                templateInfo.SetValue("#TaskDescription#", objPlanTaskVM.TaskDescription);
                templateInfo.SetValue("#Plan#", objPlanTaskVM.Plan);
                templateInfo.SetValue("#FolderName#", objPlanTaskVM.FolderName);
                templateInfo.SetValue("#DueDate#", FormatDateField(objPlanTaskVM.DueDate));
                templateInfo.SetValue("#EffectiveDate#", FormatDateField(objPlanTaskVM.EffectiveDate));
                templateInfo.SetValue("#WorkflowState#", objPlanTaskVM.WorkflowState);
                templateInfo.SetValue("#AssignedUserName#", objPlanTaskVM.AssignedUserName);

                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                emailNotificationInfo.TemplateInfo = templateInfo;
                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                emailNotificationInfo.ToAddresses = new List<string> { UserEmail };
                //emailNotificationInfo.CCAddresses = new List<string> { "cc Address1", "cc Address2", "cc Address3" };
                //emailNotificationInfo.Attachments = new List<string> { "attachement path", "attachement path1", "attachement path2" };
                emailNotificationInfo.Source = "New task assignment Email";
                emailNotificationInfo.SourceDescription = "Sending New task assignment Email to assigned user";

                _emailService.SendEmail(emailNotificationInfo);
                isEmailSend = true;
                //_emailService.Execute();
            }

            return isEmailSend;
        }

        public bool SendEmailForTaskCompletion(DPFPlanTaskUserMappingViewModel objPlanTaskVM)
        {
            bool isEmailSend = false;

            string userName = objPlanTaskVM.AssignedUserName;
            string UserEmail = string.Empty;
            string ManagerEmail = string.Empty;
            GetEmailIdForUserAndTeamManager(userName, ref UserEmail, ref ManagerEmail);
            // Get Details for the Plan Id
            List<DPFPlanTaskUserMappingViewModel> objPlanList = GetPlanTaskUserMappingList(objPlanTaskVM.ID);
            if (objPlanList != null && objPlanList.Count > 0)
            {
                objPlanTaskVM.TaskDescription = objPlanList[0].TaskDescription;
                objPlanTaskVM.Plan = objPlanList[0].Plan;
                objPlanTaskVM.FolderName = objPlanList[0].FolderName;
                objPlanTaskVM.DueDate = objPlanList[0].DueDate;
                objPlanTaskVM.EffectiveDate = objPlanList[0].EffectiveDate;
                objPlanTaskVM.WorkflowState = objPlanList[0].WorkflowState;
            }

            if (!string.IsNullOrEmpty(UserEmail) && !string.IsNullOrEmpty(ManagerEmail))
            {
                var _emailService = new EmailNotificationService(this._emailUnitOfWork);
                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.TaskCompletion);
                templateInfo.SetValue("#TaskDescription#", objPlanTaskVM.TaskDescription);
                templateInfo.SetValue("#Plan#", objPlanTaskVM.Plan);
                templateInfo.SetValue("#FolderName#", objPlanTaskVM.FolderName);
                templateInfo.SetValue("#DueDate#", FormatDateField(objPlanTaskVM.DueDate));
                templateInfo.SetValue("#EffectiveDate#", FormatDateField(objPlanTaskVM.EffectiveDate));
                templateInfo.SetValue("#WorkflowState#", objPlanTaskVM.WorkflowState);
                templateInfo.SetValue("#AssignedUserName#", objPlanTaskVM.AssignedUserName);

                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                emailNotificationInfo.TemplateInfo = templateInfo;
                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                if (UserEmail.Equals(ManagerEmail))
                {
                    emailNotificationInfo.ToAddresses = new List<string> { UserEmail };
                }
                else
                {
                    emailNotificationInfo.ToAddresses = new List<string> { UserEmail, ManagerEmail };
                }
                //emailNotificationInfo.CCAddresses = new List<string> { "cc Address1", "cc Address2", "cc Address3" };
                //emailNotificationInfo.Attachments = new List<string> { "attachement path", "attachement path1", "attachement path2" };
                emailNotificationInfo.Source = "Task Complition Email";
                emailNotificationInfo.SourceDescription = "Sending New task complition Email to assigned user and team manager";

                _emailService.SendEmail(emailNotificationInfo);
                isEmailSend = true;
            }
            //_emailService.Execute();
            return isEmailSend;
        }

        public bool SendEmailForChangesInPlanAndTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM)
        {
            bool isEmailSend = false;
            string UserEmail = this._unitOfWork.RepositoryAsync<User>().Query()
                  .Filter(c => c.UserName == objPlanTaskVM.AssignedUserName).Get().Select(c => c.Email).FirstOrDefault();

            if (!string.IsNullOrEmpty(UserEmail))
            {
                var _emailService = new EmailNotificationService(this._emailUnitOfWork);
                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.ChangesInPlanAndTaskAssignment);
                templateInfo.SetValue("#TaskDescription#", objPlanTaskVM.TaskDescription);
                templateInfo.SetValue("#Plan#", objPlanTaskVM.Plan);
                templateInfo.SetValue("#FolderName#", objPlanTaskVM.FolderName);
                templateInfo.SetValue("#DueDate#", FormatDateField(objPlanTaskVM.DueDate));
                templateInfo.SetValue("#EffectiveDate#", FormatDateField(objPlanTaskVM.EffectiveDate));
                templateInfo.SetValue("#WorkflowState#", objPlanTaskVM.WorkflowState);
                templateInfo.SetValue("#AssignedUserName#", objPlanTaskVM.AssignedUserName);

                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                emailNotificationInfo.TemplateInfo = templateInfo;
                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                emailNotificationInfo.ToAddresses = new List<string> { UserEmail };
                //emailNotificationInfo.CCAddresses = new List<string> { "cc Address1", "cc Address2", "cc Address3" };
                //emailNotificationInfo.Attachments = new List<string> { "attachement path", "attachement path1", "attachement path2" };
                emailNotificationInfo.Source = "Changes In Plan And TaskAssignment Email";
                emailNotificationInfo.SourceDescription = "Sending Changes In Plan And Task Assignment Email to assigned user";

                _emailService.SendEmail(emailNotificationInfo);
                isEmailSend = true;
            }
            //_emailService.Execute();
            return isEmailSend;
        }

        public bool SendEmailForDueDateOverForTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM)
        {
            bool isEmailSend = false;

            string userName = objPlanTaskVM.AssignedUserName;
            string UserEmail = string.Empty;
            string ManagerEmail = string.Empty;
            GetEmailIdForUserAndTeamManager(userName, ref UserEmail, ref ManagerEmail);

            if (!string.IsNullOrEmpty(UserEmail) && !string.IsNullOrEmpty(ManagerEmail))
            {
                var _emailService = new EmailNotificationService(this._emailUnitOfWork);
                EmailTemplateInfo templateInfo = _emailService.GetEmailTemplateInfo<EmailTemplateInfo>(EmailTemplateTypes.ChangesInPlanAndTaskAssignment);
                templateInfo.SetValue("#TaskDescription#", objPlanTaskVM.TaskDescription);
                templateInfo.SetValue("#Plan#", objPlanTaskVM.Plan);
                templateInfo.SetValue("#FolderName#", objPlanTaskVM.FolderName);
                templateInfo.SetValue("#DueDate#", FormatDateField(objPlanTaskVM.DueDate));
                templateInfo.SetValue("#EffectiveDate#", FormatDateField(objPlanTaskVM.EffectiveDate));
                templateInfo.SetValue("#WorkflowState#", objPlanTaskVM.WorkflowState);
                templateInfo.SetValue("#AssignedUserName#", objPlanTaskVM.AssignedUserName);

                EmailNotificationInfo emailNotificationInfo = new EmailNotificationInfo();
                emailNotificationInfo.TemplateInfo = templateInfo;
                emailNotificationInfo.ToBeSendDateTime = DateTime.Now;
                if (UserEmail.Equals(ManagerEmail))
                {
                    emailNotificationInfo.ToAddresses = new List<string> { UserEmail };
                }
                else
                {
                    emailNotificationInfo.ToAddresses = new List<string> { UserEmail, ManagerEmail };
                }
                //emailNotificationInfo.CCAddresses = new List<string> { "cc Address1", "cc Address2", "cc Address3" };
                //emailNotificationInfo.Attachments = new List<string> { "attachement path", "attachement path1", "attachement path2" };
                emailNotificationInfo.Source = "Due Date Is Over For Task Assignment Email";
                emailNotificationInfo.SourceDescription = "Sending Due Date Is Over For Task Assignment Email to assigned user and team manager";

                _emailService.SendEmail(emailNotificationInfo);
                isEmailSend = true;
            }
            //_emailService.Execute();
            return isEmailSend;
        }

        public void ExecuteNotifyTaskDueDateOverPushNotification(string loggedInUserName)
        {
            // var lateTask = this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(a => (a.Status == WatchTaskStatus.Late.ToString() && a.AssignedUserName == loggedInUserName)).OrderBy(m => m.Order).ToList();

            var lateTask = (from c in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query()
                            .Get().Where(a => (a.Status == WatchTaskStatus.Late.ToString() && a.AssignedUserName == loggedInUserName))
                            join taskLst in this._unitOfWork.RepositoryAsync<TaskList>().Get()
                               on c.TaskID equals taskLst.TaskID
                            join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                               on c.FolderVersionID equals fldrVersion.FolderVersionID
                            join folderV in this._unitOfWork.RepositoryAsync<Folder>().Get()
                               on fldrVersion.FolderID equals folderV.FolderID
                            select new DPFPlanTaskUserMappingViewModel
                            {
                                ID = c.ID,
                                //FormInstanceId = c.FormInstanceId,
                                AddedBy = c.AddedBy,
                                TaskDescription = taskLst.TaskDescription,
                                //Plan = frmInstance.Name,
                                AssignedUserName = c.AssignedUserName,
                                FolderName = folderV.Name,
                            }).OrderBy(m => m.AddedBy).ToList();

            SendLatePushNotification(lateTask, loggedInUserName);
            /*    if (lateTask != null)
                {
                    foreach (var late in lateTask)
                    {
                        _notificationService.SendNotification(
                           new NotificationInfo
                           {
                               SentTo = late.AssignedUserName,
                               MessageKey = MessageKey.TASK_LATE,
                               ParamterValues = SetEmailNotificationParamters(late),
                               loggedInUserName = loggedInUserName,
                           });
                    }
                }*/
        }

        private void SendLatePushNotification(List<DPFPlanTaskUserMappingViewModel> lateTask, string loggedInUserName)
        {

            if (lateTask != null)
            {
                foreach (var late in lateTask)
                {
                    SendLatePushNotification(late, loggedInUserName);
                }
            }
        }
        private void SendLatePushNotification(DPFPlanTaskUserMappingViewModel late, string loggedInUserName)
        {


            _notificationService.SendNotification(
                new NotificationInfo
                {
                    SentTo = late.AssignedUserName,
                    MessageKey = MessageKey.TASK_LATE,
                    ParamterValues = SetEmailNotificationParamters(late),
                    loggedInUserName = loggedInUserName,
                });

        }


        public bool ExecuteNotifyTaskDueDateOverEmail()
        {
            bool isEmailSend = false;

            var currentDateOnly = DateTime.Now.Date;

            var coll = this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get().Where(a => (a.Status != WatchTaskStatus.Completed.ToString() && a.Status != GlobalVariables.CompletedFail && a.Status != GlobalVariables.CompletedPass) && (a.DueDate < currentDateOnly)).ToList();
            if (coll != null)
            {
                foreach (var planTaskObj in coll)
                {
                    if (planTaskObj is DPFPlanTaskUserMapping)
                    {
                        DPFPlanTaskUserMapping objPlanTask = planTaskObj as DPFPlanTaskUserMapping;
                        if (objPlanTask != null)
                        {
                            if (IsFolderversionInProgress(objPlanTask.FolderVersionID))
                            {
                                List<DPFPlanTaskUserMappingViewModel> objPlanList = GetPlanTaskUserMappingList(objPlanTask.ID);
                                if (objPlanList != null && objPlanList.Count > 0)
                                {
                                    //isEmailSend = SendEmailForDueDateOverForTaskAssignment(objPlanList[0]);
                                    UpdatePlanTaskUserMappingWithLateStatus(objPlanTask);
                                    isEmailSend = true;
                                }
                            }
                        }
                    }
                }
            }
            return isEmailSend;
        }

        public List<DPFPlanTaskUserMappingViewModel>
            GetPlanTaskUserMappingList(int PlanTaskUserMappingId)
        {
            List<DPFPlanTaskUserMappingViewModel> planTaskUserMapList = null;
            try
            {

                planTaskUserMapList = (from c in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query()
                             .Get().Where(c => c.ID == PlanTaskUserMappingId)
                                           //join frmInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                           //on c.FormInstanceId equals frmInstance.FormInstanceID
                                       join dpfWFStateTaskMap in this._unitOfWork.RepositoryAsync<WorkflowTaskMap>().Get().Where(a => a.IsActive == true)
                                          on c.WFStateID equals dpfWFStateTaskMap.WFStateID
                                       join taskLst in this._unitOfWork.RepositoryAsync<TaskList>().Get()
                                          on c.TaskID equals taskLst.TaskID
                                       join fldrVersion in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                          on c.FolderVersionID equals fldrVersion.FolderVersionID
                                       join folderV in this._unitOfWork.RepositoryAsync<Folder>().Get()
                                          on fldrVersion.FolderID equals folderV.FolderID
                                       join accFolderMap in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get().Where(c => c.Account.IsActive == true)
                                          on fldrVersion.FolderID equals accFolderMap.FolderID into tmp
                                       from accFolderMap in tmp.DefaultIfEmpty()
                                       select new DPFPlanTaskUserMappingViewModel
                                       {
                                           WFStateID = c.WFStateID,
                                           TaskID = c.TaskID,
                                           AssignedDate = c.AssignedDate,
                                           AssignedUserName = c.AssignedUserName,
                                           ManagerUserName = c.ManagerUserName,
                                           StartDate = c.StartDate,
                                           DueDate = c.DueDate,
                                           Status = c.Status,
                                           CompletedDate = c.CompletedDate,
                                           AddedDate = c.AddedDate,
                                           AddedBy = c.AddedBy,
                                           UpdatedDate = c.UpdatedDate,
                                           UpdatedBy = c.UpdatedBy,
                                           TaskDescription = taskLst.TaskDescription,
                                           FolderName = folderV.Name,
                                           EffectiveDate = fldrVersion.EffectiveDate,
                                           EffectiveDateString = fldrVersion.EffectiveDate.ToString(),
                                           WorkflowState = dpfWFStateTaskMap.WorkFlowStateMaster.WFStateName,
                                           FolderVersionNumber = fldrVersion.FolderVersionNumber,
                                           FolderVersionID = fldrVersion.FolderVersionID,
                                           FolderID = folderV.FolderID,
                                           PlanTaskUserMappingDetails = c.PlanTaskUserMappingDetails,
                                           Order = c.Order,
                                           EstimatedTime = c.EstimatedTime,
                                           ActualTime = c.ActualTime,
                                           AccountName = accFolderMap == null ? "NA" : accFolderMap.Account.AccountName + "|" + accFolderMap.Account.AccountID,
                                           Duration = c.Duration == null ? 0 : c.Duration.Value
                                       }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return planTaskUserMapList;
        }

        public void GetEmailIdForUserAndTeamManager(string userName, ref string selectedUserEmail, ref string teamMangerEmail)
        {
            User selectedUser = this._unitOfWork.RepositoryAsync<User>().Query()
                .Filter(c => c.UserName == userName).Get().FirstOrDefault();
            if (selectedUser != null)
            {
                selectedUserEmail = selectedUser.Email;
                ApplicableTeamUserMap selectedUserMap = this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Query().Filter(a => (a.UserID == selectedUser.UserID && a.IsDeleted == false)).Get().FirstOrDefault();
                if (selectedUserMap != null)
                {
                    ApplicableTeamUserMap teamManager = this._unitOfWork.RepositoryAsync<ApplicableTeamUserMap>().Query().Filter(a => (a.ApplicableTeamID == selectedUserMap.ApplicableTeamID && a.IsDeleted == false && a.IsTeamManager == true)).Get().FirstOrDefault();
                    if (teamManager != null)
                    {

                        User managerUser = this._unitOfWork.RepositoryAsync<User>().Query()
                                     .Filter(c => c.UserID == teamManager.UserID).Get().FirstOrDefault();
                        if (managerUser != null)
                        {
                            teamMangerEmail = managerUser.Email;
                        }
                    }
                }
            }
        }

        public string FormatDateField(DateTime stValue)
        {
            string strFormatDate = string.Empty;
            strFormatDate = stValue.Month + "/" + stValue.Day + "/" + stValue.Year;
            return strFormatDate;
        }


        public void ResetStartDateDueDateOnFolderVersionWorkflowStateChange(int FolderVersionID, int WorkFlowVersionStateID)
        {
            ///if you face error, go to setting page : it will automatically add below values.
            //if (_settingManager.GetSettingValue<bool>(SettingConstant.ACCELERATE_START_DATE_FOR_TASK) == false)
            //    return;
            var acceslerationEnable = _settingManager.GetSettingValue(SettingConstant.ACCELERATE_START_DATE_FOR_TASK);
            if (acceslerationEnable == "false" || acceslerationEnable == "")
            {
                return;
            }
            // Get workflow id from version mapping
            int wfStateID = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query()
                 .Filter(c => c.WorkFlowVersionStateID == WorkFlowVersionStateID).Get().Select(c => c.WFStateID).FirstOrDefault();

            // get task details from taskmapping table with having above forminstances
            var taskdata = (from tsk in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query().Filter(a => a.WFStateID == wfStateID && a.FolderVersionID == FolderVersionID).Get()
                            select tsk);
            //calculate duration and update start date and end date
            foreach (var task in taskdata)
            {
                int duration = 1;
                task.StartDate = DateTime.Now.AddDays(duration);
                if (task.Duration != null)
                {
                    if (task.Duration.Value >= 0)
                    {
                        duration = task.Duration.Value;
                    }
                }

                task.DueDate = task.StartDate.AddDays(duration);
                this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(task);
            }
            if (taskdata.Count() > 0)
                this._unitOfWork.Save();
        }

        public bool ValidateTaskCompletedForWorkFlow(int FolderVersionID, int WorkFlowVersionStateID)
        {
            bool isValidated = false;

            // Get workflow id from version mapping
            int wfStateID = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Query()
                 .Filter(c => c.WorkFlowVersionStateID == WorkFlowVersionStateID).Get().Select(c => c.WFStateID).FirstOrDefault();

            // get task details from taskmapping table with having above forminstances
            var taskdata = (from tsk in this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Query().Filter(a => a.WFStateID == wfStateID && a.FolderVersionID == FolderVersionID).Get()

                                //join frmInstance in this._unitOfWork.RepositoryAsync<FormInstance>()
                                //  .Query()
                                //  .Include(c => c.FormDesign)
                                //  .Filter(c => c.FolderVersionID == FolderVersionID && c.IsActive == true
                                //      && c.FormDesignID == 2359)
                                //  .Get()
                                //on tsk.FormInstanceId equals frmInstance.FormInstanceID
                            select tsk);

            if (taskdata != null)
            {
                int allTaskCount = taskdata.Count();
                // Check if tasks has count , if yes then check for completed else send validate as true because there is not task to validate and continue for rest validations
                if (allTaskCount > 0)
                {
                    var completedTask = taskdata.Where(a => a.Status == WatchTaskStatus.Completed.ToString() || a.Status == GlobalVariables.CompletedPass).ToList();
                    if (allTaskCount == completedTask.Count)
                    {
                        isValidated = true;
                    }
                }
                else
                {
                    isValidated = true;
                }
            }

            return isValidated;
        }

        public bool UpdatePlanTaskUserMappingWithLateStatus(DPFPlanTaskUserMapping objPlanTaskUserMapping)
        {
            bool isLateStatusUpdated = false;
            try
            {
                if (objPlanTaskUserMapping != null)
                {
                    DPFPlanTaskUserMapping objDPFPlanTaskUserMapping = _unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Get()
                                                            .Where(i => i.ID == objPlanTaskUserMapping.ID)
                                                            .Select(i => i).FirstOrDefault();
                    if (objDPFPlanTaskUserMapping != null)
                    {
                        if (!objDPFPlanTaskUserMapping.LateStatusDone)
                        {
                            objDPFPlanTaskUserMapping.Status = WatchTaskStatus.Late.ToString();
                            objDPFPlanTaskUserMapping.LateStatusDone = true;
                            this._unitOfWork.RepositoryAsync<DPFPlanTaskUserMapping>().Update(objDPFPlanTaskUserMapping);
                            this._unitOfWork.Save();
                            isLateStatusUpdated = true;
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
            return isLateStatusUpdated;
        }

        public bool IsFolderversionInProgress(int folderversionId)
        {
            bool isFormInstInProgress = false;
            FolderVersion folderVersionDefValue = this._unitOfWork.RepositoryAsync<FolderVersion>().Query()
                    .Filter(c => c.FolderVersionID == folderversionId && c.FolderVersionStateID == 1).Get().FirstOrDefault();
            if (folderVersionDefValue != null)
            {
                isFormInstInProgress = true;
            }
            return isFormInstInProgress;
        }
        public IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int folderVersionId)
        {
            IList<FormDesignVersionRowModel> formDesignVersionList = null;
            try
            {
                var formDesignVersionIds = (from c in this._unitOfWork.RepositoryAsync<FormInstance>().Query().Get().Where(row => row.FolderVersionID == folderVersionId)
                                            select c.FormDesignVersionID).ToList().Distinct();

                formDesignVersionList = (from c in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Query()
                             .Get().Where(c => formDesignVersionIds.Contains(c.FormDesignVersionID))
                                         join frmDesign in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(d => d.IsActive == true && d.IsMasterList == false)
                                            on c.FormDesignID equals frmDesign.FormID
                                         select new FormDesignVersionRowModel
                                         {
                                             FormDesignVersionId = c.FormDesignVersionID,
                                             FormDesignId = c.FormDesignID,
                                             Version = c.VersionNumber,
                                             StatusId = c.StatusID,
                                             StatusText = c.Status.Status1,
                                             EffectiveDate = c.EffectiveDate,
                                             FormDesignName = frmDesign.FormName
                                         }).ToList();

                if (formDesignVersionList.Count() == 0)
                    formDesignVersionList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersionList;
        }
        public List<FormInstanceViewModel> GetFormInstanceListForFolderVersion(int tenantId, int folderVersionId, int folderId, int formDesignId, int formDesignTypeId = 0)
        {
            List<FormInstanceViewModel> formInstanceList = null;

            try
            {
                if (formDesignId != 0)
                {
                    var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Include(c => c.FormDesign)
                                                  .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && (formDesignTypeId == 0 || c.FormDesign.DocumentDesignTypeID == formDesignTypeId) && c.IsActive == true
                                                  && c.FormInstanceID == c.AnchorDocumentID)
                                                  .Get()
                                         select new FormInstanceViewModel
                                         {
                                             FormInstanceID = c.FormInstanceID,
                                             FolderVersionID = c.FolderVersionID,
                                             FormDesignID = c.FormDesignID,
                                             FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                             TenantID = c.TenantID,
                                             FormDesignVersionID = c.FormDesignVersionID,
                                         }).ToList();


                    if (formInstances != null)
                    {
                        formInstanceList = formInstances.ToList();
                    }
                }
                else
                {
                    var formInstances = (from c in this._unitOfWork.RepositoryAsync<FormInstance>()
                                                  .Query()
                                                  .Include(c => c.FormDesign)
                                                  .Filter(c => c.TenantID == tenantId && c.FolderVersionID == folderVersionId && (formDesignTypeId == 0 || c.FormDesign.DocumentDesignTypeID == formDesignTypeId) && c.IsActive == true)
                                                  .Get()
                                         select new FormInstanceViewModel
                                         {
                                             FormInstanceID = c.FormInstanceID,
                                             FolderVersionID = c.FolderVersionID,
                                             FormDesignID = c.FormDesignID,
                                             FormDesignName = String.IsNullOrEmpty(c.Name) ? c.FormDesign.FormName : c.Name,
                                             TenantID = c.TenantID,
                                             FormDesignVersionID = c.FormDesignVersionID,
                                         }).ToList();


                    if (formInstances != null)
                    {
                        formInstanceList = formInstances.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return formInstanceList;
        }

        public List<SectionDesign> GetSectionsList(int tenantId, string formDesignVersionId, IFormDesignService _formDesignService)
        {
            FormDesignVersionDetail detail = null;
            List<SectionDesign> sectionsList = new List<SectionDesign>();
            string formDesign = String.Empty;
            try
            {
                FormDesignDataCacheHandler formDesignCacheHandler = new FormDesignDataCacheHandler();
                if (!string.IsNullOrEmpty(formDesignVersionId) && formDesignVersionId.IndexOf(',') != -1)
                {
                    string[] formDesignVersionIds = formDesignVersionId.Split(',');
                    foreach (string Id in formDesignVersionIds)
                    {

                        var sections = formDesignCacheHandler.GetSectionNames(tenantId, Id, _formDesignService);
                        sectionsList.AddRange(sections);

                        /*   formDesign = formDesignCacheHandler.Get(tenantId, Convert.ToInt32(Id), _formDesignService);
                           detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
                           foreach (SectionDesign design in detail.Sections)
                           {
                               design.Label = detail.FormName + " - " + design.Label;
                           }
                           if (sectionsList.Count() <= 0)
                               sectionsList = detail.Sections;
                           else
                           {
                               sectionsList.AddRange(detail.Sections);
                           }*/
                    }
                }
                else
                {
                    sectionsList = formDesignCacheHandler.GetSectionNames(tenantId, formDesignVersionId, _formDesignService);

                    /*formDesign = formDesignCacheHandler.Get(tenantId, Convert.ToInt32(formDesignVersionId), _formDesignService);
                    detail = JsonConvert.DeserializeObject<FormDesignVersionDetail>(formDesign);
                    foreach (SectionDesign design in detail.Sections)
                    {
                        design.Label = detail.FormName + " - " + design.Label;
                    }
                    sectionsList = detail.Sections;*/

                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return sectionsList;
        }

        public bool SaveTaskComments(CommentViewModel taskCommentsModel)
        {
            bool isCommentAdded = false;
            TaskComments taskComments = new TaskComments();
            try
            {
                if (taskCommentsModel != null)
                {
                    if (!String.IsNullOrEmpty(taskCommentsModel.Attachment))
                    {
                        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"c:\");

                        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + taskCommentsModel.Attachment + "*.*");
                        string sourcePath = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/TempAttachments");
                        sourcePath = sourcePath + "\\" + taskCommentsModel.Attachment;
                        string targetPath = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/PermAttachments/");
                        string checkFileExsist = targetPath + taskCommentsModel.Attachment;
                        if (!Directory.Exists(targetPath))
                        {
                            Directory.CreateDirectory(targetPath);
                        }

                        try
                        {
                            if (!File.Exists(checkFileExsist))
                            {
                                System.IO.File.Copy(sourcePath, targetPath + Path.GetFileName(sourcePath), true);
                            }
                        }
                        catch (Exception ex)
                        {
                            sourcePath = sourcePath + "\\" + taskCommentsModel.Attachment;
                            System.IO.File.Copy(sourcePath, targetPath + Path.GetFileName(sourcePath), true);
                        }
                    }
                    taskComments.AddedBy = taskCommentsModel.AddedBy;
                    taskComments.AddedDate = taskCommentsModel.AddedDate;
                    taskComments.Comments = taskCommentsModel.Comment;
                    taskComments.TaskID = taskCommentsModel.TaskID;
                    taskComments.Attachment = taskCommentsModel.Attachment;
                    taskComments.filename = taskCommentsModel.filename;
                    taskComments.FolderVersionID = taskCommentsModel.FolderVersionID;
                    taskComments.PlanTaskUserMappingState = taskCommentsModel.Status;
                    this._unitOfWork.RepositoryAsync<TaskComments>().Insert(taskComments);
                    this._unitOfWork.Save();
                    isCommentAdded = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isCommentAdded;
        }

        public int GetFormDesignVersionByFormInstanceId(int formInstanceId)
        {
            var formDesignVersionId = 0;
            try
            {
                formDesignVersionId = this._unitOfWork.RepositoryAsync<FormInstance>().Query()
                    .Filter(c => c.FormInstanceID == formInstanceId).Get().FirstOrDefault().FormDesignVersionID;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formDesignVersionId;
        }
    }
}
