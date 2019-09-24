using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class IASElementExportMap : EntityTypeConfiguration<IASElementExport>
    {
        public IASElementExportMap()
        {
            // Primary Key
            this.HasKey(t => t.IASElementExportID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.ElementHeaderName)
                .HasMaxLength(500);

            this.Property(t => t.UIElementName)
                .HasMaxLength(100);

            this.Property(t => t.NewValue)
                .HasMaxLength(2000);

            this.Property(t => t.OldValue)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("IASElementExport", "GU");
            this.Property(t => t.IASElementExportID).HasColumnName("IASElementExportID");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.ElementHeaderName).HasColumnName("ElementHeaderName");
            this.Property(t => t.UIElementName).HasColumnName("UIElementName");
            this.Property(t => t.NewValue).HasColumnName("NewValue");
            this.Property(t => t.OldValue).HasColumnName("OldValue");
            this.Property(t => t.AcceptChange).IsRequired().HasColumnName("AcceptChange ");
            this.Property(t => t.AddedDate).IsRequired().HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.GlobalUpdate)
                .WithMany(t => t.IASElementExports)
                .HasForeignKey(d => d.GlobalUpdateID);
            
            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.IASElementExports)
                .HasForeignKey(d => d.UIElementID);

            this.HasRequired(t => t.UIElementType)
                .WithMany(t => t.IASElementExports)
                .HasForeignKey(d => d.UIElementTypeID);

        }
    }
}
