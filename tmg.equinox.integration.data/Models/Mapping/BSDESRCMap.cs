using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class BSDESRCMap : EntityTypeConfiguration<BSDESRC>
    {
        public BSDESRCMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BSDE_REC_TYPE, t.BSDE_TYPE, t.BSDE_DESC, t.BSDE_LOCK_TOKEN, t.ATXR_SOURCE_ID, t.ProcessGovernance1Up });

            // Table & Column Mappings
            this.ToTable("BSDESRC", "SRC");
            this.Property(t => t.BSDE_REC_TYPE).HasColumnName("BSDE_REC_TYPE");
            this.Property(t => t.BSDE_TYPE).HasColumnName("BSDE_TYPE");
            this.Property(t => t.BSDE_DESC).HasColumnName("BSDE_DESC");
            this.Property(t => t.BSDE_LOCK_TOKEN).HasColumnName("BSDE_LOCK_TOKEN");
            this.Property(t => t.ATXR_SOURCE_ID).HasColumnName("ATXR_SOURCE_ID");
            this.Property(t => t.Action).HasColumnName("Action");            
            this.Property(t => t.ProcessGovernance1Up).HasColumnName("ProcessGovernance1Up");
            this.Property(t => t.BSDE_KEYWORD1).HasColumnName("BSDE_KEYWORD1");
            this.Property(t => t.BSDE_KEYWORD2).HasColumnName("BSDE_KEYWORD2");
            this.Property(t => t.BSDE_KEYWORD3).HasColumnName("BSDE_KEYWORD3");
            this.Property(t => t.BSDE_KEYWORD4).HasColumnName("BSDE_KEYWORD4");
            this.Property(t => t.BSDE_KEYWORD5).HasColumnName("BSDE_KEYWORD5");
            this.Property(t => t.BSDE_KEYWORD6).HasColumnName("BSDE_KEYWORD6");
        }
    }
}
