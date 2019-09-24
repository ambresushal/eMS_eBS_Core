using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCache;
namespace tmg.equinox.repository
{
    //public class EfConfiguration : DbConfiguration
    //{
    //    public EfConfiguration()
    //    {
    //        var transactionHandler = new CacheTransactionHandler(new InMemoryCache());
    //        AddInterceptor(transactionHandler);
    //        Loaded += (sender, args) => args.ReplaceService<DbProviderServices>((s, _) => new CachingProviderServices(s, transactionHandler));
    //    }
    //}
}
