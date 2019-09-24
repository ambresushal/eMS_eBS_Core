using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tmg.equinox.applicationservices.viewmodels.MasterList;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class DropDownElementModel
    {
        public int TenantID { get; set; }
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
        public int ViewType { get; set; }
        public string SelectedValue { get; set; }
        public bool IsRequired { get; set; }
        public List<DropDownItemModel> Items { get; set; }
        public int UIElementDataTypeID { get; set; }
        public bool IsDataSourceMapped { get; set; }
        public int FormDesignID { get; set; }
        public bool IsDropDownTextBox { get; set; } //Added on 12/16/14
        public bool IsSortRequired { get; set; }
        public Nullable<bool> IsLibraryRegex { get; set; }
        public string Regex { get; set; }
        public string CustomRegexMessage { get; set; }
        public bool MaskFlag { get; set; }
        public Nullable<int> LibraryRegexID { get; set; }
        public bool AllowGlobalUpdates { get; set; }
        public bool IsDropDownFilterable { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsStandard { get; set; }
        public string MDMName { get; set; }
    }

    public class DropDownItemModel
    {
        public int ItemID { get; set; }
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public int Sequence { get; set; }
    }
}
