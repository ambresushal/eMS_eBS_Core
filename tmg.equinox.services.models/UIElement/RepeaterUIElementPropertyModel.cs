using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.UIElement
{
    public class RepeaterUIElementPropertyModel
    {
        public int RepeaterUIElementPropertyID { get; set; }
        public int RepeaterUIElementID { get; set; }
        public string RowTemplate { get; set; }
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }
    }
}
