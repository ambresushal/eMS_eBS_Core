using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CompareSync
{
    public class DocumentElementViewModel : DocumentElementBaseModel
    {
        public string ElementType { get; set; }
        public int level { get; set; }
        public string parent { get; set; }
        public int? ParentElementID { get; set; }
        public bool isLeaf { get; set; }

        public bool expanded { get; set; }
    }
}
