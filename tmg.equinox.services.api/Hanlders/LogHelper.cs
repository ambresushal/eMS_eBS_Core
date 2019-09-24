using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.WebApi;

namespace tmg.equinox.services.api.Hanlders
{
    public class LogHelper
    {
        private IApiActivityLogService _service;

        public LogHelper()
        {
            _service = UnityConfig.Resolve<IApiActivityLogService>();
        }
        public static ActivityLogViewModel CreateApiLogEntryWithRequestData(HttpRequestMessage request)
        {
            HttpContextBase context = null ;
            Microsoft.Owin.OwinContext owinContext = null; 
            foreach (var contex in  request.Properties)
            {
                if (contex.Key == "MS_HttpContext")
                {
                    context = ((HttpContextBase)contex.Value);
                }
                else if (contex.Key == "MS_OwinContext")
                {
                    owinContext = (Microsoft.Owin.OwinContext)contex.Value;
                }
            }
            
            var routeData = request.GetRouteData();

            return new ActivityLogViewModel
            {
                UserName = context.User.Identity.Name,
                Machine = Environment.MachineName,
                RequestContentType = context.Request.ContentType,
                // RequestRouteData = JsonConvert.SerializeObject(routeData, Formatting.Indented),
                RequestIpAddress = context.Request.UserHostAddress,
                RequestMethod = request.Method.Method,
                RequestHeaders = SerializeHeaders(request.Headers),
                RequestDateTime = DateTime.Now,
                RequestUri = request.RequestUri.ToString()
            };
        }

        public static string SerializeHeaders(HttpHeaders headers)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                foreach (var item in headers.ToList())
                {
                    if (item.Value != null)
                    {
                        var header = String.Empty;
                        foreach (var value in item.Value)
                        {
                            header += value + " ";
                        }

                        // Trim the trailing space and add item to the dictionary
                        header = header.TrimEnd(" ".ToCharArray());
                        dict.Add(item.Key, header);
                    }
                }

                return JsonConvert.SerializeObject(dict, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        Error = delegate (object sender, ErrorEventArgs args)
                          {
                              args.ErrorContext.Handled = true;
                          }
                    });
            }
            catch (Exception ex)
            {
                //Special value in header unable to convert into json format.                
                return "";
            }
        }

        public void InsertLog(ActivityLogViewModel model)
        {
            _service.Insert(model);
        }
    }
}