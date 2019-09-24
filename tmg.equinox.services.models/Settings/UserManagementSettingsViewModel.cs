using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class UserManagementSettingsViewModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsVisible { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool ChangeInitialPassword { get; set; }
    }

    public class UserHubViewModel
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }


    public class RoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
