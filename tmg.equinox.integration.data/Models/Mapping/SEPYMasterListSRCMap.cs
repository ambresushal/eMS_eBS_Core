using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class SEPYMasterListSRCMap : EntityTypeConfiguration<SEPYMasterListSRC>
    {
        public SEPYMasterListSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID });

            // Table & Column Mappings
            this.ToTable("SEPYMasterList", "SRC");
            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.SEPY_EFF_DT).HasColumnName("SEPY_EFF_DT");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SEPY_TERM_DT).HasColumnName("SEPY_TERM_DT");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SEPY_EXP_CAT).HasColumnName("SEPY_EXP_CAT");
            this.Property(t => t.SEPY_ACCT_CAT).HasColumnName("SEPY_ACCT_CAT");
            this.Property(t => t.SEPY_OPTS).HasColumnName("SEPY_OPTS");
            this.Property(t => t.SESE_RULE_ALT).HasColumnName("SESE_RULE_ALT");
            this.Property(t => t.SESE_RULE_ALT_COND).HasColumnName("SESE_RULE_ALT_COND");
            this.Property(t => t.SEPY_LOCK_TOKEN).HasColumnName("SEPY_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
