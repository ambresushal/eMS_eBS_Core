using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BenefitReviewSearch : Entity
    {

        public BenefitReviewSearch()
        {
            ShowCopayinGrid = "True";
            ShowCoinsuranceinGrid = "True";
            ShowAllowedAmtinGrid = "True";
            ShowAllowedCtrinGrid = "True";
            ShowDeductiblesinGrid = "True";
            ShowLimitsinGrid = "True";
            ShowMessagesinGrid = "True";
            ShowPenaltyinGrid = "True";
        }

        public string ShowCopayinGrid { get; set; }
        public string ShowCoinsuranceinGrid { get; set; }
        public string ShowAllowedAmtinGrid { get; set; }
        public string ShowAllowedCtrinGrid { get; set; }
        public string ShowDeductiblesinGrid { get; set; }
        public string ShowLimitsinGrid { get; set; }
        public string ShowMessagesinGrid { get; set; }
        public string ShowPenaltyinGrid { get; set; }
    }
}
