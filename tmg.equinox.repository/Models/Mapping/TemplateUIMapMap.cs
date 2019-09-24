using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TemplateUIMapMap : EntityTypeConfiguration<TemplateUIMap>
    {
        public TemplateUIMapMap()
        {
            // Primary Key
            this.HasKey(t => t.TemplateUIMapID);

            // Table & Column Mappings
            this.ToTable("TemplateUIMap", "UI");
            this.Property(t => t.TemplateUIMapID).HasColumnName("TemplateUIMapID");
            this.Property(t => t.TemplateID).HasColumnName("TemplateID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.IncludeInPDF).HasColumnName("IncludeInPDF");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            // Relationships           
            this.HasOptional(t => t.Template)
                .WithMany(t => t.TemplateUIMap)
                .HasForeignKey(d => d.TemplateID);

            this.HasRequired(t => t.UIElement)
                .WithMany(t => t.TemplateUIMap)
                .HasForeignKey(d => d.UIElementID);

            this.HasOptional(t => t.Tenant)
                .WithMany(t => t.TemplateUIMap)
                .HasForeignKey(d => d.TenantID);
        }
    }
}
