using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class ATNDMasterMap : EntityTypeConfiguration<ATNDMaster>
    {
        public ATNDMasterMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ATSY_ID });

            // Table & Column Mappings
            this.ToTable("ATNDMaster", "Master");
            this.Property(t => t.ATSY_ID).HasColumnName("ATSY_ID");
            this.Property(t => t.ATXR_DEST_ID).HasColumnName("ATXR_DEST_ID");
            this.Property(t => t.ATNT_SEQ_NO).HasColumnName("ATNT_SEQ_NO");
            this.Property(t => t.ATND_SEQ_NO).HasColumnName("ATND_SEQ_NO");
            this.Property(t => t.ATND_TEXT).HasColumnName("ATND_TEXT");
        }
    }
}
