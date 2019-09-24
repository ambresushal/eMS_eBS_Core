using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
  public  class DocumentVersionUIElementRowModel:ViewModelBase
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

        public string parent { get; set; }

        public bool isLeaf { get; set; }

        public bool isExt { get; set; }

        public bool IsKey { get; set; }

        public int? ParentElementID { get; set; }

        public bool isIncluded { get; set; }

        public bool AllowGlobalUpdates { get; set; }
    }
}
