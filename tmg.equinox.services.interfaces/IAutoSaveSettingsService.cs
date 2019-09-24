using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Settings;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IAutoSaveSettingsService
    {
        ServiceResult SaveAutoSaveSettings(AutoSaveSettingsViewModel settingsViewModel, int? CurrentUserId, string userName);

        AutoSaveSettingsViewModel GetAutoSaveSettingsForTenant(int tenantID);

        AutoSaveSettingsViewModel GetAutoSaveDuration(int tenantID);
    }
}
