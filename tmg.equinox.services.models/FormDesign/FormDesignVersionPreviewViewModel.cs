using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormDesignVersionPreviewViewModel : ViewModelBase
    {
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FormName { get; set; }
    }
}
