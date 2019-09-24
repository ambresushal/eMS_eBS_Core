using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class PBPMatchConfigMap : EntityTypeConfiguration<PBPMatchConfig>
    {
        public PBPMatchConfigMap()
        {
            // Primary Key
            this.HasKey(t => t.PBPMatchConfig1Up);

            this.Property(t => t.PlanName)
                .HasMaxLength(400);

            this.Property(t => t.PlanNumber)
                .HasMaxLength(400);
            this.Property(t => t.ebsPlanName)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("PBPMatchConfig", "PBP");
            this.Property(t => t.PBPMatchConfig1Up).HasColumnName("PBPMatchConfig1Up");
            this.Property(t => t.PBPImportQueueID).HasColumnName("PBPImportQueueID");
            this.Property(t => t.QID).HasColumnName("QID");
            this.Property(t => t.PlanName).HasColumnName("PlanName");
            this.Property(t => t.PlanNumber).HasColumnName("PlanNumber");
            this.Property(t => t.ebsPlanName).HasColumnName("ebsPlanName");
            this.Property(t => t.ebsPlanNumber).HasColumnName("ebsPlanNumber");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.DocId).HasColumnName("DocId");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.FolderId).HasColumnName("FolderId");
            this.Property(t => t.FolderVersionId).HasColumnName("FolderVersionId");
            this.Property(t => t.UserAction).HasColumnName("UserAction");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsIncludeInEbs).HasColumnName("IsIncludeInEbs");
            this.Property(t => t.IsTerminateVisible).HasColumnName("IsTerminateVisible");
            this.Property(t => t.IsProxyUsed).HasColumnName("IsProxyUsed");
            this.Property(t => t.IsEGWPPlan).HasColumnName("IsEGWPPlan");
            // Relationships
        }
    }
}
