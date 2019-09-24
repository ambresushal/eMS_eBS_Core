using System;

namespace tmg.equinox.integration.data.Models
{
    public partial class ACDEMaster:Entity
    {
        public string ACDE_ACC_TYPE { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string ACDE_DESC { get; set; }
        public Int16 acde_LOCK_TOKEN { get; set; }
        public DateTime ATXR_SOURCE_ID { get; set; }               
        public string BatchID{get;set;}
        public string acronym{get;set;}
    }
}
