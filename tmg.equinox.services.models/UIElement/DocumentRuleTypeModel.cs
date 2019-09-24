using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class DocumentRuleTypeModel
    {
        public int DocumentRuleTypeID { get; set; }
        public string DocumentRuleTypeCode { get; set; }
        public string DisplayText { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
