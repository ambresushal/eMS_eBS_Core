using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class BenefitSetDataMap : EntityTypeConfiguration<BenefitSet>
    {
        public BenefitSetDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSetName, t.ProcessGovernance1up });
            // Table & Column Mappings
            this.ToTable("BenefitSetModelData", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSetName).HasColumnName("BenefitSet");
            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.SEPY_PFX_DESC).HasColumnName("SEPY_PFX_DESC");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.LTLT_PFX_DESC).HasColumnName("LTLT_PFX_DESC");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.CreateSEPY).HasColumnName("CreateSEPY");
        }
    }
}
