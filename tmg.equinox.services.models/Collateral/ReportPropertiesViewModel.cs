using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Collateral
{
    public class ReportPropertiesViewModel
    {      
         public int PropertyID { get; set; }
         public int ReportDesignId { get; set; }
         public int ReportVersionID { get; set; }
         public string ReportDescription { get; set; }
         public bool? Visible { get; set; }
         public string Location { get; set; }
         public string ReportType { get; set; }
         public string HelpText { get; set; }
         public bool? IsRelease { get; set; }
         public string Parameters { get; set; }
    }
}
