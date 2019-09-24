using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ATXRSRCMap : EntityTypeConfiguration<ATXRSRC>
    {
        public ATXRSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ATSY_ID });

            // Table & Column Mappings
            this.ToTable("ATXRSRC", "SRC");
            this.Property(t => t.ATSY_ID).HasColumnName("ATSY_ID");
            this.Property(t => t.ATXR_DEST_ID).HasColumnName("ATXR_DEST_ID");
            this.Property(t => t.ATXR_DEST_ID).HasColumnName("ATXR_DEST_ID");
            this.Property(t => t.ATXR_DESC).HasColumnName("ATXR_DESC");
            this.Property(t => t.ATXR_CREATE_DT).HasColumnName("ATXR_CREATE_DT");
            this.Property(t => t.ATXR_LAST_UPD_DT).HasColumnName("ATXR_LAST_UPD_DT");
        }
    }
}
