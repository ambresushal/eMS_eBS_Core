using System;
using System.Collections.Specialized;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using tmg.equinox.identitymanagement;
using tmg.equinox.identitymanagement.Models;
using System.Configuration;
using tmg.equinox.repository;


namespace tmg.equinox.web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        // Configure the UserManager
        public void ConfigureAuth(IAppBuilder app)
        {
            //
            UIFrameworkContext.Create();
            ReportingCenterContext.Create();


            app.CreatePerOwinContext<SecurityDbContext>(SecurityDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);


            CookieAuthenticationOptions cookieAuthenticationOptions = GetOwinCookieProperties();
            app.UseCookieAuthentication(cookieAuthenticationOptions);

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            // app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            HangfireConfig.Config(app);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }

        private CookieAuthenticationOptions GetOwinCookieProperties()
        {
            NameValueCollection owinAuthenticationConfigurationSection = (NameValueCollection)
                ConfigurationManager.GetSection("owinAuthentication");

            CookieAuthenticationOptions cookieAuthentication = new CookieAuthenticationOptions();

            string caseSwitch = owinAuthenticationConfigurationSection["CookieSecure"];
            switch (caseSwitch)
            {
                case "SameAsRequest": cookieAuthentication.CookieSecure = CookieSecureOption.SameAsRequest;
                    break;
                case "Never": cookieAuthentication.CookieSecure = CookieSecureOption.Never;
                    break;
                case "Always": cookieAuthentication.CookieSecure = CookieSecureOption.Always;
                    break;
            }

            CookieAuthenticationOptions cookieAuthenticationOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                //LoginPath = new PathString(owinAuthenticationConfigurationSection["LoginUrl"]),
                SlidingExpiration =Convert.ToBoolean(owinAuthenticationConfigurationSection["SlidingExpiration"]),
                ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(owinAuthenticationConfigurationSection["TimeOut"])),
                CookieSecure = cookieAuthentication.CookieSecure,
                CookieName = owinAuthenticationConfigurationSection["CookieName"]
                //Provider = new CookieAuthenticationProvider
                //{
                //    // Use the callback method to refresh the cookie after certain time period
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
                //         validateInterval: TimeSpan.FromMinutes(20),
                //         regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                //         getUserIdCallback: (id) => (Int32.Parse(id.GetUserId())))
                //}
            };

            return cookieAuthenticationOptions;
        }
    }
}