using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class PDPDBatchMap : EntityTypeConfiguration<PDPDBatch>
    {
        public PDPDBatchMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PDPDBatch_ID });

            // Properties

            this.Property(t => t.PDPD_ID)
                .HasMaxLength(255);

            this.Property(t => t.BatchID)
                .HasMaxLength(100);

            this.Property(t => t.Flag)
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("PDPDBatch", "setup");
            this.Property(t => t.PDPDBatch_ID).HasColumnName("ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.PDPD_ID).HasColumnName("PDPD_ID");
            this.Property(t => t.PDPD_EFF_DT).HasColumnName("PDPD_EFF_DT");
            this.Property(t => t.Flag).HasColumnName("Flag");
            this.Property(t => t.FolderVersion).HasColumnName("FldrVer");
        }
    }
}
