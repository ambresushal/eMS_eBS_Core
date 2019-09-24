namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PBPBenefitMapping")]
    public partial class PBPBenefitMapping
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PBPBenefitMapping()
        {
            PBPBenefitsDictionaries = new HashSet<PBPBenefitsDictionary>();
        }

        [Key]
        public int MappingID { get; set; }

        [Required]
        [StringLength(255)]
        public string PBPFile { get; set; }

        [Required]
        [StringLength(255)]
        public string ColumnName { get; set; }

        [Required]
        [StringLength(255)]
        public string DataType { get; set; }

        public int Length { get; set; }

        [Required]
        [StringLength(255)]
        public string FieldTitle { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Codes { get; set; }

        [StringLength(255)]
        public string Code_Values { get; set; }

        public int FormDesignVersionID { get; set; }

        public int FormDesignID { get; set; }

        [Required]
        [StringLength(255)]
        public string TableName { get; set; }

        [Required]
        [StringLength(255)]
        public string MappingType { get; set; }

        [Required]
        [StringLength(1000)]
        public string DocumentPath { get; set; }

        [Required]
        [StringLength(255)]
        public string ElementType { get; set; }

        public bool IsArrayElement { get; set; }

        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PBPBenefitsDictionary> PBPBenefitsDictionaries { get; set; }

        [Required]
        public string ViewType { get; set; }

        public string SOTDocumentPath { get; set; }
        public string SOTPrefix { get; set; }
        public string SOTSuffix { get; set; }
        public string IfBlankThenValue { get; set; }
        public bool IsYesNoField { get; set; }
        public bool IsCheckBothFields { get; set; }
        public string SetSimilarValues { get; set; }
    }
}
