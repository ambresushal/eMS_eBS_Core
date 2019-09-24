using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class FormDesignGroupMapModel
    {
        public int FormDesignID { get; set; }
        public string FormName { get; set; }
        public string FolderType { get; set; }
        public int ParentFormDesignID { get; set; }
        public bool AllowMultiple { get; set; }
        public bool IsMasterList { get; set; }
    }
}
