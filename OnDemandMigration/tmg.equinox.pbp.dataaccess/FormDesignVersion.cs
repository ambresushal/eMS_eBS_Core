namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UI.FormDesignVersion")]
    public partial class FormDesignVersion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormDesignVersion()
        {
            FormInstances = new HashSet<FormInstance>();
        }

        public int FormDesignVersionID { get; set; }

        public int? FormDesignID { get; set; }

        public int? TenantID { get; set; }

        [StringLength(15)]
        public string VersionNumber { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public string FormDesignVersionData { get; set; }

        public int StatusID { get; set; }

        [Required]
        [StringLength(20)]
        public string AddedBy { get; set; }

        public DateTime AddedDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        public int? FormDesignTypeID { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public string RuleExecutionTreeJSON { get; set; }

        public string RuleEventMapJSON { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormInstance> FormInstances { get; set; }

        public virtual FormDesign FormDesign { get; set; }

        public virtual FormDesignVersion FormDesignVersion1 { get; set; }

        public virtual FormDesignVersion FormDesignVersion2 { get; set; }
    }
}
