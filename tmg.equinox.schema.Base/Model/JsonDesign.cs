using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.schema.Base.Model
{
    public class JsonDesign
    {
        public int JsonDesignId { get; set; }
        public int JsonDesignVersionId { get; set; }
        public string JsonDesignData { get; set; }
        public string TableSchemaName { get; set; }
        public string TableLabel { get; set; }
        public string TableDescription { get; set; }
        public string TableDesignType { get; set; }
        public int PreviousJsonDesignVersionId { get; set; }
        public string VersionNumber { get; set; }

    }
}
