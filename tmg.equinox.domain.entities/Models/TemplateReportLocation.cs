using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportLocation : Entity
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public int TemplateReportVersionID { get; set; }
        public string LocationDescription { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        //public virtual ICollection<TemplateReport> TemplateReports { get; set; }
        public virtual TemplateReportVersion TemplateReportVersion { get; set; }

    }
}
