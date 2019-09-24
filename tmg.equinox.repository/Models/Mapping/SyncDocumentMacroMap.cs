using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class SyncDocumentMacroMap : EntityTypeConfiguration<SyncDocumentMacro>
    {
        public SyncDocumentMacroMap()
        {
            // Primary Key
            this.HasKey(t => t.MacroID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MacroName)
                .HasMaxLength(200);

            this.Property(t => t.Notes)
                .HasMaxLength(2000);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SyncDocumentMacro", "sync");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.FormDesignID).HasColumnName("FormDesignID");
            this.Property(t => t.FormDesignVersionID).HasColumnName("FormDesignVersionID");
            this.Property(t => t.isLocked).HasColumnName("isLocked");
            this.Property(t => t.isPublic).HasColumnName("isPublic");
            this.Property(t => t.MacroID).HasColumnName("MacroID");
            this.Property(t => t.MacroJSON).HasColumnName("MacroJSON");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.MacroName).HasColumnName("MacroName");
            this.Property(t => t.Notes).HasColumnName("Notes");


        }
    }
}
