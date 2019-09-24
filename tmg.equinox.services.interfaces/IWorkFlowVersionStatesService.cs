using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowVersionStatesService
    {
        List<WorkFlowVersionStatesViewModel> GetWorkFlowVersionStatesList(int workFlowVersionID);
        ServiceResult AddWorkFlowVersionStates(int tenantID, int workFlowVersionID, int wFStateID, int sequence, string addedBy);
        ServiceResult UpdateWorkFlowVersionStates(int workFlowVersionStatesID, int wFStateID, int sequence, string updatedBy, int? wFStateGroupID);
        ServiceResult DeleteWorkFlowVersionStates(int workFlowVersionStatesID);
        int GetMaxWFStateGroupID();
    }
}
