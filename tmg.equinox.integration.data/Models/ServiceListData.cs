using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ServiceListData : Entity
    {
        public string SESE_ID { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceOfService { get; set; }
        public int ProcessGovernance1up { get; set; }

        public ServiceListData Clone()
        {
            return this.MemberwiseClone() as ServiceListData;
        }
    }
}
