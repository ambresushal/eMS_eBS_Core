using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class PrefixCounterMap : EntityTypeConfiguration<PrefixCounter>
    {
        public PrefixCounterMap()
        {
            // Primary Key
            this.HasKey(t => t.PrefixCounterId);
             
            this.Property(t => t.EntityName)
                .HasMaxLength(50);

            this.Property(t => t.Prefix)
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("PrefixCounter", "common");
            this.Property(t => t.PrefixCounterId).HasColumnName("PrefixCounterId");
            this.Property(t => t.EntityName).HasColumnName("EntityName");
            this.Property(t => t.Prefix).HasColumnName("Prefix");
        }
    }
}
