using System.Linq;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ApprovalStatusTypeRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods

        public static WorkFlowStateApprovalTypeMaster GetApprovedApprovalStatus(this IRepositoryAsync<WorkFlowStateApprovalTypeMaster> approvalStatusTypeRepository,
            int tenantID)
        {

            return approvalStatusTypeRepository
                    .Query()
                    .Filter(c => c.TenantID == tenantID && c.WorkFlowStateApprovalTypeName == GlobalVariables.APPROVED)
                    .Get()
                    .FirstOrDefault();

        }

        public static WorkFlowStateApprovalTypeMaster GetNotApprovedApprovalStatus(this IRepositoryAsync<WorkFlowStateApprovalTypeMaster> approvalStatusTypeRepository,
            int tenantID)
        {

            return approvalStatusTypeRepository
                    .Query()
                    .Filter(c => c.TenantID == tenantID && c.WorkFlowStateApprovalTypeName == GlobalVariables.NOTAPPROVED)
                    .Get()
                    .FirstOrDefault();

        }

        public static WorkFlowStateApprovalTypeMaster GetNotApplicableApprovalStatus(this IRepositoryAsync<WorkFlowStateApprovalTypeMaster> 
                    approvalStatusTypeRepository, int tenantID)
        {

            return approvalStatusTypeRepository
                    .Query()
                    .Filter(c => c.TenantID == tenantID && c.WorkFlowStateApprovalTypeName == GlobalVariables.NOTAPPLICABLE)
                    .Get()
                    .FirstOrDefault();

        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
