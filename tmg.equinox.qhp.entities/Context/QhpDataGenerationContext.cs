using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using tmg.equinox.qhp.entities.Entities.Models;

using tmg.equinox.qhp.entities.Entities.Models.Mapping;


namespace tmg.equinox.qhp.entities.Context
{
    public partial class QhpDataGenerationContext : DbContext
    {
        #region Private Memebers
        #endregion Private Members

        #region Constructor
        static QhpDataGenerationContext()
        {
            Database.SetInitializer<QhpDataGenerationContext>(null);
        }

        public QhpDataGenerationContext()
            : base("Name=UIFrameworkContext")
        {
            //Disable lazy loading & proxy creation        
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        #endregion Constructor
        

        #region Public Properties
        public IDbSet<DataMap> QhpMappingSet { get; set; }
        public IDbSet<QhpGenerationActivityLog> QhpGenerationActivityLogSet { get; set; }
        #endregion Public Properties

        #region Public\Protected Methods

        //For DBConfiguration intializations
        public static QhpDataGenerationContext Create()
        {
            return new QhpDataGenerationContext();
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DataMapMap());
            modelBuilder.Configurations.Add(new QhpGenerationActivityLogMap());
        }
        #endregion Public Methods

        #region Private Methods
        #endregion Private Methods
    }
}
