using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ReportFormatType : Entity
    {
        public int ReportFFormatTypeID { get; set; }
        public string ReportFormatTypeName { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<TemplateReportVersion> TemplateReportVersions { get; set; }
    }
}
