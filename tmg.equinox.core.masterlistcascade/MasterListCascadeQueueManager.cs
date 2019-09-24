using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.core.masterlistcascade
{
    public class MasterListCascadeQueueManager
    {

        IMasterListCascadeManager<BaseJobInfo> _mlManager;
        public MasterListCascadeQueueManager(IMasterListCascadeManager<BaseJobInfo> mlManager)
        {
            _mlManager = mlManager;

        }

        public bool Execute(BaseJobInfo jobInfo, int formDesignID, int formDesignVersionID,int folderVersionID,int masterListCascadeBatchID, int userID, string userName)
        {
            return _mlManager.Execute(jobInfo, formDesignID, formDesignVersionID,folderVersionID,masterListCascadeBatchID,userID,userName);
        }
    }
}
