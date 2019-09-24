using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.pbpexport
{
   public class PBPExportQueueInfo : BaseJobInfo
    {

        public string UserName { get; set; }
        public string RunExportRulesInWindowsService { get; set; }
        public int pbpDatabase1Up { get; set; }

        public PBPExportQueueInfo()
        {

        }
        public PBPExportQueueInfo(int _QueueId, string _UserId)
        {
            this.QueueId = _QueueId;
            this.UserId = _UserId;
        }
    }
}
