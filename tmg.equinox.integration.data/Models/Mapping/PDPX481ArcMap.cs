using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPX481ArcMap : EntityTypeConfiguration<PDPX481Arc>
    {
        public PDPX481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.PDPX_Id);

            // Properties
            this.Property(t => t.PDBC_PFX)
                .HasMaxLength(255);

            this.Property(t => t.PDBC_TYPE)
                .HasMaxLength(255);

            this.Property(t => t.PDPX_DESC)
                .HasMaxLength(255);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("PDPX481", "arc_481");
            this.Property(t => t.PDPX_Id).HasColumnName("PDPX_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDPX_DESC).HasColumnName("PDPX_DESC");
            this.Property(t => t.PDPX_LOCK_TOKEN).HasColumnName("PDPX_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
