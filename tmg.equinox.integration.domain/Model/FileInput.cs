using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.domain.Models
{
    public class FileInputViewModel
    {
        public Nullable<int> ProductId { get; set; }
        public string PluginVersion { get; set; }
        public string Plugin { get; set; }
        public string OutPutFormat { get; set; }

    }
}
