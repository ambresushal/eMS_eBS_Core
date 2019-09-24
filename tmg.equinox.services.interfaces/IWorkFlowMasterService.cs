using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowMasterService
    {
        List<WorkFlowStateMasterViewModel> GetWorkFlowStateMasterList(int tenantID);
        ServiceResult AddWorkFlowStateMaster(int tenantID, string wFStateName, string addedBy);
        ServiceResult UpdateWorkFlowStateMaster(int wFStateID, string wFStateName, string updatedBy);
        ServiceResult DeleteWorkFlowStateMaster(int wFStateID, string updatedBy);

        List<WorkFlowStateApprovalTypeMasterViewModel> GetWorkFlowStateApprovalTypeMasterList(int tenantID);
        ServiceResult AddWorkFlowStateApprovalTypeMaster(int tenantID, string workFlowStateApprovalTypeName, string addedBy);
        ServiceResult UpdateWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID, string workFlowStateApprovalTypeName, string updatedBy);
        ServiceResult DeleteWorkFlowStateApprovalTypeMaster(int workFlowStateApprovalTypeID, string updatedBy);
        List<WorkFlowStateMasterViewModel> GetWorkFlowStateListGreaterThanSelected(int tenantID, int wfStateId, int folderVersionId = 0);
    }
}
