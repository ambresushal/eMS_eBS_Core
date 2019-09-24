using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class SBCReportServiceMaster : Entity
    {
        public SBCReportServiceMaster() { }
        public int SBCReportServiceMasterID { get; set; }
        public string ReportPlaceHolderID { get; set; }
        public string Category1Name { get; set; }
        public string Category2Name { get; set; }
        public string Category3Name { get; set; }
        public string POSName { get; set; }
        public string NetworkName { get; set; }
        public int TenantID { get; set; }
        public virtual Tenant Tenant { get; set; }
        
    }
}
