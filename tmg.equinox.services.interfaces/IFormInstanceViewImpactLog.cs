using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormInstanceViewImpactLogService
    {
        ServiceResult SaveFormInstanceImpactlogData(int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<SourceElement> viewImpactLog);
        List<SourceElement> GetFormInstanceImpactLogData(int formInstanceID);
        List<ActivityLogModel> GetFormInstanceActivityLogData(int formInstanceID, string impactedField);
        List<DropDownElementItem> GetDDLDropDownItems(int formDesignID);
    }
}
