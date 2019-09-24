using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class KeyProductUIElementMapMap : EntityTypeConfiguration<KeyProductUIElementMap>
    {
        public KeyProductUIElementMapMap()
        {
            // Primary Key
            this.HasKey(t => t.KeyProductUIElementMapID);

            // Properties
            // Table & Column Mappings
            this.ToTable("KeyProductUIElementMap", "UI");
            this.Property(t => t.KeyProductUIElementMapID).HasColumnName("KeyProductUIElementMapID");
            this.Property(t => t.UIelementID).HasColumnName("UIelementID");
            this.Property(t => t.ParentUIelementID).HasColumnName("ParentUIelementID");
            this.Property(t => t.MasterTemplateID).HasColumnName("MasterTemplateID");
        }
    }
}
