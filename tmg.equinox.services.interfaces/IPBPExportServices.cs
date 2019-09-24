using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IPBPExportServices
    {
        GridPagingResponse<PBPExportQueueViewModel> GetQueuedPBPExports(GridPagingRequest gridPagingRequest);
        GridPagingResponse<PBPImportQueueViewModel> GetDatabaseNamesForPBPExports(GridPagingRequest gridPagingRequest);
        ServiceResult QueuePBPExport(string exportName, string description, string DBName, string userName, int pbpDatabase1Up,bool scheduleForProcessing,int? correntUserId, string runRuleInWindowsService);
        void ScheduleForPBPExportQueue(int pbpExportQueueID, string currentUserName);
        GridPagingResponse<PBPDatabseDetailViewModel> GetDatabaseDetails(int PBPDatabase1Up, GridPagingRequest gridPagingRequest);
        string GetZipFilePath(int PBPExportQueueID);
        DataTable GetPBPExportDataTable();
        string GetJSONString(int formInstanceID);
        IEnumerable<PBPExportToMDBMappingViewModel> GetExportMappings(int year);
        void GenerateMDBFile(int PBPExportQueueID, string userName);
        string ExitValidateGenerateMDBFile(int ExitValidateQueueID, string userName);
        PBPExportQueueViewModel GetQueuedPBPExport(int PBPExportQueueID);
        PBPExportQueueViewModel GetQueuedOrProcessingPBPExport();
        ServiceResult UpdatePBPFilePath(int PBPExportQueueID, string PBPpath, string Zippath);
        List<FormInstanceViewModel> GetFormInstanceID_Name(int PBPDatabase1Up , int formDesignId);
        List<FormInstanceViewModel> GetFormInstanceID_Name(int PBPDatabase1Up);
        bool CheckFolderIsQueued(int folderID);
        List<ResourceLock> CheckExportFolderIsLocked(int pbpDatabase1Up, int? currentUserId, string userName);
        int GetExportYear(int ExportQueueId);
        void PreProccessingLog(int PBPExportQueueID, string message, string userName, Exception ex);
        void PreProcessingLogs(List<PBPExportLogViewModel> logs);
        ServiceResult UpdateExportQueueStatus(int PBPExportQueueID, domain.entities.Enums.ProcessStatusMasterCode status);
        void GenerateMDBFile();
        IEnumerable<string> GetHiddenSectionList();
    }
}
