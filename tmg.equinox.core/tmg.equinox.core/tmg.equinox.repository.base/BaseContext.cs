using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tmg.equinox.repository.Base
{
    public partial class BaseContext : DbContext, IDbContextAsync
    {
     

        public BaseContext(string contextName)
            : base(contextName)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 180;

            //Disable lazy loading & proxy creation        
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;


        }
       

        public IQueryable<T> Table<T>() where T : class
        {
            return this.Set<T>();
        }
        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            this.ApplyStateChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var changesAsync = await base.SaveChangesAsync(cancellationToken);

            return changesAsync;
        }
       
    }
}
