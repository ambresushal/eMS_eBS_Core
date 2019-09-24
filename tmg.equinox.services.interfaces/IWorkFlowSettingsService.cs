using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IWorkFlowSettingsService
    {
        IList<KeyValue> GetApplicableTeamList(int tenantId);

        ServiceResult UpdateApplicableTeamUserMap(int tenantId, int teamId, List<ApplicableTeamMapModel> applicableTeamMapData, string userName);

        List<WorkFlowSettingsViewModel> GetApplicableTeamUserMap(int tenantId, int teamId);
    }
}
