using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class FormDesignElementValueVeiwModel : ViewModelBase
    {
        #region Instance Properties
        public Int32 FormDesignElementValueID { get; set; }
        public Int32 GlobalUpdateID { get; set; }
        public Int32 FormDesignID { get; set; }
        public Int32 FormDesignVersionID { get; set; }
        public Int32 UIElementID { get; set; }
        public String ElementFullPath { get; set; }
        public Boolean IsUpdated { get; set; }
        public string IsValueUpdated { get; set; }
        public String NewValue { get; set; }
        public String ElementHeaderName { get; set; }
        public string FormDesignName { get; set; }
        public string Name { get; set; }
        public int UIElementTypeID { get; set; }
        public string Label { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }
        public string ItemData { get; set; }
        public string UIElementName { get; set; }
        public bool IsParentRepeater { get; set; }
        #endregion Instance Properties
    }

    public class DocumentInstanceModel
    {
        public int FormDesignID { get; set; }
        public string DocumentName { get; set; }
        public string csv { get; set; }
    }
}
