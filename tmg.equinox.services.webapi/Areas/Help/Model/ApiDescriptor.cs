using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.services.webapi.Areas.Help.Model
{
    public class ApiDescriptor
    {
        public HttpMethod HttpMethod { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
        public string RelativePath { get; set; }
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public IHttpRoute Route { get; set; }
        public ICollection<ApiDescriptorParameter> Parameters { get; set; }
        public string RequestUrl { get; set; }
        public string Output { get; set; }
        public ResponseType ResponseType { get; set; }
    }
}