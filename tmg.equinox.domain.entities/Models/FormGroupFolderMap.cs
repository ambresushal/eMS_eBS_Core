using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormGroupFolderMap : Entity
    {
        public int FormGroupFolderMapID { get; set; }
        public int TenantID { get; set; }
        public int FormDesignGroupID { get; set; }
        public string FolderType { get; set; }
    }
}
