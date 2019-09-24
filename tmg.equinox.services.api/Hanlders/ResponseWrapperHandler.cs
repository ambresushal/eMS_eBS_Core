using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.Hanlders
{
    public class ResponseWrapperHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsSwagger(request))
            {
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            else
            {
                var apiLogEntry = LogHelper.CreateApiLogEntryWithRequestData(request);

                var response = await base.SendAsync(request, cancellationToken);
                
                if(response.Content != null && response.Content.Headers.ContentType.MediaType.Equals("application/octet-stream"))
                {
                    return response;
                }
                else if (response.Content != null)
                {
                    //apiLogEntry.ResponseContentBody = response.Content.ReadAsStringAsync().Result;
                    apiLogEntry.ResponseContentType = response.Content.Headers.ContentType.MediaType;
                    apiLogEntry.ResponseHeaders = LogHelper.SerializeHeaders(response.Content.Headers);
                    apiLogEntry.ResponseStatusCode = (int)response.StatusCode;
                    apiLogEntry.ResponseDateTime = DateTime.Now;
                    LogHelper logHelper = new LogHelper();
                    //logHelper.InsertLog(apiLogEntry);
                }

                return BuildApiResponse(request, response);
            }
        }

        private HttpResponseMessage BuildApiResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            object content;
            HttpError error = null;
            List<string> modelStateErrors = new List<string>();

            if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                error = content as HttpError;

                if (error != null)
                {
                    content = null;

                    var httpErrorObject = response.Content.ReadAsStringAsync().Result;

                    var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };

                    var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject,
                        new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                args.ErrorContext.Handled = true;
                            }
                        });

                    if (error.ModelState != null)
                    {
                        var modelStateValues = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));

                        for (int i = 0; i < modelStateValues.Count(); i++)
                        {
                            modelStateErrors.Add(modelStateValues.ElementAt(i));
                        }
                    }
                    else
                    {
                        modelStateErrors.Add(deserializedErrorObject.message);
                    }
                }
            }

            var newResponse = request.CreateResponse(response.StatusCode, new ResponseResult(content, modelStateErrors));

            foreach (var header in response.Headers) //Add back the response headers
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }

            return newResponse;
        }

        private bool skipLoggingAction(HttpRequestMessage request)
        {
            bool isSkip = IsSwagger(request);
            if (isSkip==false)
            {
                return IsOwinTest(request);
            }
            return isSkip;
        }
        private bool IsSwagger(HttpRequestMessage request)
        {
            return request.RequestUri.PathAndQuery.StartsWith("/swagger");            
        }

        private bool IsOwinTest(HttpRequestMessage request)
        {
            foreach (var contex in request.Properties)
            {
                if (contex.Key == "MS_OwinContext")
                {
                    return true;
                }
            }
            return false;
        }

    }
}