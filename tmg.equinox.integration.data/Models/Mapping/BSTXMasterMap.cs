using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class BSTXMasterMap : EntityTypeConfiguration<BSTXMaster>
    {
        public BSTXMasterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX });

            // Table & Column Mappings
            this.ToTable("BSTXMaster", "Master");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.BSTX_TEXT).HasColumnName("BSTX_TEXT");
            this.Property(t => t.BSBS_TYPE).HasColumnName("BSBS_TYPE");
            this.Property(t => t.BSTX_SEQ_NO).HasColumnName("BSTX_SEQ_NO");
        }
    }
}
