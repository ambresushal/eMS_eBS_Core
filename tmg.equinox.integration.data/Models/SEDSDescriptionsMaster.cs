using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class SEDSDescriptionsMaster : Entity
    {
        public string SESE_ID { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceOfService { get; set; }
        public bool isActive { get; set; }
    }
}
