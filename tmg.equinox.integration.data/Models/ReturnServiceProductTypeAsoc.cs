using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnServiceProductTypeAsoc : Entity
    {
        public string SESE_ID { get; set; }
        public string PDDS_PROD_TYPE { get; set; }
    }
}
