using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class ATXRMaster : Entity
    {
        public DateTime ATXR_SOURCE_ID { get; set; }
        public string ATSY_ID { get; set; }
        public DateTime ATXR_DEST_ID { get; set; }
        public string ATTB_ID { get; set; }
        public string ATTB_TYPE { get; set; }
        public string ATXR_DESC { get; set; }
        public DateTime ATXR_CREATE_DT { get; set; }
        public Nullable<DateTime> ATXR_LAST_UPD_DT { get; set; }             
    }
}
