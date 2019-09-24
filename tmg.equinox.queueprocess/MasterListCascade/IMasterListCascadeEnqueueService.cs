﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting;

namespace tmg.equinox.queueprocess.masterlistcascade
{
    public interface IMasterListCascadeEnqueueService
    {
        void CreateJob(MasterListCascadeQueueInfo queueInfo);
        void CreateJobWithDelay(MasterListCascadeQueueInfo queueInfo);
    }
}
