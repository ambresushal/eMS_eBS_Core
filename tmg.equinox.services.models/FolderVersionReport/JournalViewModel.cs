using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersionReport
{
    public class JournalViewModel: ViewModelBase
    {
        public int JournalID { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public string Description { get; set; }
        public string FieldName { get; set; }
        public string FieldPath { get; set; }
        public int ActionID { get; set; }
        public string ActionName { get; set; }
        public int AddedWFStateID { get; set; }
        public string AddedWFStateName { get; set; }
        public int? ClosedWFStateID { get; set; }
        public string ClosedWFStateName { get; set; }
        public string FolderVersionNumber { get; set; }
    }
}
