using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnSERVICEGROUPINGS:Entity
    {
        public int ServiceGroupID { get; set; }
        public string PDDS_PROD_TYPE { get; set; }
        public string ServiceGroupName { get; set; }
        public string SESE_ID { get; set; }
    }
}
