using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion
{
    public class DataSourceDesign
    {
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public int FormDesignID { get; set; }
        public string SourceParent { get; set; }
        public string TargetParent { get; set; }
        public string MappingType { get; set; }
        public string DisplayMode { get; set; }
        public string DataSourceModeType { get; set; }
        public string DataSourceName { get; set; }
        public List<DataSourceElementMapping> Mappings { get; set; }
    }


    public class DataSourceElementMapping
    {
        public string SourceElement { get; set; }
        public string TargetElement { get; set; }
        public bool IsKey { get; set; }
        public int CopyModeID { get; set; }
        public string Filter { get; set; }
        public int OperatorID { get; set; }
        public bool IncludeChild { get; set; }
    }

    public class DataSourceItem
    {
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int SourceParentUIElementID { get; set; }
        public int TargetParentUIElementID { get; set; }
        public string DisplayMode { get; set; }
        public int SourceUIElementID { get; set; }
        public int TargetUIElementID { get; set; }
        public string SourceUIElementName { get; set; }
        public string TargetUIElementName { get; set; }
    }
}
