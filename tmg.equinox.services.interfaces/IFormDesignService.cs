using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignBuilderFromDomainModel;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormDesignService
    {
        //Form Design Interfaces
        int GetLatestFormDesignVersion(int formDesignVersionId);
        IEnumerable<FormDesignRowModel> GetFormDesignList(int tenantId);
        List<FormDesignGroupMapModel> GetFormDesignGroupMap();
        IEnumerable<FormDesignRowModel> GetAnchorDesignList(int tenantId);
        IEnumerable<FormDesignRowModel> GetFormDesignListByDocType(int tenantId, int docType);
        IEnumerable<DocumentDesignTypeRowModel> GetDocumentDesignType();
        ServiceResult AddFormDesign(string userName, int tenantId, string formName, string displayText, string abbreviation, bool isMasterList, int docType, int srcDesign,bool isAliasDesign,bool usesAliasDesign, bool IsSectionLock);
        ServiceResult UpdateFormDesign(string userName, int tenantId, int formDesignId, string formName, string displayText, int srcDesign, bool IsSectionLock);
        ServiceResult DeleteFormDesign(string userName, int tenantId, int formDesignId);
        ServiceResult AddDesignType(string displayText);

        //Form Design Version Interfaces
        IEnumerable<FormDesignVersionRowModel> GetFormDesignVersionList(int tenantId, int formDesignId);
        ServiceResult AddFormDesignVersion(string userName, int tenantId, int formDesignId, DateTime effectiveDate, string versionNumber, string formDesignVersionData);
        ServiceResult UpdateFormDesignVersion(string userName, int tenantId, int formDesignVersionId, DateTime effectiveDate, string versionNumber);
        ServiceResult FinalizeFormDesignVersion(string userName, int tenantId, int formDesignVersionId, string comments);
        ServiceResult DeleteFormDesignVersion(string userName, int tenantId, int formDesignVersionId, int formDesignId);
        ServiceResult CopyFormDesignVersion(string userName, int tenantId, int formDesignVersionId, DateTime effectiveDate, string versionNumber, string formDesignVersionData);
        string GetVersionNumber(int formDesignVersionId, int tenantId);

        string GetFormDesignVersionData(int tenantId, int formDesignVersionId);

        FormDesignVersionDetail GetFormDesignVersionDetail(int tenantId, int formDesignVersionId);
        ServiceResult SaveCompiledFormDesignVersion(int tenantId, int formDesignVersionId, string jsonData, string userName);
        string GetCompiledFormDesignVersion(int tenantId, int formDesignVersionId);
        FormDesignVersionDetailFromDM GetFormDesignVersionDetailFromDataModel(int tenantId, int formDesignVersionId);
        ServiceResult CheckDataSourceMappings(string username, int tenantId, int formDesignVersionId);
        int GetEffectiveFormDesignVersion(string username, int tenantId, int formInstanceId, int formDesignVersionId, int folderVersionId);
        bool IsFinalizedFormDesignVersionExists(int? formDesignId);

        void SaveFormDesignHistory(int formDesignVersionId, string formDesignVersionData, string enteredBy, DateTime enteredDate, string action, int tenantId);
        bool IsMajorFormDesingVersion(int formDesignVersionId, int tenantId);
        bool IsMasterList(int formDesignVersionId);
        IEnumerable<FormDesignRowModel> GetMasterListFormDesignList(int tenantId);
        List<DesignDocumentMapViewModel> GetMappedDesignDocumentList(int tenantId, int formDesignID, DateTime? effectiveDate);
        bool isMasterList(string formName);
        string GetEventMapJSON(int tenantId, int formDesignVersionId);
        string GetExecutionTreeJSON(int tenantId, int formDesignVersionId);
        FormDesignVersionRowModel GetFormDesignVersionById(int formDesignVersionID);
        List<FormDesignGroupRowMapModel> GetFormDesignsForGroup(int tenantId, int formDesignID);
        int GetPreviousFormDesignVersion(int tenantId, int formDesignId, int formDesignVersionId);
        int GetLatestFormDesignVersionID(string formDesign, DateTime effectiveDate);
        List<JsonDesign> GetFormDesignInformation();
        List<JsonDesign> GetFormDesignInformation(int formDesignId,int formDesignVersionId);
        List<FormDesignVersionActivityLog> GetFormDesignVersionActivityLogData(int formDesignId, int formDesignVersionId, int formDesignPreviousVersionId);
    }
}
