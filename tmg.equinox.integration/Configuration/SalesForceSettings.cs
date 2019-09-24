using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.Configuration
{
    public class SalesForceSettings : ConfigurationSection
    {
        private static SalesForceSettings settings = ConfigurationManager.GetSection("SalesForceSettings") as SalesForceSettings;
        public static SalesForceSettings Settings()
        {
            return settings;
        }

        [ConfigurationProperty("authBaseUrl", IsRequired = true)]
        public string authBaseUrl
        {
            get { return (string)this["authBaseUrl"]; }
            set { this["authBaseUrl"] = value; }
        }
        [ConfigurationProperty("orgBaseUrl", IsRequired = true)]
        public string orgBaseUrl
        {
            get { return (string)this["orgBaseUrl"]; }
            set { this["orgBaseUrl"] = value; }
        }
        [ConfigurationProperty("consumerKey", IsRequired = true)]
        public string consumerKey
        {
            get { return (string)this["consumerKey"]; }
            set { this["consumerKey"] = value; }
        }

        [ConfigurationProperty("consumerSecret", IsRequired = true)]
        public string consumerSecret
        {
            get { return (string)this["consumerSecret"]; }
            set { this["consumerSecret"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = true)]
        public string userName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("securityToken", IsRequired = true)]
        public string securityToken
        {
            get { return (string)this["securityToken"]; }
            set { this["securityToken"] = value; }
        }

        [ConfigurationProperty("approverId", IsRequired = true)]
        public string approverId
        {
            get { return (string)this["approverId"]; }
            set { this["approverId"] = value; }
        }
    }
}
