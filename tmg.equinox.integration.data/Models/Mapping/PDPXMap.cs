using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PDPXMap: EntityTypeConfiguration<PDPX>
    {
        public PDPXMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX });

            // Table & Column Mappings
            this.ToTable("PDPX", "Master");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.IsUsed).HasColumnName("IsUsed");
            this.Property(t => t.isActive).HasColumnName("isActive");
            
        }
    }
}
