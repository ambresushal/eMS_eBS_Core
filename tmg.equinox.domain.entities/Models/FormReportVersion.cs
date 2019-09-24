using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormReportVersion : Entity
    {
        public FormReportVersion()
        {
            this.FormReportVersionMaps = new List<FormReportVersionMap>();
        }

        public int ReportVersionID { get; set; }
        public int ReportID { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual FormReport FormReport { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<FormReportVersionMap> FormReportVersionMaps { get; set; }
    }
}
