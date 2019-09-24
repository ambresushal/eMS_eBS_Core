using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowStateServices
    {

        List<WorkFlowVersionStatesViewModel> GetWorkFlowStateList(int tenantId, int folderVersionId);

        List<FolderVersionWorkFlowViewModel> GetFolderVersionWorkFlowList(int tenantId, int folderVersionId);

        ServiceResult UpdateWorkflowState(int tenantId, int folderVersionId, int workflowStateId, int approvalStatusId, string commenttext, int userId, string userName, string majorFolderVersionNumber, string sendGridUserName, string sendGridPassword);

        ServiceResult AddApplicableTeams(List<int> applicableTeamsIDList, int folderId, int folderVersionId, string addedBy);

        bool CheckMilestoneChecklistSection(int folderVersionId, string sectionName);

        List<WorkFlowStateFolderVersionMapViewModel> GetApplicableTeams(int folderVersionId);

        int GetFolderVersionWorkFlowId(int tenantId, int folderVersionId);

        string GetFolderVersionWorkFlowName(int tenantId, int folderVersionId);

        bool isFolderVersionAccelarated(int tenantId, int folderVersionId);

        ServiceResult UpdateWorkflowStateFolderMember(int tenantId, int folderId, int folderVersionId, IList<int> userId, string currentUserName, int accountId, int currentUserId, string sendGridUserName, string sendGridPassword);

        IList<KeyValue> GetWorkFlowTeamMembers(int tenantId, int folderId, int? currentUserId);
        void UpdateFolderVersionWorkflowState(int folderVersionId, int workflowStateId, string userName, int approvalStatusId, string commenttext, int userId);

        int GetWorkFlowStateByProductID(string productId);

        int GetApprovedWFStateId(int folderVersionId);

        string GetApprovedWFStateName(int folderVersionId);

        List<WorkFlowVersionStatesAccessViewModel> GetWorkFlowStateUserRoles(int tenantId, int folderVersionId);

        IEnumerable<KeyValue> GetCurrentWorkFlowState(int tenantId, int folderVersionId);

        List<KeyValue> GetCurrentWorkFlowStateForStatusUpdate(int tenantId, int folderVersionId);

        IEnumerable<KeyValue> GetApprovalStatusTypeForFolderVersion(int tenantId, int folderVersionId, int wfVersionStateId);

        string GetAcceleratedConfirmationMsg(int wfversionstateId);

        ServiceResult ValidateWorkflowStateUser(List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId);

        ServiceResult DeleteWorkFlowVersionStatesUser(List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId, string currentUserName);

        ServiceResult UpdateFolderVersionWorkflowStateUser(int tenantId, List<UserRoleAssignmentViewModel> assignedUserList, int folderVersionId, string currentUserName, int? currentUserId, string sendGridUserName, string sendGridPassword, string smtpUserName, string smtpPort, string smtpHostServerName);

        void SendMailForFoldersWithUnchangedWorkFlowState();

        string GetWorkflowStateName(int workflowStateId);

        WorkFlowStateMasterViewModel GetWorkFlowState(int tenantId, int folderVersionId);

    }
}
