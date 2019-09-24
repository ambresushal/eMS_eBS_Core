using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class ACDEDataMap : EntityTypeConfiguration<ACDEData>
    {
        public ACDEDataMap()
        {
            this.HasKey(t => new { t.ACAC_ACC_NO, t.ACDE_DESC, t.ProcessGovernance1up });

            this.ToTable("ACDEModelData", "ModelData");
            this.Property(t => t.ACDE_ACC_TYPE).HasColumnName("ACDE_ACC_TYPE");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.Acronym).HasColumnName("Acronym");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
