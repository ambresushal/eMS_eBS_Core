using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class DPFPlanTaskUserMappingMap : EntityTypeConfiguration<DPFPlanTaskUserMapping>
    {
        public DPFPlanTaskUserMappingMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            // Table & Column Mappings
            this.ToTable("PlanTaskUserMapping", "DPF");
            this.Property(t => t.ID).HasColumnName("ID");
            //this.Property(t => t.FormInstanceId).HasColumnName("FormInstanceId");
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.AssignedDate).HasColumnName("AssignedDate");
            this.Property(t => t.AssignedUserName).HasColumnName("AssignedUserName");
            this.Property(t => t.ManagerUserName).HasColumnName("ManagerUserName");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.DueDate).HasColumnName("DueDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.CompletedDate).HasColumnName("CompletedDate");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.MarkInterested).HasColumnName("MarkInterested");
            this.Property(t => t.LateStatusDone).HasColumnName("LateStatusDone");
            //this.Property(t => t.ViewID).HasColumnName("ViewID");
            //this.Property(t => t.SectionID).HasColumnName("SectionID");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.Duration).HasColumnName("Duration");
            //this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.PlanTaskUserMappingDetails).HasColumnName("PlanTaskUserMappingDetailsJSON");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.EstimatedTime).HasColumnName("EstimatedTime");
            this.Property(t => t.ActualTime).HasColumnName("ActualTime");
          
            // Relationships

            //this.HasRequired(t => t.FormInstances)
            //  .WithMany(t => t.DPFPlanTaskUserMappings)
            //  .HasForeignKey(d => d.FormInstanceId);

            this.HasRequired(t => t.WorkFlowStateMasters)
            .WithMany(t => t.DPFPlanTaskUserMappings)
            .HasForeignKey(d => d.WFStateID);

            this.HasRequired(t => t.WorkflowTaskMaps)
           .WithMany(t => t.DPFPlanTaskUserMappings)
           .HasForeignKey(d => d.TaskID);

            this.HasRequired(t => t.FolderVersionMap)
           .WithMany(t => t.DPFPlanTaskUserMappings)
           .HasForeignKey(d => d.FolderVersionID);

            // this.HasRequired(t => t.UIElementMaps)
            //.WithMany(t => t.DPFPlanTaskUserMappings)
            //.HasForeignKey(d => d.SectionID);

            // this.HasRequired(t => t.TaskCommentsMaps)
            //.WithMany(t => t.DPFPlanTaskUserMappings)
            //.HasForeignKey(d => d.Comments);
        }
    }
}
