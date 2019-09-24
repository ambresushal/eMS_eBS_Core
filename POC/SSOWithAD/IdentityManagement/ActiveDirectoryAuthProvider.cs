using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace IdentityManagement
{
    public class ActiveDirectoryAuthProvider
    {
        public string GetRole(string userName)
        {
            List<string> groups = null;
            UserPrincipal user = null;
            string userRoleName = string.Empty;

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain))
            {
                user = UserPrincipal.FindByIdentity(pc, userName);
                if (user != null)
                {
                    groups = user.GetGroups().ToList<Principal>().Select(m => m.Name.ToLower()).ToList<string>();
                    userRoleName = ActiveDirectoryStore.GetRoleName(groups);
                }
            }

            return userRoleName;
        }
    }
}
