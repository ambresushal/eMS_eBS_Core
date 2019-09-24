﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.reporting
{
    public class ReportJobInfo : BaseJobInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public ReportJobInfo() { }

        public ReportJobInfo(int _QueueId, string _UserId)
        {
            this.QueueId = _QueueId;
            this.UserId = _UserId;
        }

    }
}
