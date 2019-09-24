using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementRowModel : ViewModelBase
    {
        //UIElementID of the UIElement
        public int UIElementID { get; set; }

        public string UIElementName { get; set; }

        //Element Type of the UIElement
        public string ElementType { get; set; }

        //Data Type of the UIElement
        public string DataType { get; set; }

        //Sequence based on Order - see IUIElementService-GetUIElementListForFormDesignVersion method
        public int Sequence { get; set; }

        //Level in Hierarchy
        public int level { get; set; }

        //Max Length of the UIElement(applies to TextBoxes)
        public int MaxLength { get; set; }

        //Required Field or not(return Yes or No)
        public string Required { get; set; }

        public string Label { get; set; }

        public bool? IsVisible { get; set; }

        public string parent { get; set; }

        public bool isLeaf { get; set; }

        public bool isExt { get; set; }

        public bool loaded { get; set; }

        public bool IsMappedUIElement { get; set; }

        public int MappedUIElementId { get; set; }

        public string MappedUIElementName { get; set; }

        public string MappedUIElementType { get; set; }

        public Nullable<int> DataSourceFilterOperatorID { get; set; }

        public string DataSourceFilterValue { get; set; }

        public Nullable<int> DataCopyModeID { get; set; }

        public bool IsKey { get; set; }

        public int? ParentElementID { get; set; }

        public bool hasLoadFromServer { get; set; }
        public string UIElementPath { get; set; }

        public bool IsRepeaterKey { get; set; }
        public string GeneratedName { get; set; }
        public bool IsStandard { get; set; }

        public bool? Enabled { get; set; }


    }
}
