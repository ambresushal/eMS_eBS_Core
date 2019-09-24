using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPlanTaskUserMappingService
    {
        int GetFormDesignVersionByFormInstanceId(int formInstanceId);
        DPFPlanTaskUserMapping GetDPFPlanTaskUserMapping(int PlanTaskUserMappingId);

        IEnumerable<KeyValue> GetTeamMemberList(string strUserName);

        ServiceResult SavePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel);

        ServiceResult UpdatePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel);

        ServiceResult UpdateQueuePlanTaskUserMapping(DPFPlanTaskUserMappingViewModel objPlanTaskModel);

        bool SendEmailForNewTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM);

        bool SendEmailForNewTaskAssignmentForAllPlans(DPFPlanTaskUserMappingViewModel objPlanTaskVM);

        bool SendEmailForTaskCompletion(DPFPlanTaskUserMappingViewModel objPlanTaskVM);

        bool SendEmailForChangesInPlanAndTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM);

        bool SendEmailForDueDateOverForTaskAssignment(DPFPlanTaskUserMappingViewModel objPlanTaskVM);

        bool ExecuteNotifyTaskDueDateOverEmail();

        List<DPFPlanTaskUserMappingViewModel> GetPlanTaskUserMappingList(int PlanTaskUserMappingId);

        bool ValidateTaskCompletedForWorkFlow(int FolderVersionID, int WorkFlowVersionStateID);

        IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int folderVersionId);

        List<FormInstanceViewModel> GetFormInstanceListForFolderVersion(int tenantId, int folderVersionId, int folderId, int formDesignVersionId, int formDesignType = 0);

        List<SectionDesign> GetSectionsList(int tenantId, string formDesignVersionId, IFormDesignService _formDesignService);
        bool SavetaskPlanNewFolderVersion(int FolderversionId,string currentUser);

        void ExecuteNotifyTaskDueDateOverPushNotification(string loggedInUserName);

        void ResetStartDateDueDateOnFolderVersionWorkflowStateChange(int FolderVersionID, int WorkFlowVersionStateID);

        ServiceResult DeletePlanTaskUserMappingByFolderversionId(int folderversionId);
        ServiceResult DeletePlanTaskUserMappingByFormInstanceId(int folderversionId, int formInstanceId);
    }
}
