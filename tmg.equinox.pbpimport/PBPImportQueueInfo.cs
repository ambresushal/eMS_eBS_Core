using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.pbpimport
{
   public class PBPImportQueueInfo : BaseJobInfo
    {
        public PBPImportQueueInfo()
        {

        }
        public PBPImportQueueInfo(int _QueueId, string _UserId)
        {
            this.QueueId = _QueueId;
            this.UserId = _UserId;
        }
    }
}
