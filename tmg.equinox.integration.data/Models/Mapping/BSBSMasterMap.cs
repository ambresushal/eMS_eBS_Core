using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class BSBSMasterMap : EntityTypeConfiguration<BSBSMaster>
    {
        public BSBSMasterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDBC_PFX });

            // Table & Column Mappings
            this.ToTable("BSBSMaster", "Master");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.BSBS_TYPE).HasColumnName("BSBS_TYPE");
            this.Property(t => t.BSBS_DESC).HasColumnName("BSBS_DESC");
        }
    }
}
