using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportFormDesignVersionMap : Entity
    {
        public int TemplateReportFormDesignVersionMapID { get; set; }
        public int TemplateReportID { get; set; }
        public int TemplateReportVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string DataSourceName { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual TemplateReport TemplateReport { get; set; }
        public virtual TemplateReportVersion TemplateReportVersion { get; set; }     
    }
}
