using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDBC481ArcMap : EntityTypeConfiguration<PDBC481Arc>
    {
        public PDBC481ArcMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_Id, t.PDBC_TYPE });

            // Properties
            this.Property(t => t.PDBC_Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PDPD_ID)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PDBC_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDBC_PFX)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDBC_OPTS)
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
            this.ToTable("PDBC481", "arc_481");
            this.Property(t => t.PDBC_Id).HasColumnName("PDBC_Id");
            this.Property(t => t.ebsInstanceId).HasColumnName("ebsInstanceId");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDBC_EFF_DT).HasColumnName("PDBC_EFF_DT");
            this.Property(t => t.PDBC_TERM_DT).HasColumnName("PDBC_TERM_DT");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDBC_OPTS).HasColumnName("PDBC_OPTS");
            this.Property(t => t.PDBC_LOCK_TOKEN).HasColumnName("PDBC_LOCK_TOKEN");
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
