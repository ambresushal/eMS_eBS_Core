using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class MasterListTemplateMap : EntityTypeConfiguration<MasterListTemplate>
    {
        public MasterListTemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.MasterListTemplate1Up);

            // Table & Column Mappings
            this.ToTable("MasterListTemplate", "ML");
            this.Property(t => t.MasterListTemplate1Up).HasColumnName("MasterListTemplate1Up");
            this.Property(t => t.MLSectionName).HasColumnName("MLSectionName");
            this.Property(t => t.FilePath).HasColumnName("FilePath");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
