using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.data.Models
{
    public partial class ATNDSRC:Entity
    {
        public string ATSY_ID { get; set; }
        public Nullable<DateTime> ATXR_DEST_ID { get; set; }
        public Nullable<Int16> ATNT_SEQ_NO { get; set; }
        public Nullable<Int16> ATND_SEQ_NO { get; set; }
        public Byte[] ATND_TEXT { get; set; }        
        public string ATXR_Dest_Internal { get; set; }
        public string PDPD_ID { get; set; }
        public int ProcessGovernance1Up { get; set; }
    }
}
