using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class IASElementExportDataModel
    {
        public int IASElementExportID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public int IASFolderMapID { get; set; }
        public int FormInstanceID { get; set; }
        public string Label { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool AcceptChange { get; set; }
        public int UIElementTypeID { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelNo { get; set; }
        public string ItemData { get; set; }
        public int FormDesignID { get; set; }
    }
}
