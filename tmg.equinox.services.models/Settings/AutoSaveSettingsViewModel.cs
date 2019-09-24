using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class AutoSaveSettingsViewModel
    {
        public int SettingsID { get; set; }
        public int UserID { get; set; }
        public bool IsAutoSaveEnabled { get; set; }
        public int Duration { get; set; }
        public int TenantID { get; set; }
        public bool AccelerateStartDateForTask { get; set; }
    }
}
