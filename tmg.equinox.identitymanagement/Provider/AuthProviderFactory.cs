using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.identitymanagement.APIProvider;

namespace tmg.equinox.identitymanagement.Provider
{
    public class AuthProviderFactory
    {
        public static bool IsADAuthentication()
        {
            NameValueCollection authenticationSectionCollection = (NameValueCollection)ConfigurationManager.GetSection("authenticationSection");
            return Convert.ToBoolean(authenticationSectionCollection["UseADAuthentication"]);
        }

        public static bool IsSOActiveForAD()
        {
            NameValueCollection authenticationSectionCollection = (NameValueCollection)ConfigurationManager.GetSection("authenticationSection");
            return Convert.ToBoolean(authenticationSectionCollection["UseSSOForADAuthentication"]);
        }
        public static IAuthProvider Get(bool owinAuthenticationEnabled)
        {
            if (IsADAuthentication())
            {
                return new ActiveDirectoryAuthProvider(); //for Web with AD +  Windows
            }
            return new AuthProvider(owinAuthenticationEnabled); //for web normal form based
        }
        public static IADAuthProvider Get()
        {
            return new ActiveDirectoryAuthProvider(); //used by web & web API both
        }
        public static BaseAuthorizationServerProvider Get(string publicClientId)
        {
            if (IsADAuthentication())
            {
                return new ADAuthenticationProvider(publicClientId); //for web API
            }
            return new AuthenticationProvider(publicClientId); ////for WEB API without AD
        }

    }
}
