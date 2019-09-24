using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class AccountFolderCreationPermission : Entity
    {
        public AccountFolderCreationPermission()
        {
        }

        public int AccountFolderCreationPermissionID { get; set; }
        public int UserRoleID { get; set; }
        public int AccountType { get; set; }
        public bool HasAccountCreationPermission { get; set; }
        public bool HasFolderVersionCreationPermission { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
    }
}
