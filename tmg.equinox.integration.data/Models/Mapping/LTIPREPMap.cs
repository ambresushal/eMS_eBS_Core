using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTIPREPMap : EntityTypeConfiguration<LTIPREP>
    {
        public LTIPREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LTIPId});

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


            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);
            // Table & Column Mappings
            this.ToTable("LTIPREP", "Rep");
            this.Property(t => t.LTIPId).HasColumnName("LTIPId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.LTIP_IPCD_ID_LOW).HasColumnName("LTIP_IPCD_ID_LOW");
            this.Property(t => t.LTIP_IPCD_ID_HIGH).HasColumnName("LTIP_IPCD_ID_HIGH");
            this.Property(t => t.LTIP_LOCK_TOKEN).HasColumnName("LTIP_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
