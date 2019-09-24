using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class EBCLMasterMap : EntityTypeConfiguration<EBCLMaster>
    {
        public EBCLMasterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX });

            // Table & Column Mappings
            this.ToTable("EBCLMaster", "Master");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.EBCL_TYPE).HasColumnName("EBCL_TYPE");
            this.Property(t => t.EBCL_YEAR_IND).HasColumnName("EBCL_YEAR_IND");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.EBCL_PFX_MAX).HasColumnName("EBCL_PFX_MAX");
            this.Property(t => t.EBCL_ZERO_AMT_IND).HasColumnName("EBCL_ZERO_AMT_IND");
        }
    }
}
