using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using tmg.equinox.repository.Models.Mapping;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.Base;
//using tmg.equinox.repository.Models.Mapping.PBP;

namespace tmg.equinox.repository
{
    public partial class ReportingCenterContext : BaseReportingCenterContext
    {

        public static ReportingCenterContext Create()
        {
            return new ReportingCenterContext();
        }
        public override void OnCustomModelCreating(DbModelBuilder modelBuilder)
        {

          
            #region Reports
            modelBuilder.Configurations.Add(new ProcessGovernanceMap());
            modelBuilder.Configurations.Add(new CCRTranslatorQueueMap());
            #endregion



        }
    }
}
