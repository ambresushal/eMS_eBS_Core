using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.Report;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface ICollateralService
    {
        // Form Design
        IEnumerable<FormDesignRowModel> GetFormDesignList(int tenantId);

        // Form Design Version
        IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int formDesignId);


        XmlDocument GetFormDesignVersionXML(int tenantId, int formDesignVersionId, ref string FileName);

        IEnumerable<ReportTemplateModel> GetDocumentVersionInfo(int ReportTemplateVersionID);//, GridPagingRequest gridPagingRequest);

        ServiceResult UpdateReportTemplateVersion(int tenantid, string username, string path, int reportTemplateID, string reportName, string templateDocMappings, string templateProperties, string Parameters);

        ServiceResult SaveReportLocation(int tenantid, string location, int TemplateReportVersionID, ref int locationid, string locationName = "In Folder");
        GridPagingResponse<ReportDesignViewModel> GetReportNames(int tenantId, GridPagingRequest gridPagingRequest);
        IEnumerable<ReportDesignViewModel> GetReportNamesForGeneration(int tenantId, string ReportLocation);
        ServiceResult UpdateReportPropertiesParameters(List<ParameterViewModel> parameters, ReportPropertiesViewModel model);
        ReportPropertiesViewModel GetProperties(int tenantId, int FormReportId);
        int GetLatestVersionNumber(int reportTemplateID);

        IEnumerable<ReportDocumentModel> GetDocumentsforReportGeneration(int ReportTemplateVersionID, int AccountID, int FolderID, int FolderVersionID, bool fetchAccounts = false);
        List<ReportDocumentModel> GetDocumentsforReportGeneration(int ReportTemplateVersionID, int AccountID, int FolderID, int FolderVersionID, Array formInstanceIDList);

        ReportDataSource GetDataSourceForReport(Dictionary<string, int> dataSource, int reportTemplateVersionID, string reportTemplateName, string reportTemplateLocation);

        Dictionary<string, int> GetReportTemplateFromReportID(int ReportTemplateID, int FolderVersionID, IEnumerable<string> formInstanceIDList, ref int TemplateReportVersionID, ref string reportTemplateLocation, ref string status, string folderVersionEffDt);

        GridPagingResponse<ReportTemplateVersionModel> GetReportTemplateVersionList(int TenantID, int ReportTemplateID, GridPagingRequest gridPagingRequest);

        ServiceResult AddReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username);

        ServiceResult UpdateReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username);

        ServiceResult AddReportTemplateVersion(int TenantID, int ReportTemplateID, string EffectiveDate, string username, int reportTemplateVersionID);

        ServiceResult UpdateReportTemplateVersion(int TenantID, int ReportTemplateVersionID, string EffectiveDate, string username);

        ServiceResult DeleteReportTemplateVersion(int TenantId, int ReportTemplateVersionID);

        //ServiceResult DeleteReportTemplate(int TenantID, int ReportTemplateID, string ReportName, string username);


        IEnumerable<RoleViewModel> GetRoleNames(int tenantId, int reportVersionId);
        ServiceResult AddViewPermissionSet(List<RoleViewModel> viewPermissionList);
        string GetTemplateNameById(int templateReportId);
        IEnumerable<ParameterViewModel> GetParameters(int reportVersionID);

        ServiceResult DeleteReportDesign(string userName, int tenantId, int reportDesignId);
        ServiceResult FinalizeReportVersion(int tenantId, int reportVersionId, string comments);
        ServiceResult QueueCollateral(int accountID, string accountName, int folderID, string folderName, int folderVersionID, IEnumerable<string> formInstanceIDs, IEnumerable<string> folderVersionNumbers, IEnumerable<string> productIds, int reportTemplateID, string folderVersionEffDt, string username, DateTime? runDate, string collateralFolderPath, string reportName);
        void SaveTemplateActivityLog(int templVerID, string description, string username);
        GridPagingResponse<ReportUserActivityViewModel> GetReportUserActivity(GridPagingRequest gridPagingRequest);
        bool IsReportTemplateVersionAccessiable(int reportTemplateID, string folderVersionEffDt, string currentUserName);
        bool IsTemplateUploaded(int reportTemplateVersionId);

        GridPagingResponse<QueuedReportModel> GetQueuedCollateralsList(GridPagingRequest gridPagingRequest);
        GridPagingResponse<QueuedReportModel> ViewCollateralsAtFolder(int formInstanceID, GridPagingRequest gridPagingRequest);
        List<string> GetGenerateReportInputs(int formInstanceID, int reportTemplateID, int formDesignVersionID, IEnumerable<string> formInstanceIDList, string folderVersionEffDt);

        FormInstanceViewModel GetFormInstanceIdForView(int formInstanceID, int reportTemplateID, int formDesignVersionID, IEnumerable<string> formInstanceIDs, string folderVersionEffDt);
        string GetFilePath(int processQueue1Up, string fileFormat);

        QueuedReportModel IsReportAlreadyGenerated(int formInstanceId, int reportTemplateId);

        IEnumerable<ReportDesignViewModel> GetCollateralTemplatesForProduct(int formInstanceId, string reportLocation);
        GridPagingResponse<ReportDocumentModel> GetAccountAndFolders(int ReportTemplateID, int AccountID, int FolderID, int FolderVersionID,string reportName,string planFamilyName, GridPagingRequest gridPagingRequest, bool shouldSelectFormInstance);
        bool CheckIfCollateralIsOfSBDesignType(int templateReportID);

        bool CheckIfCollateralsOfEOCDesignType(int templateReportID);

        void UpdateLayoutTypeMedicate(string formInstanceIDs, int count);
        ServiceResult QueueSBDesignCollateral(string accountIDs, string accountNames, string folderIDs, string folderNames, string formInstanceIDs, string folderVersionIDs, string folderVersionNumbers, string productIds,string formDesignIds, string formDesignVersionIds, int reportTemplateID, string folderVersionEffDts, string reportName, string userName, string collateralFolderPath);

        ServiceResult SaveCollateralImages(string desciption, string imagePath, string fileName, string name);
        GridPagingResponse<CollateralImageView> GetCollateralImages(GridPagingRequest gridPagingRequest);
        List<CollateralImageView> GetCollateralImages();
        ServiceResult DeleteCollateralImages(int id);
        List<CollateralImageView> GetCollateralImages(int id);

        string[] GetPlanFamily(string reportName);

        string SaveCollatralPrintXFormatedPDF(int processQueue1Up, string CollatralFilePath);
        GridPagingResponse<UploadReportModel> GetCollateralProcessUploadList(GridPagingRequest gridPagingRequest);
        ServiceResult UploadCollateral(UploadReportViewModel uploadReportViewModel);
        DownloadFileModel GetManualUploadedFilePath(int id, string fileFormat);
    }
}
