using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class AccountFolderMapMap : EntityTypeConfiguration<AccountFolderMap>
    {
        public AccountFolderMapMap()
        {
            // Primary Key
            this.HasKey(t => t.AccountFolderMapID);

            // Table & Column Mappings
            this.ToTable("AccountFolderMap", "Accn");
            this.Property(t => t.AccountFolderMapID).HasColumnName("AccountFolderMapID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.AccountID).HasColumnName("AccountID");

            // Relationships
            this.HasRequired(t => t.Folder)
                .WithMany(t => t.AccountFolderMaps)
                .HasForeignKey(d => d.FolderID);
            this.HasRequired(t => t.Account)
                .WithMany(t => t.AccountFolderMaps)
                .HasForeignKey(d => d.AccountID);
        }
    }
}
