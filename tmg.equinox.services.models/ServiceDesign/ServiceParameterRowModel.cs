using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class ServiceParameterRowModel : ViewModelBase
    {
        #region Public Properties
        public int ServiceParameterID { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public int DataTypeID { get; set; }
        public string DataTypeName { get; set; }
        public int ServiceDesignID { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public int TenantID { get; set; }
        public int UIElementID { get; set; }
        public string UIElementFullPath { get; set; }
        #endregion Public Properties
    }
}
