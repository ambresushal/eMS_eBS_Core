using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class SEPYDataMap:EntityTypeConfiguration<SEPYData>
    {
        public SEPYDataMap()
        {
            this.HasKey(t => t.SEPY1Up);
            this.ToTable("SEPYModelData", "ModelData");
            this.Property(t => t.SEPY1Up).HasColumnName("SEPY1Up");
            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.SEPYHashCode).HasColumnName("SEPYHashCode");
            this.Property(t => t.SEPY_EFF_DT).HasColumnName("SEPY_EFF_DT");
            this.Property(t => t.SEPY_TERM_DT).HasColumnName("SEPY_TERM_DT");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SEPY_EXP_CAT).HasColumnName("SEPY_EXP_CAT");
            this.Property(t => t.SEPY_ACCT_CAT).HasColumnName("SEPY_ACCT_CAT");
            this.Property(t => t.SEPY_OPTS).HasColumnName("SEPY_OPTS");
            this.Property(t => t.SESE_RULE_ALT).HasColumnName("SESE_RULE_ALT");
            this.Property(t => t.SESE_RULE_ALT_COND).HasColumnName("SESE_RULE_ALT_COND");
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");

        }
    }
}
