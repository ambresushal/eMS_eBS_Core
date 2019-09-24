using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentFilter : Entity
    {
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public int DocumentID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FolderID { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }

        public int AnchorID;
    }
}
