using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReport : Entity
    {
        public int TemplateReportID { get; set; }
        public string TemplateReportName { get; set; }
        //public int LocationID { get; set; }
        public int TenantID { get; set; }

        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string ReportDescription { get; set; }

        public string ReportType { get; set; }
        public string HelpText { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<TemplateReportVersion> TemplateReportVersions { get; set; }
        //public virtual TemplateReportLocation TemplateReportLocation { get; set; }
        public virtual ICollection<TemplateReportFormDesignVersionMap> TemplateReportFormDesignVersionMaps { get; set; }
    }
}
