using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.services.webapi.Models
{
    public class ServiceResponse
    {
        public string Response { get; set; }
        public ResponseType ResponseType { get; set; }
    }
}