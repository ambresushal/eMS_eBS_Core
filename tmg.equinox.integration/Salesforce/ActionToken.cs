using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Helper;

namespace tmg.equinox.integration.Salesforce
{
    internal class ActionToken : Workflow
    {
        private readonly RestClient _client;
        public ActionToken()
        {
            _client = new RestClient(config.authBaseUrl);
        }

        public string GetActionToken()
        {
            string token = null;
            string postUrl = "/services/oauth2/token";

            var request = RequestBuilder.CreateTokenRequest(postUrl, Method.POST, DataFormat.Json, "application/x-www-form-urlencoded", config);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = _client.Execute(request);
            token = this.ProcessResponse(response);
            return token;
        }

        public override string ProcessResponse(IRestResponse response)
        {
            string result = string.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject data = JObject.Parse(response.Content);
                result = Convert.ToString(data["access_token"]);
            }
            else
            {
                throw new System.Net.HttpListenerException((int)response.StatusCode, response.ErrorMessage);
            }

            return result;
        }
    }
}
