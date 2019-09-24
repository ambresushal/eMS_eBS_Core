using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.viewmodels.FormDesignVersion
{
    public class RepeaterRowDesign
    {
        public int RowID { get; set; }
        public string RowName { get; set; }
        public string RowLabel { get; set; }
        public int? LayoutColumn { get; set; }
        public string LayoutClass { get; set; }
        public int ChildCount { get; set; }
        public bool isDummyRow { get; set; }
        public bool RunRulesOnLoad { get; set; }
        public List<ElementDesign> Element { get; set; }
    }
}
