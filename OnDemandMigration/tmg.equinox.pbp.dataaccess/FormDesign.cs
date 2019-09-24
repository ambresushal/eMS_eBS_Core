namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UI.FormDesign")]
    public partial class FormDesign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormDesign()
        {
            FormInstances = new HashSet<FormInstance>();
            FormDesignVersions = new HashSet<FormDesignVersion>();
        }

        [Key]
        public int FormID { get; set; }

        [Required]
        [StringLength(100)]
        public string FormName { get; set; }

        [StringLength(100)]
        public string DisplayText { get; set; }

        public bool IsActive { get; set; }

        [StringLength(7)]
        public string Abbreviation { get; set; }

        public int TenantID { get; set; }

        [StringLength(20)]
        public string AddedBy { get; set; }

        public DateTime AddedDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsMasterList { get; set; }

        public int DocumentDesignTypeID { get; set; }

        public int? Sequence { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormInstance> FormInstances { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormDesignVersion> FormDesignVersions { get; set; }
    }
}
