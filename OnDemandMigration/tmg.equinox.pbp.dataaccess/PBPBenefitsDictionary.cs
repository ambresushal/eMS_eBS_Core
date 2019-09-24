namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PBPBenefitsDictionary")]
    public partial class PBPBenefitsDictionary
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string FILE { get; set; }

        [StringLength(255)]
        public string NAME { get; set; }

        [StringLength(255)]
        public string TYPE { get; set; }

        public double? LENGTH { get; set; }

        [StringLength(255)]
        public string FIELD_TITLE { get; set; }

        [StringLength(255)]
        public string TITLE { get; set; }

        [StringLength(255)]
        public string Codes { get; set; }

        [StringLength(500)]
        public string CODE_VALUES { get; set; }

        [StringLength(4)]
        public string YEAR { get; set; }

        public int? MappingID { get; set; }

        public virtual PBPBenefitMapping PBPBenefitMapping { get; set; }
    }
}
