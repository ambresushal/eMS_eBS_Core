using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class JournalResponseMap:EntityTypeConfiguration<JournalResponse>
    {
        public JournalResponseMap()
        {
            // Primary Key
            this.HasKey(t => t.JournalResponseID);

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
            this.ToTable("JournalResponse", "Fldr");
            this.Property(t => t.JournalResponseID).HasColumnName("JournalResponseID");
            this.Property(t => t.JournalID).HasColumnName("JournalID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedBy).HasColumnName("UpdatedBy");

            // Relationships
            this.HasRequired(t => t.Journal)
                .WithMany(t => t.JournalResponses)
                .HasForeignKey(d => d.JournalID);

        }
    }
}
