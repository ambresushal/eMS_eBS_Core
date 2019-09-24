using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public class ProductCategoryLOBData:Entity
    {
        public string Category { set; get; }
        public string LOBD_ID { set; get; }
    }
}
