using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SyncDocumentLogMap : EntityTypeConfiguration<SyncDocumentLog>
    {
        public SyncDocumentLogMap()
        {
            // Primary Key
            this.HasKey(t => t.SyncDocumentLogID);

            this.Property(t => t.Notes)
            .HasMaxLength(2000);


            // Table & Column Mappings
            this.ToTable("SyncDocumentLog", "sync");
            this.Property(t => t.SyncDocumentLogID).HasColumnName("SyncDocumentLogID");
            this.Property(t => t.SyncGroupLogID).HasColumnName("SyncGroupLogID");
            this.Property(t => t.SourceDocumentID).HasColumnName("SourceDocumentID");
            this.Property(t => t.TargetDocumentID).HasColumnName("TargetDocumentID");
            this.Property(t => t.TargetDesignVersionID).HasColumnName("TargetDesignVersionID");
            this.Property(t => t.TargetDesignID).HasColumnName("TargetDesignID");
            this.Property(t => t.IsSyncAllowed).HasColumnName("IsSyncAllowed");
            this.Property(t => t.TargetFormDataBU).HasColumnName("TargetFormDataBU");
            this.Property(t => t.SyncCompleted).HasColumnName("SyncCompleted");
            this.Property(t => t.SyncJSONData).HasColumnName("SyncJSONData");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.LastUpdatedDate).HasColumnName("LastUpdatedDate");
            this.Property(t => t.Notes).HasColumnName("Notes");            
            // Relationships
            this.HasRequired(t => t.SyncGroupLog)
                .WithMany(t => t.SyncDocumentLogs)
                .HasForeignKey(d => d.SyncGroupLogID);        
        }
    }
}
