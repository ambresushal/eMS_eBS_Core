using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.identitymanagement.Models
{
    public class ChangePasswordModel
    {
        public int UserId { get; set; }

        [Required]
		[Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(@"^.*(?=.{8})(?!.{13})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage = "Password should be of Min.8 and Max.12 characters with at least one Uppercase,Lowercase,Number and one Special Character.")]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [RegularExpression(@"^.*(?=.{8})(?!.{13})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage = "Password should be of Min.8 and Max.12 characters with at least one Uppercase,Lowercase,Number and one Special Character.")]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
