using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTLT481ArcMap : EntityTypeConfiguration<LTLT481Arc>
    {
        public LTLT481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.LTLT_Id);

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LTLT_DESC)
                .HasMaxLength(70);

            this.Property(t => t.LTLT_CAT)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LTLT_LEVEL)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LTLT_PERIOD_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LTLT_RULE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LTLT_IX_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LTLT_IX_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.LTLT_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LTLT_SAL_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("LTLT481", "arc_481");
            this.Property(t => t.LTLT_Id).HasColumnName("LTLT_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTLT_DESC).HasColumnName("LTLT_DESC");
            this.Property(t => t.LTLT_CAT).HasColumnName("LTLT_CAT");
            this.Property(t => t.LTLT_LEVEL).HasColumnName("LTLT_LEVEL");
            this.Property(t => t.LTLT_PERIOD_IND).HasColumnName("LTLT_PERIOD_IND");
            this.Property(t => t.LTLT_RULE).HasColumnName("LTLT_RULE");
            this.Property(t => t.LTLT_IX_IND).HasColumnName("LTLT_IX_IND");
            this.Property(t => t.LTLT_IX_TYPE).HasColumnName("LTLT_IX_TYPE");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.LTLT_AMT1).HasColumnName("LTLT_AMT1");
            this.Property(t => t.LTLT_AMT2).HasColumnName("LTLT_AMT2");
            this.Property(t => t.LTLT_OPTS).HasColumnName("LTLT_OPTS");
            this.Property(t => t.LTLT_SAL_IND).HasColumnName("LTLT_SAL_IND");
            this.Property(t => t.LTLT_DAYS).HasColumnName("LTLT_DAYS");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.LTLT_LOCK_TOKEN).HasColumnName("LTLT_LOCK_TOKEN");
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
