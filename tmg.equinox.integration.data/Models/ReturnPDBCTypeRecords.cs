using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnPDBCTypeRecords : Entity
    {
        public string PDBC_TYPE { get; set; }
        public string PDBC_DESCRIPTION { get; set; }
    }
}     
