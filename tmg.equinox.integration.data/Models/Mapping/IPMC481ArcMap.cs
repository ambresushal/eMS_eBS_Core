using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class IPMC481ArcMap : EntityTypeConfiguration<IPMC481Arc>
    {
        public IPMC481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.IPMC_Id);

            // Properties
            this.Property(t => t.PDBC_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.IPCD_LOW)
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.IPCD_HIGH)
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.IPMC_UM_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("IPMC481", "arc_481");
            this.Property(t => t.IPMC_Id).HasColumnName("IPMC_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.IPCD_LOW).HasColumnName("IPCD_LOW");
            this.Property(t => t.IPMC_EFF_DT).HasColumnName("IPMC_EFF_DT");
            this.Property(t => t.IPMC_TERM_DT).HasColumnName("IPMC_TERM_DT");
            this.Property(t => t.IPCD_HIGH).HasColumnName("IPCD_HIGH");
            this.Property(t => t.IPMC_UM_IND).HasColumnName("IPMC_UM_IND");
            this.Property(t => t.IPMC_UM_AUTH_AMT).HasColumnName("IPMC_UM_AUTH_AMT");
            this.Property(t => t.IPMC_UM_UNITS_WAIVE).HasColumnName("IPMC_UM_UNITS_WAIVE");
            this.Property(t => t.IPMC_AUTH_WV_DAYS).HasColumnName("IPMC_AUTH_WV_DAYS");
            this.Property(t => t.IPMC_LOCK_TOKEN).HasColumnName("IPMC_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
