using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class RoleViewModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsVisible { get; set; }
        public int TemplateReportVersionID { set; get; }
    }
}
