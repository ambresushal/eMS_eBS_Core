using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public partial class ServiceParameterMap : EntityTypeConfiguration<ServiceParameter>
    {
        public ServiceParameterMap()
        {
            // Primary Key
            this.HasKey(t => t.ServiceParameterID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.GeneratedName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.DataTypeID)
                .IsRequired();

            this.Property(t => t.IsRequired)
                .IsRequired();

            this.Property(t => t.ServiceDesignID)
                .IsRequired();

            this.Property(t => t.ServiceDesignVersionID)
                .IsRequired();

            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.AddedDate)
                .IsRequired();

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.IsActive)
                .IsRequired();

            this.Property(t => t.TenantID)
                .IsRequired();

            this.Property(t => t.UIElementID);

            this.Property(t => t.UIElementFullPath)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ServiceParameter", "ws");
            this.Property(t => t.ServiceParameterID).HasColumnName("ServiceParameterID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.GeneratedName).HasColumnName("GeneratedName");
            this.Property(t => t.IsRequired).HasColumnName("IsRequired");
            this.Property(t => t.ServiceDesignID).HasColumnName("ServiceDesignID");
            this.Property(t => t.ServiceDesignVersionID).HasColumnName("ServiceDesignVersionID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.UIElementFullPath).HasColumnName("UIElementFullPath");

            // Relationships
            this.HasRequired(c => c.ApplicationDataType)
                .WithMany(c => c.ServiceParameters)
                .HasForeignKey(c => c.DataTypeID);

            this.HasRequired(c => c.ServiceDesign)
                .WithMany(c => c.ServiceParameters)
                .HasForeignKey(c => c.ServiceDesignID);

            this.HasRequired(c => c.ServiceDesignVersion)
                .WithMany(c => c.ServiceParameters)
                .HasForeignKey(c => c.ServiceDesignVersionID);

            this.HasRequired(c => c.Tenant)
                .WithMany(c => c.ServiceParameters)
                .HasForeignKey(c => c.TenantID);

            this.HasOptional(c => c.UIElement)
                .WithMany(c => c.ServiceParameters)
                .HasForeignKey(c => c.UIElementID);
        }
    }
}
