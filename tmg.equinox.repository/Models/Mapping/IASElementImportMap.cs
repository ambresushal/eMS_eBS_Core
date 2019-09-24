using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class IASElementImportMap : EntityTypeConfiguration<IASElementImport>
    {
        public IASElementImportMap()
        {
            // Primary Key
            this.HasKey(t => t.IASElementImportID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.UIElementName)
                .HasMaxLength(100);

            this.Property(t => t.ElementHeaderName)
                .HasMaxLength(500);

            this.Property(t => t.OldValue)
                .HasMaxLength(2000);

            this.Property(t => t.NewValue)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("IASElementImport", "GU");
            this.Property(t => t.IASElementImportID).HasColumnName("IASElementImportID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.IASFolderMapID).HasColumnName("IASFolderMapID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.UIElementName).HasColumnName("UIElementName");
            this.Property(t => t.ElementHeaderName).HasColumnName("ElementHeaderName");
            this.Property(t => t.OldValue).HasColumnName("OldValue");
            this.Property(t => t.NewValue).HasColumnName("NewValue");
            this.Property(t => t.AcceptChange).IsRequired().HasColumnName("AcceptChange");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.IASElementImports)
                .HasForeignKey(d => d.GlobalUpdateID);

            this.HasRequired(t => t.IASFolderMap)
                .WithMany(t => t.IASElementImports)
                .HasForeignKey(d => d.IASFolderMapID);

            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.IASElementImports)
                .HasForeignKey(d => d.UIElementID);

            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.IASElementImports)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
