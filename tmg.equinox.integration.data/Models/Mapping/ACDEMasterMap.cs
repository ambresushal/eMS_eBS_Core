using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;


namespace tmg.equinox.integration.data.Models.Mapping
{
    public class ACDEMasterMap : EntityTypeConfiguration<ACDEMaster>
    {
        public ACDEMasterMap()
        {
            this.HasKey(t => new { t.ACAC_ACC_NO, t.ACDE_DESC });
            // Table & Column Mappings
            this.ToTable("ACDEmaster", "Master");
            this.Property(t => t.ACDE_ACC_TYPE).HasColumnName("ACDE_ACC_TYPE");
            this.Property(t => t.ACAC_ACC_NO).HasColumnName("ACAC_ACC_NO");
            this.Property(t => t.ACDE_DESC).HasColumnName("ACDE_DESC");
            this.Property(t => t.acde_LOCK_TOKEN).HasColumnName("acde_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.BatchID).HasColumnName("BatchID");
            this.Property(t => t.acronym).HasColumnName("acronym");     
        }
    }
}
