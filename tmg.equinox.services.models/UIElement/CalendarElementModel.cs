using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tmg.equinox.applicationservices.viewmodels.MasterList;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class CalendarElementModel
    {
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UIElementID { get; set; }
        public int ParentUIElementID { get; set; }
        public bool Enabled { get; set; }
        public int Sequence { get; set; }
        public bool IsRequired { get; set; }
        public bool Visible { get; set; }
        public string Label { get; set; }
        public string HelpText { get; set; }
        public bool HasCustomRule { get; set; }
        public string CustomRule { get; set; }
        public Nullable<DateTime> DefaultDate { get; set; }
        public Nullable<DateTime> MaxDate { get; set; }
        public Nullable<DateTime> MinDate { get; set; }
        public bool IsDataSourceMapped { get; set; }
        public int FormDesignID { get; set; }
        public bool AllowGlobalUpdates { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }
        public string MDMName { get; set; }
    }
}
