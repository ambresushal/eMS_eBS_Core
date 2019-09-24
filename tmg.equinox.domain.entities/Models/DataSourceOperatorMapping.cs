using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataSourceOperatorMapping : Entity
    {
        public DataSourceOperatorMapping()
        {
            this.DataSourceMappings = new List<DataSourceMapping>();
        }

        public int DataSourceOperatorID { get; set; }
        public string DataSourceOperatorCode { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
    }
}
