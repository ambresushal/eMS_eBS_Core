using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class ElementAccessViewModel : ViewModelBase
    {
        public int? RoleID { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public int? ResourceID { get; set; }
        public String RoleName { get; set; }
    }
}
