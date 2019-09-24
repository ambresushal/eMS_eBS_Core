using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowCategoryMappingMap : EntityTypeConfiguration<WorkFlowCategoryMapping>
    {
        public WorkFlowCategoryMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.WorkFlowVersionID);

            this.ToTable("WorkFlowCategoryMapping", "WF");
            this.Property(t => t.WorkFlowVersionID).HasColumnName("WorkFlowVersionID");
            this.Property(t => t.FolderVersionCategoryID).HasColumnName("FolderVersionCategoryID");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
            this.Property(t => t.WorkFlowType).HasColumnName("WorkFlowType");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.IsFinalized).HasColumnName("IsFinalized");

             //Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.WorkFlowCategoryMapping)
                .HasForeignKey(d => d.TenantID);

            this.HasRequired(t => t.FolderVersionCategory)
                .WithMany(t => t.WorkFlowCategoryMapping)
                .HasForeignKey(d => d.FolderVersionCategoryID);
        }
    }
}
