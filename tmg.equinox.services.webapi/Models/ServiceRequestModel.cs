using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.services.webapi.Framework.Routing;

namespace tmg.equinox.services.webapi.Models
{
    [ModelBinder(typeof(ServiceRequestModelBinder))]
    public class ServiceRequestModel
    {
        public int FormInstanceID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public IList<ServiceRouteParameterViewModel> SearchParameterList { get; set; }
        public IDictionary<string, object> SearchParametersDictionary { get; set; }
        public int TenantID { get; set; }

        public ServiceRequestModel()
        {
            SearchParameterList = new List<ServiceRouteParameterViewModel>();
        }
    }
}