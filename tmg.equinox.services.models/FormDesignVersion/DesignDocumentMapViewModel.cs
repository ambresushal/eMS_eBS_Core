using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class DesignDocumentMapViewModel : ViewModelBase
    {

        public int FormDesignID { get; set; }
        public int FormDesignTypeID { get; set; }
        public string FormDesignName { get; set; }
        public int FormDesignVersionID { get; set; }
        public string DisplayText { get; set; }
        public int TenantID { get; set; }
        public bool IsMasterList { get; set; }	
    }
}
