using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tmg.equinox.services.webapi
{
    public static class UrlHelperExtensions
    {
        public static string AreaContent(this UrlHelper url, string path)
        {
            var area = url.RequestContext.RouteData.DataTokens["area"];

            if (area != null)
            {
                if (!string.IsNullOrEmpty(area.ToString()))
                    area = "Areas/" + area;

                // Simple checks for '~/' and '/' at the
                // beginning of the path.
                if (path.StartsWith("~/"))
                    path = path.Remove(0, 2);

                if (path.StartsWith("/"))
                    path = path.Remove(0, 1);

                path = path.Replace("../", string.Empty);

                return VirtualPathUtility.ToAbsolute("~/" + area + "/" + path);
            }

            return string.Empty;
        }
    }
}