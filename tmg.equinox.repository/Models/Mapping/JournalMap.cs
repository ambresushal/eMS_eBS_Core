using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class JournalMap : EntityTypeConfiguration<Journal>
    {
        public JournalMap()
        {
            // Primary Key
            this.HasKey(t => t.JournalID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("Journal", "Fldr");
            this.Property(t => t.JournalID).HasColumnName("JournalID");
            this.Property(t => t.FormInstanceID).HasColumnName("FormInstanceID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.FieldPath).HasColumnName("FieldPath");
            this.Property(t => t.ActionID).HasColumnName("ActionID");
            this.Property(t => t.AddedWFStateID).HasColumnName("AddedWFStateID");
            this.Property(t => t.ClosedWFStateID).HasColumnName("ClosedWFStateID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.FormInstance)
                .WithMany(t => t.Journals)
                .HasForeignKey(d => d.FormInstanceID);
            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.Journals)
                .HasForeignKey(d => d.FolderVersionID);
            this.HasRequired(t => t.JournalAction)
                .WithMany(t => t.Journals)
                .HasForeignKey(d => d.ActionID);
            this.HasRequired(t => t.AddedWorkFlowState)
                .WithMany(t => t.Journals)
                .HasForeignKey(d => d.AddedWFStateID);
            this.HasOptional(t => t.ClosedWorkFlowState)
                .WithMany(t => t.Journals1)
                .HasForeignKey(d => d.ClosedWFStateID);

        }
    }
}
