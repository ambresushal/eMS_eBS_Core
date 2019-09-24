using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowCategoryMappingService
    {
        List<WorkFlowCategoryMappingViewModel> GetWorkFlowCategoryMappingList(int tenantId);
        ServiceResult AddWorkFlowCategoryMapping(int tenantID, int workFlowType, int accountType, int folderVersionCategoryID, string addedBy);
        ServiceResult UpdateWorkFlowCategoryMapping(int tenantID, int workFlowVersionID, int workFlowType, int accountType, int folderVersionCategoryID, string updatedBy);
        ServiceResult DeleteWorkFlowCategoryMapping(int tenantID, int workFlowVersionID);
        ServiceResult FinalizeWorkFlowVersion(int workFlowVersionID, string updatedBy);
        ServiceResult CopyWorkFlowCategorymapping(int folderVersionCategoryID, int workFlowVersionID, string updatedBy);
    }
}
