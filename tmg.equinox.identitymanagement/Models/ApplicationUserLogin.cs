using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Script.Serialization;

namespace tmg.equinox.identitymanagement.Models
{
    /*[NotMapped]*/
    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
        
        public virtual ApplicationUser User { get; set; }        
    }
}