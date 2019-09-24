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
//using tmg.equinox.applicationservices.interfaces;

namespace tmg.equinox.pbp.Import.processing
{
  public  class PBPImportEnqueueService
    {
        private readonly IBackgroundJobManager _backgroundJobManager;

        public PBPImportEnqueueService(IBackgroundJobManager backgroundJobManager)
        {
            _backgroundJobManager = backgroundJobManager;
        }

        [Queue("high")]
        public void CreateJob(PBPImportQueueInfo pBPImportQueueInfo)
        {
            pBPImportQueueInfo.AssemblyName = "tmg.equinox.pbpimport";
            pBPImportQueueInfo.ClassName = "ImportProcess";

            _backgroundJobManager.EnqueueAsync<PBPImportBackgroundJob, PBPImportQueueInfo>(pBPImportQueueInfo);

        }
    }
}
