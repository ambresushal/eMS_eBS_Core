using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using tmg.equinox.identitymanagement;
using System.Collections.Generic;

namespace tmg.equinox.web.Framework.ActionFilters
{
    public class PostAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool authorize = true;
            if (filterContext.HttpContext.Request.IsLocal == true)
            {
                string absolutePath = "";
                if (filterContext.HttpContext.Request.UrlReferrer != null) {
                    absolutePath = filterContext.HttpContext.Request.UrlReferrer.AbsolutePath;
                }
                authorize = BypassAuthForLocal(absolutePath);
            }
            if (authorize == true) 
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    IdentityManager.ExecutePostAuthenticate(authCookie);
                }
            }
        }

        //For PDF Reports generated on Server through Webkit
        //need to bypass authorization for local requests
        private bool BypassAuthForLocal(string absolutePath) 
        {
            bool authorize = true;
            if (absolutePath == "/FolderVersion/PreviewFormInstance" || absolutePath == "/FolderVersion/PreviewAllInstances") {
                authorize = false;
            }
            return authorize;
        }
    }
}