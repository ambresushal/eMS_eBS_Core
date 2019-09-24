using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;


namespace tmg.equinox.repository.Models.Mapping
{
    public class JsonMap: EntityTypeConfiguration<JsonResultMapping>
    {
        public JsonMap()
        {
            // Primary Key
            this.HasKey(t => t.Label);

            // Table & Column Mappings
            this.ToTable("JsonResultMapping", "Fldr");
            this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.JSONPath).HasColumnName("JSONPath");
            this.Property(t => t.DesignType).HasColumnName("DesignType");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
