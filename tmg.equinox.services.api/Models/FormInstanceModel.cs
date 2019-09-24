using System;

namespace tmg.equinox.services.api.Models
{
    public class FormInstanceModel
    {
        public int FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignID { get; set; }
        public string FormDesignName { get; set; }
        public int FormDesignVersionID { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
    }
}