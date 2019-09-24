using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
    public class ReportSettingViewModel : ViewModelBase
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportTemplatePath { get; set; }
        public string OutputPath { get; set; }
        public string ReportNameFormat { get; set; }
        public string Description { get; set; }
        public string SQLstatement { get; set; }
        public bool Visible { get; set; }
    }
}
