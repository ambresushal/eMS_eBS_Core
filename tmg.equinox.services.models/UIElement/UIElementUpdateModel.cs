using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementUpdateModel
    {

        public int TenantID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ParentUIElementID { get; set; }

        public int UIElementID { get; set; }

        public string ElementType { get; set; }
        public int UIElementDataTypeID { get; set; }
        public string DefaultValue { get; set; }
        public bool DefaultBoolValue { get; set; }
        public bool? DefaultBoolValueForRadio { get; set; }
        public bool Enabled { get; set; }
        public bool HasCustomRule { get; set; }
        public string CustomRule { get; set; }
        public string HelpText { get; set; }
        public bool IsLabel { get; set; }
        public bool IsMultiLine { get; set; }
        public string Label { get; set; }
        public string AlternateLabel { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }
        public bool IsYesNo { get; set; }
        public int MaxLength { get; set; }
        public int Sequence { get; set; }
        public bool SpellCheck { get; set; }
        public bool IsRequired { get; set; }
        public bool Visible { get; set; }
        public Nullable<DateTime> DefaultDate { get; set; }
        public Nullable<DateTime> MaxDate { get; set; }
        public Nullable<DateTime> MinDate { get; set; }
        public int LayoutTypeID { get; set; }
        public string Layout { get; set; }
        public int ChildCount { get; set; }
        public List<DropDownItemModel> Items { get; set; }
        public Nullable<bool> IsLibraryRegex { get; set; }
        public string Regex { get; set; }
        public string CustomRegexMessage { get; set; }
        public bool MaskFlag { get; set; }
        public Nullable<int> LibraryRegexID { get; set; }
        public IEnumerable<RuleRowModel> Rules { get; set; }
        public bool AreRulesModified { get; set; }
        public bool IsCustomRulesModified { get; set; }
        public bool IsDataSource { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDescription { get; set; }
        public bool IsDropDownTextBox { get; set; }
        public bool IsDropDownFilterable { get; set; }
        public bool LoadFromServer { get; set; }
        public bool IsDataRequired { get; set; }
        public string CustomHtml { get; set; }
        public bool IsSortRequired { get; set; }
        public bool AllowBulkUpdate { get; set; }
        public bool AllowGlobalUpdates { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }
        public string MDMName { get; set; }
        public RepeaterElementModel AdvancedConfiguration { get; set; }
        public RepeaterUIElementPropertyModel RepeaterTemplates { get; set; }

        public bool IsMultiSelect { get; set; }
        public bool IsContainer { get; set; }
        public string ExtendedProperties { get; set; }
        public int SourceUIElementId { get; set; }
    }
}
