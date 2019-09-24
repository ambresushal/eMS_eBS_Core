using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DataSourceModeMap : EntityTypeConfiguration<DataSourceMode>
    {
         public DataSourceModeMap()
        {
            // Primary Key
            this.HasKey(t => t.DataSourceModeID);

            // Properties
            this.Property(t => t.DataSourceModeType)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("DataSourceMode", "UI");
            this.Property(t => t.DataSourceModeID).HasColumnName("DataSourceModeID");
            this.Property(t => t.DataSourceModeType).HasColumnName("DataSourceModeType");
        }
    }
}
