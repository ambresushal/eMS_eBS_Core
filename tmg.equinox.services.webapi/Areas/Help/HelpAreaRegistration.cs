using System.Web.Http;
using System.Web.Mvc;

namespace tmg.equinox.services.webapi.Areas.HelpPage
{
    public class HelpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Help";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Help_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", AreaName = "Help", apiId = UrlParameter.Optional }, new { name = @"[^\.]*" });

            //context.MapRoute(
            //    "Help_Default",
            //    "Help/Index",
            //    new { controller = "Help", action = "Index", AreaName = "Help" });
        }
    }
}