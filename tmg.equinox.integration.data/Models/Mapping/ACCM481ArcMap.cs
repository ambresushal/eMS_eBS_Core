using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ACCM481ArcMap : EntityTypeConfiguration<ACCM481Arc>
    {
        public ACCM481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.BatchID);

            // Properties
            this.Property(t => t.PDPD_ID)
                .HasMaxLength(255);

            this.Property(t => t.ACCM_TYPE)
                .HasMaxLength(255);

            this.Property(t => t.ACCM_DESC)
                .HasMaxLength(255);

            this.Property(t => t.ACCM_PFX)
                .HasMaxLength(255);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ACCM481", "arc_481");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.ACCM_TYPE).HasColumnName("ACCM_TYPE");
            this.Property(t => t.ACCM_EFF_DT).HasColumnName("ACCM_EFF_DT");
            this.Property(t => t.ACCM_SEQ_NO).HasColumnName("ACCM_SEQ_NO");
            this.Property(t => t.ACCM_TERM_DT).HasColumnName("ACCM_TERM_DT");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACCM_DESC).HasColumnName("ACCM_DESC");
            this.Property(t => t.ACCM_PFX).HasColumnName("ACCM_PFX");
            this.Property(t => t.ACCM_LOCK_TOKEN).HasColumnName("ACCM_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
