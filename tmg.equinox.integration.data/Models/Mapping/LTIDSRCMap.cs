using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class LTIDSRCMap : EntityTypeConfiguration<LTIDSRC>
    {
        public LTIDSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LTIDId });

            // Properties
            this.Property(t => t.LTLT_PFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ACAC_ACC_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IDCD_ID_REL)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);            

            // Table & Column Mappings
            this.ToTable("LTIDSRC", "SRC");
            this.Property(t => t.LTIDId).HasColumnName("LTIDId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.IDCD_ID_REL).HasColumnName("IDCD_ID_REL");
            this.Property(t => t.LTID_LOCK_TOKEN).HasColumnName("LTID_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");            
        }
    }
}
