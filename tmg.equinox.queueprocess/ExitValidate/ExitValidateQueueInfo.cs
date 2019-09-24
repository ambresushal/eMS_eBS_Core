using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.queueprocess.exitvalidate
{
    public class ExitValidateQueueInfo : BaseJobInfo
    {
        public ExitValidateQueueInfo() { }
        public ExitValidateQueueInfo(int _QueueId, string _UserId)
        {
            this.QueueId = _QueueId;
            this.UserId = _UserId;
        }
    }
}
