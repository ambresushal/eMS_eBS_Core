﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class SETRREP:Entity
    {
        public int SETRId { get; set; }
        public string SESE_ID { get; set; }
        public string SESE_RULE { get; set; }
        public short SETR_TIER_NO { get; set; }
        public decimal SETR_ALLOW_AMT { get; set; }
        public short SETR_ALLOW_CTR { get; set; }
        public decimal SETR_COPAY_AMT { get; set; }
        public decimal SETR_COIN_PCT { get; set; }
        public short ACAC_ACC_NO { get; set; }
        public string SETR_OPTS { get; set; }
        public short SETR_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string Hashcode { get; set; }
        
    }
}
