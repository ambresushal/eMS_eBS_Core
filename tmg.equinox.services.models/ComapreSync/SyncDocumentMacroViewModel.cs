using System;
using System.Collections.Generic;

namespace tmg.equinox.applicationservices.viewmodels.comparesync
{
    public class SyncDocumentMacroViewModel : ViewModelBase
    {
        public int MacroID { get; set; }
        public string MacroJSON { get; set; }
        public string MacroName { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        //public System.DateTime AddedDate { get; set; }
        //public string AddedBy { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
        //public string UpdatedBy { get; set; }
        //public bool isLocked { get; set; }
        public bool IsPublic { get; set; }
        public string Notes { get; set; }

    }
}
