using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class PDPDDataMap : EntityTypeConfiguration<PDPDData>
    {
        public PDPDDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.ProcessGovernance1up });
            this.ToTable("PDPDModeldata", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDPD_RISK_IND).HasColumnName("PDPD_RISK_IND");
            this.Property(t => t.LOBD_ID).HasColumnName("LOBD_ID");
            this.Property(t => t.LOBD_ALT_RISK_ID).HasColumnName("LOBD_ALT_RISK_ID");
            this.Property(t => t.PDPD_ACC_SFX).HasColumnName("PDPD_ACC_SFX");
            this.Property(t => t.PDPD_CAP_POP_LVL).HasColumnName("PDPD_CAP_POP_LVL");
            this.Property(t => t.PDPD_CAP_RET_MOS).HasColumnName("PDPD_CAP_RET_MOS");
            this.Property(t => t.PDPD_MCTR_CCAT).HasColumnName("PDPD_MCTR_CCAT");
            this.Property(t => t.PDPD_ACC_SHDW_SFX_NVL).HasColumnName("PDPD_ACC_SHDW_SFX_NVL");
            this.Property(t => t.IsSHDW).HasColumnName("IsSHDW");
            this.Property(t => t.IsProductNew).HasColumnName("IsProductNew");
            this.Property(t => t.GenerateNewProduct).HasColumnName("GenerateNewProduct");
            this.Property(t => t.StandardProduct).HasColumnName("StandardProduct");
            this.Property(t => t.BPL).HasColumnName("BPL");
            this.Property(t => t.Product_Category).HasColumnName("Product_Category");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
            this.Property(t => t.PDPD_OPTS).HasColumnName("PDPD_OPTS");
            this.Property(t => t.IsRetro).HasColumnName("IsRetro");
        }
    }
}
