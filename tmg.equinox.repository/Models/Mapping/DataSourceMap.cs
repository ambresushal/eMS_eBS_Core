using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DataSourceMap : EntityTypeConfiguration<DataSource>
    {
        public DataSourceMap()
        {
            // Primary Key
            this.HasKey(t => t.DataSourceID);

            // Properties
            this.Property(t => t.DataSourceName)
                .HasMaxLength(100);

            this.Property(t => t.DataSourceDescription)
                .HasMaxLength(500);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("DataSource", "UI");
            this.Property(t => t.DataSourceID).HasColumnName("DataSourceID");
            this.Property(t => t.DataSourceName).HasColumnName("DataSourceName");
            this.Property(t => t.DataSourceDescription).HasColumnName("DataSourceDescription");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");

            // Relationships
            this.HasRequired(t => t.FormDesign)
                .WithMany(t => t.DataSources)
                .HasForeignKey(d => d.FormDesignID);
            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.DataSources)
                .HasForeignKey(d => d.FormDesignVersionID);

        }
    }
}
