using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataSourceMode : Entity
    {
        public DataSourceMode()
        {
            //this.UIElements = new List<UIElement>();
            this.DataSourceMappings = new List<DataSourceMapping>();
        }

        public int DataSourceModeID { get; set; }
        public string DataSourceModeType { get; set; }
        //public virtual ICollection<UIElement> UIElements { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }
    }
}
