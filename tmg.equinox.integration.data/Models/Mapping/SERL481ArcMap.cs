using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SERL481ArcMap : EntityTypeConfiguration<SERL481Arc>
    {
        public SERL481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.SERL_Id);

            // Properties
            this.Property(t => t.SERL_REL_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SERL_REL_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_REL_PER_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_DIAG_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_NTWK_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_PC_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_REF_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERL_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SERL_COPAY_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.NewBatchID)
                .HasMaxLength(100);

            this.Property(t => t.SERL_DESC)
                .HasMaxLength(70);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            // Table & Column Mappings
            this.ToTable("SERL481", "arc_481");
            this.Property(t => t.SERL_Id).HasColumnName("SERL_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SERL_REL_TYPE).HasColumnName("SERL_REL_TYPE");
            this.Property(t => t.SERL_REL_PER_IND).HasColumnName("SERL_REL_PER_IND");
            this.Property(t => t.SERL_DIAG_IND).HasColumnName("SERL_DIAG_IND");
            this.Property(t => t.SERL_NTWK_IND).HasColumnName("SERL_NTWK_IND");
            this.Property(t => t.SERL_PC_IND).HasColumnName("SERL_PC_IND");
            this.Property(t => t.SERL_REF_IND).HasColumnName("SERL_REF_IND");
            this.Property(t => t.SERL_PER).HasColumnName("SERL_PER");
            this.Property(t => t.SERL_OPTS).HasColumnName("SERL_OPTS");
            this.Property(t => t.SERL_COPAY_IND).HasColumnName("SERL_COPAY_IND");
            this.Property(t => t.SERL_LOCK_TOKEN).HasColumnName("SERL_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.NewBatchID).HasColumnName("NewBatchID");
            this.Property(t => t.ArchiveDate).HasColumnName("ArchiveDate");
            this.Property(t => t.SERL_DESC).HasColumnName("SERL_DESC");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BenefitOfferingID).HasColumnName("BenefitOfferingID");
        }
    }
}
