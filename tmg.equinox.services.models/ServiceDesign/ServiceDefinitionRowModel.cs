using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class ServiceDefinitionRowModel : ViewModelBase
    {
        #region Public Properties
        public int ServiceDefinitionID { get; set; }
        public int UIElementID { get; set; }
        public string UIElementLabel { get; set; }
        public string UIElementFullPath { get; set; }
        public int UIElementDataTypeID { get; set; }
        public string UIElementDataType { get; set; }
        public int UIElementTypeID { get; set; }
        public string UIElementType { get; set; }
        public string DisplayName { get; set; }
        public int? ParentServiceDefinitionID { get; set; }
        public int Sequence { get; set; }
        public bool IsKey { get; set; }
        public string Required { get; set; }

        public int level { get; set; }
        public string parent { get; set; }
        public bool isLeaf { get; set; }
        public bool isExt { get; set; }
        public bool loaded { get; set; }
        #endregion Public Properties
    }
}
