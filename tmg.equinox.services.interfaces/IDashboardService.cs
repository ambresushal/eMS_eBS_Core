using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.DashBoard;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IDashboardService
    {

        IQueryable<FormUpdatesViewModel> GetFormUpdatesList(int tenantId);

        IQueryable<WorkQueueViewModel> GetWorkQueueList(int tenantId, int? userId);

        GridPagingResponse<WatchListViewModel> GetWatchList(int tenantId, int? userId, GridPagingRequest gridPagingRequest, bool isViewInterested, int? RoleID);

        IQueryable<WorkQueueViewModel> GetWorkQueueListNotReleasedAndApproved(int tenantId, int? userId);

        GridPagingResponse<UserRoleAssignmentViewModel> GetUserRoleAssignment(int folderVersionID, int? currentUserId, string userAssignmentDialogState, GridPagingRequest gridPagingRequest);

        DataTable GetWatchListListDataTable(int tenantId, int? userId);

        IList<KeyValue> GetTeamsOfFolderVersion(int tenantId, int folderVersionID);

        ServiceResult SaveInterestedWatchList(int folderVersionId, int? currentUserId);
        ServiceResult SaveInterestedWatchList(int TaskMappingID, bool markInterested);
        ServiceResult DeleteInterestedWatchList(int folderVersionId);
        ServiceResult SaveInterestedAllWatchList(int[] taskMappingIDs, bool value);
        bool CheckIsManager(int? currentUserId);
        string GetLastUserActivity(string UserName);
        IList<UserActivity> GetActivity(string UserName);

        GridPagingResponse<ViewModelForProofingTasks> GetWorkQueueListForProofingTasks(int tenantId, int? userId, string UserName, string viewMode, int taskFolderVersionId,bool isDownload, GridPagingRequest gridPagingRequest);
        GridPagingResponse<ViewModelForProofingTasks> GetWatchListForProofingTasks(int tenantId, int? userId, string UserName, bool isViewInterested, string viewMode, GridPagingRequest gridPagingRequest, int taskFolderVersionId, bool isDownload);
        DataTable GetWatchListListForProofingTasksDataTable(int tenantId, int? userId, string UserName, bool isViewInterested, string viewMode, int taskFolderVersionId);
        DataTable GetWorkQueueListListForProofingTasksDataTable(int tenantId, int? userId, string UserName, string viewMode, int taskFolderVersionId);

        int GetUserID(string username);

        int GetNotificationCount(int userid);
        GridPagingResponse<NotificationstatusViewModel> GetNotificationstatusDataList(bool viewMode, int? CurrentUserId, GridPagingRequest gridPagingRequest);
        IList<NotificationstatusViewModel> GetNotificationisreadClrData(bool viewMode, int? CurrentUserId);
        bool SaveNotificationDataList(string Message, int SendToUserId, string loggedinUserName);
        IQueryable<ViewModelForProofingTasks> GetSubworkQueueList(int FolderVersionID, string viewMode);
        List<CommentViewModel> GetComment(int ID, bool isAttachmentsOnly);
        IQueryable<ViewModelForProofingTasks> GetSubwatchQueueList(int FolderVersionID, string viewMode);

    }
}