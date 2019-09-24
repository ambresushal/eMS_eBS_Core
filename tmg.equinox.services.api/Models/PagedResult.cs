using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; }
        public PagingInfo Paging { get; set; }

        public PagedResult(IEnumerable<T> items, int pageNo, int pageSize, int totalCount)
        {
            Data = new List<T>(items);
            Paging = new PagingInfo()
            {
                PageNumber = pageNo,
                PageSize = pageSize,
                TotalRecordCount = totalCount,
                PageCount = totalCount > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0
            };
        }
    }

    public class PagingInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int TotalRecordCount { get; set; }
    }
}