using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SEPYMap : EntityTypeConfiguration<SEPY>
    {
        public SEPYMap()
        {
            this.HasKey(t => new { t.SEPY_PFX });            
            // Table & Column Mappings
            this.ToTable("SEPY", "Master");
            this.Property(t => t.SEPY_PFX).HasColumnName("SEPY_PFX");
            this.Property(t => t.SEPY_EFF_DT).HasColumnName("SEPY_EFF_DT");
            this.Property(t => t.SEPYHashcode).HasColumnName("SEPYHashcode");
            this.Property(t => t.SEPY_TERM_DT).HasColumnName("SEPY_TERM_DT");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.IsUsed).HasColumnName("IsUsed");            
        }
    }
}
