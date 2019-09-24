using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class BSDEDataMap : EntityTypeConfiguration<BSDEData>
    {
        public BSDEDataMap()
        {
            this.HasKey(t => new { t.BSDE_REC_TYPE,t.BSDE_TYPE,t.BSDE_DESC,t.ProcessGovernance1up });

            this.ToTable("BSDEModelData", "ModelData");
            this.Property(t => t.BSDE_REC_TYPE).HasColumnName("BSDE_REC_TYPE");
            this.Property(t => t.BSDE_TYPE).HasColumnName("BSDE_TYPE");
            this.Property(t => t.BSDE_DESC).HasColumnName("BSDE_DESC");
            this.Property(t => t.BSDE_KEYWORD1).HasColumnName("BSDE_KEYWORD1");
            this.Property(t => t.BSDE_KEYWORD2).HasColumnName("BSDE_KEYWORD2");
            this.Property(t => t.BSDE_KEYWORD3).HasColumnName("BSDE_KEYWORD3");
            this.Property(t => t.BSDE_KEYWORD4).HasColumnName("BSDE_KEYWORD4");
            this.Property(t => t.BSDE_KEYWORD5).HasColumnName("BSDE_KEYWORD5");
            this.Property(t => t.BSDE_KEYWORD6).HasColumnName("BSDE_KEYWORD6");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
