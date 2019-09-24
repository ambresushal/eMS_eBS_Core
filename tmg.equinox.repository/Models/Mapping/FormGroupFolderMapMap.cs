using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FormGroupFolderMapMap : EntityTypeConfiguration<FormGroupFolderMap>
    {
        public FormGroupFolderMapMap()
        {
            // Primary Key
            this.HasKey(t => t.FormGroupFolderMapID);

            // Properties
            this.Property(t => t.FolderType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("FormGroupFolderMap", "UI");
            this.Property(t => t.FormGroupFolderMapID).HasColumnName("FormGroupFolderMapID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.FormDesignGroupID).HasColumnName("FormDesignGroupID");
            this.Property(t => t.FolderType).HasColumnName("FolderType");
        }
    }
}
