using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class DataSourceUiElementMappingModel : ViewModelBase
    {
       public int DataSourceId {get;set;}
       public int UIElementID { get; set; }
       public int MappedUIElementID { get; set; }
       public int? DataSourceElementDisplayModeID { get; set; }
       public int? DataSourceModeID { get; set; }
       public bool IsPrimary { get; set; }
       public int? DataSourceMappingOperatorID { get; set; }
       public string DataSourceFilter { get; set; }
       public int? DataCopyModeID { get; set; }
       public bool IsKey { get; set; }
        
    }
    
}
