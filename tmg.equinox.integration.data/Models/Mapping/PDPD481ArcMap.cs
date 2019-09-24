using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPD481ArcMap : EntityTypeConfiguration<PDPD481Arc>
    {
        public PDPD481ArcMap()
        {
            // Primary Key
            this.HasKey(t => t.PDPD_Id);

            // Properties
            this.Property(t => t.PDPD_ID)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PDPD_RISK_IND)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LOBD_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LOBD_ALT_RISK_ID)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_ACC_SFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_OPTS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_CAP_POP_LVL)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDPD_MCTR_CCAT)
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
            this.ToTable("PDPD481", "arc_481");
            this.Property(t => t.PDPD_Id).HasColumnName("PDPD_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDPD_EFF_DT).HasColumnName("PDPD_EFF_DT");
            this.Property(t => t.PDPD_TERM_DT).HasColumnName("PDPD_TERM_DT");
            this.Property(t => t.PDPD_RISK_IND).HasColumnName("PDPD_RISK_IND");
            this.Property(t => t.LOBD_ID).HasColumnName("LOBD_ID");
            this.Property(t => t.LOBD_ALT_RISK_ID).HasColumnName("LOBD_ALT_RISK_ID");
            this.Property(t => t.PDPD_ACC_SFX).HasColumnName("PDPD_ACC_SFX");
            this.Property(t => t.PDPD_OPTS).HasColumnName("PDPD_OPTS");
            this.Property(t => t.PDPD_CAP_POP_LVL).HasColumnName("PDPD_CAP_POP_LVL");
            this.Property(t => t.PDPD_CAP_RET_MOS).HasColumnName("PDPD_CAP_RET_MOS");
            this.Property(t => t.PDPD_MCTR_CCAT).HasColumnName("PDPD_MCTR_CCAT");
            this.Property(t => t.PDPD_LOCK_TOKEN).HasColumnName("PDPD_LOCK_TOKEN");
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
