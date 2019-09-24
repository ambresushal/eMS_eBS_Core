using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.MasterList;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IMasterListService
    {
        DateTime GetEffectiveDate(int folderVersionId);
        List<int> GetFormInstanceIds(DateTime EffectiveDate, string ruleAlias, string documentFilter);
        List<int> GetFormInstanceIds(DateTime EffectiveDate);
        string GetSectionNameFromFormInstanceID(int tenantId, int formInstanceID);
        string GetFormInstanceData(int tenantId, int formInstanceID);

        IEnumerable<KeyValue> GetUIElementTypes(int tenantId);

        IEnumerable<KeyValue> GetApplicationDataTypes(int tenantId);

        IEnumerable<KeyValue> GetLayoutTypes(int tenantId);

        IEnumerable<KeyValue> GetLogicalOperatorTypes(int tenantId);

        IEnumerable<KeyValue> GetOperatorTypes(int tenantId);

        IEnumerable<KeyValue> GetStatusTypes(int tenantId);

        IEnumerable<KeyValue> GetTargetPropertyTypes(int tenantId);

        IEnumerable<KeyValue> GetLibraryRegexes(int tenantId);

        IEnumerable<KeyValue> GetApprovalStatusTypeList(int tenantId);

        IEnumerable<KeyValue> GetOwnerList(int tenantId);

        List<int> GetFormInstanceIds(int folderVersionId);

        ServiceResult SaveMasterListImportData(string FileName, string FilePath, int FormInstanceID, string Comment, string AddedBy, DateTime AddedDate, string Status);

        MasterListVersions GetMasterListVersions(int formDesignVersionID,int folderVersionID);

        MasterListFormDesignViewModel GetFolderVersionFormDesign(int folderVersionID);

        int GetAliasMasterListForEffectiveDate(int tenantId, DateTime effectiveDate);
        JObject GetAliasMasterListDataForDesignVersion(int tenantId, int formDesignVersionId);

    }
}
