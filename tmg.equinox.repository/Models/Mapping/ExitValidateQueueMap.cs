using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class ExitValidateQueueMap : EntityTypeConfiguration<ExitValidateQueue>
    {
        public ExitValidateQueueMap()
        {
            // Primary Key
            this.HasKey(t => t.ExitValidateQueueID);

            // Properties
            this.Property(t => t.Status)
                .HasMaxLength(100);
            this.Property(t => t.ProductID)
                .HasMaxLength(50);
            this.Property(t => t.Sections)
                .HasMaxLength(1000);
            this.Property(t => t.ExitValidateFilePath)
                .HasMaxLength(500);
            this.Property(t => t.PlanAreaFilePath)
                .HasMaxLength(500);
            this.Property(t => t.VBIDFilePath)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("ExitValidateQueue", "Setup");
            this.Property(t => t.ExitValidateQueueID).HasColumnName("ExitValidateQueueID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.JobId).HasColumnName("JobId");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.ProductID).HasColumnName("ProductID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Sections).HasColumnName("Sections");
            this.Property(t => t.SetAsDefault).HasColumnName("SetAsDefault");
            this.Property(t => t.ExitValidateFilePath).HasColumnName("ExitValidateFilePath");
            this.Property(t => t.PlanAreaFilePath).HasColumnName("PlanAreaFilePath");
            this.Property(t => t.VBIDFilePath).HasColumnName("VBIDFilePath");
            this.Property(t => t.ErrorMessage).HasColumnName("ErrorMessage");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.CompletedDate).HasColumnName("CompletedDate");
            this.Property(t => t.IsLatest).HasColumnName("IsLatest");
            this.Property(t => t.IsNotificationSent).HasColumnName("IsNotificationSent");
            this.Property(t => t.IsQueuedForWFStateUpdate).HasColumnName("IsQueuedForWFStateUpdate");
            this.Property(t => t.UsersInterestedInStatus).HasColumnName("UsersInterestedInStatus");
        }
    }
}
