using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using tmg.equinox.repository.Base;
using tmg.equinox.repository.Models.Mapping;
//using tmg.equinox.repository.Models.Mapping.PBP;

namespace tmg.equinox.repository
{
    public partial class BaseReportingCenterContext : BaseContext
    {
        static BaseReportingCenterContext()
        {
            Database.SetInitializer<BaseReportingCenterContext>(null);

        }

        public BaseReportingCenterContext()
            : base(string.Format("Name=ReportingCenterContext"))
        {
            //var objectContext = (this as IObjectContextAdapter).ObjectContext;
            //objectContext.CommandTimeout = 180;

            ////Disable lazy loading & proxy creation        
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;


        }
        //For DBConfiguration intializations
        //public static UIFrameworkContext Create()
        //{
        //    return new UIFrameworkContext();
        //}

       
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

          
            #region Reports
            modelBuilder.Configurations.Add(new ReportSettingMap());
            modelBuilder.Configurations.Add(new ReportQueueMap());
            modelBuilder.Configurations.Add(new ReportQueueDetailMap());
            modelBuilder.Configurations.Add(new ReportingTableColumnInfoMap());
            modelBuilder.Configurations.Add(new ReportingTableInfoMap());
            modelBuilder.Configurations.Add(new SchemaVersionActivityLogMap());
            #endregion

            OnCustomModelCreating(modelBuilder);

        }

        public virtual void OnCustomModelCreating(DbModelBuilder modelBuilder)
        {

        }

    }
}
