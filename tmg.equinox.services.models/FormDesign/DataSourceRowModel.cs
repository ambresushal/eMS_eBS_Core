using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class DataSourceRowModel : ViewModelBase
    {
        public int DataSourceId { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDescription { get; set; }      
        public string DataSourceType { get; set; }
        public string IsCurrentDS { get; set; }
        public int? DisplayMode { get; set; }
        public string IsPrimary { get; set; }
        public int? DispalyDataSourceMode { get; set; }
        public string DocumentName { get; set; }
        public int UIElementID { get; set; }

    }
}
