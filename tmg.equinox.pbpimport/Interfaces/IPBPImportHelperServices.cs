using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.pbpimport.Interfaces
{
    public interface IPBPImportHelperServices
    {
        void InitializeVariables(IUnitOfWorkAsync unitOfWorkAsync);
        int GetFormDesignVersionID(string docName, int year);
        int GetPBPViewFormInstanceID(int folderVersionId, int pBPFromdesignVersionID, int? formInstanceID);
        int GetMedicareFormInstanceID(int folderVersionId, int medicareFormdesignVersionID,int DocumentId);
        ServiceResult UpdateImportQueueStatus(int PBPImportQueueID, domain.entities.Enums.ProcessStatusMasterCode status);
        int GetMedicareDocumentID(int folderVersionId, int formInstanceID);
        int GetMedicareDocumentIDByName(int MEDICAREFORMDESIGNVERSIONID, int DocId, int newFolderVersionId);
        PBPPlanConfigViewModel GetFormInstanceIdForDelete(int MEDICAREFORMDESIGNVERSIONID, PBPPlanConfigViewModel ViewModel);
        int GetFormInstanceForTerminate(int folderVersionId, int documentId);
        int GetEffectiveFormDesignVersionID(string formDesignName, int planYear);
    }
}
