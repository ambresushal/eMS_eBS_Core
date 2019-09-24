namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fldr.FolderVersion")]
    public partial class FolderVersion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FolderVersion()
        {
            FormInstances = new HashSet<FormInstance>();
        }

        public int FolderVersionID { get; set; }

        public int FolderID { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int? WFStateID { get; set; }

        [Required]
        [StringLength(50)]
        public string FolderVersionNumber { get; set; }

        public int VersionTypeID { get; set; }

        public bool IsActive { get; set; }

        public int TenantID { get; set; }

        public DateTime AddedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string AddedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        [StringLength(2000)]
        public string Comments { get; set; }

        public int FolderVersionStateID { get; set; }

        public int? FolderVersionBatchID { get; set; }

        public int? ConsortiumID { get; set; }

        public int? CategoryID { get; set; }

        [StringLength(50)]
        public string CatID { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual FolderVersion FolderVersion1 { get; set; }

        public virtual FolderVersion FolderVersion2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormInstance> FormInstances { get; set; }
    }
}
