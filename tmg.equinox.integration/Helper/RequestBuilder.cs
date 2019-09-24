using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Configuration;

namespace tmg.equinox.integration.Helper
{
    public static class RequestBuilder
    {
        public static RestRequest CreateTokenRequest(string requestUrl, Method method, DataFormat format, string contentType, SalesForceSettings settings)
        {
            var request = new RestRequest(requestUrl, method) { RequestFormat = format };

            //Add request headers
            request.AddHeader("Content-Type", contentType);

            //Add request parameters
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", settings.consumerKey);
            request.AddParameter("client_secret", settings.consumerSecret);
            request.AddParameter("username", settings.userName);
            request.AddParameter("password", settings.password + settings.securityToken);

            return request;
        }

        public static RestRequest CreateRequest(string requestUrl, Method method, DataFormat format, string contentType, string token)
        {
            var request = new RestRequest(requestUrl, method) { RequestFormat = format };
            request.AddHeader("Authorization", token);

            //Add request headers
            if (!string.IsNullOrEmpty(contentType))
                request.AddHeader("Content-Type", contentType);

            return request;
        }
    }
}
