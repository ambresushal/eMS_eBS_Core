using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class WorkFlowStateGroupMap : EntityTypeConfiguration<WorkFlowStateGroup>
    {
        public WorkFlowStateGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.WFStateGroupID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.WFStateGroupName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("WorkFlowStateGroup", "Fldr");
            this.Property(t => t.WFStateGroupID).HasColumnName("WFStateGroupID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.WFStateGroupName).HasColumnName("WFStateGroupName");

            // Relationships
            this.HasRequired(t => t.Tenant)
                .WithMany(t => t.WorkFlowStateGroups)
                .HasForeignKey(d => d.TenantID);

        }
    }
}
