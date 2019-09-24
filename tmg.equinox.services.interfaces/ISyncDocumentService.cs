using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.comparesync;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface ISyncDocumentService
    {
        GridPagingResponse<SyncDocumentMacroViewModel> GetMacroList(int tenantId, int formInstanceID, GridPagingRequest request, string userName, int roleID);
        SyncDocumentMacroViewModel GetMacroById(int macroID);
        GridPagingResponse<SyncGroupLogViewModel> GetGroupLogList(int tenantId, int macroId, GridPagingRequest request);
        GridPagingResponse<SyncDocumentLogViewModel> GetDocumentListForGroup(int tenantId, int syncGroupLogId, GridPagingRequest request);
        string GetMacroJSONString(int tenantId, int macroId);
        ServiceResult InsertMacro(int tenantId, int formInstanceID, SyncDocumentMacroViewModel model);
        ServiceResult UpdateMacro(int tenantId, int macroId, string macroJSON);
        ServiceResult DeleteMacro(int tenantId, int macroId);
        ServiceResult CopyMacro(int tenantId, int macroId, string macroName, string notes, bool isPublic);
        string GetSourceDocumentData(int formInstanceID);

        ServiceResult InsertSyncGroupLog(SyncGroupLogViewModel model, string currentUser);
        ServiceResult InsertSyncDocumentLogs(IList<SyncDocumentLogViewModel> models);

        ServiceResult isMacroExist(string macroName);
    }
}
