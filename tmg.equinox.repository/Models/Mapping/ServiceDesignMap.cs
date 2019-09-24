using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ServiceDesignMap : EntityTypeConfiguration<ServiceDesign>
    {
        public ServiceDesignMap()
        {
            // Primary Key
            this.HasKey(t => t.ServiceDesignID);

            // Properties
            this.Property(t => t.ServiceName)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.ServiceMethodName)
                .HasMaxLength(200);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.TenantID)
                .IsRequired();

            this.Property(t => t.DoesReturnList)
                .IsRequired();

            this.Property(t => t.IsReturnJSON)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ServiceDesign", "ws");
            this.Property(t => t.ServiceDesignID).HasColumnName("ServiceDesignID");
            this.Property(t => t.ServiceName).HasColumnName("ServiceName");
            this.Property(t => t.ServiceMethodName).HasColumnName("ServiceMethodName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.DoesReturnList).HasColumnName("DoesReturnList");
            this.Property(t => t.IsReturnJSON).HasColumnName("IsReturnJSON");

            // Relationship
            this.HasRequired(c => c.Tenant)
                .WithMany(c => c.ServiceDesigns)
                .HasForeignKey(c => c.TenantID);

        }
    }
}
