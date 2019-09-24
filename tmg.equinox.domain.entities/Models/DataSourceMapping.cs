using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DataSourceMapping : Entity
    {
        public int DataSourceMappingID { get; set; }
        public int UIElementID { get; set; }
        public int DataSourceID { get; set; }
        public int MappedUIElementID { get; set; }
        public bool IsPrimary { get; set; }
        public Nullable<int> DataSourceElementDisplayModeID { get; set; }
        public string DataSourceFilter { get; set; }
        public Nullable<int> DataCopyModeID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public bool IsKey { get; set; }
        public Nullable<int> DataSourceOperatorID { get; set; }
        public string Value { get; set; }
        public virtual DataCopyMode DataCopyMode { get; set; }
        public virtual DataSource DataSource { get; set; }
        public virtual DataSourceElementDisplayMode DataSourceElementDisplayMode { get; set; }
        public virtual DataSourceOperatorMapping DataSourceOperatorMapping { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual UIElement UIElement { get; set; }
        public virtual UIElement UIElement1 { get; set; }
        public Nullable<int> DataSourceModeID { get; set; }
        public virtual DataSourceMode DataSourceMode { get; set; }
        public bool IncludeChild { get; set; }
    }
}
