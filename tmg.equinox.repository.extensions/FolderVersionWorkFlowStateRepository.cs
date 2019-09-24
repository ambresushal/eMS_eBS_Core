using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class FolderVersionWorkFlowStateRepository
    {
        public static bool IsWorkflowStateExist(this IRepositoryAsync<FolderVersionWorkFlowState> folderVersionWorkFlowStateRepository, int folderVersionId, int workflowStateId)
        {

            return folderVersionWorkFlowStateRepository
                .Query()
                .Filter(c => c.FolderVersionID == folderVersionId && c.WFStateID == workflowStateId && c.IsActive == true)
                .Get()
                .Any();
        }
        
        public static IList<FolderVersionWorkFlowState> GetAllApprovedAndNotApplicableStates(this IRepositoryAsync<FolderVersionWorkFlowState> folderVersionWorkFlowStateRepository, int tenantId, int folderVersionId)
        {
            var approvalID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.APPROVED);
            //var notapplicablelID = Convert.ToInt32(tmg.equinox.domain.entities.ApprovalStatus.NOTAPPLICABLE);
            return folderVersionWorkFlowStateRepository
                    .Query()
                    .Filter(c => (c.TenantID == tenantId && c.FolderVersionID == folderVersionId && c.IsActive == true) && (c.ApprovalStatusID == approvalID))
                    .Get()
                    .ToList();
        }

    }
}
