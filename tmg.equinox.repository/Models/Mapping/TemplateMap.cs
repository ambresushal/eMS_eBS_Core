using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace tmg.equinox.repository.Models.Mapping
{
    public class TemplateMap : EntityTypeConfiguration<Template>
    {
        public TemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.TemplateID);

            // Properties
            this.Property(t => t.TemplateName)
                .IsRequired()
                .HasMaxLength(200);
            // Properties
            this.Property(t => t.Description)                
                .HasMaxLength(1000);
            // Table & Column Mappings
            this.ToTable("Template", "UI");
            this.Property(t => t.TemplateID).HasColumnName("TemplateID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.TemplateName).HasColumnName("TemplateName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            // Relationships           
            this.HasOptional(t => t.FormDesign)
                .WithMany(t => t.Template)
                .HasForeignKey(d => d.FormDesignID);

            this.HasRequired(t => t.FormDesignVersion)
                .WithMany(t => t.Template)
                .HasForeignKey(d => d.FormDesignVersionID);

            this.HasOptional(t => t.Tenant)
                .WithMany(t => t.Template)
                .HasForeignKey(d => d.TenantID);           
        }
    }
}
