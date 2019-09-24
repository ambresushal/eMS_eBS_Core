using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ReportingCenter;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IMDMSyncDataService
    {
        List<int> GetReadyForUpdateDocumentUpdateTracker(int status);
        void UpdateDocumentUpdateTracker(DocumentUpdateTracker documentUpdateTracker);
        void UpdateDocumentUpdateTrackerStatus(int formInstanceID, int status);
        DocumentUpdateTracker GetDocumentUpdateTrackerStatusByFormInstanceId(int formInstanceID);
        List<SchemaUpdateTracker> GetSchemaUpdateTracker();
        void UpdateSchemaUpdateTracker(SchemaUpdateTracker documentUpdateTracker);
        void AddLogForMDMProcess(MDMLog mDMLog);
        GridPagingResponse<ReportingCenterDataSyncViewModel> GetReportCenterDataSynclogList(GridPagingRequest gridPagingRequest);
        string GetMDMErrordata(int formInstanceId, int formDesignVersionId);
        void UpdateDocumentUpdateTracker(int formInstanceID, int status);
    }
}
