using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Xml;
using System.Xml.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.dependencyresolution;
using tmg.equinox.services.webapi.Framework;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Controllers
{
    [AllowAnonymous]
    public class ProductController : ApiController
    {
        public ProductController()
        {

        }

        [HttpGet]
        public IHttpActionResult Get([ModelBinder(typeof(tmg.equinox.services.webapi.Framework.Routing.ServiceRequestModelBinder))]ServiceRequestModel model)
        {
            if (ModelState.IsValid)
            {
                ServiceResponseGenerator generator = new ServiceResponseGenerator(model);
                ServiceResponse response = generator.GetResponse();
                if (!string.IsNullOrEmpty(response.Response))
                {
                    if (response.ResponseType == ResponseType.Json)
                    {
                        return Content(HttpStatusCode.OK, JObject.Parse(response.Response), GlobalConfiguration.Configuration.Formatters.JsonFormatter);
                    }
                    else
                    {
                        response.Response = response.Response.Replace("\"", "'").Replace("],}", "]}").Replace(",}", "}");

                        XmlDocument xdoc = JsonConvert.DeserializeXmlNode(response.Response, "root", true);
                        var formatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
                        formatter.UseXmlSerializer = true;
                        return Content(HttpStatusCode.OK, xdoc, formatter, "application/xml");
                    }
                }
                else
                    return NotFound();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
