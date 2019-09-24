using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PrefixDescriptionLogMap : EntityTypeConfiguration<PrefixDescriptionLog>
    {
        public PrefixDescriptionLogMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PrefixDescriptionLogID});

            // Table & Column Mappings
            this.ToTable("PrefixDescriptionLog", "Log");
            this.Property(t => t.PrefixDescriptionLogID).HasColumnName("PrefixDescriptionLogID");
            this.Property(t => t.PDBC_PFX).HasColumnName("PDBC_PFX");
            this.Property(t => t.PDBC_TYPE).HasColumnName("PDBC_TYPE");
            this.Property(t => t.Old_Desc).HasColumnName("Old_Desc");
            this.Property(t => t.New_Desc).HasColumnName("New_Desc");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
        }
    }
}
