using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.domain.entities.Models
{
    public class DocumentMapDetails: Entity
    {
        public DocumentMapDetails()
        {
        }
        public int DocumentMapDetailsID { get; set; }
        public int MappingID { get; set; }
        public string ColumnName { get; set; }
        public string ColumnMapTo { get; set; }
        public DocumentMap DocumentMap { get; set; }
    }
}

