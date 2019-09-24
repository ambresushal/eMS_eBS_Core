using System;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
   public class NotificationstatusMap:EntityTypeConfiguration<Notificationstatus>
    {
        public NotificationstatusMap()
        {
            this.HasKey(t => t.ID);
            // Table & Column Mappings
            this.ToTable("NotificationStatus", "Setup");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.MessageKey).HasColumnName("MessageKey");
            this.Property(t => t.Userid).HasColumnName("UserId");
            this.Property(t => t.IsRead).HasColumnName("IsRead");

            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");

            // Relationships
            //this.HasRequired(t => t)
            //    .WithMany(t => t.AccessFilesMap)
            //    .HasForeignKey(d => d.BatchId);


        }
    }
}
