using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESP481ArcMap : EntityTypeConfiguration<SESP481Arc>
    {
        public SESP481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SESP_Id);

            // Properties
            this.Property(t => t.SESE_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESE_RULE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SESP_PEN_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SESP_PEN_CALC_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.EXCD_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SERL_REL_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SESP_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SESP481", "arc_481");
            this.Property(t => t.SESP_Id).HasColumnName("SESP_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SESP_PEN_IND).HasColumnName("SESP_PEN_IND");
            this.Property(t => t.SESP_PEN_CALC_IND).HasColumnName("SESP_PEN_CALC_IND");
            this.Property(t => t.SESP_PEN_AMT).HasColumnName("SESP_PEN_AMT");
            this.Property(t => t.SESP_PEN_PCT).HasColumnName("SESP_PEN_PCT");
            this.Property(t => t.SESP_PEN_MAX_AMT).HasColumnName("SESP_PEN_MAX_AMT");
            this.Property(t => t.EXCD_ID).HasColumnName("EXCD_ID");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESP_OPTS).HasColumnName("SESP_OPTS");
            this.Property(t => t.SESP_LOCK_TOKEN).HasColumnName("SESP_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
