using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework
{
    public interface IApiResponseBuilder
    {
        ServiceResponse GetResponse(ServiceRequestModel model);
    }
}