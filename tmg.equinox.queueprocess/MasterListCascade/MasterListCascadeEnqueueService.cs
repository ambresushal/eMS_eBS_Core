using Hangfire;
using System;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.hangfire;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.repository;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.entities;
using System.Collections.Generic;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.reporting;

namespace tmg.equinox.queueprocess.masterlistcascade
{
    public class MasterListCascadeEnqueueService : IMasterListCascadeEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public MasterListCascadeEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;

        }

        [Queue("mlcascade")]
        public void CreateJob(MasterListCascadeQueueInfo queueInfo)
        {
            _backgroundJobManager.EnqueueAsync<MasterListCascadeBackgroundJob, MasterListCascadeQueueInfo>(queueInfo);
        }

        [Queue("mlcascade")]
        public void CreateJobWithDelay(MasterListCascadeQueueInfo queueInfo)
        {
            _backgroundJobManager.EnqueueAsync<MasterListCascadeBackgroundJob, MasterListCascadeQueueInfo>(queueInfo, TimeSpan.FromMinutes(5));
        }
    }
}
