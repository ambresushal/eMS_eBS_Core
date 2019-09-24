using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.web.App_Start
{
    public class StartupHelper
    {
        public static Dictionary<string, string> Urls = new Dictionary<string, string>();

        static StartupHelper()
        {
            Urls.Add("Dashboard", "DashBoard/Index");
            Urls.Add("Portfolio Search", "ConsumerAccount/PortfolioSearch");
            Urls.Add("Account Search", "ConsumerAccount/AccountSearch");
        }

        public static void GetUrl(string key, out string action, out string controller)
        {
            action = string.Empty;
            controller = string.Empty;
            string value = string.Empty;
            Urls.TryGetValue(key, out value);
            if (!string.IsNullOrEmpty(value))
            {
                string[] parts = value.Split('/');
                action = parts[1];
                controller = parts[0];
            }
        }
    }
}