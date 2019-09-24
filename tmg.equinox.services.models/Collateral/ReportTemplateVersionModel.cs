using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportTemplateVersionModel
    {
        public int ReportTemplateVersionID { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Status { get; set; }
        public string VersionNumber { get; set; }
        public string TemplateLocation { get; set; }
        public string TemplateName { get; set; }
    }
}
