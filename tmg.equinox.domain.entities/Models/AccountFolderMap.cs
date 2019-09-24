using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class AccountFolderMap:Entity
    {
        public AccountFolderMap()
        {
        }

        public int AccountFolderMapID { get; set; }
        public int AccountID { get; set; }
        public int FolderID { get; set; }
        public Account Account { get; set; }
        public Folder Folder { get; set; }
    }
}
