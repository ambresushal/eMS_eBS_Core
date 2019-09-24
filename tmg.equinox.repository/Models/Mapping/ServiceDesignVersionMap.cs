using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ServiceDesignVersionMap : EntityTypeConfiguration<ServiceDesignVersion>
    {
        public ServiceDesignVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.ServiceDesignVersionID);

            // Properties
            this.Property(t => t.ServiceDesignID)
                .IsRequired();

            this.Property(t => t.VersionNumber)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.EffectiveDate)
                .IsRequired();

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.TenantID)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ServiceDesignVersion", "ws");
            this.Property(t => t.ServiceDesignVersionID).HasColumnName("ServiceDesignVersionID");
            this.Property(t => t.ServiceDesignID).HasColumnName("ServiceDesignID");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.VersionNumber).HasColumnName("VersionNumber");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.IsFinalized).HasColumnName("IsFinalized");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.ServiceDesignData).HasColumnName("ServiceDesignData");

            // Relationships
            this.HasRequired(c => c.FormDesign)
                .WithMany(c => c.ServiceVersions)
                .HasForeignKey(c => c.FormDesignID);

            this.HasRequired(c => c.FormDesignVersion)
                .WithMany(c => c.ServiceVersions)
                .HasForeignKey(c => c.FormDesignVersionID);

            this.HasRequired(c => c.Tenant)
                .WithMany(c => c.ServiceDesignVersions)
                .HasForeignKey(c => c.TenantID);
        }
    }
}
