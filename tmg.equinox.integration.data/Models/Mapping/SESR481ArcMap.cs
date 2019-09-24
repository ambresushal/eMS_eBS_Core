using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESR481ArcMap : EntityTypeConfiguration<SESR481Arc>
    {
        public SESR481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SESR_Id);

            // Properties
            this.Property(t => t.SERL_REL_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESR_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SESR481", "arc_481");
            this.Property(t => t.SESR_Id).HasColumnName("SESR_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESR_WT_CTR).HasColumnName("SESR_WT_CTR");
            this.Property(t => t.SESR_OPTS).HasColumnName("SESR_OPTS");
            this.Property(t => t.SESR_LOCK_TOKEN).HasColumnName("SESR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
