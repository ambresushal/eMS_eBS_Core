using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ServiceDefinitionMap : EntityTypeConfiguration<ServiceDefinition>
    {
        public ServiceDefinitionMap()
        {
            // Primary Key
            this.HasKey(t => t.ServiceDefinitionID);

            // Properties
            this.Property(t => t.UIElementFullPath)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.UIElementDataTypeID)
                .IsRequired();

            this.Property(t => t.UIElementID)
                .IsRequired();

            this.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.UIElementTypeID)
                .IsRequired();

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.TenantID)
                .IsRequired();

            this.Property(t => t.IsKey)
                .IsRequired();

            this.Property(t => t.Sequence)
                .IsRequired();

            this.Property(t => t.IsRequired)
                .IsRequired();

            this.Property(t => t.IsPartOfDataSource)
                .IsRequired();

            this.Property(t => t.ServiceDesignID)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ServiceDefinition", "ws");
            this.Property(t => t.ServiceDefinitionID).HasColumnName("ServiceDefinitionID");
            this.Property(t => t.UIElementFullPath).HasColumnName("UIElementFullPath");
            this.Property(t => t.UIElementDataTypeID).HasColumnName("UIElementDataTypeID");
            this.Property(t => t.UIElementTypeID).HasColumnName("UIElementTypeID");
            this.Property(t => t.UIElementID).HasColumnName("UIElementID");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.ParentServiceDefinitionID).HasColumnName("ParentServiceDefinitionID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.IsKey).HasColumnName("IsKey");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.IsRequired).HasColumnName("IsRequired");
            this.Property(t => t.IsPartOfDataSource).HasColumnName("IsPartOfDataSource");
            this.Property(t => t.ServiceDesignID).HasColumnName("ServiceDesignID");

            // Relationships
            this.HasRequired(c => c.ApplicationDataType)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.UIElementDataTypeID);

            this.HasRequired(c => c.UIElementType)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.UIElementTypeID);

            this.HasRequired(c => c.UIElement)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.UIElementID);

            this.HasOptional(c => c.ParentServiceDefinition)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.ParentServiceDefinitionID);

            this.HasRequired(c => c.Tenant)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.TenantID);

            this.HasRequired(c => c.ServiceDesign)
                .WithMany(c => c.ServiceDefinitions)
                .HasForeignKey(c => c.ServiceDesignID);
        }
    }
}
