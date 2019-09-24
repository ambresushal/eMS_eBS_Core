namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FormDesignVersionJSON")]
    public partial class FormDesignVersionJSON
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FormDesignVersionID { get; set; }

        [Required]
        public string DefaultJSON { get; set; }
    }
}
