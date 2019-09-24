using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IExitValidateService
    {
        ExitValidateViewModel Validate(ExitValidateViewModel model);
        ExitValidateViewModel GetDefaultSectionData(int? CurrentUserId);
        ServiceResult UpdateQueue(int ExitValidateQueueID, ExitValidateViewModel model);
        void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync);
        ServiceResult UpdateExitValidateFilePath(int QueueID, ExitValidateViewModel model, string folderPath);
        ExitValidateViewModel GetExitValidateMappings(int ExitValidateQueueID);
        string GetPBPPlanAreaFileName(int formInstanceId);
        int GetPBPViewFormInstanceID(int FolderVersionID, int? FormInstanceID);
        int GetPBPDatabase1Up(int? anchorDocumentID);
        List<ExitValidateResultViewModel> GetExitValidateErrors(int formInstanceId);
        void DeleteDirectory(string path);
        ServiceResult AddExitValidateResults(ExitValidateViewModel model,string reportFileName);
        string ProcessExitValidateAutomation(ExitValidateViewModel model);
        List<ExitValidateResultViewModel> GetLatestExitValidateResults(int formInstanceID);
        int DeleteExtraRowsPlanAreaFile(string QID, System.Data.OleDb.OleDbConnection conn);
        int AddPlanAreaFileRow(JObject source, System.Data.OleDb.OleDbConnection conn);
        bool IsExitValidateInProgress(int formInstanceId, int folderVersionId);
        int UpdatePBPFileRow(System.Data.OleDb.OleDbConnection conn);
        List<ExitValidateExportedList> GetExitValidateExportedList();
        string ProcessPBPExitValidate(ExitValidateViewModel model, out bool isKilledExitValidate);
        ServiceResult AddExitValidatePBPExportError(ExitValidateViewModel evModel, PBPExportToMDBMappingViewModel model);
        List<ExitValidateMapModel> GetSectionMapModels();
        DataTable GetExitValidateResultDataTable(int formInstanceId);
        bool CheckExitValidationCompletedForFolderVersion(int folderversionId);
        ServiceResult ExitValidateFolderversion(int folderversionId, int userId, string userName);
        bool? CheckEVNotificationSentToUser(int folderversionId);
        List<domain.entities.Models.FormInstance> GetFormInstancesForEV(int folderversionId);
        ExitValidateViewModel GetFormInstancesEVStatus(int formInstanceId);
        void SendEVCompletionotification(int exitValidateQueueID, bool hasError, int userId);
        ServiceResult AddExitValidateVBIDExportError(ExitValidateViewModel evModel, VBIDExportToMDBMappingViewModel model);
        List<ExitValidateResultViewModel> GetVBIDViewExitValidateResults(int formInstanceId);
        domain.entities.Models.ExitValidateQueue GetExitValidateQueue(int ExitValidateQueueID);
    }
}
