using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWFVersionStatesApprovalTypeService
    {
        List<WFVersionStatesApprovalTypeViewModel> GetWFVersionStatesApprovalTypeList(int workFlowVersionStateID);
        ServiceResult AddWFVersionStatesApprovalType(int workFlowStateApprovalTypeID, int workFlowVersionStateID, string addedBy);
        ServiceResult UpdateWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID, int workFlowStateApprovalTypeID, string updatedBy);
        ServiceResult DeleteWFVersionStatesApprovalType(int wFVersionStatesApprovalTypeID);

        //Approval Type Actions
        List<WFStatesApprovalTypeActionViewModel> GetWFStatesApprovalTypeActionList(int wFVersionStatesApprovalTypeID);
        ServiceResult AddWFStatesApprovalTypeAction(int wFVersionStatesApprovalTypeID, int actionID, string actionResponse, string addedBy);
        ServiceResult UpdateWFStatesApprovalTypeAction(int wFStatesApprovalTypeActionID, string actionResponse, string updatedBy);
        ServiceResult DeleteWFStatesApprovalTypeAction(int wFStatesApprovalTypeActionID);
    }
}
