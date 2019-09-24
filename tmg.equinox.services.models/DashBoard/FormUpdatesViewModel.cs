using System;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class FormUpdatesViewModel
    {
        public string FormName { get; set; }
        public string VersionNumber { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Comments { get; set; }
        public string TenantID { get; set; }
        public int FormDesignVersionId { get; set; }
    }
}
