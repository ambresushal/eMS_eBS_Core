using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DataSourceElementDisplayModeMap : EntityTypeConfiguration<DataSourceElementDisplayMode>
    {
        public DataSourceElementDisplayModeMap()
        {
            // Primary Key
            this.HasKey(t => t.DataSourceElementDisplayModeID);

            // Properties
            this.Property(t => t.DisplayMode)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("DataSourceElementDisplayMode", "UI");
            this.Property(t => t.DataSourceElementDisplayModeID).HasColumnName("DataSourceElementDisplayModeID");
            this.Property(t => t.DisplayMode).HasColumnName("DisplayMode");
        }
    }
}
