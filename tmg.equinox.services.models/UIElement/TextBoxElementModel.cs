using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class TextBoxElementModel
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
        public int UIElementDataTypeID { get; set; }
        public string UIElementDataTypeName { get; set; }
        public string UIElementDataTypeDisplayText { get; set; }
        public string DefaultValue { get; set; }
        public bool IsLabel { get; set; }
        public bool IsMultiLine { get; set; }
        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }
        public bool SpellCheck { get; set; }
        public Nullable<bool> IsLibraryRegex { get; set; }
        public string Regex { get; set; }
        public string CustomRegexMessage { get; set; }
        public bool MaskFlag { get; set; }
        public Nullable<int> LibraryRegexID { get; set; }
        public int FormDesignID { get; set; }
        public bool IsDataSourceMapped { get; set; }
        public bool AllowGlobalUpdates { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }

        public string MDMName { get; set; }
    }
}
