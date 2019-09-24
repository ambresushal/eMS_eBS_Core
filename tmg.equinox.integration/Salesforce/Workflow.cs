using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.Configuration;

namespace tmg.equinox.integration.Salesforce
{
    public abstract class Workflow
    {
        public SalesForceSettings config;

        public Workflow()
        {
            config = SalesForceSettings.Settings();
        }

        public abstract string ProcessResponse(IRestResponse response);
    }
}
