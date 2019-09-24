using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEPYSRCMap : EntityTypeConfiguration<SEPYSRC>
    {
        public SEPYSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID });

            // Properties
            this.Property(t => t.SEPY_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SEPY_EXP_CAT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPY_ACCT_CAT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPY_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE_ALT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_RULE_ALT_COND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);


            

            // Table & Column Mappings
            this.ToTable("SEPYSRC", "SRC");
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
            this.Property(t => t.SEPYHashCode).HasColumnName("SEPYHashCode");
        }
    }
}
