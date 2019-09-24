using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace tmg.equinox.caching.Interfaces
{
    /// <summary>
    /// Wrapper interface providing  more specific contracts for HTTPcaching
    /// </summary>
    public interface IHttpCahingStore : ICachingStore
    {
         void SetCache(string key, object cachedata, CacheDependency dependencies);
    }
}
