using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign
{
    public class UIElementModel : ViewModelBase
    {
        #region Public Properties
        public int UIElementID { get; set; }
        public int? ParentUIElementID { get; set; }
        public string UIelementFullPath { get; set; }
        public string DataType { get; set; }
        public string ElementType { get; set; }
        public string Label { get; set; }
        #endregion Public Properties
    }
}
