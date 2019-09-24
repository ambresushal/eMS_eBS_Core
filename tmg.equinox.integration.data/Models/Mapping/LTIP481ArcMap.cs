using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTIP481ArcMap : EntityTypeConfiguration<LTIP481Arc>
    {
        public LTIP481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.LTPI_Id);

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LTIP_IPCD_ID_LOW)
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.LTIP_IPCD_ID_HIGH)
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("LTIP481", "arc_481");
            this.Property(t => t.LTPI_Id).HasColumnName("LTPI_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTIP_IPCD_ID_LOW).HasColumnName("LTIP_IPCD_ID_LOW");
            this.Property(t => t.LTIP_IPCD_ID_HIGH).HasColumnName("LTIP_IPCD_ID_HIGH");
            this.Property(t => t.LTIP_LOCK_TOKEN).HasColumnName("LTIP_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
