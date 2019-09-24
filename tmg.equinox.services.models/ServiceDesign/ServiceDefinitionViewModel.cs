using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class ServiceDefinitionViewModel : ViewModelBase
    {
        #region Public Properties
        public int ServiceDefinitionID { get; set; }
        public string UIElementFullPath { get; set; }
        public int UIElementDataTypeID { get; set; }
        public int UIElementTypeID { get; set; }
        public string UIElementType { get; set; }
        public int UIElementID { get; set; }
        public string Label { get; set; }
        public string DisplayName { get; set; }
        public int? ParentServiceDefinitionID { get; set; }
        public int Sequence { get; set; }
        public bool IsKey { get; set; }
        public bool IsRequired { get; set; }
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public int TenantID { get; set; }
        #endregion Public Properties
    }
}
