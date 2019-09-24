namespace tmg.equinox.pbp.dataaccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MigrationPlan")]
    public partial class MigrationPlan
    {
        public int MigrationPlanID { get; set; }

        public int FileID { get; set; }

        public int FolderId { get; set; }

        public int FolderVersionId { get; set; }

        public int FormInstanceId { get; set; }

        public int FormDesignVersionId { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(50)]
        public string QID { get; set; }

        [Required]
        public string ViewType { get; set; }
    }
}
