using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models.Mapping
{
    public class ATNTSRCMap : EntityTypeConfiguration<ATNTSRC>
    {
        public ATNTSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ATSY_ID });

            // Table & Column Mappings
            this.ToTable("ATNTSRC", "SRC");
            this.Property(t => t.ATSY_ID).HasColumnName("ATSY_ID");
            this.Property(t => t.ATXR_DEST_ID).HasColumnName("ATXR_DEST_ID");
            this.Property(t => t.ATNT_SEQ_NO).HasColumnName("ATNT_SEQ_NO");
            this.Property(t => t.ATNT_TYPE).HasColumnName("ATNT_TYPE");
            this.Property(t => t.ATXR_ATTACH_ID).HasColumnName("ATXR_ATTACH_ID");
            this.Property(t => t.ATNT_LOCK_TOKEN).HasColumnName("ATNT_LOCK_TOKEN");
        }
    }
}
