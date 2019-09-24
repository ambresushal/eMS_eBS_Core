using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportRoleAccessPermission : Entity
    {
        public int TemplateReportPermissionID { get; set; }
        public int TemplateReportVersionID { get; set; }
        public int RoleID { get; set; }
        public int TenantID { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual TemplateReportVersion TemplateReportVersion { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
