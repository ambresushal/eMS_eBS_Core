using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.core.masterlistcascade
{
    public interface IMasterListCascadeManager<T>
    {
        bool Execute(T queueItem,int formDesignID, int formDesignVersionID,int folderVersionID,int masterListCascadeBatchID, int userID, string userName);
    }
}
