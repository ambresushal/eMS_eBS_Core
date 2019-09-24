using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;


namespace tmg.equinox.qhp.entities.Entities.Models.Mapping
{
    public class DataMapMap : EntityTypeConfiguration<DataMap>
    {
        public DataMapMap()
        {
            // Primary Key
            this.HasKey(t => t.DataMapID);

            // Properties
            this.Property(t => t.FieldType)
                .IsFixedLength()
                .HasMaxLength(20);

            this.Property(t => t.Comments)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DataMap", "Qhp");
            this.Property(t => t.JsonAttribute).HasColumnName("JsonAttribute");
            this.Property(t => t.QhpAttribute).HasColumnName("QhpAttribute");
            this.Property(t => t.JsonXPath).HasColumnName("JsonXPath");
            this.Property(t => t.FieldType).HasColumnName("FieldType");
            this.Property(t => t.Comments).HasColumnName("Notes");
            this.Property(t => t.Version).HasColumnName("Version");
            this.Property(t => t.RelationType).HasColumnName("RelationType");
            this.Property(t => t.IsParent).HasColumnName("IsParent");
            this.Property(t => t.IsChild).HasColumnName("IsChild");
        }
    }
}
