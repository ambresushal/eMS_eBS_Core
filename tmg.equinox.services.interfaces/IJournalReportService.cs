using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersionReport;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IJournalReportService
    {
        List<JournalViewModel> GetAllJournalsList(int formInstanceId, int folderVersionId, int folderId, FormDesignVersionDetail detail);
        List<JournalViewModel> GetCurrentVersionJournalsList(int formInstanceId, int folderVersionId, int formDesignVersionId, int tenantId, FormDesignVersionDetail detail);
        List<JournalResponseViewModel> GetAllJournalResponsesList(int journalId);
        List<JournalViewModel> GetCurrentJournal(int journalId);
        List<JournalActionViewModel> GetAllActionList();
        ServiceResultStatus SaveJournalEntry(int formInstanceID, int folderVersionID, string description, string fieldName, string fieldPath, int actionID, int addedWFStateID, int? closedWFStateID, string addedBy);
        ServiceResultStatus UpdateJournalEntry(int actionID, int closedWFStateID, string updatedBy, int journalId);
        ServiceResultStatus SaveJournalResponse(string response, string addedBy, int journalId);
        bool CheckAllJournalEntryIsClosed(int folderVersionId, int formInstanceId);
    }
}
