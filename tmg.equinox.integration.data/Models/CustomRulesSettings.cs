using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.facet.data.Models
{
    public class CustomRulesSettings
    {

        public CustomRulesSettings()
        {
            IsBenefitSetGridLock = "Yes";
        }

        public string IsBenefitSetGridLock { get; set; }
    }
}
