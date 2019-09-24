using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
   public class ElementHeaderViewModel
    {
       public string ElementHeaderText { get; set; }
       public string ValidationMessage { get; set; }
       public bool HasRule { get; set; }
       public bool HasValidation { get; set; }
       public bool HasCustomeRule { get; set; }
       public bool HasDataSource { get; set; }
       public bool HasCascadingRules { get; set; }
    }
}
