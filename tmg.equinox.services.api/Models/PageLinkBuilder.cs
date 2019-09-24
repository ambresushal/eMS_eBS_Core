using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;

namespace tmg.equinox.services.api.Models
{
    public class PageLinkBuilder
    {
        public Uri FirstPage { get; private set; }
        public Uri LastPage { get; private set; }
        public Uri NextPage { get; private set; }
        public Uri PreviousPage { get; private set; }

        public PageLinkBuilder(UrlHelper urlHelper, string routeName, object routeValues, int pageNo, int pageSize, long totalRecordCount)
        {
            // Determine total number of pages
            var pageCount = totalRecordCount > 0 ? (int)Math.Ceiling(totalRecordCount / (double)pageSize) : 0;

            string link = urlHelper.Request.RequestUri.ToString();

            // Create them page links 
            FirstPage = new Uri(link + "?pageNo=1&pageSize=" + pageSize);
            LastPage = new Uri(link + "?pageNo=" + pageCount + "&pageSize=" + pageSize);

            if (pageNo > 1)
            {
                PreviousPage = new Uri(link + "?pageNo=" + (pageNo - 1) + "&pageSize=" + pageSize);
            }

            if (pageNo < pageCount)
            {
                NextPage = new Uri(link + "?pageNo=" + (pageNo + 1) + "&pageSize=" + pageSize);
            }
        }

        public static string GetPageLink(PageLinkBuilder linkBuilder)
        {
            string LinkHeaderTemplate = "<{0}>; rel=\"{1}\"";

            List<string> links = new List<string>();
            if (linkBuilder.FirstPage != null)
                links.Add(string.Format(LinkHeaderTemplate, linkBuilder.FirstPage, "first"));
            if (linkBuilder.PreviousPage != null)
                links.Add(string.Format(LinkHeaderTemplate, linkBuilder.PreviousPage, "previous"));
            if (linkBuilder.NextPage != null)
                links.Add(string.Format(LinkHeaderTemplate, linkBuilder.NextPage, "next"));
            if (linkBuilder.LastPage != null)
                links.Add(string.Format(LinkHeaderTemplate, linkBuilder.LastPage, "last"));

            return string.Join(", ", links);
        }
    }
}