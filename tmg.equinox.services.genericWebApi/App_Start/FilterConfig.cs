using System.Web;
using System.Web.Mvc;

namespace tmg.equinox.services.genericWebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
