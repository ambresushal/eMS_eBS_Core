using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class FormDesignUIElementMap
    {
        public int UIElementID { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    internal class FormDesignIgnoreUIElementMap
    {
        public int UIElementID { get; set; }
        public string Operation { get; set; }
    }
}
