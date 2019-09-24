using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManagement
{
    public class ActiveDirectoryStore
    {

        private static IList<ActiveDirective> GetADList()
        {
            IList<ActiveDirective> _ADlist = new List<ActiveDirective>();
            _ADlist.Add(new ActiveDirective("Viewer", "eBenSnc_PRD_PSOT_BNF_VWR"));
            _ADlist.Add(new ActiveDirective("Viewer", "eBenSnc_UAT_PSOT_BNF_VWR"));
            _ADlist.Add(new ActiveDirective("Product SME", "eBenSnc_UAT_PSOT_PRO_SME"));
            _ADlist.Add(new ActiveDirective("Product SME", "eBenSnc_PRD_PSOT_PRO_SME"));
            _ADlist.Add(new ActiveDirective("Product Designer Level 2", "eBenSnc_UAT_PSOT_PRO_DS_LV2"));
            _ADlist.Add(new ActiveDirective("Product Designer Level 2", "eBenSnc_PRD_PSOT_PRO_DS_LV2"));
            _ADlist.Add(new ActiveDirective("Product Designer Level 1", "eBenSnc_UAT_PSOT_PRO_DS_LV1"));
            _ADlist.Add(new ActiveDirective("Product Designer Level 1", "eBenSnc_PRD_PSOT_PRO_DS_LV1"));
            _ADlist.Add(new ActiveDirective("TMG SuperUser", "eBenSnc_UAT_PSOT_TG_SPR_USR"));
            _ADlist.Add(new ActiveDirective("TMG SuperUser", "eBenSnc_PRD_PSOT_TG_SPR_USR"));
            _ADlist.Add(new ActiveDirective("WC SuperUser", "eBenSnc_UAT_PSOT_WC_SPR_USR"));
            _ADlist.Add(new ActiveDirective("WC SuperUser", "eBenSnc_PRD_PSOT_WC_SPR_USR"));

            return _ADlist;
        }

        public static string GetRoleName(List<string> authGroups)
        {
            string roleName = string.Empty;

            var seletedRoleName = GetADList().Where(y => authGroups.Any(x => x.Contains(y.RoleName.ToLower()))).FirstOrDefault();
            if (seletedRoleName != null)
                roleName = seletedRoleName.RoleValue;

            return roleName;
        }
    }

    public class ActiveDirective
    {
        public string RoleName { get; set; }
        public string RoleValue { get; set; }
        public ActiveDirective(string roleValue, string roleName)
        {
            RoleName = roleName;
            RoleValue = roleValue;
        }

    }
}
