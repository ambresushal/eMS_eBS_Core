using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.Collateral;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFormInstanceService
    {
        int GetAnchorDocumentID(int formInstanceID);
        int GetViewDocumentID(int anchorDocumentID, int formDesignID);
        List<int> GetViewDocumentIDs(int anchorDocumentID, int formDesignID);
        int GetDocumentID(int folderVersionID, int formDesignID);
        int GetDocID(int formInstanceID);
        SourceDesignDetails GetViewByAnchor(int folderVersionID, int viewDesignID, int anchorDocumentID);
        string GetProxyNumber(int formInstanceId);
        List<OONGroupEntryModel> GetOONGroupEntries(int formDesignVersionId);

        void UpdateFormInstanceComplianceValidationlog( int formInstanceId, List<FormInstanceComplianceValidationlog> validationErrors, int collateralProcessQueue1Up);

        IEnumerable<ComplianceValidationlogModel> GetComplianceValidationlog(int formInstanceId, string userName, int collateralQueueId);

        FormInstanceExportPDF GetFormInstanceDetails(int formInstanceId);

        List<SourceDesignDetails> GetQHPViewByAnchor(List<int> formInstanceIds, int formDesignID, bool offExchangeOnly);

        List<JsonFieldMappingViewModelExtended> GetJsonFieldsData();
        FormInstanceViewModel GetFormInstancesByName(string documentName, string EffectiveDate, int formDesignID);
        void UpdateFormInstanceComplianceValidationlogForPrintX(int formInstanceId, FormInstanceComplianceValidationlog logs, int collateralProcessQueue1Up);
    }
}
