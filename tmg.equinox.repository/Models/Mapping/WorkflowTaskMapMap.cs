using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class WorkflowTaskMapMap : EntityTypeConfiguration<WorkflowTaskMap>
    {
        public WorkflowTaskMapMap()
        {
            // Primary Key
            this.HasKey(t => t.WFStateTaskID);

            this.Property(t => t.AddedBy)
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);
            // Table & Column Mappings

            this.ToTable("WFStateTaskMapping", "DPF");
            this.Property(t => t.WFStateTaskID).HasColumnName("WFStateTaskID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.WFStateID).HasColumnName("WFStateID");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive");


            // Relationships
            this.HasRequired(t => t.WorkFlowStateMaster)
                .WithMany(t => t.applicableWorkflowTaskMap)
                .HasForeignKey(d => d.WFStateID);

            this.HasRequired(t => t.Tasklist)
                .WithMany(t => t.applicableWorkflowTaskMap)
                .HasForeignKey(d => d.TaskID);

        }
    }
}
