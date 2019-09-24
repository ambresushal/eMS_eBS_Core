using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.WCReport;


namespace tmg.equinox.applicationservices.interfaces
{
    public interface IReportQueueServices
    {
        IList<ReportQueueViewModel> GetReportQueueList();

        GridPagingResponse<ReportQueueViewModel> GetReportQueueList(GridPagingRequest gridPagingRequest);

        ServiceResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId, string AddedBy,int UserId, DateTime AddedDate, string Status);
        List<ReportQueueDetailsViewModel> GetReportQueueFolderDetailsList(int queueId);
        ServiceResult DeleteReportQueue(int ReportQueueId);

        ServiceResult UpdateReportQueue(int ReportQueueId, ReportQueueViewModel model);

        List<FormInstanceViewModel> GetFormInstanceDetails(List<int> formInstanceIDs);

        ServiceResult AddReportQueue(int ReportId, int[] FolderId, int[] FolderVersionId, int[] FormInstanceId, string AddedBy, int UserId, DateTime AddedDate, string Status);
    }
}
