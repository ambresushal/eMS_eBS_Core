using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Helper;
using tmg.equinox.integration.Model;

namespace tmg.equinox.integration.Salesforce
{
    public class WorkFlowProcessor : Workflow
    {
        private string _objectId { get; set; }
        private string _comments { get; set; }
        private string _postUrl = "/services/data/v30.0/process/approvals/";
        private readonly RestClient _client;

        public WorkFlowProcessor(string objectId, string comments)
        {
            this._objectId = objectId;
            this._comments = comments;
            _client = new RestClient(config.orgBaseUrl);
        }

        public bool ApproveWorkflow()
        {
            bool isApproved = false;

            ActionToken objToken = new ActionToken();
            string token = objToken.GetActionToken();

            if (!string.IsNullOrEmpty(token))
            {
                string accesstoken = ("Bearer " + token);

                PlanObject objPlanObject = new PlanObject(_objectId);
                string instanceID = objPlanObject.GetPlanObjectId(accesstoken);

                if (!string.IsNullOrEmpty(instanceID))
                {
                    ApprovalContext objContext = new ApprovalContext(instanceID);
                    string contextid = objContext.GetApprovalContextID(accesstoken);

                    if (!string.IsNullOrEmpty(contextid))
                    {
                        var request = RequestBuilder.CreateRequest(_postUrl, Method.GET, DataFormat.Json, "application/json", accesstoken);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        RootObject root = new RootObject();
                        root.requests = new List<Request>();
                        root.requests.Add(new Request() { actionType = "Approve", contextId = contextid, nextApproverIds = new List<string>() { config.approverId }, comments = _comments });
                        string body = JsonConvert.SerializeObject(root);
                        request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);

                        var response = _client.Execute(request);

                        if (!string.IsNullOrEmpty(this.ProcessResponse(response)))
                        {
                            isApproved = true;
                        }
                    }
                }
            }
            return isApproved;
        }

        public override string ProcessResponse(IRestResponse response)
        {
            string result = string.Empty;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = "true";
            }

            return result;
        }
    }
}
