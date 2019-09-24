namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AccessPBPColumn
    {
        [Key]
        public int ColumnID { get; set; }

        [Required]
        [StringLength(255)]
        public string TableName { get; set; }

        [Required]
        [StringLength(255)]
        public string ColumnName { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }
    }
}
