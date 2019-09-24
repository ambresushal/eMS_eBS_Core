using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.applicationservices.viewmodels;
using System.Data;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.repository.interfaces;
using tmg.equinox.domain.viewmodels;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IMasterListCascadeService
    {
        List<MasterListCascadeViewModel> GetMasterListCascade(int formDesignID, int formDesignVersionID);
        int AddMasterListCascadeBatch(MasterListCascadeViewModel mlCascade,string userName);
        JObject GetMasterListSectionData(MasterListCascadeViewModel mlCascade, int folderversionID, int formInstanceID);
        List<DocumentFilterResult> FilterDocuments(DateTime effectiveDate, string filterPath, int formDesignID, int formDesignVersionID, List<MLPlanCode> planCodes, int folderVersionID, int masterListCascadeID);
        List<MasterListCascadeDocumentRuleViewModel> GetRules(int formDesignID, int formDesignVersionID, int masterListFormDesignID, int masterListFormDesignVersionID);
        ServiceResult UpdateMasterListCascadeBatch(int masterListCascadebatchID, int status,string message);
        int AddMasterListCascadeBatchDetail(MasterListCascadeBatchDetailViewModel batchDetailModel);
        ServiceResult UpdateMasterListCascadeBatchDetail(int masterListCascadeBatchDetailID, int status, string message);
        List<MasterListCascadeBatchViewModel> GetMasterListCascadeBatch();
        List<MasterListCascadeBatchDetailViewModel> GetMasterListCascadeBatchDetail(int masterListCascadeBatchID);
        ElementDocumentRuleViewModel GetElementDocumentRule(int formDesignVersionID, string sourceElementPath);
        List<ElementDocumentRuleViewModel> GetElementDocumentRules(int formDesignVersionID, string sourceElementPath);
        int getFormInstancePBPView(int forminstanceid, int FormDesignVersionID);
        MasterListCascadeBatchViewModel GetQueuedBatch();
        List<ElementDocumentRuleViewModel> GetODMElementDocumentRules(int formDesignVersionId);
        MasterListCascadeBatchDetailViewModel GetInProgressMatchCascade(int formInstanceId);
    }
}
