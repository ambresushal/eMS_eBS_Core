using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SETR481ArcMap : EntityTypeConfiguration<SETR481Arc>
    {
        public SETR481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SETR_Id);

            // Properties
            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SETR_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SETR481", "arc_481");
            this.Property(t => t.SETR_Id).HasColumnName("SETR_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SETR_TIER_NO).HasColumnName("SETR_TIER_NO");
            this.Property(t => t.SETR_ALLOW_AMT).HasColumnName("SETR_ALLOW_AMT");
            this.Property(t => t.SETR_ALLOW_CTR).HasColumnName("SETR_ALLOW_CTR");
            this.Property(t => t.SETR_COPAY_AMT).HasColumnName("SETR_COPAY_AMT");
            this.Property(t => t.SETR_COIN_PCT).HasColumnName("SETR_COIN_PCT");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SETR_OPTS).HasColumnName("SETR_OPTS");
            this.Property(t => t.SETR_LOCK_TOKEN).HasColumnName("SETR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
