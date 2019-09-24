using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FolderVersionWorkFlowStateMap : EntityTypeConfiguration<FolderVersionWorkFlowState>
    {
        public FolderVersionWorkFlowStateMap()
        {
            // Primary Key
            this.HasKey(t => t.FVWFStateID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Comments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("FolderVersionWorkFlowState", "Fldr");
            this.Property(t => t.FVWFStateID).HasColumnName("FVWFStateID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.ApprovalStatusID).HasColumnName("ApprovalStatusID");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.UserID).HasColumnName("UserID");

            // Relationships
            this.HasRequired(t => t.ApprovalStatusType)
                .WithMany(t => t.FolderVersionWorkFlowStates)
                .HasForeignKey(d => d.ApprovalStatusID);
            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.FolderVersionWorkFlowStates)
                .HasForeignKey(d => d.FolderVersionID);
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.FolderVersionWorkFlowStates)
                .HasForeignKey(d => d.TenantID);
            this.HasOptional(t => t.User)
                .WithMany(t => t.FolderVersionWorkFlowStates)
                .HasForeignKey(d => d.UserID);
            this.HasRequired(t => t.WorkFlowVersionState)
                .WithMany(t => t.FolderVersionWorkFlowStates)
                .HasForeignKey(d => d.WFStateID);

        }
    }
}
