using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Scheduler;
using tmg.equinox.integration.infrastructure.Util;
using tmg.equinox.applicationservices.viewmodels;
using Microsoft.Win32.TaskScheduler;
using tmg.equinox.integration.translator.dao.Models;
using tmg.equinox.integration.infrastructure.exceptionhandling;
using System.Security;

namespace tmg.equinox.integration.domain.services.Impl
{
    public class SchedulerService : ISchedulerService
    {
        private IUnitOfWork _unitOfWork { get; set; }

        public SchedulerService(IUnitOfWork unitOfWork)
        {
            //_unitOfWork = new UnitOfWork();
            _unitOfWork = unitOfWork;
        }

        public IList<Models.Job> GetScheduledJobs()
        {

            ISchedulerTask taskScheduler = new SchedulerTask();
            JobModel jobModel = new JobModel();
            IList<Job> schedulerModelList = new List<Job>();
            try
            {
                var schedulerList = taskScheduler.GetAllTask().Where(x => x.Name.Contains("~TR") || x.Name.Contains("~TM"));

                //IList<Job> schedulerModelList = new List<Job>();
                Job schedulerModel;
                //int count = 1;
                foreach (var task in schedulerList)
                {
                    schedulerModel = new Job();
                    schedulerModel.Id = task.Name;
                    schedulerModel.Name = task.Name.Split('~')[0];
                    schedulerModel.State = task.State.ToString();
                    schedulerModel.StartDate = task.Definition.Triggers.Count != 0 ? task.Definition.Triggers.First().StartBoundary : DateTime.Now;
                    schedulerModel.StartTime = schedulerModel.StartDate.ToShortTimeString();
                    //schedulerModel.StartTime = task.Definition.Triggers.Count != 0 ? task.Definition.Triggers.First().StartBoundary.ToShortDateString() : "";
                    schedulerModel.EndDate = task.Definition.Triggers.Count != 0 ? task.Definition.Triggers.First().EndBoundary : DateTime.MaxValue;
                    schedulerModel.JobType = task.Name.Split('~')[1];
                    schedulerModel.Parameter = ((Microsoft.Win32.TaskScheduler.ExecAction)(task.Definition.Actions[0])).Arguments;
                    schedulerModelList.Add(schedulerModel);
                }
                //return schedulerModelList;
                jobModel.schedulerModels = schedulerModelList.ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return jobModel.schedulerModels;
        }

        /// <summary>
        /// Get the task details
        /// </summary>
        /// <returns></returns>
        public Job GetTaskDetails(string id)
        {
            ISchedulerTask taskScheduler = new SchedulerTask();
            Job details = new Job();
            try
            {
                var taskDetails = taskScheduler.GetTask(id);
                if (taskDetails != null)
                {
                    //Job details = new Job();
                    details.Id = taskDetails.Name;
                    details.Name = taskDetails.Name.Split('~')[0];
                    details.Desc = taskDetails.Definition.RegistrationInfo.Description;
                    if (taskDetails.Definition.Triggers.Count != 0)
                    {
                        Trigger tr = taskDetails.Definition.Triggers[0];

                        details.StartDate = tr.StartBoundary;
                        details.EndDate = tr.EndBoundary;
                        details.StartTime = tr.StartBoundary.ToShortTimeString();
                        details.TriggerType = (int)tr.TriggerType;
                        if (details.TriggerType == 2)
                        {
                            DailyTrigger dtr = (DailyTrigger)tr;
                            details.Interval = dtr.DaysInterval;
                        }
                        else if (details.TriggerType == 3)
                        {
                            WeeklyTrigger wtr = (WeeklyTrigger)tr;
                            details.Interval = wtr.WeeksInterval;
                            details.WeekDaysList = wtr.DaysOfWeek.ToString().ToLower();
                        }

                        ExecAction taskAction = (Microsoft.Win32.TaskScheduler.ExecAction)taskDetails.Definition.Actions.Where(x => x.Id == details.Id).FirstOrDefault();
                        if (taskAction != null)
                        {
                            details.Parameter = taskAction.Arguments;
                        }
                    }

                    details.JobType = taskDetails.Name.Split('~')[1];
                    details.TriggerEnable = taskDetails.Enabled;
                    //return details;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return details;
        }


        public IList<VersionRowModel> GetPluginVersions()
        {
            List<VersionRowModel> pluginVersionList = null;   
            try
            {
                pluginVersionList = (from c in this._unitOfWork.Repository<PluginVersion>()
                                                                           .Query()
                                                                           .Get()
                                         select new VersionRowModel
                                       {
                                           Id = c.PluginVersionId,
                                           PluginId = (int)c.PluginId,
                                           Name = c.Plugin.Name,
                                           Version = c.Description
                                       }).ToList();

                //return pluginVersionList;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return pluginVersionList;
        }

        public string AddTask(Job schedulerJob)
        {
            string msg = string.Empty;
            try
            {
                string executableFileLocation = string.Empty;
                if (schedulerJob != null)
                {
                    DateTime jobStartDate = schedulerJob.StartDate;
                    DateTime jobEndDate = schedulerJob.EndDate != null ? schedulerJob.EndDate : DateTime.MaxValue;
                    ISchedulerTask taskScheduler = new SchedulerTask();

                    if (schedulerJob.JobType.Equals("TR"))
                    {
                        executableFileLocation = Util.GetKeyValue("TranslatorEXEPath");
                    }
                    else
                    {
                        executableFileLocation = Util.GetKeyValue("TransmitterEXEPath");
                    }

                    List<string> weekDaysList = schedulerJob.WeekDaysList != null ? schedulerJob.WeekDaysList.Split(',').ToList() : null;

                    SchedulerInfo schedulerInfo = new SchedulerInfo();
                    schedulerInfo.Action = 0;
                    schedulerInfo.TaskName = schedulerJob.Name;
                    schedulerInfo.Description = schedulerJob.Desc;
                    schedulerInfo.ExecutableFileName = executableFileLocation;
                    schedulerInfo.ScheduleTime = schedulerJob.TriggerType;
                    //To add specific time with date to the dafault time of DateTime
                    schedulerInfo.StartDate = schedulerJob.StartDate.Add(TimeSpan.Parse(schedulerJob.StartTime));
                    schedulerInfo.EndDate = schedulerJob.EndDate;
                    schedulerInfo.Interval = schedulerJob.Interval;
                    schedulerInfo.TriggerEnable = schedulerJob.TriggerEnable;
                    schedulerInfo.WeekDaysList = weekDaysList;
                    schedulerInfo.Parameter = schedulerJob.Parameter;
                    schedulerInfo.UserNameWithDomain = Util.GetKeyValue("SchedulerUserName");
                    schedulerInfo.UserPassword = Util.GetKeyValue("SchedulerUserPwd");

                    taskScheduler.DefineTask(schedulerInfo);
                    msg = "Task Created Successfully";
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return msg;
        }

        public string EditTask(Models.Job schedulerJob)
        {
            string executableLocation = string.Empty;
            string msg = string.Empty;
            try
            {

                if (schedulerJob != null)
                {
                    DateTime jobStartDate = schedulerJob.StartDate;
                    DateTime jobEndDate = schedulerJob.EndDate != null ? schedulerJob.EndDate : DateTime.MaxValue;
                    ISchedulerTask taskScheduler = new SchedulerTask();

                    if (schedulerJob.JobType.Equals("TR"))
                    {
                        executableLocation = Util.GetKeyValue("TranslatorEXEPath");
                    }
                    else
                    {
                        executableLocation = Util.GetKeyValue("TransmitterEXEPath");
                    }

                    SchedulerInfo schedulerInfo = new SchedulerInfo();
                    schedulerInfo.Action = 1;
                    schedulerInfo.TaskName = schedulerJob.Name;
                    schedulerInfo.Description = schedulerJob.Desc;
                    schedulerInfo.ExecutableFileName = executableLocation;
                    schedulerInfo.ScheduleTime = schedulerJob.TriggerType;
                    schedulerInfo.StartDate = schedulerJob.StartDate;
                    schedulerInfo.EndDate = schedulerJob.EndDate;
                    schedulerInfo.Interval = schedulerJob.Interval;
                    schedulerInfo.TriggerEnable = schedulerJob.TriggerEnable;
                    schedulerInfo.WeekDaysList = null;
                    schedulerInfo.Parameter = schedulerJob.Parameter;

                    taskScheduler.DefineTask(schedulerInfo);
                    msg = "Task Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return msg;
        }

        public string DeleteTask(string Id)
        {
            string msg = string.Empty;
            try
            {
                ISchedulerTask taskScheduler = new SchedulerTask();
                taskScheduler.DeleteTask(Id);
                msg = "Task Deleted Successfully";
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return msg;
        }


        public void Run(string Id)
        {
            ISchedulerTask taskScheduler = new SchedulerTask();
            try
            {
                var ss = convertToSecureString("password");
                taskScheduler.TaskRunning = true;
                taskScheduler.RunNow(Id, "", "", ss);

                //while (taskScheduler.TaskRunning)
                //{
                //    if (taskScheduler.TaskRunning)
                //    {
                //        throw new NotImplementedException();
                //    }
                //}

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
        }

        public SecureString convertToSecureString(string strPassword)
        {
            SecureString secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }
    }
}
