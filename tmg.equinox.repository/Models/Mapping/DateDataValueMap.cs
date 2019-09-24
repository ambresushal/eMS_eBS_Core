using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DateDataValueMap : EntityTypeConfiguration<DateDataValue>
    {
        public DateDataValueMap()
        {
            // Primary Key
            this.HasKey(t => t.ValueID);

            // Properties
            // Table & Column Mappings
            this.ToTable("DateDataValue", "Data");
            this.Property(t => t.ValueID).HasColumnName("ValueID");
            this.Property(t => t.ObjVerID).HasColumnName("ObjVerID");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.ObjInstanceID).HasColumnName("ObjInstanceID");
            this.Property(t => t.ParentObjInstanceID).HasColumnName("ParentObjInstanceID");
            this.Property(t => t.RowIDInfo).HasColumnName("RowIDInfo");
            this.Property(t => t.RootObjInstanceID).HasColumnName("RootObjInstanceID");

            // Relationships
            this.HasRequired(t => t.ObjectVersionAttribXref)
                .WithMany(t => t.DateDataValues)
                .HasForeignKey(d => d.ObjVerID);

        }
    }
}
