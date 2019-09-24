using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTSE481ArcMap : EntityTypeConfiguration<LTSE481Arc>
    {
        public LTSE481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.LTSE_Id);

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
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
            this.ToTable("LTSE481", "arc_481");
            this.Property(t => t.LTSE_Id).HasColumnName("LTSE_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.LTSE_WT_CTR).HasColumnName("LTSE_WT_CTR");
            this.Property(t => t.LTSE_LOCK_TOKEN).HasColumnName("LTSE_LOCK_TOKEN");
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
