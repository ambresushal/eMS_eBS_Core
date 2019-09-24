using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTIPTRANSMap : EntityTypeConfiguration<LTIPTRANS>
    {
        public LTIPTRANSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LTLT_PFX});

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LTIP_IPCD_ID_LOW)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.LTIP_IPCD_ID_HIGH)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(7);

            this.Property(t => t.BatchID)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Action)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //this.Property(t => t.Hashcode)
            //    .IsRequired()
            //    .HasMaxLength(50);
            // Table & Column Mappings
            this.ToTable("LTIPTRANS", "Trans");            
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTIP_IPCD_ID_LOW).HasColumnName("LTIP_IPCD_ID_LOW");
            this.Property(t => t.LTIP_IPCD_ID_HIGH).HasColumnName("LTIP_IPCD_ID_HIGH");
            this.Property(t => t.LTIP_LOCK_TOKEN).HasColumnName("LTIP_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.Action).HasColumnName("Action");
            //this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
