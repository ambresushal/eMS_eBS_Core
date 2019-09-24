using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
//using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.core.Models.Mapping;
using tmg.equinox.repository.Base;
//using tmg.equinox.repository.Models.Mapping.PBP;

namespace tmg.equinox.repository.core
{

    public partial class CoreFrameworkContext : BaseContext
    {
        static CoreFrameworkContext()
        {
            Database.SetInitializer<CoreFrameworkContext>(null);

        }

        public CoreFrameworkContext()
            : base("Name=UIFrameworkContext")
        {
            //var objectContext = (this as IObjectContextAdapter).ObjectContext;
            //objectContext.CommandTimeout = 180;

            ////Disable lazy loading & proxy creation        
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;


        }
       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new SettingDefinitionMap());

        }
    }

    /*
      public partial class CoreFrameworkContext : DbContext, IDbContextAsync
     {
         static CoreFrameworkContext()
         {
             Database.SetInitializer<CoreFrameworkContext>(null);

         }

         public CoreFrameworkContext()
             : base("Name=UIFrameworkContext")
         {
             var objectContext = (this as IObjectContextAdapter).ObjectContext;
             objectContext.CommandTimeout = 180;

             //Disable lazy loading & proxy creation        
             this.Configuration.LazyLoadingEnabled = false;
             this.Configuration.ProxyCreationEnabled = false;


         }
         //For DBConfiguration intializations
         public static CoreFrameworkContext Create()
         {
             return new CoreFrameworkContext();
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
         protected override void OnModelCreating(DbModelBuilder modelBuilder)
         {

             modelBuilder.Configurations.Add(new SettingDefinitionMap());


         }
     }
      */
}
