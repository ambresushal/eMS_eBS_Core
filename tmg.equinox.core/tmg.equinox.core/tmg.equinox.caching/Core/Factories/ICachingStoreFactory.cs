using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.caching.Interfaces;

namespace tmg.equinox.caching
{
    public interface ICachingStoreFactory
    {
        ICachingStore CreateCachingStore(CachingSetting cachingSetting);
    }
}
