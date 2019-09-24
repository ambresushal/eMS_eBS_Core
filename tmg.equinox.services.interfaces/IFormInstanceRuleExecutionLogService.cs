using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormInstanceRuleExecutionLogService
    {
        GridPagingResponse<FormInstanceRuleExecutionLogViewModel> GetRuleExecutionLogData(int formInstanceId, int parentRowID, bool isParentData, string sessionId, GridPagingRequest gridPagingRequest);
        ServiceResult SaveFormInstanceRuleExecutionlogData(int tenantId, int formInstanceId, int folderId, int folderVersionId, int formDesignId, int formDesignVersionId, IList<FormInstanceRuleExecutionLogViewModel> loggerDataJsonObject);
        RuleRowModel GetRuleDescription(int ruleID);
        List<FormInstanceRuleExecutionServerLogViewModel> GetRuleExecutionServerLogData(int formInstnaceId, string sessionID, int parentRowID, bool isParentData);
        void SaveFormInstanceRuleExecutionServerlogData(int formInstanceId, int parentRowID, RuleDesign rule, bool result);
        int SaveFormInstanceRuleExecutionServerlogDataOnLoad(int formInstanceId, int elementID, string loadedElement);
    }
}
