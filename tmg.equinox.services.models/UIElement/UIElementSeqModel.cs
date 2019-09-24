using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class UIElementSeqModel : ViewModelBase
    {
        //UIElementID of the UIElement
        public int UIElementID { get; set; }

        //Element Type of the UIElement
        public string ElementType { get; set; }

        //Sequence based on Order - see IUIElementService-GetUIElementListForFormDesignVersion method
        public int Sequence { get; set; }

        public string Label { get; set; }

        public bool CheckDuplicate { get; set; }

        public bool IsKey { get; set; }

        public bool IsRepeaterKeyMapped { get; set; }

        public bool IsElementMapped { get; set; }
        public bool IsStandard { get; set; }
    }

    public class UIElementModelSerializerHelper
    {
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ParentUIElementID { get; set; }
        public List<UIElementSeqModel> Models { get; set; }
    }
}
