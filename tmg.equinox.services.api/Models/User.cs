using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Models
{
    public class User
    {
        List<UserInfo> info { get; set; }
    }

    public class UserInfo
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}