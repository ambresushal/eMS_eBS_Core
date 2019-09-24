using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementRowViewModel : ViewModelBase
    {
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        //Element Type of the UIElement
        public string ElementType { get; set; }
        public string Layout { get; set; }
        public string HasOptions { get; set; }
        public List<DropDownItemModel> Items { get; set; }
        public string DataSource { get; set; }
        public bool IsMultiselect { get; set; }
        //Data Type of the UIElement
        public string DataType { get; set; }
        public int ApplicationDataTypeID { get; set; }
        //Sequence based on Order - see IUIElementService-GetUIElementListForFormDesignVersion method
        public int Sequence { get; set; }
        //Max Length of the UIElement(applies to TextBoxes)
        public string MaxLength { get; set; }
        //Required Field or not(return Yes or No)
        public bool Required { get; set; }
        public int? LibraryRegexID { get; set; }
        public string Formats { get; set; }
        public string Label { get; set; }
        public string Element { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsEnable { get; set; }
        public string Parent { get; set; }
        public string Section { get; set; }
        public int? ParentUIElementId { get; set; }
        public bool IsKey { get; set; }
        public string HasRules { get; set; }
        public string HelpText { get; set; }
        public bool IsContainer { get; set; }
        public bool AreRulesModified { get; set; }
        public List<RuleRowModel> Rules { get; set; }
        public string HasConfig { get; set; }
        public string HasRoleAccess { get; set; }
        public string HasExpRules { get; set; }
        public AdvancedConfiguration AdvancedConfiguration { get; set; }
        public JObject ExtProp { get; set; }
        public string DefaultValue { get; set; }
        public string ViewType { get; set; }
        public bool AllowBulkUpdate { get; set; }
        public int SourceUIElementId { get; set; }
        public string OptionYes { get; set; }
        public string OptionNo { get; set; }
        public string CustomHtml { get; set; }
        public bool IsSameSectionRuleSource { get; set; }
    }

    public class ConfigViewRowModel
    {
        public List<DropdownList> DataTypes { get; set; }
        public List<DropdownList> ElementTypes { get; set; }
        public List<DropdownList> DataFormats { get; set; }
        public List<ParentList> SectionElements { get; set; }
        public List<UIElementRowViewModel> UIElementList { get; set; }
        public List<Comments> Comments { get; set; }

        public List<ExtendedProperties> ExtendedProperties { get; set; }
    }

    public class ExtendedProperties
    {
        public string Header { get; set; }
        public string Name { get; set; }
    }

    public class DropdownList
    {
        public int Value { get; set; }
        public string DisplayText { get; set; }
    }

    public class ParentList
    {
        public int Value { get; set; }
        public string DisplayText { get; set; }
        public string ElementType { get; set; }
    }

    public class Comments
    {
        public string row { get; set; }
        public string col { get; set; }
        public Comment comment { get; set; }
    }

    public class Comment
    {
        public string value { get; set; }
    }

    public class AdvancedConfiguration
    {
        public bool DisplayTopHeader { get; set; }
        public bool DisplayTitle { get; set; }
        public int FrozenColCount { get; set; }
        public int FrozenRowCount { get; set; }
        public bool AllowPaging { get; set; }
        public int RowsPerPage { get; set; }
        public bool AllowExportToExcel { get; set; }
        public bool AllowExportToCSV { get; set; }
        public string FilterMode { get; set; }
    }
}