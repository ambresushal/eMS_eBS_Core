using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignVersionExt : Entity
    {
        public FormDesignVersionExt()
        {
        }
        public int FormDesignVersionExtID { get; set; }
        public int FormDesignVersionID { get; set; }
        public Nullable<int> FormDesignID { get; set; }
        public string Comments { get; set; }
        public string ExtendedColNames { get; set; }
        public string ExcelConfiguration { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

    }
}
