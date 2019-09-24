using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLTSERecords : Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string LTLT_DESC { get; set; }
        public string SESE_ID { get; set; }
        public int LTSE_WT_CTR { get; set; }
        public string SEDS_DESC { get; set; }
        public string ACDE_DESC { get; set; }
        public string BENEFIT_CATEGORY1 { get; set; }//BenefitCategory1
        public string BENEFIT_CATEGORY2 { get; set; }//BenefitCategory2
        public string BENEFIT_CATEGORY3 { get; set; }//BenefitCategory3
        public string POS { get; set; }
        public string BenefitSetName { get; set; }
    }
}
