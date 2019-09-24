using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderVersionHistoryViewModel
    {
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; } 
        public DateTime EffectiveDate { get; set; }
        public string FolderVersionNumber { get; set; }
        public string User { get; set; }
        public int WFStateId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string WFStateName { get; set; }
        public string FolderVersionStateName { get; set; }
        public string VersionType { get; set; }
        public DateTime AddedDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CatID { get; set; }
    }
}
