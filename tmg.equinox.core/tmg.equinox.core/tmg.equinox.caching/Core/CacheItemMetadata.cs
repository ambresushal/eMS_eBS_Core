using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching
{
    public class CacheItemMetadata
    {
        public Byte[] Key { get; set; }
        public DateTime LastAccessed { get; set; }
        public long Size { get; set; }
        public string Domain { get; set; }
    }
}
