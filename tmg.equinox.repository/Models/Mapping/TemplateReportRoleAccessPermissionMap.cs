using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class TemplateReportRoleAccessPermissionMap : EntityTypeConfiguration<TemplateReportRoleAccessPermission>
    {
        public TemplateReportRoleAccessPermissionMap()
        {
            //Primary Key
            this.HasKey(t => t.TemplateReportPermissionID);

            //Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TemplateReportRoleAccessPermission", "TmplRpt");
            this.Property(t => t.TemplateReportPermissionID).HasColumnName("TemplateReportPermissionID");
            this.Property(t => t.TemplateReportVersionID).HasColumnName("TemplateReportVersionID");
            this.Property(t => t.RoleID).HasColumnName("RoleID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");

            this.HasRequired(t => t.UserRole)
                    .WithMany(t => t.TemplateReportRoleAccessPermissions)
                    .HasForeignKey(d => d.RoleID);

            this.HasRequired(t => t.TemplateReportVersion)
                    .WithMany(t => t.TemplateReportRoleAccessPermissions)
                    .HasForeignKey(d => d.TemplateReportVersionID);

        }
    }
}
