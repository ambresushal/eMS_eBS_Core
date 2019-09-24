using System;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class CodeDescription : Entity
    {
        public int CodeDescriptionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Section { get; set; }
        public string Dropdown { get; set; }
        public string CodeDesc { get; set; }
    }
}
