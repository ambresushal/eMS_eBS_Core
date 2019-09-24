using System;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FolderVersionModel
    {
        public int FolderVersionID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string VersionNumber { get; set; }
        public string WorkFlowStatus { get; set; }
        public string Status { get; set; } //In-PRogress,Baselined
        public string Comments { get; set; }
    }
}