using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowVersionStatesAccessService
    {
        List<WorkFlowVersionStatesAccessViewModel> GetWorkFlowVersionStatesAccessList(int workFlowVersionStateID);
        ServiceResult AddWorkFlowVersionStatesAccess(int workFlowVersionStateID, int roleID, string addedBy);
        ServiceResult UpdateWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID, int roleID, string updatedBy);
        ServiceResult DeleteWorkFlowVersionStatesAccess(int workFlowVersionStatesAccessID);
    }
}
