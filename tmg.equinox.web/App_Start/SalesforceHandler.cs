using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using tmg.equinox.web.Salesforce;

namespace tmg.equinox.web.App_Start
{
    public class SalesforceHandler : WebHookHandler
    {
        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            SalesforceNotifications dataOrDefault = context.GetDataOrDefault<SalesforceNotifications>();
            string sessionId = dataOrDefault.SessionId;
            this.processSalesForceRequest(dataOrDefault);
            return Task.FromResult<bool>(true);
        }

        private void processSalesForceRequest(SalesforceNotifications updates)
        {
            Notification objNotification = Notification.GetNotification(updates.Document);
            JToken first = JToken.FromObject(objNotification);

            FolderManager folderManager = new FolderManager();
            folderManager.CreateFolder(first);
        }
    }
}