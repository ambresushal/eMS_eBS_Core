using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTPRTRANSMap : EntityTypeConfiguration<LTPRTRANS>
    {
        public LTPRTRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LTLT_PFX });

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PRPR_MCTR_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //this.Property(t => t.Hashcode)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("LTPRTRANS", "Trans");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.PRPR_MCTR_TYPE).HasColumnName("PRPR_MCTR_TYPE");
            this.Property(t => t.LTPR_LOCK_TOKEN).HasColumnName("LTPR_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            //this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
