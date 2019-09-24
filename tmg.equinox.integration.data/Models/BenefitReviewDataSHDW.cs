using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class BenefitReviewDataSHDW
    {
        public string SESE_ID { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }        

        public Dictionary<string, BenefitReviewDetails> networkList { get; set; }

        public BenefitReviewDataSHDW()
        {
            networkList = new Dictionary<string, BenefitReviewDetails>();
        }

        public BenefitReviewDataSHDW Clone()
        {
            return this.MemberwiseClone() as BenefitReviewDataSHDW;
        }
    }
}
