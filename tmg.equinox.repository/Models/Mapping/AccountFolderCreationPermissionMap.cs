using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class AccountFolderCreationPermissionMap : EntityTypeConfiguration<AccountFolderCreationPermission>
    {
        public AccountFolderCreationPermissionMap()
        {
            this.ToTable("AccountFolderCreationPermission", "Accn");
            this.Property(t => t.AccountFolderCreationPermissionID).HasColumnName("AccountFolderCreationPermissionID");
            this.Property(t => t.UserRoleID).HasColumnName("UserRoleID");
            this.Property(t => t.HasAccountCreationPermission).HasColumnName("HasAccountCreationPermission");
            this.Property(t => t.HasFolderVersionCreationPermission).HasColumnName("HasFolderVersionCreationPermission");
            this.Property(t => t.AddedDate).HasColumnName("AddedDate");
            this.Property(t => t.AddedBy).HasColumnName("AddedBy");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
        }
    }
}
