﻿using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ServiceTierOtherData : Entity
    {
        public string PDPD_ID { get; set; }
        public string BenefitSet { get; set; }
        public string SESE_ID { get; set; }
        public Int16 SETR_TIER_No { get; set; }
        public decimal SETR_COPAY_AMT { get; set; }
        public decimal SETR_COIN_PCT { get; set; }
        public decimal SETR_ALLOW_AMT { get; set; }
        public Int16 SETR_ALLOW_CTR { get; set; }
        public Int16 ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string POS { get; set; }
        public int ProcessGovernance1up { get; set; }
        public string SESP_PEN_IND { get; set; }
        public string SESP_PEN_CALC_IND { get; set; }
        public decimal? SESP_PEN_AMT { get; set; }
        public decimal? SESP_PEN_PCT { get; set; }
        public decimal? SESP_PEN_MAX_AMT { get; set; }
        public string EXCD_ID { get; set; }
        public string SESE_COV_TYPE { get; set; }
        public string SERL_DESC { get; set; }
        public string SESE_DESC { get; set; }

        public ServiceTierOtherData Clone()
        {
            return this.MemberwiseClone() as ServiceTierOtherData;
        }
    }
}