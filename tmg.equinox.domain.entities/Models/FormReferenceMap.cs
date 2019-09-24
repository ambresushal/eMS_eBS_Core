using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormReferenceMap : Entity
    {
        public FormReferenceMap()
        {

        }

        public int FormReferenceID { get; set; }
        public int ReferenceAccountID { get; set; }
        public int ReferenceFolderID { get; set; }
        public int ReferenceFolderVersionID { get; set; }
        public int ReferenceFormInstanceID { get; set; }
        public int? ReferenceConsortiumID { get; set; }
        public int TargetFormInstanceID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }


    }
}
