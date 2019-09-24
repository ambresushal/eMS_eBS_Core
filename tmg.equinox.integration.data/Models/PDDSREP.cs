using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class PDDSREP:Entity
    {
        public string PDPD_ID { get; set; }
        public string PDDS_DESC { get; set; }
        public string PDDS_UM_IND { get; set; }
        public string PDDS_MED_PRICE_IND { get; set; }
        public string PDDS_MED_CLMS_IND { get; set; }
        public string PDDS_DEN_UM_IND { get; set; }
        public string PDDS_DEN_PD_IND { get; set; }
        public string PDDS_DEN_PRICE_IND { get; set; }
        public string PDDS_DEN_CLMS_IND { get; set; }
        public string PDDS_PREM_IND { get; set; }
        public string PDDS_CLED_IND { get; set; }
        public string PDDS_CAP_IND { get; set; }
        public string PDDS_INT_STATE_IND { get; set; }
        public string PDDS_MCTR_BCAT { get; set; }
        public string PDDS_MCTR_VAL1 { get; set; }
        public string PDDS_MCTR_VAL2 { get; set; }
        public string PDDS_APP_TYPE { get; set; }
        public string PDDS_PROD_TYPE { get; set; }
        public string PDDS_DOFR_IND { get; set; }
        public string PDDS_OPTOUT_IND { get; set; }
        public int PDDS_LOCK_TOKEN { get; set; }
        public System.DateTime ATXR_SOURCE_ID { get; set; }
        public Nullable<System.DateTime> SYS_LAST_UPD_DTM { get; set; }
        public string SYS_USUS_ID { get; set; }
        public string SYS_DBUSER_ID { get; set; }
        public string PDDS_OOA_IND { get; set; }
        public string PDDS_DISP_IND { get; set; }
        public string PDDS_ALT_DISP_IND { get; set; }
        public string PDDS_ORD_SYS_IND { get; set; }
        public string BatchID { get; set; }
        public string Hashcode { get; set; }
    }
}
