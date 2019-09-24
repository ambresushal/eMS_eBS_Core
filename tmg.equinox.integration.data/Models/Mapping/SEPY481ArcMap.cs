using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEPY481ArcMap : EntityTypeConfiguration<SEPY481Arc>
    {
        public SEPY481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SEPY_Id);

            // Properties
            this.Property(t => t.SEPY_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SEPY_EXP_CAT)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPY_ACCT_CAT)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPY_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE_ALT)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESE_RULE_ALT_COND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SEPY481", "arc_481");
            this.Property(t => t.SEPY_Id).HasColumnName("SEPY_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
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
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
