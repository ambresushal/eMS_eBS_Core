using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementAddModel : ViewModelBase
    {

        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ParentUIElementID { get; set; }

        //UIElementID of the UIElement
        public int UIElementID { get; set; }

        //Element Type of the UIElement
        public string ElementType { get; set; }

        public string Label { get; set; }

        public bool IsRequired { get; set; }
        public bool IsEnable { get; set; }
        public bool IsVisible { get; set; }
        public int MaxLength { get; set; }
        public int Sequence { get; set; }
        public string DataType { get; set; }
        public int ApplicationDataTypeID { get; set; }
        public bool IsMultiselect { get; set; }
        public string HelpText { get; set; }
        public string Format { get; set; }
        public int? LibraryRegexID { get; set; }
        public string OptionYes { get; set; }
        public string OptionNo { get; set; }
        public bool IsContainer { get; set; }
        public List<DropDownItemModel> Items { get; set; }
        public bool AreRulesModified { get; set; }
        public List<RuleRowModel> Rules { get; set; }
        public RepeaterElementModel AdvancedConfiguration { get; set; }
        public string ExtendedProperties { get; set; }
        public string Layout { get; set; }
        public int ViewType { get; set; }
        public bool IsStandard { get; set; }
        public string DefaultValue { get; set; }
        public bool AllowBulkUpdate { get; set; }
        public int SourceUIElementId { get; set; }
        public string CustomHtml { get; set; }
    }
}
