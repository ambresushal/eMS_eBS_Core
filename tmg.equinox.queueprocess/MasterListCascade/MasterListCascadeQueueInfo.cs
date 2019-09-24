using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.queueprocess.masterlistcascade
{
    public class MasterListCascadeQueueInfo : BaseJobInfo
    {

        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }

        public int MasterListCascadeBatchID { get; set; }

        public int FolderVersionID { get; set; }

        public int UserID { get; set; }

        public string UserName{ get; set; }

        public MasterListCascadeQueueInfo() { }

        public MasterListCascadeQueueInfo(int _QueueId, string _UserId, int formDesignID, int formDesignVersionID,int folderVersionID,int userId, string userName)
        {
            this.QueueId = _QueueId;
            this.UserID = userId;
            this.FormDesignID = formDesignID;
            this.FormDesignVersionID = formDesignVersionID;
            this.FolderVersionID = folderVersionID;
        }
    }
}
