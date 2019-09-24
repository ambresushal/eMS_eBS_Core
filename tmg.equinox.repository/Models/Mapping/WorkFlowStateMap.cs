using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    //public class WorkFlowStateMap : EntityTypeConfiguration<WorkFlowState>
    //{
    //    public WorkFlowStateMap()
    //    {
    //        // Primary Key
    //        this.HasKey(t => t.WFStateID);

    //        // Properties
    //        this.Property(t => t.AddedBy)
    //            .IsRequired()
    //            .HasMaxLength(20);

    //        this.Property(t => t.UpdatedBy)
    //            .HasMaxLength(20);

    //        this.Property(t => t.WFStateName)
    //            .IsRequired()
    //            .HasMaxLength(100);

    //        // Table & Column Mappings
    //        this.ToTable("WorkFlowState", "Fldr");
    //        this.Property(t => t.WFStateID).HasColumnName("WFStateID");
    //        this.Property(t => t.TenantID).HasColumnName("TenantID");
    //        this.Property(t => t.AddedDate).HasColumnName("AddedDate");
    //        this.Property(t => t.AddedBy).HasColumnName("AddedBy");
    //        this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
    //        this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
    //        this.Property(t => t.WFStateName).HasColumnName("WFStateName");
    //        this.Property(t => t.Sequence).HasColumnName("Sequence");
    //        this.Property(t => t.IsActive).HasColumnName("IsActive");
    //        this.Property(t => t.WFStateGroupID).HasColumnName("WFStateGroupID");
    //        this.Property(t => t.NotApprovedWFStateID).HasColumnName("NotApprovedWFStateID");
    //        this.Property(t => t.ErrorWFStateID).HasColumnName("ErrorWFStateID");

    //        // Relationships
    //        this.HasRequired(t => t.Tenant)
    //            .WithMany(t => t.WorkFlowStates)
    //            .HasForeignKey(d => d.TenantID);
    //        this.HasRequired(t => t.WorkFlowStateGroup)
    //            .WithMany(t => t.WorkFlowStates)
    //            .HasForeignKey(d => d.WFStateGroupID);

    //    }
    //}
}
