using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class SERLDataMap : EntityTypeConfiguration<SERLData>
    {
        public SERLDataMap()
        {
            this.HasKey(t => new { t.SERL_REL_ID, t.SERL_REL_TYPE, t.SERL_DESC, t.ProcessGovernance1up });

            this.ToTable("SERLModelData", "ModelData");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SERL_REL_TYPE).HasColumnName("SERL_REL_TYPE");
            this.Property(t => t.SERL_DESC).HasColumnName("SERL_DESC");
            this.Property(t => t.SERL_REL_PER_IND).HasColumnName("SERL_REL_PER_IND");
            this.Property(t => t.SERL_DIAG_IND).HasColumnName("SERL_DIAG_IND");
            this.Property(t => t.SERL_NTWK_IND).HasColumnName("SERL_NTWK_IND");
            this.Property(t => t.SERL_PC_IND).HasColumnName("SERL_PC_IND");
            this.Property(t => t.SERL_REF_IND).HasColumnName("SERL_REF_IND");
            this.Property(t => t.SERL_PER).HasColumnName("SERL_PER");
            this.Property(t => t.SERL_COPAY_IND).HasColumnName("SERL_COPAY_IND");
            this.Property(t => t.SERL_OPTS).HasColumnName("SERL_OPTS");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
