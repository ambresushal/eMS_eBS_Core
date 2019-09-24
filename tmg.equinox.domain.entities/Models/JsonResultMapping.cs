using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class JsonResultMapping : Entity
    {
        public string Label { get; set; }
        public string JSONPath { get; set; }
        public string DesignType { get; set; }
        public string FieldName { get; set; }
        public bool IsActive { get; set; }
    }
}
