namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fldr.Folder")]
    public partial class Folder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Folder()
        {
            Folder1 = new HashSet<Folder>();
            FolderVersions = new HashSet<FolderVersion>();
        }

        public int FolderID { get; set; }

        public bool IsPortfolio { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public int MarketSegmentID { get; set; }

        [Required]
        [StringLength(100)]
        public string PrimaryContent { get; set; }

        public int? PrimaryContentID { get; set; }

        public int TenantID { get; set; }

        public DateTime AddedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string AddedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string UpdatedBy { get; set; }

        public int? ParentFolderId { get; set; }

        public int? FormDesignGroupId { get; set; }

        public int? MasterListFormDesignID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Folder> Folder1 { get; set; }

        public virtual Folder Folder2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FolderVersion> FolderVersions { get; set; }
    }
}
