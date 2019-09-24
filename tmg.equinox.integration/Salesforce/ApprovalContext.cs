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
    internal class ApprovalContext : Workflow
    {
        private string _instanceId { get; set; }
        private string _postUrl = "/services/data/v30.0/query/?q=SELECT+ProcessInstanceId+from+ProcessInstanceWorkitem";
        private readonly RestClient _client;

        public ApprovalContext(string instanceId)
        {
            this._instanceId = instanceId;
            _client = new RestClient(config.orgBaseUrl);
        }

        public string GetApprovalContextID(string token)
        {
            string contextid = null;

            var request = RequestBuilder.CreateRequest(_postUrl, Method.GET, DataFormat.Json, string.Empty, token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = _client.Execute(request);
            contextid = this.ProcessResponse(response);

            return contextid;
        }

        public override string ProcessResponse(IRestResponse response)
        {
            string result = string.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject data = JObject.Parse(response.Content);
                foreach (var item in data["records"])
                {
                    if (string.Equals(Convert.ToString(item["ProcessInstanceId"]), _instanceId))
                    {
                        string[] path = Convert.ToString(item["attributes"]["url"]).Split('/');
                        result = path[path.Length - 1].Substring(0, path[path.Length - 1].Length - 3);
                        break;
                    }
                }
            }
            else
            {
                throw new System.Net.HttpListenerException((int)response.StatusCode, response.ErrorMessage);
            }

            return result;
        }
    }
}
