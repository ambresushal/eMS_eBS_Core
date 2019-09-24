using System.Web.Mvc;
using tmg.equinox.web.Controllers;
using tmg.equinox.web.Framework.ActionFilters;


namespace tmg.equinox.web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new PostAuthorizeAttribute());
            filters.Add(new NoCacheAttribute());
        }
    }
}