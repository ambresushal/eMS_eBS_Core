using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEPYTRANSMap : EntityTypeConfiguration<SEPYTRANS>
    {
        public SEPYTRANSMap()
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

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SEPYTRANS", "Trans");
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
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
