using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class ServiceListDataMap : EntityTypeConfiguration<ServiceListData>
    {
        public ServiceListDataMap()
        {
            this.HasKey(t => new { t.SESE_ID, t.BenefitCategory1, t.BenefitCategory2, t.BenefitCategory3, t.PlaceOfService, t.ProcessGovernance1up });

            this.ToTable("ServiceListModelData", "ModelData");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.BenefitCategory1).HasColumnName("BenefitCategory1");
            this.Property(t => t.BenefitCategory2).HasColumnName("BenefitCategory2");
            this.Property(t => t.BenefitCategory3).HasColumnName("BenefitCategory3");
            this.Property(t => t.PlaceOfService).HasColumnName("PlaceOfService");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
