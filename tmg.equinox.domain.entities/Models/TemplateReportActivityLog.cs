using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportActivityLog:Entity
    {
        public int ActivityLoggerID { get; set; }
        public int TemplateReportVersionID { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }        
        public virtual TemplateReportVersion TemplateReportVersion { get; set; }        
    }
}
