using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.pbpimport.Base;

namespace tmg.equinox.pbp.Import.processing
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
