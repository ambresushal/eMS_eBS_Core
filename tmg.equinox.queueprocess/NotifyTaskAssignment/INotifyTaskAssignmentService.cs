﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.reportingprocess.NotifyTaskAssignment
{
    public interface INotifyTaskAssignmentService
    {
        void CreateJob(NotifyTaskAssignmentInfo notifyTaskAssignmentInfo);
    }
}
