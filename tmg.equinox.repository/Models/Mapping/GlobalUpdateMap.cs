using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class GlobalUpdateMap : EntityTypeConfiguration<GlobalUpdate>
    {
        public GlobalUpdateMap()
        {
            // Primary Key
            this.HasKey(t => t.GlobalUpdateID);

            // Properties
            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.GlobalUpdateName)
                .IsRequired()
                .HasMaxLength(100);

            this.ToTable("GlobalUpdate", "GU");
            this.Property(t => t.GlobalUpdateID).HasColumnName("GlobalUpdateID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.GlobalUpdateStatusID).HasColumnName("GlobalUpdateStatusID");
            this.Property(t => t.IASWizardStepID).HasColumnName("IASWizardStepID");
            this.Property(t => t.GlobalUpdateName).IsRequired().HasColumnName("GlobalUpdateName");
            this.Property(t => t.EffectiveDateFrom).IsRequired().HasColumnName("EffectiveDateFrom");
            this.Property(t => t.EffectiveDateTo).IsRequired().HasColumnName("EffectiveDateTo");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsIASDownloaded).IsRequired().HasColumnName("IsIASDownloaded");
            this.Property(t => t.IsErrorLogDownloaded).IsRequired().HasColumnName("IsErrorLogDownloaded");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.GlobalUpdates)
                .HasForeignKey(d => d.TenantID);

            this.HasRequired(t => t.GlobalUpdateStatus)
                .WithMany(t => t.GlobalUpdates)
                .HasForeignKey(d => d.GlobalUpdateStatusID);

            this.HasRequired(t => t.iasWizardStep)
              .WithMany(t => t.GlobalUpdates)
              .HasForeignKey(d => d.IASWizardStepID);
        }
    }
}
