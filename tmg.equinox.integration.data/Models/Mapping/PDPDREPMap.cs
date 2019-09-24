using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPDREPMap : EntityTypeConfiguration<PDPDREP>
    {
        public PDPDREPMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDPD_ID });

            // Properties
            this.Property(t => t.PDPD_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PDPD_RISK_IND)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LOBD_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.LOBD_ALT_RISK_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_ACC_SFX)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_OPTS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.PDPD_CAP_POP_LVL)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PDPD_MCTR_CCAT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.SYS_USUS_ID)
                .HasMaxLength(48);

            this.Property(t => t.SYS_DBUSER_ID)
                .HasMaxLength(48);
            
            this.Property(t => t.PDPD_ACC_SHDW_SFX_NVL)
                .HasMaxLength(255);

            this.Property(t => t.Hashcode)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PDPDREP", "Rep");            
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
            this.Property(t => t.PDPD_ACC_SHDW_SFX_NVL).HasColumnName("PDPD_ACC_SHDW_SFX_NVL");
            this.Property(t => t.Hashcode).HasColumnName("Hashcode");
        }
    }
}
