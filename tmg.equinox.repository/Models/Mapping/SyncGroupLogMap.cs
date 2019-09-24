using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SyncGroupLogMap : EntityTypeConfiguration<SyncGroupLog>
    {
        public SyncGroupLogMap()
        {
            // Primary Key
            this.HasKey(t => t.SyncGroupLogID);

            this.Property(t => t.SyncBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Notes)
                .HasMaxLength(2000);


            // Table & Column Mappings
            this.ToTable("SyncGroupLog", "sync");
            this.Property(t => t.SyncGroupLogID).HasColumnName("SyncGroupLogID");
            this.Property(t => t.SyncBy).HasColumnName("SyncBy");
            this.Property(t => t.SyncDate).HasColumnName("SyncDate");
            this.Property(t => t.MacroID).HasColumnName("MacroID");
            this.Property(t => t.SourceDocumentID).HasColumnName("SourceDocumentID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.FolderVersionNumber).HasColumnName("FolderVersionNumber");
            this.Property(t => t.LastUpdatedDate).HasColumnName("LastUpdatedDate");
            this.Property(t => t.Notes).HasColumnName("Notes");
            // Relationships
            this.HasRequired(t => t.SyncDocumentMacro)
                .WithMany(t => t.SyncGroupLogs)
                .HasForeignKey(d => d.MacroID);
        }
    }
}
