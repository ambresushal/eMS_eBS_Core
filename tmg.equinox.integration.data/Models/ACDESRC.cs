using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ACDESRC : Entity
    {
        public string ACDE_ACC_TYPE { get; set; }
        public short ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public short ACDE_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public int ProcessGovernance1up { get; set; }
        public int Action { get; set; }
        public string Acronym { get; set; }
               
    }
}
