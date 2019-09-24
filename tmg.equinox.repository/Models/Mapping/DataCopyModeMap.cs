using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DataCopyModeMap : EntityTypeConfiguration<DataCopyMode>
    {
        public DataCopyModeMap()
        {
            // Primary Key
            this.HasKey(t => t.DataCopyModeID);

            // Properties
            this.Property(t => t.CopyData)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("DataCopyMode", "UI");
            this.Property(t => t.DataCopyModeID).HasColumnName("DataCopyModeID");
            this.Property(t => t.CopyData).HasColumnName("CopyData");
        }
    }
}
