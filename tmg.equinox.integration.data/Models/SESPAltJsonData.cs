using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class SESPAltJsonData : Entity
    {
        public string ProductID { get; set; }
        public string SESE_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public string SESP_PEN_AMT { get; set; }
        public float SESP_PEN_PCT { get; set; }
        public decimal SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SERLMessage { get; set; }
        public string DisallowedMessage { get; set; }
        public string IsCovered { get; set; }
    }
}
