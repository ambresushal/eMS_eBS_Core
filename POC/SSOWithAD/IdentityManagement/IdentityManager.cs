using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement
{
    public static class IdentityManager
    {
        private static ActiveDirectoryAuthProvider authProvider { get; set; }

        public static string GetRole(string userName)
        {
            authProvider = new ActiveDirectoryAuthProvider();
            return authProvider.GetRole(userName);
        }
    }
}
