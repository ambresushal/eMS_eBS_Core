using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.translator.dao.Models;


namespace tmg.equinox.integration.data.Models.Mapping
{
    public class LTSEMasterListSRCMap: EntityTypeConfiguration<LTSEMasterListSRC>
    {
        public LTSEMasterListSRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.LTSEId });

            // Table & Column Mappings
            this.ToTable("LTSEMasterList", "SRC");
            this.Property(t => t.LTSEId).HasColumnName("LTSEId");
            this.Property(t => t.LTLT_PFX).HasColumnName("LTLT_PFX");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.SESE_ID).HasColumnName("SESE_ID");
            this.Property(t => t.LTSE_WT_CTR).HasColumnName("LTSE_WT_CTR");
            this.Property(t => t.LTSE_LOCK_TOKEN).HasColumnName("LTSE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.SYS_LAST_UPD_DTM).HasColumnName("SYS_LAST_UPD_DTM");
            this.Property(t => t.SYS_USUS_ID).HasColumnName("SYS_USUS_ID");
            this.Property(t => t.SYS_DBUSER_ID).HasColumnName("SYS_DBUSER_ID");
            this.Property(t => t.ProcessGovernance1up).HasColumnName("ProcessGovernance1up");          
        }
    }
}
