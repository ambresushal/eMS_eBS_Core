using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnProductType : Entity
    {
        public int PRIORITY_INDEX { get; set; } 
        public string PDDS_PROD_TYPE { get; set; }       
    }
}
