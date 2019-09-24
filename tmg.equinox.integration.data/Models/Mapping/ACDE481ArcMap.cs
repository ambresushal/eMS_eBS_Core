using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ACDE481ArcMap : EntityTypeConfiguration<ACDE481Arc>
    {
        public ACDE481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.ACDE_Id);

            // Properties
            this.Property(t => t.ACDE_ACC_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ACDE_DESC)
                .IsFixedLength()
                .HasMaxLength(70);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ACDE481", "arc_481");
            this.Property(t => t.ACDE_Id).HasColumnName("ACDE_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.ACDE_ACC_TYPE).HasColumnName("ACDE_ACC_TYPE");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.ACDE_LOCK_TOKEN).HasColumnName("ACDE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
