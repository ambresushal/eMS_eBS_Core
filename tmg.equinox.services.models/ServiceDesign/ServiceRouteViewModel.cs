using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign
{
    public class ServiceRouteViewModel
    {
        #region Public Properties
        public int? ServiceDesignId { get; set; }
        public string ServiceDesignName { get; set; }
        public string ServiceDesignMethodName { get; set; }
        public int ServiceDesignVersionId { get; set; }
        public string VersionNumber { get; set; }
        public int FormDesignID { get; set; }
        public string FormDesignName { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FormDesignVersionNumber { get; set; }
        public List<ServiceRouteParameterViewModel> ServiceParameterList { get; set; }
        #endregion Public Properties
    }
}
