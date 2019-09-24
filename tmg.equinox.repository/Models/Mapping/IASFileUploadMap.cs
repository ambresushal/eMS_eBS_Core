using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class IASFileUploadMap : EntityTypeConfiguration<IASFileUpload>
    {
        public IASFileUploadMap()
        {
            // Primary Key
            this.HasKey(t => t.IASFileUploadID);

            // Properties
            this.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.FileExtension)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TemplateGuid)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("IASFileUpload", "GU");
            this.Property(t => t.IASFileUploadID).HasColumnName("IASFileUploadID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.FileExtension).HasColumnName("FileExtension");
            this.Property(t => t.TemplateGuid).HasColumnName("TemplateGuid");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.IASFileUploads)
                .HasForeignKey(d => d.GlobalUpdateID);
        }
    }
}
