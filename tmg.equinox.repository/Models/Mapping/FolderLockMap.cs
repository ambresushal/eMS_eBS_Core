using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.repository.Models.Mapping
{
    public class FolderLockMap : EntityTypeConfiguration<FolderLock>
    {
        public FolderLockMap()
        {
            //Primary key
            this.HasKey(t => t.FolderLockID);

            // Properties

          
            // Table & Column Mappings
            this.ToTable("FolderLock", "Sec");
            this.Property(t => t.FolderLockID).HasColumnName("FolderLockID");
            this.Property(t => t.FolderID).HasColumnName("FolderID");
            this.Property(t => t.TenantID).HasColumnName("TenantID");
            this.Property(t => t.IsLocked).HasColumnName("IsLocked");
            this.Property(t => t.LockedBy).HasColumnName("LockedBy");
            this.Property(t => t.LockedDate).HasColumnName("LockedDate");
        

            this.HasRequired(t => t.Tenant)
             .WithMany(t => t.FolderLock)
             .HasForeignKey(d => d.TenantID);
            this.HasRequired(t => t.User)
                .WithMany(t => t.FolderLock)
                .HasForeignKey(d => d.LockedBy);
            this.HasRequired(t => t.Folder)
               .WithMany(t => t.FolderLock)
               .HasForeignKey(d => d.FolderID);

        }

    }
}
