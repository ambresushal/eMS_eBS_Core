using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormReport : Entity
    {
        public FormReport()
        {
            this.FormReportVersions = new List<FormReportVersion>();
        }

        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<FormReportVersion> FormReportVersions { get; set; }
    }
}
