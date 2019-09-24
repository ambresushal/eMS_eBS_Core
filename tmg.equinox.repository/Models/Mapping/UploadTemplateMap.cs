using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class UploadTemplateMap : EntityTypeConfiguration<UploadTemplate>
    {
        public UploadTemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.QhpTemplateID);

            // Properties
            this.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.FileType)
                .HasMaxLength(100);

            this.Property(t => t.TemplateGuid)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.FolderID)
                .IsRequired();

            this.Property(t => t.FolderVersionID)
               .IsRequired();

            this.Property(t => t.TenantID)
               .IsRequired();

            this.Property(t => t.UplodedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UploadDate)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("UploadTemplate", "Qhp");
            this.Property(t => t.QhpTemplateID).HasColumnName("QhpTemplateID");
            this.Property(t => t.TemplateName).HasColumnName("TemplateName");
            this.Property(t => t.FileType).HasColumnName("FileType");
            this.Property(t => t.TemplateGuid).HasColumnName("TemplateGuid");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.UplodedBy).HasColumnName("UplodedBy");
            this.Property(t => t.UploadDate).HasColumnName("UploadDate");
            this.Property(t => t.IsTemplateImported).HasColumnName("IsTemplateImported");
            this.Property(t => t.TenantID).HasColumnName("TenantID");

        }
    }
}
