namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fldr.FormInstance")]
    public partial class FormInstance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormInstance()
        {
            FormInstanceDataMaps = new HashSet<FormInstanceDataMap>();
        }

        public int FormInstanceID { get; set; }

        public int TenantID { get; set; }

        public DateTime AddedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string AddedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public int FolderVersionID { get; set; }

        public int FormDesignID { get; set; }

        public int FormDesignVersionID { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [StringLength(200)]
        public string ProductJsonHash { get; set; }

        public int DOCID { get; set; }

        public int? AnchorDocumentID { get; set; }

        public virtual FolderVersion FolderVersion { get; set; }

        public virtual FormDesign FormDesign { get; set; }

        public virtual FormDesignVersion FormDesignVersion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormInstanceDataMap> FormInstanceDataMaps { get; set; }
    }
}
