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
    public class TaskCommentsMap : EntityTypeConfiguration<TaskComments>
    {
        public TaskCommentsMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            // Table & Column Mappings
            this.ToTable("TaskComments", "DPF");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.Comments).HasColumnName("Comments");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.filename).HasColumnName("FileName");
            this.Property(t => t.PlanTaskUserMappingState).HasColumnName("PlanTaskUserMappingState");
            // Relationships

            this.HasRequired(t => t.PlanTaskUserMappings)
           .WithMany(t => t.TaskCommentsMappings)
           .HasForeignKey(d => d.TaskID);

            this.HasRequired(t => t.FolderVersionMap)
           .WithMany(t => t.TaskCommentsMappings)
           .HasForeignKey(d => d.FolderVersionID);
        }
    }
}
