using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataSourceElementDisplayMode : Entity
    {
        public DataSourceElementDisplayMode()
        {
            this.UIElements = new List<UIElement>();
            this.DataSourceMappings = new List<DataSourceMapping>();
        }

        public int DataSourceElementDisplayModeID { get; set; }
        public string DisplayMode { get; set; }
        public virtual ICollection<UIElement> UIElements { get; set; }
        public virtual ICollection<DataSourceMapping> DataSourceMappings { get; set; }


    }
}
