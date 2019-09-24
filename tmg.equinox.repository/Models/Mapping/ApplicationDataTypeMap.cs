using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ApplicationDataTypeMap : EntityTypeConfiguration<ApplicationDataType>
    {
        public ApplicationDataTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ApplicationDataTypeID);

            // Properties
            this.Property(t => t.ApplicationDataTypeName)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.DisplayText)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ApplicationDataType", "UI");
            this.Property(t => t.ApplicationDataTypeID).HasColumnName("ApplicationDataTypeID");
            this.Property(t => t.ApplicationDataTypeName).HasColumnName("ApplicationDataTypeName");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
        }
    }
}
