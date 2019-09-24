using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class IASElementImportViewModel : ViewModelBase
    {
        #region Instance Properties
        public int IASElementImportID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int IASFolderMapID { get; set; }
        public int UIElementID { get; set; }
        public int UIElementTypeID { get; set; }
        public string UIElementName { get; set; }
        public string ElementHeaderName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool AcceptChange { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }
        public string ItemData { get; set; }
        public int FormDesignID { get; set; }
        public string ElementFullPath { get; set; }
        public bool IsParentRepeater { get; set; }
        //Fields from Global Update table      
        public string GlobalUpdateName { get; set; }
        public System.DateTime EffectiveDateFrom { get; set; }
        public System.DateTime EffectiveDateTo { get; set; }

        public bool Include{ get; set; }
        #endregion Instance Properties
    }
}
