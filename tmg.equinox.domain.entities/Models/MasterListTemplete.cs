using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListTemplate : Entity
    {
        public int MasterListTemplate1Up { get; set; }
        public string MLSectionName { get; set; }
        public string FilePath { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
