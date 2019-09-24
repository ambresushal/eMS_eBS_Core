using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportVersion : Entity
    {
        public int TemplateReportVersionID { get; set; }
        public int TemplateReportID { get; set; }
        public string Description { get; set; }
        public string VersionNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string HelpText { get; set; }
        public int ReportFormatTypeID { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Nullable<bool> IsVisible { get; set; }
        public Nullable<bool> IsReleased { get; set; }
        public string TemplateFileName { get; set; }
        public string Location { get; set; }
        //public string Comments { get; set; }
        public bool IsActive { get; set; }

        public virtual TemplateReport TemplateReport { get; set; }
        public virtual ICollection<TemplateReportLocation> TemplateReportLocations { get; set; }
        public virtual ReportFormatType ReportFormatType { get; set; }
        public virtual ICollection<TemplateReportFormDesignVersionMap> TemplateReportFormDesignVersionMaps { get; set; }
        public virtual ICollection<TemplateReportRoleAccessPermission> TemplateReportRoleAccessPermissions { get; set; }
        public virtual ICollection<TemplateReportVersionParameter> TemplateReportVersionParameters { get; set; }
        public virtual ICollection<TemplateReportActivityLog> TemplateReportActivityLogs { get; set; } 
    }
}
