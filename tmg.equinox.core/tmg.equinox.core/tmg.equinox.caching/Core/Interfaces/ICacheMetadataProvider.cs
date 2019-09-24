using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching.Interfaces
{
    public interface ICacheMetadataProvider
    {
        IDictionary<string, long> GetDomainSizes();
        CacheItemMetadata GetEarliestAccessedItem(string domain);
        CacheItemMetadata GetEarliestAccessedItem();
    }
}
