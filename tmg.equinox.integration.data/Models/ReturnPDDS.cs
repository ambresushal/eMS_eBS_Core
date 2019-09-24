using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnPDDS : Entity
    {
        public Int64 PDDSId { get; set; }
        public string PDPD_ID { get; set; }
        public string PDDS_DESC { get; set; }
        public string PDDS_PROD_TYPE { get; set; }
        public string PDDS_UM_IND { get; set; }
    }
    public class hover : Entity
    {
        public string ListName { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class PDPDDao : Entity
    {
        public string PDPDID { get; set; }
        public string ProductDescription { get; set; }
        public string ProductType { get; set; }
        public int? ProcessGovernance1Up { get; set; } 
        public DateTime PDPD_EFF_DT { get; set; }

    }
}
