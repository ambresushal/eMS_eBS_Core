using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentMap:Entity
    {
        public DocumentMap()
        {
            this.DocumentMapDetails = new List<DocumentMapDetails>();
        }
        public int MappingID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; } 
        public int TenantID { get; set; }
        public string SourceMapTable { get; set; }
        public string DestinationMapTable { get; set; }
        public virtual List<DocumentMapDetails> DocumentMapDetails { get; set; }
    }
}
