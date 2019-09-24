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
    internal class PlanObject : Workflow
    {
        private string _objectId { get; set; }
        private string _postUrl = "/services/data/v30.0/query/?q=SELECT+targetobjectid,id+from+ProcessInstance+WHERE+Status+=+'Pending'";
        private readonly RestClient _client;

        public PlanObject(string objectId)
        {
            this._objectId = objectId;
            _client = new RestClient(config.orgBaseUrl);
        }

        public string GetPlanObjectId(string token)
        {
            string processInstanceId = null;

            var request = RequestBuilder.CreateRequest(_postUrl, Method.GET, DataFormat.Json, string.Empty, token);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = _client.Execute(request);
            processInstanceId = this.ProcessResponse(response);

            return processInstanceId;

        }

        public override string ProcessResponse(IRestResponse response)
        {
            string result = string.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject data = JObject.Parse(response.Content);
                foreach (var item in data["records"])
                {
                    string modObjectId = "";
                    string targetObjectId = Convert.ToString(item["TargetObjectId"]);
                    if (targetObjectId.Length != _objectId.Length)
                    {
                        if (targetObjectId.Length > 15)
                        {
                            targetObjectId = targetObjectId.Substring(0, 15);
                        }
                        if (_objectId.Length > 15)
                        {
                            modObjectId = _objectId.Substring(0, 15);
                        }
                        else
                        {
                            modObjectId = _objectId;
                        }
                    }
                    else
                    {
                        modObjectId = _objectId;
                    }
                    if (targetObjectId == modObjectId)
                    {
                        result = Convert.ToString(item["Id"]);
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
