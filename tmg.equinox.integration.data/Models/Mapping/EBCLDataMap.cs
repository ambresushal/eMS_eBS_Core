using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.facet.data.Models.Mapping
{
    public class EBCLDataMap:EntityTypeConfiguration<EBCLData>
    {
        public EBCLDataMap()
        {
            this.HasKey(t => new { t.PDPD_ID, t.BenefitSet, t.EBCL_TYPE, t.ACAC_ACC_NO, t.ProcessGovernance1up });

            this.ToTable("EBCLModelData", "ModelData");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.BenefitSet).HasColumnName("BenefitSet");
            this.Property(t => t.EBCL_TYPE).HasColumnName("EBCL_TYPE");
            this.Property(t => t.EBCL_YEAR_IND).HasColumnName("EBCL_YEAR_IND");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.EBCL_ZERO_AMT_IND).HasColumnName("EBCL_ZERO_AMT_IND");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");
        }
    }
}
