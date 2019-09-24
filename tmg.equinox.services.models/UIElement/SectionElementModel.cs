using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class SectionElementModel
    {
        public int TenantID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UIElementID { get; set; }
        public int ParentUIElementID { get; set; }
        public bool Enabled { get; set; }
        public int Sequence { get; set; }
        public bool Visible { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public bool HasCustomRule { get; set; }
        public string CustomRule { get; set; }
        public int ChildCount { get; set; }
        public int LayoutTypeID { get; set; }
        //get from the LayoutType enum - or add a table for layout type(2 col, 3 col)
        public string LayoutType { get; set; }
        public bool IsDataSource { get; set; }
        public string DataSourceName { get; set; }      
        public string DataSourceDescription { get; set; }
        public bool IsDataSourceEnabled { get; set; }
        public string CustomHtml { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }

        public string MDMName { get; set; }
    }

    public class SectionElementModelSerializerHelper
    {
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public List<SectionElementModel> Models {get; set;}
    }
}
