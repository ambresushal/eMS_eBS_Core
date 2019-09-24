using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class JournalActionMap: EntityTypeConfiguration<JournalAction>
    {
        public JournalActionMap()
        {
            // Primary Key
            this.HasKey(t => t.ActionID);

            // Properties
            this.Property(t => t.AddedBy)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UpdatedBy)
                .HasMaxLength(20);

            this.Property(t => t.ActionName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Action", "Fldr");
            this.Property(t => t.ActionID).HasColumnName("ActionID");
            this.Property(t => t.ActionName).HasColumnName("ActionName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
           
              
        }
    }
}
