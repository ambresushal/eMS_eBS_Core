using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPDJsonDataMap : EntityTypeConfiguration<PDPDJsonData>
    {
        public PDPDJsonDataMap()
        {
            // Primary Key
            this.HasKey(t => t.PDPD_ID);
            
            // Properties
            this.Property(t => t.PDPD_RISK_IND)
                .HasMaxLength(510);

            this.Property(t => t.LOBD_ID)
                .HasMaxLength(510);

            this.Property(t => t.LOBD_ALT_RISK_ID)
                .HasMaxLength(510);

            this.Property(t => t.PDPD_ACC_SFX)
                .HasMaxLength(510);
            
            this.Property(t => t.PDPD_OPTS)
                .HasMaxLength(510);
            
            this.Property(t => t.PDPD_CAP_POP_LVL)
                .HasMaxLength(510);

             this.Property(t => t.PDPD_MCTR_CCAT)
                .HasMaxLength(510);
          
            // Table & Column Mappings
            this.ToTable("PDPDJsonData", "jsondata");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDPD_RISK_IND).HasColumnName("PDPD_RISK_IND");
            this.Property(t => t.LOBD_ID).HasColumnName("LOBD_ID");
            this.Property(t => t.LOBD_ALT_RISK_ID).HasColumnName("LOBD_ALT_RISK_ID");
            this.Property(t => t.PDPD_ACC_SFX).HasColumnName("PDPD_ACC_SFX");
            this.Property(t => t.PDPD_OPTS).HasColumnName("PDPD_OPTS");
            this.Property(t => t.PDPD_CAP_POP_LVL).HasColumnName("PDPD_CAP_POP_LVL");
            this.Property(t => t.PDPD_CAP_RET_MOS).HasColumnName("PDPD_CAP_RET_MOS");
            this.Property(t => t.PDPD_MCTR_CCAT).HasColumnName("PDPD_MCTR_CCAT");
        }
    }
}
