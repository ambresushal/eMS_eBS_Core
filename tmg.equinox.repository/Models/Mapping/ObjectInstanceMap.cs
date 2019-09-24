using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ObjectInstanceMap : EntityTypeConfiguration<ObjectInstance>
    {
        public ObjectInstanceMap()
        {
            // Primary Key
            this.HasKey(t => t.ObjectInstanceID);

            // Properties
            this.Property(t => t.ObjectInstanceID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ObjectInstance", "DM");
            this.Property(t => t.ObjectInstanceID).HasColumnName("ObjectInstanceID");
        }
    }
}
