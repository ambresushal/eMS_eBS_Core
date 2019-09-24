using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataCopyMode : Entity
    {
        public DataCopyMode()
        {
            this.DataSourceMappings = new List<DataSourceMapping>();
        }

        public int DataCopyModeID { get; set; }
        public string CopyData { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
    }
}
