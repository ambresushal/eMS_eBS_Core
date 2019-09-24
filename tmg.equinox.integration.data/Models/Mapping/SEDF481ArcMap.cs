using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEDF481ArcMap : EntityTypeConfiguration<SEDF481Arc>
    {
        public SEDF481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SEDF_Id);

            // Properties
            this.Property(t => t.PDBC_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEPC_PRICE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SEDF_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_UM_REQ_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_UM_BP_REF_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_RWH_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_CAP_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SEDF_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SEDF481", "arc_481");
            this.Property(t => t.SEDF_Id).HasColumnName("SEDF_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.SEDF_EFF_DT).HasColumnName("SEDF_EFF_DT");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SEDF_TERM_DT).HasColumnName("SEDF_TERM_DT");
            this.Property(t => t.SEPC_PRICE_ID).HasColumnName("SEPC_PRICE_ID");
            this.Property(t => t.SEDF_TYPE).HasColumnName("SEDF_TYPE");
            this.Property(t => t.SEDF_UM_REQ_IND).HasColumnName("SEDF_UM_REQ_IND");
            this.Property(t => t.SEDF_UM_BP_REF_IND).HasColumnName("SEDF_UM_BP_REF_IND");
            this.Property(t => t.SEDF_UM_MAT_IND).HasColumnName("SEDF_UM_MAT_IND");
            this.Property(t => t.SEDF_RWH_IND).HasColumnName("SEDF_RWH_IND");
            this.Property(t => t.SEDF_CAP_IND).HasColumnName("SEDF_CAP_IND");
            this.Property(t => t.SEDF_OPTS).HasColumnName("SEDF_OPTS");
            this.Property(t => t.SEDF_UM_AUTH_WAIVE).HasColumnName("SEDF_UM_AUTH_WAIVE");
            this.Property(t => t.SEDF_UM_AUTH_AMT).HasColumnName("SEDF_UM_AUTH_AMT");
            this.Property(t => t.SEDF_UM_UNITS_WAIVE).HasColumnName("SEDF_UM_UNITS_WAIVE");
            this.Property(t => t.SEDF_AUTH_WV_DAYS).HasColumnName("SEDF_AUTH_WV_DAYS");
            this.Property(t => t.SEDF_LOCK_TOKEN).HasColumnName("SEDF_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
