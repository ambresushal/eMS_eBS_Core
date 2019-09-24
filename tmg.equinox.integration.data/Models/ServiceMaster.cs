using System;

namespace tmg.equinox.integration.data.Models
{
    public partial class ServiceMaster:Entity
    {        
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public string SESEHashCode { get; set; }
        public string SETRHashCode { get; set; }
        public string SESPHashCode { get; set; }
        public string SESERuleCategory { get; set; }       
    }
}
