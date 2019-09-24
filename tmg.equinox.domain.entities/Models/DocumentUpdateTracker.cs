using System;
using System.ComponentModel.DataAnnotations;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentUpdateTracker : Entity
    {
        public int ForminstanceID { get; set; }
        public int Status { get; set; }
        public string OldJsonHash { get; set; }
        public string CurrentJsonHash { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public partial class SchemaUpdateTracker : Entity
    {
        [Key]
        public int SchemaUpdateTrackerID { get; set; }
        public int FormdesignID { get; set; }
        public int FormdesignVersionID { get; set; }
        public int Status { get; set; }
        public string OldJsonHash { get; set; }
        public string CurrentJsonHash { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public partial class MDMLog : Entity
    {
        public int LogId { get; set; }
        public int ForminstanceID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string ErrorDescription { get; set; }
        public string Error { get; set; }
        public System.DateTime AddedDate { get; set; }
       
    }
}
