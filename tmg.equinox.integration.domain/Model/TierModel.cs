using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.domain.Models
{
    public class TierModel
    {
        public List<TierDataModel> TierDataModelList = new List<TierDataModel>();
        public char TypeIndicator { get; set; }
        public string SERLId { get; set; }
        public string AccumNumber { get; set; }
        public string MessageDescription { get; set; }
    }

    public class TierDataModel
    {
        public string AllowedAmount { get; set; }
        public string AllowedCounter { get; set; }
        public string Coinsurance { get; set; }
        public string Copay { get; set; }
        public string TierNo { get; set; } 

    }
}
