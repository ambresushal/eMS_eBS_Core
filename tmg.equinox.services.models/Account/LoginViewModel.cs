using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int UserId { get; set; }

        public List<string> StartIN { get; set; }

        public List<RolesViewModel> Roles { get; set; }

    }

    public class LandingPage
    {
        public static List<string> StartUpList;
        static LandingPage()
        {
            StartUpList = new List<string>();
            StartUpList.Add("Dashboard");
            StartUpList.Add("Portfolio Search");
            StartUpList.Add("Account Search");
        }
    }

    public class RolesViewModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public List<ClaimsViewModel> Claims { get; set; }

    }

    public class ClaimsViewModel
    {
        public int UserId { get; set; }
        public int ClaimId { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }
    }
}
