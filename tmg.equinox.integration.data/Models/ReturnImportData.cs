using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnImportData : Entity
    {
        public string TableName { get; set; }
        public string IsRecordPresent { get; set; }
        public string TableNameDisplayText { get; set; }
    }
}
