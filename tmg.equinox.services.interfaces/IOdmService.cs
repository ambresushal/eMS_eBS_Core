using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.applicationservices.viewmodels.CompareSync;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using System.Web;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IOdmService
    {
        IEnumerable<ODMParentSectionDeatilsViewModel> GetParentSectionsFromFolderVersion(int tenantId, int formDesignVersionId);

        List<string> SaveFiles(HttpRequestBase Request, string ODMFILEPATH);

        ODMConfigrationViewModel GetPlanDetails(HttpRequestBase Request, string file, string ODMFILEPATH);

        List<ODMMigrationQueueViewModel> GetMigrationQueue(GridPagingRequest gridPagingRequest);

        ServiceResult QueueForMigration(string planData, string pbpSections, string sotSections, string description, string year, string fileName, string originalFileName, int formDesignVersionIDSOT, int formDesignVersionIDPBP, string username);

        void BaselineAndCreateNewMinorVersion(List<ODMPlanConfigViewModel> migrationList, int BatchId, int userId, string username, bool isAfterODM);

        List<ODMPlanConfigViewModel> planList(int BatchId);

        bool CheckFolderIsQueued(int folderID);
    }
}
