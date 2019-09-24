﻿using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ServiceGroupDetailListData : Entity
    {
        public string ServiceGroupHeader { get; set; }
        public string SESE_ID { get; set; }
        public string ServiceDefaultRule { get; set; }
        public string ServiceDefaultAltRule { get; set; }
        public string ServiceModelSESEID { get; set; }
        public string AccumulatorsList { get; set; }
        public string LimitModelSESEID { get; set; }
        public string SESE_RULE_ALT_COND { get; set; }
        public string WeightCounter { get; set; }
        public int ProcessGovernance1up { get; set; }

        public ServiceGroupDetailListData Clone()
        {
            return this.MemberwiseClone() as ServiceGroupDetailListData;
        }
    }
}
