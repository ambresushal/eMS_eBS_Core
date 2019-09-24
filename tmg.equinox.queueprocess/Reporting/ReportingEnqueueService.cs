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

namespace tmg.equinox.queueprocess.reporting
{
    public class ReportingEnqueueService : IReportEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public ReportingEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;

        }

        [Queue("report")]
        public void CreateJob(ReportQueueInfo reportQueueInfo)
        {
            _backgroundJobManager.EnqueueAsync<ReportBackgroundJob, ReportQueueInfo>(reportQueueInfo);
        }


    }
}
