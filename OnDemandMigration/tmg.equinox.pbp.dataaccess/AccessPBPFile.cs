namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccessPBPFile")]
    public partial class AccessPBPFile
    {
        [Key]
        public int FileID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        public bool IsActive { get; set; }
      
    }
}
