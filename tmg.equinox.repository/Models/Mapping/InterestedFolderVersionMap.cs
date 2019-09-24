using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class InterestedFolderVersionMap : EntityTypeConfiguration<InterestedFolderVersion>
    {
        public InterestedFolderVersionMap()
        {
            this.HasKey(t => t.InterstedFolderVersionID);

            this.Property(t => t.AddedBy)
               .IsRequired()
               .HasMaxLength(20);
          

            // Table & Column Mappings
            this.ToTable("InterstedFolderVersion", "Fldr");
            this.Property(t => t.InterstedFolderVersionID).HasColumnName("InterstedFolderVersionID");
            this.Property(t => t.FolderVersionID).HasColumnName("FolderVersionID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
           
           
           
            // Relationships
            this.HasRequired(t => t.FolderVersion)
                .WithMany(t => t.InterstedFolderVersions)
                .HasForeignKey(d => d.FolderVersionID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.InterstedFolderVersions)
                .HasForeignKey(d => d.UserID);
           
        }
    }
}
