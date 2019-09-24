using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnLimitRecords:Entity
    {
        public int LTLT_DISTINCT_ID { get; set; }
        public string LIMITNAME { get; set; }
        public string LIMITDESCRIPTION { get; set; }
        public string LIMITCATEGORY { get; set; }
        public string LIMITCOVERAGELEVEL { get; set; }
        public string LIMITTILEPERIOD { get; set; }                 
        public string ISBENEFITAMOUNTINDOLLARSFLAG { get; set; }
        public string ISBENEFITAMOUNTINPERCENTFLAG { get; set; }
        public string BENEFITAMOUNT { get; set; }                   
        public string REINSTATEMENTAMOUNT { get; set; }             
        public char ISAMOUNTSETINBENEFITFLAG { get; set; }
        public string QUANTITYFROM { get; set; }                    
        public string QUANTITYTO { get; set; }                      
        public string QUANTITYQUALIFIER { get; set; }               
        public string SEQUQENCEORDER { get; set; }                  
        public string ISQUANTITYSETINBENEFITFLAG { get; set; }
        public int ACAC_ACC_NO { get; set; }
        public string LTLT_CAT { get; set; }
        public string LTLT_LEVEL { get; set; } 
        public string LTLT_PERIOD_IND { get; set; } 
        public string LTLT_RULE { get; set; }
        public string LTLT_OPTS { get; set; }
        public string LTLT_IX_IND { get; set; }
        public string LTLT_IX_TYPE { get; set; }
        public string EXCD_ID { get; set; }  
        public string LTLT_SAL_IND { get; set; }
        public int WMDS_SEQ_NO { get; set; }                        
    }
}
