using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementSummaryModel : ViewModelBase
    {
        //UIElementID of the UIElement
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        //Element Type of the UIElement
        public string ElementType { get; set; }
        public string Label { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsEnable { get; set; }
        public int? ParentUIElementId { get; set; }
        public string Parent { get; set; }
        public string GeneratedName { get; set; }
        public string Section { get; set; }
        public string Element { get; set; }
       
    }
}
