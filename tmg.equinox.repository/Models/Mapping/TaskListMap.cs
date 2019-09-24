using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
  public  class TaskListMap : EntityTypeConfiguration<TaskList>
    {
        public TaskListMap()
        {
            // Primary Key
            this.HasKey(t => t.TaskID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.TaskDescription)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Tasks", "DPF");
            this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.TaskDescription).HasColumnName("TaskDescription");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsStandardTask).HasColumnName("IsStandardTask");
        }
    }
}
