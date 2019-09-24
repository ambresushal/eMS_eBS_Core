using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class RepeaterElementModel
    {
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UIElementID { get; set; }
        public int ParentUIElementID { get; set; }
        public bool Enabled { get; set; }
        public int Sequence { get; set; }
        public bool Visible { get; set; }
        public bool IsRequired { get; set; }       
        public string Label { get; set; }
        public string HelpText { get; set; }
        public bool HasCustomRule { get; set; }
        public string CustomRule { get; set; }
        public int ChildCount { get; set; }
        public int LayoutTypeID { get; set; }
        public bool IsDataSource { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDescription { get; set; }
        public bool IsDataSourceEnabled { get; set; }
        public string DuplicationCheck { get; set; }
        ///*** Master List Changes
        public  bool LoadFromServer { get; set; }
        public bool IsLoadFromServerEnabled { get; set; }
        public bool IsDataRequired { get; set; }
        public bool AllowBulkUpdate { get; set; }
        public int RepeaterKeyElementID { get; set; }

        // Properties for Configuring Param Query Features 
        public bool DisplayTopHeader { get; set; }
        public bool DisplayTitle { get; set; }
        public int FrozenColCount { get; set; }
        public int FrozenRowCount { get; set; }
        public bool AllowPaging { get; set; }
        public int RowsPerPage { get; set; }
        public bool AllowExportToExcel { get; set; }
        public bool AllowExportToCSV { get; set; }
        public string FilterMode { get; set; }
        public RepeaterUIElementPropertyModel RepeaterUIElementProperties { get; set; }
        public bool IsStandard { get; set; }
        public string MDMName { get; set; }

     // */
    }
}
