using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Storage;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.backgroundjob;
using System.Reflection;

namespace tmg.equinox.hangfire.Jobfilters
{
    public class HangfireStateFilter : JobFilterAttribute,
    IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private static readonly ILog _logger = LogProvider.For<HangfireStateFilter>();
        CustomQueueMangement _customQueueMangement = new CustomQueueMangement();

        private const string methodNameUpdateQueue = "UpdateQueue";
        private const string methodNameUpdateFailQueue = "UpdateFailQueue";
        public HangfireStateFilter()
        {

        }





        //private static readonly ILog _logger = LogProvider.For<HangfireStateFilter>();
        public void OnStateElection(ElectStateContext context)
        {
            try
            {
                if (context.BackgroundJob.Job == null)
                    return;


                BaseJobInfo jobInof = context.BackgroundJob.Job.Args[0] as BaseJobInfo;
                jobInof.JobId = context.BackgroundJob.Id;
                jobInof.Status = context.CandidateState.Name;


                var failedState = context.CandidateState as FailedState;
                var methodName = methodNameUpdateQueue;
                if (failedState != null)
                {
                    jobInof.Status = failedState.Name;
                    jobInof.Error = failedState.Exception.Message;
                    methodName = methodNameUpdateFailQueue;
                    _logger.ErrorFormat("OnStateElection", context);
                }

                updateStatusOrFailure(methodName, jobInof);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Occur ", ex);
                throw ex;
            }

        }

        private void updateStatusOrFailure(string methodName, BaseJobInfo jobInof)
        {
            try
            {
                if (jobInof.Error != null)
                    _logger.Error(jobInof.Error);
                
                object instanceCustomQueue = _customQueueMangement.CreateInstanceCustomQueue(jobInof);
                if (instanceCustomQueue != null)
                {
                    _customQueueMangement.ExecuteCustomQueueMethod(instanceCustomQueue, jobInof, methodName);
                }
                _logger.DebugFormat("updateStatusOrFailure", jobInof);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Occur ", ex);
                throw ex;
            }
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            try
            {
                if (context.BackgroundJob.Job == null)
                    return;

                BaseJobInfo jobInof = context.BackgroundJob.Job.Args[0] as BaseJobInfo;
                jobInof.JobId = context.BackgroundJob.Id;
                jobInof.Status = context.NewState.Name;

                if (context.NewState.Name != "Failed")
                {
                    updateStatusOrFailure(methodNameUpdateQueue, jobInof);
                }
                _logger.DebugFormat("OnStateApplied", jobInof);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Occur ", ex);
                throw ex;
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {

        }
        public void OnCreating(CreatingContext context)
        {
            try
            {
                BaseJobInfo jobInof = context.Job.Args[0] as BaseJobInfo;
                jobInof.Status = context.InitialState.Name;

                updateStatusOrFailure(methodNameUpdateQueue, jobInof);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Occur ", ex);
                throw ex;
            }
        }

        public void OnCreated(CreatedContext context)
        {

        }
        public void OnPerforming(PerformingContext context)
        {

        }

        public void OnPerformed(PerformedContext context)
        {
            try
            {
                if (context.Exception != null)
                {
                    BaseJobInfo jobInof = context.BackgroundJob.Job.Args[0] as BaseJobInfo;
                    jobInof.JobId = context.BackgroundJob.Id;
                    jobInof.Status = "Failed";

                    var exceptionMsg = (context.Exception.InnerException == null ? context.Exception.Message : context.Exception.InnerException.Message);
                    jobInof.Error = exceptionMsg;
                    updateStatusOrFailure(methodNameUpdateFailQueue, jobInof);
                    _logger.ErrorException("OnPerformed", context.Exception);
                    _logger.ErrorFormat("OnPerformed", context);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error Occur ", ex);
                throw ex;
            }
        }

    }
}
