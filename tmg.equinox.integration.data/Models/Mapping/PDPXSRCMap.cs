using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPXSRCMap : EntityTypeConfiguration<PDPXSRC>
    {
        public PDPXSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX });

            // Table & Column Mappings
            this.ToTable("PDPXSRC", "SRC");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.PDPX_DESC).HasColumnName("PDPX_DESC");
            this.Property(t => t.PDPX_LOCK_TOKEN).HasColumnName("PDPX_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");            
        }
    }
}
